namespace PlusUi.core;

/// <summary>
/// Interface for controls that support drag interactions.
/// Used by controls like Slider that need to respond to drag gestures.
/// </summary>
public interface IDraggableControl : IInteractiveControl
{
    void HandleDrag(float deltaX, float deltaY);
    bool IsDragging { get; set; }
}
