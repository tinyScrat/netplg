namespace BlazorApp.Application.Abstractions;

public abstract record AppEvent;

public sealed record SessionExpiredEvent : AppEvent;

public interface IAppEventBus
{
   void Publish(AppEvent evt);
}
