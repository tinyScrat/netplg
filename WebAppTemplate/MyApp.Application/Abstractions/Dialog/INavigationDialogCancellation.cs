namespace MyApp.Application.Abstractions;

/// <summary>
/// Monitors navigation events and notifies subscribers when
/// active dialogs should be cancelled.
/// 
/// Typically used to automatically close or cancel dialogs
/// when the user navigates to a different page.
/// </summary>
public interface INavigationDialogCancellation
{
    /// <summary>
    /// Raised when navigation occurs and dialogs should be cancelled.
    /// </summary>
    event Action? NavigationTriggered;

    /// <summary>
    /// Enables navigation monitoring.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops navigation monitoring.
    /// </summary>
    void Stop();
}
