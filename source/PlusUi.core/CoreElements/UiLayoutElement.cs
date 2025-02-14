using PlusUi.core.Enumerations;
using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.CoreElements;

public abstract class UiLayoutElement<T> : UiLayoutElement where T : UiLayoutElement<T>
{
    public new T AddChild(UiElement child)
    {
        child.Parent = this;
        _children.Add(child);
        return (T)this;
    }
    public new T RemoveChild(UiElement child)
    {
        _children.Remove(child);
        return (T)this;
    }
    public new T ClearChildren()
    {
        _children.Clear();
        return (T)this;
    }

}

public abstract class UiLayoutElement : UiElement
{
    protected readonly List<UiElement> _children = [];

    #region children
    public UiElement AddChild(UiElement child)
    {
        child.Parent = this;
        _children.Add(child);
        return this;
    }
    public UiElement RemoveChild(UiElement child)
    {
        _children.Remove(child);
        return this;
    }
    public UiElement ClearChildren()
    {
        _children.Clear();
        return this;
    }
    #endregion

    #region rendering
    public override void Render(SKCanvas canvas)
    {
        foreach (var child in _children)
        {
            child.Render(canvas);
        }
    }
    #endregion

    #region bindings
    protected override void UpdateBindingsInternal(string propertyName)
    {
        foreach (var child in _children)
        {
            child.UpdateBindings(propertyName);
        }
    }
    #endregion

    #region measure/arrange
    protected override Size MeasureInternal(Size availableSize)
    {
        var width = 0f;
        var height = 0f;
        foreach (var child in _children)
        {
            var size = child.Measure(availableSize);
            width = Math.Max(width, size.Width);
            height += size.Height + child.Margin.Top + child.Margin.Bottom;
        }

        if (HorizontalAlignment == HorizontalAlignment.Stretch)
        {
            width = availableSize.Width;
        }

        return new Size(width, height);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        //arrange children
        var y = bounds.Top + Margin.Top;
        var x = bounds.Left + Margin.Left;
        foreach (var child in _children)
        {
            var size = child.ElementSize;
            var childX = x;

            switch (child.HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    childX = x + (bounds.Width - size.Width) / 2;
                    break;
                case HorizontalAlignment.Right:
                    childX = x + (bounds.Width - size.Width) - child.Margin.Right;
                    break;
                case HorizontalAlignment.Stretch:
                    size = new Size(bounds.Width, size.Height);
                    break;
            }

            var childBounds = new Rect(childX, y, size.Width, size.Height);
            child.Arrange(childBounds);
            y += size.Height + child.Margin.Top + child.Margin.Bottom;
        }

        //return own final position
        return new Point(bounds.Left, bounds.Top);
    }
    #endregion
}
