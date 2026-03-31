namespace Lpc.Application.Abstractions;

public abstract record AppEvent;

public interface IAppEventBus
{
   IObservable<TEvent> OfType<TEvent>() where TEvent : AppEvent;

   void Publish(AppEvent evt);
}
