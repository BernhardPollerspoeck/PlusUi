using PlusUi.core.Attributes;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// Detects swipe gestures on the wrapped content.
/// Supports filtering by allowed directions.
/// </summary>
[GenerateShadowMethods]
public partial class SwipeGestureDetector(UiElement content) : GestureDetector<SwipeGestureDetector>(content), ISwipeGestureControl
{

    #region Command
    public ICommand? SwipeCommand { get; private set; }

    public SwipeGestureDetector SetCommand(ICommand command)
    {
        SwipeCommand = command;
        return this;
    }

    public SwipeGestureDetector BindCommand(string propertyName, Func<ICommand> propertyGetter)
    {
        RegisterBinding(propertyName, () => SwipeCommand = propertyGetter());
        return this;
    }

    public void OnSwipe(SwipeDirection direction)
    {
        if (SwipeCommand?.CanExecute(direction) == true)
        {
            SwipeCommand.Execute(direction);
        }
    }
    #endregion

    #region AllowedDirections
    public SwipeDirection AllowedDirections { get; private set; } = SwipeDirection.All;

    public SwipeGestureDetector SetAllowedDirections(SwipeDirection directions)
    {
        AllowedDirections = directions;
        return this;
    }

    public SwipeGestureDetector BindAllowedDirections(string propertyName, Func<SwipeDirection> propertyGetter)
    {
        RegisterBinding(propertyName, () => AllowedDirections = propertyGetter());
        return this;
    }
    #endregion
}
