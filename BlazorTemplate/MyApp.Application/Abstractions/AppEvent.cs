namespace MyApp.Application.Abstractions;

public abstract record AppEvent;

public sealed record SessionExpiredEvent : AppEvent;

public sealed record UserLoggedOutEvent : AppEvent;

public interface IAppEventBus
{
   void Publish(AppEvent evt);
}
