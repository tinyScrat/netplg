namespace MyApp.Application.Abstractions;

public sealed record DialogPolicy(
    bool IsModal = true,
    bool EnforceExclusiveModal = true,

    bool CloseOnOverlayClick = false,
    bool Draggable = true,
    bool Resizable = false,
    string? Width = null,
    string? Height = null,

    string? SingleInstanceKey = null,
    SingleInstanceBehavior InstanceBehavior = SingleInstanceBehavior.AllowMultiple,

    int Priority = 0,
    DialogQueueMode QueueMode = DialogQueueMode.Immediate
);

public enum SingleInstanceBehavior
{
    AllowMultiple,   // Default
    IgnoreIfExists,  // Return existing task
    ReplaceExisting, // Cancel previous and open new
    Throw            // Throw exception
}

public enum DialogQueueMode
{
    Immediate,     // Open immediately (default)
    Enqueue,       // Wait for turn
    ReplaceLower   // Replace lower priority active dialogs
}
