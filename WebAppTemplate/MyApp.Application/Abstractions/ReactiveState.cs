namespace MyApp.Application.Abstractions;

using System.Reactive.Linq;
using System.Reactive.Subjects;

/// <summary>
/// to represent state
/// </summary>
public sealed class ReactiveState<T>(T initial) : IDisposable
{
    private readonly BehaviorSubject<T> _subject = new(initial);
    private bool _disposed = false;

    public T Value
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _subject.Value;
        }
    }

    public IObservable<T> Changes =>
        _subject
            .AsObservable()
            .DistinctUntilChanged();

    public void Set(T value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _subject.OnNext(value);
    }

    public void Update(Func<T, T> updater)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _subject.OnNext(updater(_subject.Value));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _subject.OnCompleted();
        _subject.Dispose();
    }
}
