using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A visual separator line that can be horizontal or vertical.
/// Used to create visual divisions between UI sections.
/// </summary>
[GenerateShadowMethods]
public partial class Separator : UiElement
{
    #region Color
    internal SKColor Color
    {
        get => field;
        set
        {
            field = value;
            LinePaint = CreateLinePaint();
        }
    } = new SKColor(200, 200, 200);

    public Separator SetColor(SKColor color)
    {
        Color = color;
        return this;
    }

    public Separator BindColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => Color = propertyGetter());
        return this;
    }
    #endregion

    #region Thickness
    internal float Thickness
    {
        get => field;
        set
        {
            field = value;
            LinePaint = CreateLinePaint();
            InvalidateMeasure();
        }
    } = 1f;

    public Separator SetThickness(float thickness)
    {
        Thickness = Math.Max(0, thickness);
        return this;
    }

    public Separator BindThickness(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => Thickness = propertyGetter());
        return this;
    }
    #endregion

    #region Orientation
    internal Orientation Orientation
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = Orientation.Horizontal;

    public Separator SetOrientation(Orientation orientation)
    {
        Orientation = orientation;
        return this;
    }

    public Separator BindOrientation(string propertyName, Func<Orientation> propertyGetter)
    {
        RegisterBinding(propertyName, () => Orientation = propertyGetter());
        return this;
    }
    #endregion

    protected SKPaint LinePaint { get; set; } = null!;

    public Separator()
    {
        LinePaint = CreateLinePaint();

        // Set default alignments for separator
        if (Orientation == Orientation.Horizontal)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }
        else
        {
            VerticalAlignment = VerticalAlignment.Stretch;
        }
    }

    private SKPaint CreateLinePaint()
    {
        return new SKPaint
        {
            Color = Color,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = Thickness,
            IsAntialias = true
        };
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        if (Orientation == Orientation.Horizontal)
        {
            return new Size(
                Math.Min(availableSize.Width, availableSize.Width),
                Math.Min(Thickness, availableSize.Height));
        }
        else
        {
            return new Size(
                Math.Min(Thickness, availableSize.Width),
                Math.Min(availableSize.Height, availableSize.Height));
        }
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        if (Orientation == Orientation.Horizontal)
        {
            // Draw horizontal line
            var y = Position.Y + VisualOffset.Y + (Thickness / 2);
            canvas.DrawLine(
                Position.X + VisualOffset.X,
                y,
                Position.X + VisualOffset.X + ElementSize.Width,
                y,
                LinePaint);
        }
        else
        {
            // Draw vertical line
            var x = Position.X + VisualOffset.X + (Thickness / 2);
            canvas.DrawLine(
                x,
                Position.Y + VisualOffset.Y,
                x,
                Position.Y + VisualOffset.Y + ElementSize.Height,
                LinePaint);
        }
    }
}
