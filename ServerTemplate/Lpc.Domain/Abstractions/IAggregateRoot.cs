namespace Lpc.Domain.Abstractions;

public interface IAggregateRoot<out TId>
    where TId : EntityId<TId>
{
    TId GetId();
}

public abstract class AggregateRoot<TId>
    : EntityBase<TId>, IAggregateRoot<TId>
    where TId : EntityId<TId>
{
    private readonly List<DomainEvent> _domainEvents = [];

    public int Version { get; private set; } = 0;

    protected AggregateRoot(TId id) : base(id) { }

    TId IAggregateRoot<TId>.GetId() => Id;

    protected void RaiseDomainEvent(DomainEvent @event)
    {
        _domainEvents.Add(@event);
        Version += 1;
    }

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();
}
