namespace Lpc.WebUI.Services;

using Microsoft.AspNetCore.Components;
using Lpc.Application.Abstractions;
using Radzen;

public sealed class RadzenDialogHandler<TComponent, TRequest, TResult>(DialogService dialogService)
    : IDialogHandler<TRequest, TResult>
    where TComponent : ComponentBase, IDialogComponent<TRequest, TResult>
    where TRequest : IDialogRequest<TResult>
{
    public Task<TResult?> HandleAsync(TRequest request, CancellationToken ct)
    {
        return dialogService.OpenTypedAsync<
            TComponent,
            TRequest,
            TResult>(request);
    }
}
