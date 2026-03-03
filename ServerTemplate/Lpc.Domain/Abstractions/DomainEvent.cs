namespace Lpc.Domain.Abstractions;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredOnUtc { get; }
}

public abstract record DomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
