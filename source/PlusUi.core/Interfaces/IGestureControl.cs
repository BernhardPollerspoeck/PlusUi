using System.Windows.Input;

namespace PlusUi.core;

public interface IGestureControl : IInteractiveControl
{
}

public interface ILongPressGestureControl : IGestureControl
{
    ICommand? LongPressCommand { get; }
    void OnLongPress();
}

public interface IDoubleTapGestureControl : IGestureControl
{
    ICommand? DoubleTapCommand { get; }
    void OnDoubleTap();
}

public interface ISwipeGestureControl : IGestureControl
{
    ICommand? SwipeCommand { get; }
    SwipeDirection AllowedDirections { get; }
    void OnSwipe(SwipeDirection direction);
}

public interface IPinchGestureControl : IGestureControl
{
    ICommand? PinchCommand { get; }
    void OnPinch(float scale);
}
