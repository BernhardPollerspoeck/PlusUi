using PlusUi.core.Attributes;
using System.Linq.Expressions;

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
    public override string? GetComputedAccessibilityLabel()
    {
        if (AccessibilityLabel != null) return AccessibilityLabel;
        return Children.Count > 0 ? "Vertical stack" : null;
    }

    public override string? GetComputedAccessibilityValue()
    {
        if (AccessibilityValue != null) return AccessibilityValue;
        return Children.Count > 0 ? $"{Children.Count} item{(Children.Count == 1 ? "" : "s")}" : null;
    }

    public VStack(params UiElement[] elements)
    {
        Spacing = PlusUiDefaults.StackSpacing;
        Wrap = PlusUiDefaults.StackWrap;

        foreach (var element in elements)
        {
            element.Parent = this;
        }
        Children.AddRange(elements);
    }

    #region Spacing
    /// <summary>
    /// Gets or sets the spacing between child elements.
    /// </summary>
    internal float Spacing
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Sets the spacing between child elements.
    /// </summary>
    public VStack SetSpacing(float spacing)
    {
        Spacing = spacing;
        return this;
    }

    /// <summary>
    /// Binds the spacing property to a data source.
    /// </summary>
    public VStack BindSpacing(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Spacing = getter());
        return this;
    }
    #endregion

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
    public VStack BindWrap(Expression<Func<bool>> propertyExpression)
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

        var childAvailableSize = new Size(
            Math.Max(0, availableSize.Width - Margin.Horizontal),
            Math.Max(0, availableSize.Height - Margin.Vertical));

        // First measure all non-stretching children
        foreach (var child in Children.Where(c => c.VerticalAlignment is not VerticalAlignment.Stretch).ToList())
        {
            var result = child.Measure(childAvailableSize, dontStretch);
            childAvailableSize = new Size(
                childAvailableSize.Width,
                Math.Max(0, childAvailableSize.Height - (result.Height + child.Margin.Vertical)));
        }

        // Measure stretching children - they share the remaining space
        var stretchingChildren = Children.Where(c => c.VerticalAlignment is VerticalAlignment.Stretch).ToList();
        if (stretchingChildren.Count > 0)
        {
            var stretchHeight = childAvailableSize.Height / stretchingChildren.Count;
            stretchingChildren.ForEach(child => child.Measure(new Size(childAvailableSize.Width, stretchHeight), false));
        }

        var width = Children.Count > 0
            ? Children.Max(c => c.ElementSize.Width + c.Margin.Left + c.Margin.Right)
            : 0;
        var height = Children.Sum(c => c.ElementSize.Height + c.Margin.Top + c.Margin.Bottom);
        // Add spacing between children
        if (Children.Count > 1)
        {
            height += (Children.Count - 1) * Spacing;
        }

        // Respect DesiredSize if set
        if (DesiredSize?.Width > 0)
        {
            width = DesiredSize.Value.Width;
        }
        if (DesiredSize?.Height > 0)
        {
            height = DesiredSize.Value.Height;
        }

        return new Size(width, height);
    }

    private Size MeasureWrapped(Size availableSize, bool dontStretch)
    {
        // Measure all children first to get their natural sizes
        foreach (var child in Children.ToList())
        {
            child.Measure(availableSize, true);
        }

        // Calculate columns
        var columns = new List<List<UiElement>>();
        var currentColumn = new List<UiElement>();
        var currentColumnHeight = 0f;

        foreach (var child in Children.ToList())
        {
            var childHeight = child.ElementSize.Height + child.Margin.Vertical;
            var spacingToAdd = currentColumn.Count > 0 ? Spacing : 0;

            if (currentColumn.Count > 0 && currentColumnHeight + spacingToAdd + childHeight > availableSize.Height)
            {
                // Start new column
                columns.Add(currentColumn);
                currentColumn = [child];
                currentColumnHeight = childHeight;
            }
            else
            {
                currentColumn.Add(child);
                currentColumnHeight += spacingToAdd + childHeight;
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
            if (column.Count > 1)
            {
                columnHeight += (column.Count - 1) * Spacing;
            }

            totalWidth += columnWidth;
            maxHeight = Math.Max(maxHeight, columnHeight);
        }

        var width = totalWidth;
        var height = Math.Min(maxHeight, availableSize.Height);

        // Respect DesiredSize if set
        if (DesiredSize?.Width > 0)
        {
            width = DesiredSize.Value.Width;
        }
        if (DesiredSize?.Height > 0)
        {
            height = DesiredSize.Value.Height;
        }

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

        if (Wrap)
        {
            ArrangeWrapped(positionX, positionY, bounds.Height);
            return new Point(positionX, positionY);
        }

        var y = positionY;
        var x = positionX;
        var isFirst = true;

        foreach (var child in Children.ToList())
        {
            if (!isFirst)
            {
                y += Spacing;
            }
            isFirst = false;

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
        var isFirstInColumn = true;

        foreach (var child in Children.ToList())
        {
            var childHeight = child.ElementSize.Height + child.Margin.Vertical;
            var spacingToAdd = isFirstInColumn ? 0 : Spacing;

            // Check if we need to wrap to next column
            if (!isFirstInColumn && y - startY + spacingToAdd + childHeight > availableHeight)
            {
                x += columnWidth;
                y = startY;
                columnWidth = 0f;
                isFirstInColumn = true;
                spacingToAdd = 0;
            }

            y += spacingToAdd;

            // Don't add margins here - UiElement.Arrange handles them internally
            child.Arrange(new Rect(
                x,
                y,
                child.ElementSize.Width + child.Margin.Horizontal,
                child.ElementSize.Height + child.Margin.Vertical));

            y += childHeight;
            columnWidth = Math.Max(columnWidth, child.ElementSize.Width + child.Margin.Horizontal);
            isFirstInColumn = false;
        }
    }
    #endregion


    public override UiElement? HitTest(Point point)
    {
        foreach (var child in Children.ToList())
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
