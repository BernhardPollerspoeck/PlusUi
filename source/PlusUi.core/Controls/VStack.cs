using PlusUi.core.CoreElements;
using PlusUi.core.Enumerations;
using PlusUi.core.Structures;

namespace PlusUi.core.Controls;

public class VStack : UiLayoutElement<VStack>
{
    #region Spacing
    public float Spacing
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }
    public VStack SetSpacing(float spacing)
    {
        Spacing = spacing;
        return this;
    }
    public VStack BindSpacing(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => Spacing = propertyGetter());
        return this;
    }
    #endregion

    public VStack(params UiElement[] elements)
    {
        foreach (var element in elements)
        {
            element.Parent = this;
        }
        Children.AddRange(elements);
    }

    #region measure/arrange
    protected override Size MeasureInternal(Size availableSize)
    {
        Children.ForEach(c => c.Measure(availableSize));

        var width = HorizontalAlignment switch
        {
            HorizontalAlignment.Stretch => availableSize.Width,
            _ => Children.Max(c => c.ElementSize.Width + c.Margin.Left + c.Margin.Right),
        };
        var height = VerticalAlignment switch
        {
            VerticalAlignment.Stretch => availableSize.Height,
            _ => Children.Sum(c => c.ElementSize.Height) + (Spacing * (Children.Count - 1)),
        };
        return new Size(width, height);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        //TODO: rework!!!
        //arrange children
        var y = bounds.Top + Margin.Top;
        var x = bounds.Left + Margin.Left;
        foreach (var child in Children)
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


    public override UiElement? HitTest(Point point)
    {
        foreach (var child in Children)
        {
            var result = child.HitTest(point);
            if (result is not null)
            {
                return result;
            }
        }
        return base.HitTest(point);
    }


}
