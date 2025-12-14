namespace PlusUi.core;

/// <summary>
/// Service for managing overlay elements that render above the page but below popups.
/// </summary>
public interface IOverlayService
{
    /// <summary>
    /// Registers an overlay element to be rendered.
    /// </summary>
    void RegisterOverlay(UiElement element);

    /// <summary>
    /// Unregisters an overlay element.
    /// </summary>
    void UnregisterOverlay(UiElement element);
}
