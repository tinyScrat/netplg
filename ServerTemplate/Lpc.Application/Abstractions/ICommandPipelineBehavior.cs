namespace Lpc.Application.Abstractions;

public delegate Task CommandHandlerDelegate();

public interface ICommandPipelineBehavior<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CommandHandlerDelegate next, CancellationToken ct = default);
}

public delegate Task<TResult> CommandHandlerDelegate<TResult>();

public interface ICommandPipelineBehavior<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CommandHandlerDelegate<TResult> next, CancellationToken ct = default);
}
