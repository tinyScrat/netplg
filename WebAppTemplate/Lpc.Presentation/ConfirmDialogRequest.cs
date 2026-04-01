namespace Lpc.Presentation.Features;

using Lpc.Application.Abstractions;

/// <summary>
/// Example dialog request for confirmation dialogs.
/// The result is a boolean indicating whether the user confirmed or cancelled.
/// </summary>
/// <param name="Message"></param>
public sealed record ConfirmDialogRequest(string Title) : DialogRequest<bool>(
    Title,
    new DialogPolicy(
        SingleInstanceKey: "confirm-dialog",
        InstanceBehavior: SingleInstanceBehavior.IgnoreIfExists
    ))
{
    public string Message { get; init; } = null!;
}
