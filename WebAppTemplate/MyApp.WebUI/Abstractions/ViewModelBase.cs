namespace MyApp.WebUI.Abstractions;

using System.Reactive.Linq;
using System.Reactive.Disposables;
using MyApp.Application.Abstractions;

// UI → Dispatch(Command) → Effect → State update

public abstract class ViewModelBase(GlobalErrorStore errorStore) : IDisposable
{
    private readonly CompositeDisposable _disposables = [];
    private bool _disposed;
    public event Action? StateChanged;
    public event Action<Exception>? ExceptionOccurred;

    protected void RaiseStateChanged() => StateChanged?.Invoke();
    
    protected void RaiseException(Exception ex)
    {
        errorStore.Publish(ex);
        ExceptionOccurred?.Invoke(ex);
    }

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
            ex =>
            {
                onError?.Invoke(ex);
                RaiseException(ex);
            });

        return Track(sub);
    }

    protected IDisposable SubscribeAsync<T>(
        IObservable<T> source,
        Func<T, CancellationToken, Task> handler,
        Action<Exception>? onError = null)
    {
        var disposeCts = new CancellationTokenSource();

        var sub = source
            .Select(v =>
                Observable.FromAsync(async rxCt =>
                {
                    // Cancel when either:
                    // - Switch() unsubscribes (rxCt), or
                    // - VM is disposed (disposeCts)
                    using var linked = CancellationTokenSource.CreateLinkedTokenSource(rxCt, disposeCts.Token);
                    await handler(v, linked.Token).ConfigureAwait(false);
                }))
            .Switch()
            .Subscribe(
                _ => RaiseStateChanged(),
                ex =>
                {
                    onError?.Invoke(ex);
                    RaiseException(ex);
                });

        return Track(Disposable.Create(() =>
        {
            disposeCts.Cancel();
            disposeCts.Dispose();
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
