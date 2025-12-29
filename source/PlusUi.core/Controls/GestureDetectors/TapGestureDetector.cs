using PlusUi.core.Attributes;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// Detects tap/click gestures on the wrapped content.
/// </summary>
[GenerateShadowMethods]
public partial class TapGestureDetector(UiElement content) : GestureDetector<TapGestureDetector>(content), IInputControl
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Button;

    #region Command
    internal ICommand? Command { get; private set; }
    internal object? CommandParameter { get; private set; }

    public TapGestureDetector SetCommand(ICommand command)
    {
        Command = command;
        return this;
    }

    public TapGestureDetector BindCommand(string propertyName, Func<ICommand> propertyGetter)
    {
        RegisterBinding(propertyName, () => Command = propertyGetter());
        return this;
    }

    public TapGestureDetector SetCommandParameter(object parameter)
    {
        CommandParameter = parameter;
        return this;
    }

    public TapGestureDetector BindCommandParameter(string propertyName, Func<object> propertyGetter)
    {
        RegisterBinding(propertyName, () => CommandParameter = propertyGetter());
        return this;
    }

    public void InvokeCommand()
    {
        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }
    #endregion

    public override UiElement? HitTest(Point point)
    {
        // Check if point is within the content bounds
        if (Content.HitTest(point) != null)
        {
            return this; // Return self to receive tap events
        }
        return null;
    }
}
