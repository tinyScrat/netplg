namespace Lpc.Application.Abstractions;

public delegate Task<TResult?> DialogHandlerDelegate<TResult>();

/// <summary>
/// Represents a middleware in the dialog handling pipeline.
/// 
/// Use cases for dialog middleware include:
/// Logging
/// Telemetry
/// Validation
/// Audit
/// Global error wrapping
/// Policy enforcement
/// Analytics
/// 
/// TRequest: The strongly-typed dialog request.
/// TResult: The type of result returned by the dialog.
/// 
/// Middleware can inspect, modify, short-circuit, or augment the handling of dialogs.
/// The <paramref name="next"/> delegate invokes the next middleware or final handler.
/// </summary>
public interface IDialogMiddleware<TRequest, TResult>
    where TRequest : IDialogRequest<TResult>
{
    Task<TResult?> HandleAsync(
        TRequest request,
        DialogHandlerDelegate<TResult?> next,
        CancellationToken ct);
}
