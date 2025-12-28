using System.Windows.Input;
using SkiaSharp;

namespace PlusUi.core;

public abstract class GestureDetector<T> : UiLayoutElement where T : GestureDetector<T>
{
    protected UiElement Content { get; }

    protected GestureDetector(UiElement content)
    {
        Content = content;
        content.Parent = this;
        Children.Add(content);
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        Content.Measure(availableSize, dontStretch);
        return Content.ElementSize;
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        Content.Arrange(bounds);
        return new Point(bounds.X, bounds.Y);
    }

    public override void Render(SKCanvas canvas)
    {
        Content.Render(canvas);
    }
}

public class LongPressGestureDetector : GestureDetector<LongPressGestureDetector>, ILongPressGestureControl
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

public class DoubleTapGestureDetector : GestureDetector<DoubleTapGestureDetector>, IDoubleTapGestureControl
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

public class SwipeGestureDetector : GestureDetector<SwipeGestureDetector>, ISwipeGestureControl
{
    public SwipeGestureDetector(UiElement content) : base(content) { }

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

public class PinchGestureDetector : GestureDetector<PinchGestureDetector>, IPinchGestureControl
{
    public PinchGestureDetector(UiElement content) : base(content) { }

    #region Command
    public ICommand? PinchCommand { get; private set; }

    public PinchGestureDetector SetCommand(ICommand command)
    {
        PinchCommand = command;
        return this;
    }

    public PinchGestureDetector BindCommand(string propertyName, Func<ICommand> propertyGetter)
    {
        RegisterBinding(propertyName, () => PinchCommand = propertyGetter());
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
