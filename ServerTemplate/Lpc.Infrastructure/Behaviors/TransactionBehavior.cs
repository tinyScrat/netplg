using System.Text.Json;
using Lpc.Application.Abstractions;
using Lpc.Domain.Abstractions;
using Lpc.Infrastructure.Persistence;

namespace Lpc.Infrastructure.Behaviors;

internal sealed class TransactionBehavior<TCommand, TResult>(AppDbContext db)
    : ICommandPipelineBehavior<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public async Task<TResult> HandleAsync(TCommand command, CommandHandlerDelegate<TResult> next, CancellationToken ct = default)
    {
        try
        {
            await using var tx = await db.Database.BeginTransactionAsync(ct);

            // Execute handler
            var result = await next();

            // Extract domain events BEFORE saving
            var domainEvents = db.ChangeTracker
                .Entries<IAggregateRoot>()
                .Select(e => e.Entity)
                .SelectMany(aggregate =>
                {
                    var events = aggregate.DomainEvents.ToList();
                    aggregate.ClearDomainEvents();
                    return events;
                })
                .ToList();

            // Convert to outbox messages
            if (domainEvents.Count > 0)
            {
                var outboxMessages = domainEvents.Select(e =>
                    new OutboxMessage
                    {
                        Id = e.Id,
                        OccurredOnUtc = e.OccurredOnUtc,
                        Type = e.GetType().FullName!,
                        Payload = JsonSerializer.Serialize(e, e.GetType())
                    });

                db.Set<OutboxMessage>().AddRange(outboxMessages);
            }

            // Single SaveChanges
            await db.SaveChangesAsync(ct);

            // Commit
            await tx.CommitAsync(ct);

            return result;
        }
        catch
        {
            await db.Database.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
