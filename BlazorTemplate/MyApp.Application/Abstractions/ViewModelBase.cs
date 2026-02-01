namespace MyApp.Application.Abstractions;

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

// UI → Dispatch(Command) → Effect → State update

public abstract class ViewModelBase : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    internal void AddDisposable(IDisposable disposable)
        => _disposables.Add(disposable);

    public virtual void Dispose()
        => _disposables.Dispose();
}

public abstract class ReactiveViewModel<TCommand> : ViewModelBase
    where TCommand : ICommand
{
    private readonly ObservableEvent<TCommand> _commands = new();

    protected readonly ObservableState<bool> _isBusy = new(false);
    protected readonly ObservableState<string?> _error = new(null);

    protected IObservable<TCommand> Commands => _commands.Events;
    public IObservable<bool> IsBusy => _isBusy.Changes;
    public IObservable<string?> Error => _error.Changes.Where(e => e is not null);

    protected void Dispatch(TCommand command) => _commands.Emit(command);

    protected IDisposable TrackEffect(IObservable<Unit> effect)
    {
        _isBusy.Set(true);

        return effect
            .Materialize()
            .Do(notification =>
            {
                if (notification.Kind == NotificationKind.OnError)
                    _error.Set(notification.Exception!.Message);

                if (notification.Kind != NotificationKind.OnNext)
                    _isBusy.Set(false);
            })
            .Dematerialize()
            .Subscribe()
            .DisposeWith(this);
    }

    public override void Dispose()
    {
        _commands.Dispose();
        _isBusy.Dispose();
        _error.Dispose();

        base.Dispose();
    }
}

public static class DisposableExtensions
{
    public static T DisposeWith<T>(this T disposable, ViewModelBase vm)
        where T : IDisposable
    {
        vm.AddDisposable(disposable);
        return disposable;
    }
}
