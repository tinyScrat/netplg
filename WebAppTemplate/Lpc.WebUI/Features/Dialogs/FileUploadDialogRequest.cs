namespace Lpc.WebUI.Components;

using Lpc.Application.Abstractions;

public sealed record FileUploadDialogRequest(string Title) : DialogRequest<bool>(
    Title,
    new DialogPolicy(
        SingleInstanceKey: "file-upload-dialog",
        InstanceBehavior: SingleInstanceBehavior.IgnoreIfExists
    ))
{
    public string Message { get; init; } = null!;
}
