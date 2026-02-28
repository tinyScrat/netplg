namespace MyApp.WebUI.Services;

using Microsoft.AspNetCore.Components;
using MyApp.Application.Abstractions;
using Radzen;

public static class DialogServiceExtensions
{
    /// <summary>
    /// Open a dialog using strongly typed request & component, no magic dictionary in caller code.
    /// </summary>
    public static async Task<TResult?> OpenTypedAsync<TComponent, TRequest, TResult>(
        this DialogService dialogService,
        TRequest request,
        TResult defaultResult = default!)
        where TComponent : ComponentBase, IDialogComponent<TRequest, TResult>
        where TRequest : IDialogRequest<TResult>
    {
        // The only dictionary creation is here
        var parameters = new Dictionary<string, object>
        {
            { nameof(IDialogComponent<,>.Request), request }
        };

        var result = await dialogService.OpenAsync<TComponent>(
            request.Title,
            parameters,
            request.Policy.ToOptions());

        return result is TResult typedResult ? typedResult : defaultResult;
    }

    public static DialogOptions ToOptions(this DialogPolicy policy)
    {
        return new DialogOptions
        {
            CloseDialogOnOverlayClick = policy.CloseOnOverlayClick,
            Draggable = policy.Draggable,
            Resizable = policy.Resizable,
            Width = policy.Width,
            Height = policy.Height,
            ShowClose = true,
            CloseDialogOnEsc = policy.IsModal,
            // Modal behavior:
            Style = policy.IsModal ? null : "position: fixed;"
        };
    }
}
