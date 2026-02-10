namespace MyApp.Application.Abstractions;

public interface IAppEventBus
{
   IObservable<TEvent> OfType<TEvent>()
        where TEvent : AppEvent;
   void Publish(AppEvent evt);
}
