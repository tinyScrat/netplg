namespace MyApp.Application.Abstractions;

public interface IEventBus
{
    Task PublishAsync(object evt, CancellationToken ct = default);

    void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
}
