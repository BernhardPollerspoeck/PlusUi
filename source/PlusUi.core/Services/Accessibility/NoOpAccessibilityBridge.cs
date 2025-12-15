namespace PlusUi.core.Services.Accessibility;

/// <summary>
/// No-op implementation of <see cref="IAccessibilityBridge"/> that does nothing.
/// Used as a fallback when no platform-specific accessibility bridge is available.
/// </summary>
public sealed class NoOpAccessibilityBridge : IAccessibilityBridge
{
    /// <inheritdoc />
    public bool IsEnabled => false;

    /// <inheritdoc />
    public void Initialize(Func<UiElement?> rootProvider)
    {
        // No-op
    }

    /// <inheritdoc />
    public void Announce(string message, bool interrupt = false)
    {
        // No-op
    }

    /// <inheritdoc />
    public void NotifyFocusChanged(UiElement? element)
    {
        // No-op
    }

    /// <inheritdoc />
    public void NotifyValueChanged(UiElement element)
    {
        // No-op
    }

    /// <inheritdoc />
    public void NotifyStructureChanged()
    {
        // No-op
    }

    /// <inheritdoc />
    public void NotifyPropertyChanged(UiElement element)
    {
        // No-op
    }

    /// <inheritdoc />
    public object? GetAccessibilityNode(UiElement element)
    {
        return null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // No-op
    }
}
