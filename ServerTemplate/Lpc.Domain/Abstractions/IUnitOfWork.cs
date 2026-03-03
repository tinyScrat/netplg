namespace Lpc.Domain.Abstractions;

/// <summary>
/// Unit of Work interface for managing transactions.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);

    bool HasActiveTransaction { get; }
}
