namespace MyApp.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using MyApp.Application.Abstractions;

public sealed class DialogLoggingMiddleware<TRequest, TResult>(ILogger<DialogLoggingMiddleware<TRequest, TResult>> logger)
    : IDialogMiddleware<TRequest, TResult>
    where TRequest : IDialogRequest<TResult>
{
    private readonly ILogger<DialogLoggingMiddleware<TRequest, TResult>> _logger = logger;

    public async Task<TResult?> HandleAsync(
        TRequest request,
        DialogHandlerDelegate<TResult?> next,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Opening dialog {Request}", typeof(TRequest).Name);

        var result = await next();

        _logger.LogInformation("Dialog {Request} completed", typeof(TRequest).Name);

        return result;
    }
}
