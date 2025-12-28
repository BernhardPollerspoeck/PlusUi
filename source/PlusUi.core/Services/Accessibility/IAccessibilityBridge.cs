namespace PlusUi.core.Services.Accessibility;

/// <summary>
/// Platform-specific bridge for accessibility services (screen readers, etc.).
/// Each platform implements this interface to integrate with native accessibility APIs.
/// </summary>
public interface IAccessibilityBridge
{
    /// <summary>
    /// Gets whether accessibility services are currently enabled on the platform.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Initializes the accessibility bridge with the given root element provider.
    /// </summary>
    /// <param name="rootProvider">Function that returns the root UI element.</param>
    void Initialize(Func<UiElement?> rootProvider);

    /// <summary>
    /// Announces a message to the screen reader.
    /// Used for important notifications that should interrupt the user.
    /// </summary>
    /// <param name="message">The message to announce.</param>
    /// <param name="interrupt">Whether to interrupt any current announcement.</param>
    void Announce(string message, bool interrupt = false);

    /// <summary>
    /// Notifies the accessibility system that focus has changed.
    /// </summary>
    /// <param name="element">The element that received focus, or null if focus was cleared.</param>
    void NotifyFocusChanged(UiElement? element);

    /// <summary>
    /// Notifies the accessibility system that an element's value has changed.
    /// </summary>
    /// <param name="element">The element whose value changed.</param>
    void NotifyValueChanged(UiElement element);

    /// <summary>
    /// Notifies the accessibility system that the UI structure has changed.
    /// Call this when elements are added, removed, or significantly reorganized.
    /// </summary>
    void NotifyStructureChanged();

    /// <summary>
    /// Notifies the accessibility system that an element's properties have changed.
    /// </summary>
    /// <param name="element">The element whose properties changed.</param>
    void NotifyPropertyChanged(UiElement element);

    /// <summary>
    /// Gets the accessibility node for a specific element.
    /// Returns platform-specific object (e.g., UIA AutomationElement, ATK object, etc.).
    /// </summary>
    /// <param name="element">The UI element to get the accessibility node for.</param>
    /// <returns>Platform-specific accessibility node, or null if not available.</returns>
    object? GetAccessibilityNode(UiElement element);

    /// <summary>
    /// Disposes of the accessibility bridge and releases resources.
    /// </summary>
    void Dispose();
}
