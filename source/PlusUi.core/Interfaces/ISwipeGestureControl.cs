using System.Windows.Input;

namespace PlusUi.core;

public interface ISwipeGestureControl : IGestureControl
{
    ICommand? SwipeCommand { get; }
    SwipeDirection AllowedDirections { get; }
    void OnSwipe(SwipeDirection direction);
}
