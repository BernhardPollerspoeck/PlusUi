namespace PlusUi.core.Services.Accessibility;

/// <summary>
/// Core accessibility service that coordinates accessibility features across the application.
/// This service integrates with the focus manager and platform-specific accessibility bridges.
/// </summary>
public interface IAccessibilityService
{
    /// <summary>
    /// Gets whether accessibility services are currently active.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the platform-specific accessibility bridge.
    /// </summary>
    IAccessibilityBridge Bridge { get; }

    /// <summary>
    /// Initializes the accessibility service with the root element provider.
    /// This should be called after the UI is ready.
    /// </summary>
    /// <param name="rootProvider">Function that returns the root UI element.</param>
    void Initialize(Func<UiElement?> rootProvider);

    /// <summary>
    /// Announces a message to the screen reader.
    /// </summary>
    /// <param name="message">The message to announce.</param>
    /// <param name="interrupt">Whether to interrupt any current announcement.</param>
    void Announce(string message, bool interrupt = false);

    /// <summary>
    /// Notifies the accessibility system that an element's value has changed.
    /// </summary>
    /// <param name="element">The element whose value changed.</param>
    void NotifyValueChanged(UiElement element);

    /// <summary>
    /// Notifies the accessibility system that the UI structure has changed.
    /// </summary>
    void NotifyStructureChanged();

    /// <summary>
    /// Notifies the accessibility system that an element's properties have changed.
    /// </summary>
    /// <param name="element">The element whose properties changed.</param>
    void NotifyPropertyChanged(UiElement element);
}
