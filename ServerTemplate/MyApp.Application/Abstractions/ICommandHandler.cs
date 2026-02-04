namespace MyApp.Application.Abstractions;

/// Handles a single command by executing domain logic, enforcing invariants,
/// performing validation/idempotency, persisting changes, and returning a result if needed.
/// Each handler maps to one business action (user story) in the application layer.

public interface ICommandHandler<in TCommand, out TResult> where TCommand : ICommand
{
    TResult Handle(TCommand command);
}

public interface ICommandHandlerWithResult<in TCommand, out TResult> where TCommand : ICommand<TResult>
{
    TResult Handle(TCommand command);
}
