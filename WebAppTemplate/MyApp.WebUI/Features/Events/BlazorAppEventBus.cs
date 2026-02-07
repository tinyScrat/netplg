namespace MyApp.WebUI.Features.Events;

using System.Reactive.Linq;
using System.Reactive.Subjects;
using MyApp.Application.Abstractions;

public sealed class BlazorAppEventBus : IAppEventBus, IDisposable
{
    private readonly Subject<AppEvent> _events = new();

    public IObservable<TEvent> OfType<TEvent>()
        where TEvent : AppEvent
        => _events.OfType<TEvent>();

    public void Publish(AppEvent evt) => _events.OnNext(evt);

    public void Dispose() => _events.Dispose();
}
