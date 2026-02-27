namespace MyApp.WebUI.Services;

using MyApp.Application.Abstractions;
using Radzen;

internal static class DialogPolicyMapper
{
    public static DialogOptions ToOptions(DialogPolicy policy)
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
