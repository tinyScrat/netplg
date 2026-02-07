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

    public void Publish(AppEvent evt)
        => _events.OnNext(evt);

    public void Dispose()
        => _events.Dispose();
}

public static class AuthRxExtensions
{
   public static IObservable<T> WithAuthRedirect<T>(
        this IObservable<T> source,
        BlazorAppEventBus bus)
    {
        return source.TakeUntilEvent<T, SessionExpiredEvent>(bus);
    }

   public static IObservable<T> TakeUntilEvent<T, TEvent>(
        this IObservable<T> source,
        IObservable<AppEvent> events)
        where TEvent : AppEvent
    {
        return source.TakeUntil(
            events.OfType<TEvent>().Take(1)
        );
    }

    public static IObservable<T> TakeUntilEvent<T, TEvent>(
        this IObservable<T> source,
        BlazorAppEventBus bus)
        where TEvent : AppEvent
    {
        return source.TakeUntil(
            bus.OfType<TEvent>().Take(1)
        );
    }

    public static IObservable<T> CancelOnSessionExpired<T>(
        this IObservable<T> source,
        BlazorAppEventBus bus)
        => source.TakeUntilEvent<T, SessionExpiredEvent>(bus);
}
