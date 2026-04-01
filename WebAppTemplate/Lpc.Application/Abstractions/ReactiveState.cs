namespace Lpc.Application.Abstractions;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

public interface IReactiveState
{
    IObservable<Unit> Signal { get; }
}

/// <summary>
/// to represent state
/// </summary>
public sealed class ReactiveState<T> : IDisposable, IReactiveState
{
    private readonly IObservable<T> _changes;
    private readonly IObservable<Unit> _signal;
    private readonly BehaviorSubject<T> _subject;
    private bool _disposed = false;

    public ReactiveState(T initial)
    {
        _subject = new (initial);

        _changes = _subject
            .DistinctUntilChanged();

        _signal = _changes.Select(static _ => Unit.Default);
    }

    public T Value
    {
        get
        {
            ThrowIfDisposed();
            return _subject.Value;
        }
    }

    public IObservable<T> Changes => _changes;

    // New: a "signal" stream for components
    public IObservable<Unit> Signal => _signal;

    public void Set(T value)
    {
        ThrowIfDisposed();

        if (EqualityComparer<T>.Default.Equals(_subject.Value, value)) return;

        _subject.OnNext(value);
    }

    public void Update(Func<T, T> updater)
    {
        ThrowIfDisposed();
        _subject.OnNext(updater(_subject.Value));
    }

    public void Update(Action<T> mutation)
    {
        ThrowIfDisposed();
        var current = _subject.Value;
        mutation(current);
        _subject.OnNext(current);
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
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
