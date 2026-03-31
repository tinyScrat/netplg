namespace Lpc.Application.Abstractions;

/// <summary>
/// Handles a strongly-typed dialog request.
/// 
/// TRequest: The dialog request type implementing <see cref="IDialogRequest{TResult}"/>.
/// TResult:  The type of result returned when the dialog completes.
/// 
/// Implementations contain the logic to show the dialog and produce a result.
/// </summary>
public interface IDialogHandler<TRequest, TResult>
    where TRequest : IDialogRequest<TResult>
{
    Task<TResult?> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken);
}
