using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A scrollable container that allows scrolling through content larger than the visible area.
/// Supports both horizontal and vertical scrolling with mouse wheel and drag gestures.
/// </summary>
/// <example>
/// <code>
/// // Vertical scrolling
/// new ScrollView(
///     new VStack(
///         new Label().SetText("Item 1"),
///         new Label().SetText("Item 2"),
///         // ... many more items
///     )
/// )
/// .SetCanScrollHorizontally(false)
/// .SetCanScrollVertically(true);
///
/// // Both directions
/// new ScrollView(largeContent)
///     .SetCanScrollHorizontally(true)
///     .SetCanScrollVertically(true);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class ScrollView : UiLayoutElement, IScrollableControl
{
    private readonly UiElement _content;

    #region CanScrollHorizontally
    internal bool CanScrollHorizontally
    {
        get => field;
        set => field = value;
    } = true;

    public ScrollView SetCanScrollHorizontally(bool canScroll)
    {
        CanScrollHorizontally = canScroll;
        return this;
    }

    public ScrollView BindCanScrollHorizontally(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => CanScrollHorizontally = propertyGetter());
        return this;
    }
    #endregion

    #region CanScrollVertically
    internal bool CanScrollVertically
    {
        get => field;
        set => field = value;
    } = true;

    public ScrollView SetCanScrollVertically(bool canScroll)
    {
        CanScrollVertically = canScroll;
        return this;
    }

    public ScrollView BindCanScrollVertically(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => CanScrollVertically = propertyGetter());
        return this;
    }
    #endregion

    #region ScrollFactor
    internal float ScrollFactor
    {
        get => field;
        set => field = value;
    } = 1.0f;

    public ScrollView SetScrollFactor(float factor)
    {
        ScrollFactor = factor;
        return this;
    }

    public ScrollView BindScrollFactor(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => ScrollFactor = propertyGetter());
        return this;
    }
    #endregion

    #region HorizontalOffset
    internal float HorizontalOffset
    {
        get => field;
        set
        {
            var maxOffset = Math.Max(0, _content.ElementSize.Width - ElementSize.Width);
            field = Math.Clamp(value, 0, maxOffset);
            InvalidateMeasure();
        }
    }

    public ScrollView SetHorizontalOffset(float offset)
    {
        HorizontalOffset = offset;
        return this;
    }

    public ScrollView BindHorizontalOffset(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => HorizontalOffset = propertyGetter());
        return this;
    }
    #endregion

    #region VerticalOffset
    internal float VerticalOffset
    {
        get => field;
        set
        {
            var maxOffset = Math.Max(0, _content.ElementSize.Height - ElementSize.Height);
            field = Math.Clamp(value, 0, maxOffset);
            InvalidateMeasure();
        }
    }

    public ScrollView SetVerticalOffset(float offset)
    {
        VerticalOffset = offset;
        return this;
    }

    public ScrollView BindVerticalOffset(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => VerticalOffset = propertyGetter());
        return this;
    }
    #endregion

    public ScrollView(UiElement content)
    {
        _content = content;
        _content.Parent = this;
        Children.Add(_content);
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // Measure content with appropriate constraints for scrollable/non-scrollable directions
        // For scrollable directions, we force dontStretch=true to get natural size instead of stretched size
        _content.Measure(new Size(
            CanScrollHorizontally ? float.MaxValue : availableSize.Width,
            CanScrollVertically ? float.MaxValue : availableSize.Height),
            // Force dontStretch=true for scrollable directions to get natural size
            CanScrollHorizontally || CanScrollVertically || dontStretch);

        return availableSize;
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

        // Arrange content with offset for scrolling
        _content.Arrange(new Rect(
            positionX - HorizontalOffset,
            positionY - VerticalOffset,
            _content.ElementSize.Width,
            _content.ElementSize.Height));

        return new(positionX, positionY);
    }

    public override void Render(SKCanvas canvas)
    {
        if (IsVisible)
        {
            // Save canvas state and apply clipping
            canvas.Save();
            var rect = new SKRect(
                Position.X + VisualOffset.X,
                Position.Y + VisualOffset.Y,
                Position.X + VisualOffset.X + ElementSize.Width,
                Position.Y + VisualOffset.Y + ElementSize.Height);

            // Apply clipping with corner radius if needed
            if (CornerRadius > 0)
            {
                canvas.ClipRoundRect(new SKRoundRect(rect, CornerRadius, CornerRadius));
            }
            else
            {
                canvas.ClipRect(rect);
            }
        }

        // Call base to render background (now clipped)
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        // Store the original visual offset of the content
        var originalContentOffset = _content.VisualOffset;

        // For ScrollView, we need to apply our VisualOffset but not include the scroll offsets
        // in the child's VisualOffset since they're already applied via positioning
        _content.SetVisualOffset(new Point(
            originalContentOffset.X + VisualOffset.X,
            originalContentOffset.Y + VisualOffset.Y
        ));

        // Render content (already clipped)
        _content.Render(canvas);

        // Restore original visual offset
        _content.SetVisualOffset(originalContentOffset);

        // Restore canvas state
        canvas.Restore();
    }

    public override UiElement? HitTest(Point point)
    {
        // First check if the point is within our bounds
        if (!(point.X >= Position.X && point.X <= Position.X + ElementSize.Width &&
              point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height))
        {
            return null;
        }

        // Check if any child was hit
        // No need to adjust the point - the child's Position was already adjusted during Arrange
        var childHit = _content.HitTest(point);

        // If no child hit, return this ScrollView
        if (childHit == null)
        {
            return this;
        }

        // Return interactive controls (that take input) directly
        // These are controls that implement any of the input interfaces
        if (childHit is IInteractiveControl)
        {
            return childHit;
        }

        // For non-interactive controls (like Labels, Images) or layout controls (Grid, Stack),
        // return this ScrollView to handle scrolling
        return this;
    }

    #region IScrollableControl Implementation
    public bool IsScrolling { get; set; }

    public void HandleScroll(float deltaX, float deltaY)
    {
        if (CanScrollHorizontally)
        {
            HorizontalOffset += deltaX * ScrollFactor;
        }

        if (CanScrollVertically)
        {
            VerticalOffset += deltaY * ScrollFactor;
        }
    }
    #endregion
}