using System.Windows.Input;

namespace PlusUi.core;

public interface IPinchGestureControl : IGestureControl
{
    ICommand? PinchCommand { get; }
    void OnPinch(float scale);
}
