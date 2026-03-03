namespace Lpc.Application.Abstractions;

/// Handles a single command by executing domain logic, enforcing invariants,
/// performing validation/idempotency, persisting changes, and returning a result if needed.
/// Each handler maps to one business action (user story) in the application layer.

public interface ICommand { }

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface ICommand<TResult> : ICommand { }

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}
