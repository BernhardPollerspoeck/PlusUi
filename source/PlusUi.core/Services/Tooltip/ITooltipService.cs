namespace PlusUi.core;

/// <summary>
/// Service interface for managing tooltip display and lifecycle.
/// </summary>
public interface ITooltipService
{
    /// <summary>
    /// Called when the mouse enters an element that may have a tooltip.
    /// </summary>
    /// <param name="element">The element being hovered, or null if hovering over nothing.</param>
    void OnHoverEnter(UiElement? element);

    /// <summary>
    /// Called when the mouse leaves the currently hovered element.
    /// </summary>
    /// <param name="element">The element that was being hovered.</param>
    void OnHoverLeave(UiElement? element);
}
