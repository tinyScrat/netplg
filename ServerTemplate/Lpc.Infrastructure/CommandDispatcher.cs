namespace Lpc.Infrastructure;

using Lpc.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public class CommandDispatcher(IServiceProvider sp) : ICommandDispatcher
{
    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);

        var handler = sp.GetRequiredService<ICommandHandler<TCommand>>();
        var behaviors = sp.GetServices<ICommandPipelineBehavior<TCommand>>().Reverse().ToList();

        // Build pipeline: behaviors wrap the handler
        CommandHandlerDelegate pipeline = () => handler.HandleAsync(command, ct);

        foreach (var behavior in behaviors)
        {
            var next = pipeline;
            pipeline = () => behavior.HandleAsync(command, next, ct);
        }

        await pipeline();
    }

    public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(command);

        var handler = sp.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        var behaviors = sp.GetServices<ICommandPipelineBehavior<TCommand, TResult>>().Reverse().ToList();

        // Build pipeline: behaviors wrap the handler
        CommandHandlerDelegate<TResult> pipeline = () => handler.HandleAsync(command, ct);

        foreach (var behavior in behaviors)
        {
            var next = pipeline;
            pipeline = () => behavior.HandleAsync(command, next, ct);
        }

        return await pipeline();
    }
}
