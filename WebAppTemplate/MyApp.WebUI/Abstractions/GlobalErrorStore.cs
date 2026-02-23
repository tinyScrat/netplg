namespace MyApp.WebUI.Abstractions;

using System.Reactive.Subjects;
using System.Reactive.Linq;

public sealed class GlobalErrorStore : IDisposable
{
    private readonly BehaviorSubject<Exception?> _errors = new(null);

    public IObservable<Exception?> Errors => _errors.AsObservable();

    public void Publish(Exception ex) => _errors.OnNext(ex);

    public void Clear() => _errors.OnNext(null);

    public void Dispose() => _errors.Dispose();
}
