namespace MyApp.Application.Abstractions;

using System.Reactive.Linq;
using System.Reactive.Disposables;

// UI → Dispatch(Command) → Effect → State update

public abstract class ViewModelBase : IDisposable
{
    private readonly CompositeDisposable _disposables = [];
    private bool _disposed;
    public event Action? StateChanged;

    protected void RaiseStateChanged()
        => StateChanged?.Invoke();

    internal IDisposable Track(IDisposable disposable)
    {
        if (_disposed)
        {
            disposable.Dispose();
            return disposable;
        }

        _disposables.Add(disposable);
        return disposable;
    }

    protected IDisposable Subscribe<T>(
        IObservable<T> source,
        Action<T> onNext,
        Action<Exception>? onError = null)
    {
        var sub = source.Subscribe(
            onNext,
            ex => onError?.Invoke(ex));

        return Track(sub);
    }

    protected IDisposable SubscribeAsync<T>(
        IObservable<T> source,
        Func<T, CancellationToken, Task> handler,
        Action<Exception>? onError = null)
    {
        var cts = new CancellationTokenSource();

        var sub = source
            .Select(v =>
                Observable.FromAsync(ct => handler(v, ct)))
            .Switch()
            .Subscribe(
                _ => RaiseStateChanged(),
                ex => onError?.Invoke(ex));

        return Track(Disposable.Create(() =>
        {
            cts.Cancel();
            cts.Dispose();
            sub.Dispose();
        }));
    }

    public virtual void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _disposables.Dispose();
        GC.SuppressFinalize(this);
    }
}

public static class DisposableExtensions
{
    public static T DisposeWith<T>(this T disposable, ViewModelBase vm)
        where T : IDisposable
    {
        vm.Track(disposable);
        return disposable;
    }
}
