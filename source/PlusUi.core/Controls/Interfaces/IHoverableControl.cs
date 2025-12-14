namespace PlusUi.core;

/// <summary>
/// Interface for controls that respond to mouse hover events.
/// </summary>
public interface IHoverableControl
{
    /// <summary>
    /// Gets or sets whether the control is currently being hovered over.
    /// </summary>
    bool IsHovered { get; set; }
}
