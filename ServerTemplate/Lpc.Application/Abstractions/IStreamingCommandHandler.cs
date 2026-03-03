namespace Lpc.Application.Abstractions;

public interface IStreamingCommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    // Returns an async enumerable for streaming results
    // Implementations should be async (e.g., yielding results over time)
    IAsyncEnumerable<TResult> Handle(TCommand command, CancellationToken ct = default);
}
