using PlusUi.core.Attributes;

namespace PlusUi.core;

/// <summary>
/// A vertical stack layout that arranges child elements from top to bottom.
/// Inherits from <see cref="UiLayoutElement"/> and automatically manages child layout.
/// </summary>
/// <remarks>
/// Elements are positioned vertically with optional spacing. The stack respects each
/// child's vertical alignment and automatically calculates the required size.
/// </remarks>
/// <example>
/// <code>
/// // Simple vertical layout
/// new VStack(
///     new Label().SetText("Title"),
///     new Label().SetText("Subtitle").SetFontSize(12),
///     new Button().SetText("Action")
/// ).SetSpacing(8);
///
/// // Form layout
/// new VStack(
///     new Label().SetText("Username"),
///     new Entry().SetPlaceholder("Enter username"),
///     new Label().SetText("Password"),
///     new Entry().SetIsPassword(true)
/// )
/// .SetSpacing(4)
/// .SetPadding(new Margin(16));
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class VStack : UiLayoutElement
{
    public VStack(params UiElement[] elements)
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
        var width = Children.Max(c => c.ElementSize.Width + c.Margin.Left + c.Margin.Right);
        var height = Children.Sum(c => c.ElementSize.Height + c.Margin.Top + c.Margin.Bottom);
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
            var childLeftBound = child.HorizontalAlignment switch
            {
                HorizontalAlignment.Center => x + ((ElementSize.Width - child.ElementSize.Width) / 2),
                HorizontalAlignment.Right => x + ElementSize.Width - child.ElementSize.Width,
                _ => x,
            };
            child.Arrange(new Rect(
                childLeftBound,
                y,
                child.ElementSize.Width,
                child.ElementSize.Height + child.Margin.Top + child.Margin.Bottom));
            y += child.ElementSize.Height + child.Margin.Top + child.Margin.Bottom;
        }
        return new(positionX, positionY);
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
