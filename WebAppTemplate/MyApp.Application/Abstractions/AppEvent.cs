using System.Security.Claims;

namespace MyApp.Application.Abstractions;

public abstract record AppEvent;

public sealed record SessionExpiredEvent : AppEvent;

public sealed record UserLoggedOutEvent : AppEvent;

public sealed record AuthStateChangedEvent(ClaimsPrincipal User) : AppEvent;

public interface IAppEventBus
{
   IObservable<TEvent> OfType<TEvent>()
        where TEvent : AppEvent;
   void Publish(AppEvent evt);
}
