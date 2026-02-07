namespace MyApp.Application.Abstractions;

using System.Reactive.Linq;

public interface IAppEventBus
{
   IObservable<TEvent> OfType<TEvent>()
        where TEvent : AppEvent;
   void Publish(AppEvent evt);
}

public static class AuthRxExtensions
{
   public static IObservable<T> WithAuthRedirect<T>(
        this IObservable<T> source,
        IAppEventBus bus)
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
        IAppEventBus bus)
        where TEvent : AppEvent
    {
        return source.TakeUntil(
            bus.OfType<TEvent>().Take(1)
        );
    }

    public static IObservable<T> CancelOnSessionExpired<T>(
        this IObservable<T> source,
        IAppEventBus bus)
        => source.TakeUntilEvent<T, SessionExpiredEvent>(bus);
}
