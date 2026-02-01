using System.Reactive.Linq;

namespace MyApp.Application.Abstractions;

public static class AsyncStatusExtensions
{
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

    public static AsyncStatus FromError(Exception ex)
        => new Error(ex);
}


// public abstract record AsyncState<T>
// {
//     private AsyncState() { }

//     public sealed record Idle : AsyncState<T>;
//     public sealed record Loading : AsyncState<T>;
//     public sealed record Success(T Value) : AsyncState<T>;
//     public sealed record Error(string Message, Exception? Exception = null) : AsyncState<T>;

//     public static AsyncState<T> IdleState { get; } = new Idle();
//     public static AsyncState<T> LoadingState { get; } = new Loading();
// }

public sealed class AsyncState<T>
{
    public ReactiveState<T> Data { get; }
    public ReactiveState<AsyncStatus> Status { get; }

    public AsyncState(T initial)
    {
        Data = new ReactiveState<T>(initial);
        Status = new ReactiveState<AsyncStatus>(AsyncStatus.IdleInstance);
    }

    public IObservable<TResult> TrackAsync<TResult>(IObservable<TResult> source)
    {
        return Observable.Defer(() =>
        {
            Status.Set(AsyncStatus.LoadingInstance);

            return source
                .Do(
                    onNext: _ =>
                    {
                        Status.Set(AsyncStatus.IdleInstance);
                    },
                    onError: ex =>
                    {
                        Status.Set(AsyncStatus.FromError(ex));
                    }
                );
        });
    }
}
