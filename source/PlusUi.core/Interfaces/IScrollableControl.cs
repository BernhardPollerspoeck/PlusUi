namespace PlusUi.core;

public interface IScrollableControl : IInteractiveControl
{
    void HandleScroll(float deltaX, float deltaY);
    bool IsScrolling { get; set; }
}