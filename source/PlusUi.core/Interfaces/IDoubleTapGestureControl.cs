using System.Windows.Input;

namespace PlusUi.core;

public interface IDoubleTapGestureControl : IGestureControl
{
    ICommand? DoubleTapCommand { get; }
    void OnDoubleTap();
}
