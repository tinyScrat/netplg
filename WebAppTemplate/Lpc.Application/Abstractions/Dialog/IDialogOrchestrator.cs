namespace Lpc.Application.Abstractions;

/// <summary>
/// Coordinates the execution of dialog requests.
/// 
/// Responsible for applying dialog policies such as single-instance guards,
/// modal exclusivity, priority queuing, cancellation, and middleware execution.
/// 
/// TRequest: The strongly-typed dialog request.
/// TResult:  The type of result returned when the dialog completes.
/// 
/// If the dialog is dismissed (e.g., closed via X button) or cancelled,
/// <paramref name="defaultResult"/> is returned.
/// </summary>
public interface IDialogOrchestrator
{
    Task<TResult?> RequestAsync<TRequest, TResult>(
        TRequest request,
        TResult? defaultResult = default,
        CancellationToken ct = default)
        where TRequest : IDialogRequest<TResult>;
}
