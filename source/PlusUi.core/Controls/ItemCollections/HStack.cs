using PlusUi.core.Attributes;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A horizontal stack layout that arranges child elements from left to right.
/// </summary>
/// <remarks>
/// Elements are positioned horizontally with optional spacing. The stack respects each
/// child's horizontal alignment and automatically calculates the required size.
/// When Wrap is enabled, elements wrap to the next row when they exceed the available width.
/// </remarks>
/// <example>
/// <code>
/// // Simple horizontal layout
/// new HStack(
///     new Label().SetText("Name:"),
///     new Entry().SetPlaceholder("Enter name")
/// );
///
/// // Wrapping layout (like a WrapPanel)
/// new HStack(tag1, tag2, tag3, tag4, tag5)
///     .SetWrap(true);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class HStack : UiLayoutElement
{
    public override string? GetComputedAccessibilityLabel()
    {
        if (AccessibilityLabel != null) return AccessibilityLabel;
        return Children.Count > 0 ? "Horizontal stack" : null;
    }

    public override string? GetComputedAccessibilityValue()
    {
        if (AccessibilityValue != null) return AccessibilityValue;
        return Children.Count > 0 ? $"{Children.Count} item{(Children.Count == 1 ? "" : "s")}" : null;
    }

    public HStack(params UiElement[] elements)
    {
        foreach (var element in elements)
        {
            element.Parent = this;
        }
        Children.AddRange(elements);
    }

    #region Wrap
    /// <summary>
    /// Gets whether elements wrap to the next row when they exceed the available width.
    /// </summary>
    internal bool Wrap { get; set; }

    /// <summary>
    /// Sets whether elements should wrap to the next row when they exceed the available width.
    /// </summary>
    public HStack SetWrap(bool wrap)
    {
        Wrap = wrap;
        ForceInvalidateMeasureToRoot();
        InvalidateArrangeChildren();
        return this;
    }

    /// <summary>
    /// Binds the wrap property to a data source.
    /// </summary>
    public HStack BindWrap(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SetWrap(getter()));
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
        foreach (var child in Children.Where(c => c.HorizontalAlignment is not HorizontalAlignment.Stretch))
        {
            var result = child.Measure(childAvailableSize, dontStretch);
            childAvailableSize = new Size(
                Math.Max(0, childAvailableSize.Width - (result.Width + child.Margin.Horizontal)),
                childAvailableSize.Height);
        }

        //split available space for stretching children
        var stretchingChildren = Children.Where(c => c.HorizontalAlignment is HorizontalAlignment.Stretch).ToList();
        var stretchWidth = stretchingChildren.Count > 0
            ? childAvailableSize.Width / stretchingChildren.Count
            : 0;
        stretchingChildren.ForEach(child => child.Measure(new Size(stretchWidth, childAvailableSize.Height), dontStretch));

        var width = Children.Sum(c => c.ElementSize.Width + c.Margin.Left + c.Margin.Right);
        var height = Children.Count > 0
            ? Children.Max(c => c.ElementSize.Height + c.Margin.Top + c.Margin.Bottom)
            : 0;
        return new Size(width, height);
    }

    private Size MeasureWrapped(Size availableSize, bool dontStretch)
    {
        // Measure all children first to get their natural sizes
        foreach (var child in Children)
        {
            child.Measure(availableSize, true);
        }

        // Calculate rows
        var rows = new List<List<UiElement>>();
        var currentRow = new List<UiElement>();
        var currentRowWidth = 0f;

        foreach (var child in Children)
        {
            var childWidth = child.ElementSize.Width + child.Margin.Horizontal;

            if (currentRow.Count > 0 && currentRowWidth + childWidth > availableSize.Width)
            {
                // Start new row
                rows.Add(currentRow);
                currentRow = new List<UiElement> { child };
                currentRowWidth = childWidth;
            }
            else
            {
                currentRow.Add(child);
                currentRowWidth += childWidth;
            }
        }

        if (currentRow.Count > 0)
        {
            rows.Add(currentRow);
        }

        // Calculate total size
        var totalHeight = 0f;
        var maxWidth = 0f;

        foreach (var row in rows)
        {
            var rowHeight = row.Count > 0
                ? row.Max(c => c.ElementSize.Height + c.Margin.Vertical)
                : 0f;
            var rowWidth = row.Sum(c => c.ElementSize.Width + c.Margin.Horizontal);

            totalHeight += rowHeight;
            maxWidth = Math.Max(maxWidth, rowWidth);
        }

        return new Size(Math.Min(maxWidth, availableSize.Width), totalHeight);
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
            ArrangeWrapped(positionX, positionY, bounds.Width);
            return new Point(positionX, positionY);
        }

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

    private void ArrangeWrapped(float startX, float startY, float availableWidth)
    {
        var x = startX;
        var y = startY;
        var rowHeight = 0f;

        foreach (var child in Children)
        {
            var childWidth = child.ElementSize.Width + child.Margin.Horizontal;

            // Check if we need to wrap to next row
            if (x > startX && x - startX + childWidth > availableWidth)
            {
                x = startX;
                y += rowHeight;
                rowHeight = 0f;
            }

            // Don't add margins here - UiElement.Arrange handles them internally
            child.Arrange(new Rect(
                x,
                y,
                child.ElementSize.Width + child.Margin.Horizontal,
                child.ElementSize.Height + child.Margin.Vertical));

            x += childWidth;
            rowHeight = Math.Max(rowHeight, child.ElementSize.Height + child.Margin.Vertical);
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
