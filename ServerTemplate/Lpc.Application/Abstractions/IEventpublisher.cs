namespace Lpc.Application.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync(object evt, CancellationToken ct);
}
