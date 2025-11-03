using PlusUi.core.Attributes;

namespace PlusUi.core;

/// <summary>
/// A horizontal stack layout that arranges child elements from left to right.
/// </summary>
/// <remarks>
/// Elements are positioned horizontally with optional spacing. The stack respects each
/// child's horizontal alignment and automatically calculates the required size.
/// </remarks>
/// <example>
/// <code>
/// // Simple horizontal layout
/// new HStack(
///     new Label().SetText("Name:"),
///     new Entry().SetPlaceholder("Enter name")
/// ).SetSpacing(8);
///
/// // Aligned buttons
/// new HStack(
///     new Button().SetText("Cancel"),
///     new Button().SetText("OK")
/// )
/// .SetSpacing(12)
/// .SetHorizontalAlignment(HorizontalAlignment.Right);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class HStack : UiLayoutElement
{
    public HStack(params UiElement[] elements)
    {
        foreach (var element in elements)
        {
            element.Parent = this;
        }
        Children.AddRange(elements);
    }

    #region measure/arrange
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        Children.ForEach(c => c.Measure(availableSize, dontStretch));
        var width = Children.Sum(c => c.ElementSize.Width + c.Margin.Left + c.Margin.Right);
        var height = Children.Max(c => c.ElementSize.Height + c.Margin.Top + c.Margin.Bottom);
        return new Size(width, height);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var positionX = HorizontalAlignment switch
        {
            HorizontalAlignment.Center => bounds.Left + ((bounds.Width - ElementSize.Width) / 2),
            HorizontalAlignment.Right => bounds.Right - ElementSize.Width - Margin.Right,
            _ => bounds.Left + Margin.Left,
        };
        var positionY = VerticalAlignment switch
        {
            VerticalAlignment.Center => bounds.Top + ((bounds.Height - ElementSize.Height) / 2),
            VerticalAlignment.Bottom => bounds.Bottom - ElementSize.Height - Margin.Bottom,
            _ => bounds.Top + Margin.Top,
        };
        var y = positionY;
        var x = positionX;
        foreach (var child in Children)
        {
            var childTopBound = child.VerticalAlignment switch
            {
                VerticalAlignment.Center => y + ((ElementSize.Height - child.ElementSize.Height) / 2),
                VerticalAlignment.Bottom => y + ElementSize.Height - child.ElementSize.Height,
                _ => y,
            };
            child.Arrange(new Rect(x, childTopBound, child.ElementSize.Width, child.ElementSize.Height));
            x += child.ElementSize.Width + child.Margin.Left + child.Margin.Right;
        }
        return new Point(positionX, positionY);
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
