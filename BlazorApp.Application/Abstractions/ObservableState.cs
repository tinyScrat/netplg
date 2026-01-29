namespace BlazorApp.Application.Abstractions;

using System.Reactive.Linq;
using System.Reactive.Subjects;

public sealed class ObservableState<T>(T initial) : IDisposable
{
    private readonly BehaviorSubject<T> _subject = new(initial);

    public T Value => _subject.Value;

    public IObservable<T> Changes => _subject.AsObservable();

    public void Update(Func<T, T> updater)
    {
        _subject.OnNext(updater(_subject.Value));
    }

    public void Dispose() => _subject.Dispose();
}
