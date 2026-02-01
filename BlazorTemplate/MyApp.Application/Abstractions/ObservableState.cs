namespace MyApp.Application.Abstractions;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

/// <summary>
/// to represent state
/// </summary>
public sealed class ObservableState<T>(T initial) : IDisposable
{
    private readonly BehaviorSubject<T> _subject = new(initial);
    private bool _disposed;

    public T Value => _subject.Value;

    public IObservable<T> Changes =>
        _subject
            .AsObservable()
            .DistinctUntilChanged();

    public void Set(T value)
    {
        ThrowIfDisposed();
        _subject.OnNext(value);
    }

    public void Update(Func<T, T> updater)
    {
        ThrowIfDisposed();
        _subject.OnNext(updater(_subject.Value));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _subject.OnCompleted();
        _subject.Dispose();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ObservableState<T>));
    }
}

/// <summary>
/// Represent intent
/// </summary>
public sealed class ObservableEvent<T> : IDisposable where T : ICommand
{
    private readonly Subject<T> _subject = new();

    public IObservable<T> Events => _subject.AsObservable();

    public void Emit(T value) => _subject.OnNext(value);

    public void Dispose()
    {
        _subject.OnCompleted();
        _subject.Dispose();
    }
}

public abstract record AsyncState<T>
{
    private AsyncState() { }

    public sealed record Idle : AsyncState<T>;
    public sealed record Loading : AsyncState<T>;
    public sealed record Success(T Value) : AsyncState<T>;
    public sealed record Error(string Message, Exception? Exception = null) : AsyncState<T>;

    public static AsyncState<T> IdleState { get; } = new Idle();
    public static AsyncState<T> LoadingState { get; } = new Loading();
}

public sealed class AsyncStateStore<T> : IDisposable
{
    private readonly ObservableState<AsyncState<T>> _state =
        new ObservableState<AsyncState<T>>(AsyncState<T>.IdleState);

    public AsyncState<T> Value => _state.Value;

    public IObservable<AsyncState<T>> Changes => _state.Changes;

    public bool IsLoading =>
        _state.Value is AsyncState<T>.Loading;

    public bool HasValue =>
        _state.Value is AsyncState<T>.Success;

    public T? CurrentValue =>
        HasValue ? (_state.Value as AsyncState<T>.Success)!.Value : default(T);

    public void SetIdle() =>
        _state.Set(AsyncState<T>.IdleState);

    public void SetLoading() =>
        _state.Set(AsyncState<T>.LoadingState);

    public void SetSuccess(T value) =>
        _state.Set(new AsyncState<T>.Success(value));

    public void SetError(Exception ex) =>
        _state.Set(new AsyncState<T>.Error(ex.Message, ex));

    public void Dispose() => _state.Dispose();
}

public static class AsyncStateExtensions
{
    public static IDisposable TrackAsync<T>(this AsyncStateStore<T> state, IObservable<T> source)
    {
        state.SetLoading();

        return source
            .Materialize()
            .Subscribe(notification =>
            {
                switch (notification.Kind)
                {
                    case NotificationKind.OnNext:
                        state.SetSuccess(notification.Value!);
                        break;

                    case NotificationKind.OnError:
                        state.SetError(notification.Exception!);
                        break;

                    case NotificationKind.OnCompleted:
                        break;
                }
            });
    }
}
