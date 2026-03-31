namespace Lpc.WebUI.Abstractions;

using Microsoft.AspNetCore.Components;
using Lpc.Application.Abstractions;
using Radzen;

public abstract class DialogComponentBase<TRequest, TResult>
    : RxComponentBase, IDialogComponent<TRequest, TResult>
    where TRequest : IDialogRequest<TResult>
{
    [Parameter]
    public TRequest Request { get; set; } = default!;

    [Inject]
    protected DialogService DialogService { get; set; } = default!;

    /// <summary>
    /// Close dialog with default result.
    /// </summary>
    protected void Cancel()
    {
        Close(default!);
    }

    public void Close(TResult result)
    {
        DialogService.Close(result);
    }

    /// <summary>
    /// Access dialog policy if needed.
    /// </summary>
    protected DialogPolicy Policy => Request.Policy;
}
