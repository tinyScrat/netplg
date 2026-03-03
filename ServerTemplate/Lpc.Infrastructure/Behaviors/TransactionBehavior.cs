using Lpc.Application.Abstractions;
using Lpc.Domain.Abstractions;

namespace Lpc.Infrastructure.Behaviors;

internal sealed class TransactionBehavior<TCommand, TResult>(IUnitOfWork uow)
    : ICommandPipelineBehavior<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public async Task<TResult> HandleAsync(TCommand command, CommandHandlerDelegate<TResult> next, CancellationToken ct = default)
    {
        await uow.BeginTransactionAsync(ct);

        try
        {
            var result = await next();

            await uow.CommitTransactionAsync(ct);
            return result;
        }
        catch
        {
            await uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
