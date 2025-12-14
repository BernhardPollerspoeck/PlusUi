namespace PlusUi.core;

/// <summary>
/// Interface for overlays that can be dismissed when clicking outside.
/// </summary>
public interface IDismissableOverlay
{
    /// <summary>
    /// Dismisses/closes the overlay.
    /// </summary>
    void Dismiss();
}
