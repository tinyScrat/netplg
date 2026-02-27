namespace MyApp.Application.Abstractions;

/// <summary>
/// Represents a strongly-typed request to open a dialog.
/// 
/// TResult: The type of value that will be returned when the dialog closes.
/// 
/// Properties:
/// - <see cref="Title"/>: Display title of the dialog.
/// - <see cref="Policy"/>: Dialog behavior and lifecycle policy (e.g., modal, priority, single-instance).
/// </summary>
public interface IDialogRequest<TResult>
{
    string Title { get; }
    DialogPolicy Policy { get; }
}

public abstract record DialogRequest<TResult>(string Title, DialogPolicy? Policy = null) : IDialogRequest<TResult>
{
    public DialogPolicy Policy { get; } = Policy ?? new();
    public string Title { get; } = Title;
}
