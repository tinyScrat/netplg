namespace WebApp.Domain.Abstractions;

public interface IRepository<TAggregate, TId>
    where TAggregate : IAggregateRoot<TId>
    where TId : EntityId<TId>
{
    Task<TAggregate?> GetAsync(TId id);
    Task AddAsync(TAggregate aggregate);
}
