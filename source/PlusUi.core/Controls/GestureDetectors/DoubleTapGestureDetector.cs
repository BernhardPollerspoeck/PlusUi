using PlusUi.core.Attributes;
using System.Linq.Expressions;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// Detects double tap gestures on the wrapped content.
/// </summary>
[GenerateShadowMethods]
public partial class DoubleTapGestureDetector(UiElement content) : GestureDetector<DoubleTapGestureDetector>(content), IDoubleTapGestureControl
{

    #region Command
    public ICommand? DoubleTapCommand { get; private set; }

    public DoubleTapGestureDetector SetCommand(ICommand command)
    {
        DoubleTapCommand = command;
        return this;
    }

    public DoubleTapGestureDetector BindCommand(Expression<Func<ICommand>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DoubleTapCommand = getter());
        return this;
    }

    public void OnDoubleTap()
    {
        if (DoubleTapCommand?.CanExecute(null) == true)
        {
            DoubleTapCommand.Execute(null);
        }
    }
    #endregion
}
