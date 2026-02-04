namespace MyApp.Application.Abstractions;

// For commands without a specific result type
public interface IAsyncCommandHandler<in TCommand, TResult> where TCommand : ICommand
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}

// For commands with a typed result
public interface IAsyncCommandHandlerWithResult<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}
