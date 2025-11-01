using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Defines the stroke pattern types available for Border control.
/// </summary>
public enum StrokeType
{
    /// <summary>Solid continuous stroke</summary>
    Solid,
    /// <summary>Dashed stroke pattern</summary>
    Dashed,
    /// <summary>Dotted stroke pattern</summary>
    Dotted
}

/// <summary>
/// Border control that provides background color, stroke color, stroke thickness, and stroke type.
/// Inherits from UiLayoutElement to support a single child element with proper border spacing.
/// </summary>
/// <example>
/// Basic usage:
/// <code>
/// new Border()
///     .SetStrokeColor(SKColors.Red)
///     .SetStrokeThickness(2f)
///     .SetStrokeType(StrokeType.Dashed)
///     .SetBackgroundColor(SKColors.LightBlue)
///     .AddChild(new Label().SetText("Content"));
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class Border : UiLayoutElement
{
    #region StrokeColor
    internal SKColor StrokeColor
    {
        get => field;
        set
        {
            field = value;
            StrokePaint = CreateStrokePaint();
        }
    } = SKColors.Black;

    public Border SetStrokeColor(SKColor color)
    {
        StrokeColor = color;
        return this;
    }
    public Border BindStrokeColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => StrokeColor = propertyGetter());
        return this;
    }
    #endregion

    #region StrokeThickness
    internal float StrokeThickness
    {
        get => field;
        set
        {
            field = value;
            StrokePaint = CreateStrokePaint();
        }
    } = 1f;

    public Border SetStrokeThickness(float thickness)
    {
        StrokeThickness = Math.Max(0, thickness); // Ensure non-negative
        return this;
    }
    public Border BindStrokeThickness(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => StrokeThickness = propertyGetter());
        return this;
    }
    #endregion

    #region StrokeType
    internal StrokeType StrokeType
    {
        get => field;
        set
        {
            field = value;
            StrokePaint = CreateStrokePaint();
        }
    } = StrokeType.Solid;

    public Border SetStrokeType(StrokeType strokeType)
    {
        StrokeType = strokeType;
        return this;
    }
    public Border BindStrokeType(string propertyName, Func<StrokeType> propertyGetter)
    {
        RegisterBinding(propertyName, () => StrokeType = propertyGetter());
        return this;
    }
    #endregion

    protected SKPaint StrokePaint { get; set; } = null!;

    public Border()
    {
        StrokePaint = CreateStrokePaint();
    }

    private SKPaint CreateStrokePaint()
    {
        var paint = new SKPaint
        {
            Color = StrokeColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = StrokeThickness,
            IsAntialias = true
        };

        // Apply stroke pattern based on StrokeType
        switch (StrokeType)
        {
            case StrokeType.Dashed:
                paint.PathEffect = SKPathEffect.CreateDash([StrokeThickness * 4, StrokeThickness * 2], 0);
                break;
            case StrokeType.Dotted:
                paint.PathEffect = SKPathEffect.CreateDash([StrokeThickness, StrokeThickness], 0);
                break;
            case StrokeType.Solid:
            default:
                paint.PathEffect = null;
                break;
        }

        return paint;
    }

    public override void Render(SKCanvas canvas)
    {
        // Let base class render the background
        base.Render(canvas);

        // Draw stroke border
        if (StrokeThickness > 0 && StrokeColor != SKColors.Transparent)
        {
            var strokeRect = new SKRect(
                Position.X + StrokeThickness / 2,
                Position.Y + StrokeThickness / 2,
                Position.X + ElementSize.Width - StrokeThickness / 2,
                Position.Y + ElementSize.Height - StrokeThickness / 2);

            if (CornerRadius > 0)
            {
                canvas.DrawRoundRect(strokeRect, CornerRadius, CornerRadius, StrokePaint);
            }
            else
            {
                canvas.DrawRect(strokeRect, StrokePaint);
            }
        }

        // Render children
        foreach (var child in Children)
        {
            child.Render(canvas);
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var borderThickness = StrokeThickness * 2; // Both sides
        var availableChildSize = new Size(
            Math.Max(0, availableSize.Width - borderThickness),
            Math.Max(0, availableSize.Height - borderThickness));

        Size childSize = Size.Empty;

        // Measure the single child if it exists
        if (Children.Count > 0)
        {
            var child = Children[0];
            child.Measure(availableChildSize);
            childSize = child.ElementSize + child.Margin;
        }

        // Return the child size plus border thickness
        return new Size(
            childSize.Width + borderThickness,
            childSize.Height + borderThickness);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var position = base.ArrangeInternal(bounds);
        
        // Arrange the single child with border spacing
        if (Children.Count > 0)
        {
            var child = Children[0];
            var childBounds = new Rect(
                position.X + StrokeThickness,
                position.Y + StrokeThickness,
                ElementSize.Width - StrokeThickness * 2,
                ElementSize.Height - StrokeThickness * 2);
            
            child.Arrange(childBounds);
        }

        return position;
    }

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