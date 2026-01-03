using System.Windows.Input;

namespace PlusUi.core;

public interface ILongPressGestureControl : IGestureControl
{
    ICommand? LongPressCommand { get; }
    void OnLongPress();
}
