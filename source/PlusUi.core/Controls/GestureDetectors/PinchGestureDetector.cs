using PlusUi.core.Attributes;
using System.Linq.Expressions;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// Detects pinch gestures on the wrapped content.
/// Provides scale factor to the command.
/// </summary>
[GenerateShadowMethods]
public partial class PinchGestureDetector(UiElement content) : GestureDetector<PinchGestureDetector>(content), IPinchGestureControl
{

    #region Command
    public ICommand? PinchCommand { get; private set; }

    public PinchGestureDetector SetCommand(ICommand command)
    {
        PinchCommand = command;
        return this;
    }

    public PinchGestureDetector BindCommand(Expression<Func<ICommand>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => PinchCommand = getter());
        return this;
    }

    public void OnPinch(float scale)
    {
        if (PinchCommand?.CanExecute(scale) == true)
        {
            PinchCommand.Execute(scale);
        }
    }
    #endregion
}
