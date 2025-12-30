using PlusUi.core.Attributes;

namespace PlusUi.core;

/// <summary>
/// A vertical stack layout that arranges child elements from top to bottom.
/// </summary>
/// <remarks>
/// Elements are positioned vertically with optional spacing. The stack respects each
/// child's vertical alignment and automatically calculates the required size.
/// When Wrap is enabled, elements wrap to the next column when they exceed the available height.
/// </remarks>
/// <example>
/// <code>
/// // Simple vertical layout
/// new VStack(
///     new Label().SetText("Title"),
///     new Label().SetText("Subtitle"),
///     new Button().SetText("Action")
/// );
///
/// // Wrapping layout (columns)
/// new VStack(item1, item2, item3, item4, item5)
///     .SetWrap(true);
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

    #region Wrap
    /// <summary>
    /// Gets whether elements wrap to the next column when they exceed the available height.
    /// </summary>
    internal bool Wrap { get; set; }

    /// <summary>
    /// Sets whether elements should wrap to the next column when they exceed the available height.
    /// </summary>
    public VStack SetWrap(bool wrap)
    {
        Wrap = wrap;
        ForceInvalidateMeasureToRoot();
        InvalidateArrangeChildren();
        return this;
    }

    /// <summary>
    /// Binds the wrap property to a data source.
    /// </summary>
    public VStack BindWrap(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => SetWrap(propertyGetter()));
        return this;
    }
    #endregion

    #region measure/arrange
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        if (Wrap)
        {
            return MeasureWrapped(availableSize, dontStretch);
        }

        var childAvailableSize = new Size(availableSize.Width, availableSize.Height);

        //first measure all not stretching children
        foreach (var child in Children.Where(c => c.VerticalAlignment is not VerticalAlignment.Stretch))
        {
            var result = child.Measure(childAvailableSize, dontStretch);
            childAvailableSize = new Size(
                childAvailableSize.Width,
                Math.Max(0, childAvailableSize.Height - (result.Height + child.Margin.Vertical)));
        }

        //split available space for stretching children
        var stretchingChildren = Children.Where(c => c.VerticalAlignment is VerticalAlignment.Stretch).ToList();
        var stretchHeight = stretchingChildren.Count > 0
            ? childAvailableSize.Height / stretchingChildren.Count
            : 0;
        stretchingChildren.ForEach(child => child.Measure(new Size(childAvailableSize.Width, stretchHeight), dontStretch));

        var width = Children.Count > 0
            ? Children.Max(c => c.ElementSize.Width + c.Margin.Left + c.Margin.Right)
            : 0;
        var height = Children.Sum(c => c.ElementSize.Height + c.Margin.Top + c.Margin.Bottom);
        return new Size(width, height);
    }

    private Size MeasureWrapped(Size availableSize, bool dontStretch)
    {
        // Measure all children first to get their natural sizes
        foreach (var child in Children)
        {
            child.Measure(availableSize, true);
        }

        // Calculate columns
        var columns = new List<List<UiElement>>();
        var currentColumn = new List<UiElement>();
        var currentColumnHeight = 0f;

        foreach (var child in Children)
        {
            var childHeight = child.ElementSize.Height + child.Margin.Vertical;

            if (currentColumn.Count > 0 && currentColumnHeight + childHeight > availableSize.Height)
            {
                // Start new column
                columns.Add(currentColumn);
                currentColumn = new List<UiElement> { child };
                currentColumnHeight = childHeight;
            }
            else
            {
                currentColumn.Add(child);
                currentColumnHeight += childHeight;
            }
        }

        if (currentColumn.Count > 0)
        {
            columns.Add(currentColumn);
        }

        // Calculate total size
        var totalWidth = 0f;
        var maxHeight = 0f;

        foreach (var column in columns)
        {
            var columnWidth = column.Count > 0
                ? column.Max(c => c.ElementSize.Width + c.Margin.Horizontal)
                : 0f;
            var columnHeight = column.Sum(c => c.ElementSize.Height + c.Margin.Vertical);

            totalWidth += columnWidth;
            maxHeight = Math.Max(maxHeight, columnHeight);
        }

        return new Size(totalWidth, Math.Min(maxHeight, availableSize.Height));
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

        if (Wrap)
        {
            ArrangeWrapped(positionX, positionY, bounds.Height);
            return new Point(positionX, positionY);
        }

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

    private void ArrangeWrapped(float startX, float startY, float availableHeight)
    {
        var x = startX;
        var y = startY;
        var columnWidth = 0f;

        foreach (var child in Children)
        {
            var childHeight = child.ElementSize.Height + child.Margin.Vertical;

            // Check if we need to wrap to next column
            if (y > startY && y - startY + childHeight > availableHeight)
            {
                x += columnWidth;
                y = startY;
                columnWidth = 0f;
            }

            // Don't add margins here - UiElement.Arrange handles them internally
            child.Arrange(new Rect(
                x,
                y,
                child.ElementSize.Width + child.Margin.Horizontal,
                child.ElementSize.Height + child.Margin.Vertical));

            y += childHeight;
            columnWidth = Math.Max(columnWidth, child.ElementSize.Width + child.Margin.Horizontal);
        }
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
