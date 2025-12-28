using PlusUi.core.Attributes;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// Detects long press gestures on the wrapped content.
/// </summary>
[GenerateShadowMethods]
public partial class LongPressGestureDetector : GestureDetector<LongPressGestureDetector>, ILongPressGestureControl
{
    public LongPressGestureDetector(UiElement content) : base(content) { }

    #region Command
    public ICommand? LongPressCommand { get; private set; }

    public LongPressGestureDetector SetCommand(ICommand command)
    {
        LongPressCommand = command;
        return this;
    }

    public LongPressGestureDetector BindCommand(string propertyName, Func<ICommand> propertyGetter)
    {
        RegisterBinding(propertyName, () => LongPressCommand = propertyGetter());
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
