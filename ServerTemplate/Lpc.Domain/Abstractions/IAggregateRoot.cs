namespace Lpc.Domain.Abstractions;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

public interface IAggregateRoot<out TId> : IAggregateRoot where TId : EntityId<TId>
{
}

public abstract class AggregateRoot<TId>(TId id)
    : EntityBase<TId>(id), IAggregateRoot<TId>
    where TId : EntityId<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public int Version { get; private set; } = 0;

    protected void RaiseDomainEvent(IDomainEvent evt)
    {
        _domainEvents.Add(evt);
        Version += 1;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
