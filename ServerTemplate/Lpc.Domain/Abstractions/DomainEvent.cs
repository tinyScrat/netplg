namespace Lpc.Domain.Abstractions;

public abstract record DomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
