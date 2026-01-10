using PlusUi.core.Attributes;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

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
    internal Color StrokeColor
    {
        get => field;
        set
        {
            field = value;
            StrokePaint = CreateStrokePaint();
        }
    } = Colors.Black;

    public Border SetStrokeColor(Color color)
    {
        StrokeColor = color;
        return this;
    }
    public Border BindStrokeColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => StrokeColor = getter());
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
    public Border BindStrokeThickness(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => StrokeThickness = getter());
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
    public Border BindStrokeType(Expression<Func<StrokeType>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => StrokeType = getter());
        return this;
    }
    #endregion

    #region UiElement
    internal override HorizontalAlignment HorizontalAlignment
    {
        get => field is HorizontalAlignment.Undefined && this.Children.FirstOrDefault() is not { DesiredSize.Width: > 0 }
            ? this.Children.FirstOrDefault()?.HorizontalAlignment ?? field
            : field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }

    internal override VerticalAlignment VerticalAlignment
    {
        get => field is VerticalAlignment.Undefined && this.Children.FirstOrDefault() is not { DesiredSize.Height: > 0 }
            ? this.Children.FirstOrDefault()?.VerticalAlignment ?? field
            : field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }

    internal override Size? DesiredSize
    {
        get
        {
            // Don't inherit child's DesiredSize when Border has Stretch alignment
            if (field is not null)
                return field;
            if (HorizontalAlignment == HorizontalAlignment.Stretch || VerticalAlignment == VerticalAlignment.Stretch)
                return null;
            return this.Children.FirstOrDefault()?.DesiredSize;
        }
        set
        {
            field = value;
            InvalidateMeasure();
        }
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
        base.Render(canvas);

        // Draw stroke border
        if (StrokeThickness > 0 && StrokeColor != SKColors.Transparent)
        {
            var strokeRect = new SKRect(
                Position.X + VisualOffset.X + StrokeThickness / 2,
                Position.Y + VisualOffset.Y + StrokeThickness / 2,
                Position.X + VisualOffset.X + ElementSize.Width - StrokeThickness / 2,
                Position.Y + VisualOffset.Y + ElementSize.Height - StrokeThickness / 2);

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
        // Available space for child = availableSize - Margin - StrokeThickness
        // StrokeThickness is INSIDE the border (reduces content area)
        var availableChildSize = new Size(
            Math.Max(0, availableSize.Width - Margin.Horizontal - StrokeThickness * 2),
            Math.Max(0, availableSize.Height - Margin.Vertical - StrokeThickness * 2));

        Size childSize = Size.Empty;

        // Measure the single child if it exists
        if (Children.Count > 0)
        {
            var child = Children[0];
            child.Measure(availableChildSize, dontStretch);
            childSize = child.ElementSize + child.Margin;
        }

        // Return child size plus stroke thickness (border is inside, adds to total size)
        return new Size(
            childSize.Width + StrokeThickness * 2,
            childSize.Height + StrokeThickness * 2);
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