using PlusUi.core.Attributes;
using System.Linq.Expressions;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// Detects long press gestures on the wrapped content.
/// </summary>
[GenerateShadowMethods]
public partial class LongPressGestureDetector(UiElement content) : GestureDetector<LongPressGestureDetector>(content), ILongPressGestureControl
{

    #region Command
    public ICommand? LongPressCommand { get; private set; }

    public LongPressGestureDetector SetCommand(ICommand command)
    {
        LongPressCommand = command;
        return this;
    }

    public LongPressGestureDetector BindCommand(Expression<Func<ICommand>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => LongPressCommand = getter());
        return this;
    }

    public void OnLongPress()
    {
        if (LongPressCommand?.CanExecute(null) == true)
        {
            LongPressCommand.Execute(null);
        }
    }
    #endregion
}
