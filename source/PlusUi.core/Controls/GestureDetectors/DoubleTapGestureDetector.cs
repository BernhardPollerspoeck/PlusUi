using PlusUi.core.Attributes;
using System.Windows.Input;

namespace PlusUi.core;

/// <summary>
/// Detects double tap gestures on the wrapped content.
/// </summary>
[GenerateShadowMethods]
public partial class DoubleTapGestureDetector : GestureDetector<DoubleTapGestureDetector>, IDoubleTapGestureControl
{
    public DoubleTapGestureDetector(UiElement content) : base(content) { }

    #region Command
    public ICommand? DoubleTapCommand { get; private set; }

    public DoubleTapGestureDetector SetCommand(ICommand command)
    {
        DoubleTapCommand = command;
        return this;
    }

    public DoubleTapGestureDetector BindCommand(string propertyName, Func<ICommand> propertyGetter)
    {
        RegisterBinding(propertyName, () => DoubleTapCommand = propertyGetter());
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
