namespace PlusUi.core;

public interface IScrollableControl
{
    void HandleScroll(float deltaX, float deltaY);
    bool IsScrolling { get; set; }
}