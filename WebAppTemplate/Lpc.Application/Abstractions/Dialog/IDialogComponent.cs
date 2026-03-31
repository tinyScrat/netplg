namespace Lpc.Application.Abstractions;

/// <summary>
/// Contract for a strongly-typed Blazor dialog component.
/// 
/// TRequest: Input model provided when opening the dialog.
/// TResult:  Result type returned to the caller when the dialog closes.
/// 
/// Implementations must expose the request via <see cref="Request"/> and
/// call <see cref="Close(TResult)"/> to return a result.
/// </summary>
public interface IDialogComponent<TRequest, TResult>
    where TRequest : IDialogRequest<TResult>
{
    /// <summary>
    /// Input request for this dialog.
    /// </summary>
    TRequest Request { get; set; }

    /// <summary>
    /// Close the dialog and return a result.
    /// </summary>
    void Close(TResult result);
}
