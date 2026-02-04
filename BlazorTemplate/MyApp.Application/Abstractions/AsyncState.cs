using System.Reactive.Linq;

namespace MyApp.Application.Abstractions;

public interface IAsyncTracker
{
    IObservable<T> TrackAsync<T>(IObservable<T> source);
}

public static class AsyncStatusExtensions
{
    public static bool IsIdle(this AsyncStatus status)
        => status is AsyncStatus.Idle;
    
    public static bool IsLoading(this AsyncStatus status)
        => status is AsyncStatus.Loading;

    public static Exception? ErrorOrNull(this AsyncStatus status)
        => status is AsyncStatus.Error e ? e.Exception : null;
}

public abstract record AsyncStatus
{
    private AsyncStatus() { }

    public sealed record Idle : AsyncStatus;
    public sealed record Loading : AsyncStatus;
    public sealed record Error(Exception Exception) : AsyncStatus;

    public static readonly AsyncStatus IdleInstance = new Idle();
    public static readonly AsyncStatus LoadingInstance = new Loading();

    public static AsyncStatus FromError(Exception ex) => new Error(ex);
}

public sealed class AsyncState<T>(T initial) : IAsyncTracker, IDisposable
{
    private bool _disposed = false;

    // NEW: counts concurrent tracked operations
    private int _inFlight = 0;

    public ReactiveState<T> Data { get; } = new ReactiveState<T>(initial);
    public ReactiveState<AsyncStatus> Status { get; } = new ReactiveState<AsyncStatus>(AsyncStatus.IdleInstance);

    public IObservable<TResult> TrackAsync<TResult>(IObservable<TResult> source)
    {
        return Observable.Defer(() =>
        {
            Exception? error = null;

            // Enter loading state when first operation starts
            if (Interlocked.Increment(ref _inFlight) == 1)
                Status.Set(AsyncStatus.LoadingInstance);

            try
            {
                return source
                    .Do(
                        onNext: _ => { },
                        onError: ex => error = ex,
                        onCompleted: () => { })
                    .Finally(() =>
                    {
                        var remaining = Interlocked.Decrement(ref _inFlight);

                        // Error wins (don’t overwrite it with Idle)
                        if (error != null)
                        {
                            Status.Set(AsyncStatus.FromError(error));
                            return;
                        }

                        // Only go Idle when nothing else is running
                        if (remaining == 0)
                            Status.Set(AsyncStatus.IdleInstance);
                    });
            }
            catch (Exception ex)
            {
                // Synchronous error
                Status.Set(AsyncStatus.FromError(ex));
                Interlocked.Decrement(ref _inFlight);
                return Observable.Throw<TResult>(ex);
            }
        });
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        Data.Dispose();
        Status.Dispose();
    }
}
