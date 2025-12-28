namespace PlusUi.core;

/// <summary>
/// Configuration options for popup behavior and appearance.
/// </summary>
public interface IPopupConfiguration
{
    /// <summary>
    /// Gets or sets whether the popup should close when clicking outside its bounds on the background overlay.
    /// Default is true.
    /// </summary>
    bool CloseOnBackgroundClick { get; set; }

    /// <summary>
    /// Gets or sets whether the popup should close when the Escape key is pressed.
    /// Default is true.
    /// </summary>
    bool CloseOnEscape { get; set; }

    /// <summary>
    /// Gets or sets the background overlay color behind the popup.
    /// Default is a semi-transparent black (RGB 0,0,0 with alpha 220).
    /// </summary>
    Color BackgroundColor { get; set; }
}
