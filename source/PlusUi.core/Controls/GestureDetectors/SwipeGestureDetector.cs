using PlusUi.core.Attributes;
using System.Linq.Expressions;
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

    public SwipeGestureDetector BindCommand(Expression<Func<ICommand>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SwipeCommand = getter());
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

    public SwipeGestureDetector BindAllowedDirections(Expression<Func<SwipeDirection>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => AllowedDirections = getter());
        return this;
    }
    #endregion
}
