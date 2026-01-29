namespace BlazorApp.Application.Abstractions;

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

public abstract class ViewModelBase : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    internal void AddDisposable(IDisposable disposable)
        => _disposables.Add(disposable);

    public virtual void Dispose()
        => _disposables.Dispose();
}

// UI → Dispatch(Command) → Effect → State update

// public abstract class ReactiveViewModel<TCommand> : ViewModelBase
//     where TCommand : ICommand
// {
//     private readonly Subject<TCommand> _commands = new();

//     protected IObservable<TCommand> Commands => _commands.AsObservable();

//     protected void Dispatch(TCommand command)
//         => _commands.OnNext(command);
// }

public abstract class ReactiveViewModel<TCommand> : ViewModelBase
    where TCommand : ICommand
{
    private readonly Subject<TCommand> _commands = new();

    protected IObservable<TCommand> Commands => _commands.AsObservable();

    protected readonly BehaviorSubject<bool> _isBusy = new(false);
    protected readonly BehaviorSubject<string?> _error = new(null);

    public IObservable<bool> IsBusy => _isBusy.DistinctUntilChanged();
    public IObservable<string?> Error => _error;

    protected void Dispatch(TCommand command)
        => _commands.OnNext(command);

    protected IDisposable TrackEffect(IObservable<Unit> effect)
    {
        _isBusy.OnNext(true);

        // var subscription = effect
        //     .Materialize()
        //     .Do(n =>
        //     {
        //         if (n.Kind == NotificationKind.OnError)
        //             _error.OnNext(n.Exception!.Message);

        //         if (n.Kind != NotificationKind.OnNext)
        //             _isBusy.OnNext(false);
        //     })
        //     .Dematerialize()
        //     .Subscribe();

        // DisposeWith(subscription);
        // return subscription;

        return effect
            .Materialize()
            .Do(notification =>
            {
                if (notification.Kind == NotificationKind.OnError)
                    _error.OnNext(notification.Exception!.Message);

                if (notification.Kind != NotificationKind.OnNext)
                    _isBusy.OnNext(false);
            })
            .Dematerialize()
            .Subscribe()
            .DisposeWith(this);
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
