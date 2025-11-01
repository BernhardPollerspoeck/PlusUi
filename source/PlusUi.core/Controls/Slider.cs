using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A slider control for selecting a value within a range.
/// Supports horizontal orientation with customizable min/max values.
/// </summary>
[GenerateShadowMethods]
public partial class Slider : UiElement, IDraggableControl
{
    public Slider()
    {
        SetDesiredHeight(30);
        HorizontalAlignment = HorizontalAlignment.Stretch;
    }

    #region Value
    internal float Value
    {
        get => field;
        set
        {
            var clampedValue = Math.Clamp(value, Minimum, Maximum);
            if (field != clampedValue)
            {
                field = clampedValue;
            }
        }
    } = 0f;

    public Slider SetValue(float value)
    {
        Value = value;
        return this;
    }

    public Slider BindValue(string propertyName, Func<float> propertyGetter, Action<float> propertySetter)
    {
        RegisterBinding(propertyName, () => Value = propertyGetter());
        RegisterSetter(nameof(Value), propertySetter);
        return this;
    }
    #endregion

    #region Minimum
    internal float Minimum
    {
        get => field;
        set
        {
            field = value;
            Value = Math.Max(Value, value); // Ensure current value is not below new minimum
        }
    } = 0f;

    public Slider SetMinimum(float minimum)
    {
        Minimum = minimum;
        return this;
    }

    public Slider BindMinimum(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => Minimum = propertyGetter());
        return this;
    }
    #endregion

    #region Maximum
    internal float Maximum
    {
        get => field;
        set
        {
            field = value;
            Value = Math.Min(Value, value); // Ensure current value is not above new maximum
        }
    } = 100f;

    public Slider SetMaximum(float maximum)
    {
        Maximum = maximum;
        return this;
    }

    public Slider BindMaximum(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => Maximum = propertyGetter());
        return this;
    }
    #endregion

    #region MinimumTrackColor
    internal SKColor MinimumTrackColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = new SKColor(0, 122, 255); // iOS blue

    public Slider SetMinimumTrackColor(SKColor color)
    {
        MinimumTrackColor = color;
        return this;
    }

    public Slider BindMinimumTrackColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => MinimumTrackColor = propertyGetter());
        return this;
    }
    #endregion

    #region MaximumTrackColor
    internal SKColor MaximumTrackColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = new SKColor(230, 230, 230);

    public Slider SetMaximumTrackColor(SKColor color)
    {
        MaximumTrackColor = color;
        return this;
    }

    public Slider BindMaximumTrackColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => MaximumTrackColor = propertyGetter());
        return this;
    }
    #endregion

    #region ThumbColor
    internal SKColor ThumbColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = SKColors.White;

    public Slider SetThumbColor(SKColor color)
    {
        ThumbColor = color;
        return this;
    }

    public Slider BindThumbColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => ThumbColor = propertyGetter());
        return this;
    }
    #endregion

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        var trackHeight = 4f;
        var thumbRadius = 14f;
        var centerY = Position.Y + VisualOffset.Y + (ElementSize.Height / 2);

        // Calculate thumb position based on value
        var normalizedValue = (Value - Minimum) / (Maximum - Minimum);
        var thumbX = Position.X + VisualOffset.X + (normalizedValue * (ElementSize.Width - thumbRadius * 2)) + thumbRadius;

        // Draw maximum track (background)
        var maxTrackRect = new SKRect(
            Position.X + VisualOffset.X + thumbRadius,
            centerY - (trackHeight / 2),
            Position.X + VisualOffset.X + ElementSize.Width - thumbRadius,
            centerY + (trackHeight / 2));

        using var maxTrackPaint = new SKPaint
        {
            Color = MaximumTrackColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRoundRect(maxTrackRect, trackHeight / 2, trackHeight / 2, maxTrackPaint);

        // Draw minimum track (filled portion)
        if (normalizedValue > 0)
        {
            var minTrackRect = new SKRect(
                Position.X + VisualOffset.X + thumbRadius,
                centerY - (trackHeight / 2),
                thumbX,
                centerY + (trackHeight / 2));

            using var minTrackPaint = new SKPaint
            {
                Color = MinimumTrackColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            canvas.DrawRoundRect(minTrackRect, trackHeight / 2, trackHeight / 2, minTrackPaint);
        }

        // Draw thumb shadow
        using var thumbShadowPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 40),
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 2)
        };
        canvas.DrawCircle(thumbX, centerY + 1, thumbRadius, thumbShadowPaint);

        // Draw thumb
        using var thumbPaint = new SKPaint
        {
            Color = ThumbColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawCircle(thumbX, centerY, thumbRadius, thumbPaint);

        // Draw thumb border
        using var thumbBorderPaint = new SKPaint
        {
            Color = new SKColor(200, 200, 200),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = true
        };
        canvas.DrawCircle(thumbX, centerY, thumbRadius - 1, thumbBorderPaint);
    }

    public override UiElement? HitTest(Point point)
    {
        if (!IsVisible)
        {
            return null;
        }

        var thumbRadius = 14f;
        var centerY = Position.Y + VisualOffset.Y + (ElementSize.Height / 2);

        // Calculate thumb position based on value
        var normalizedValue = (Value - Minimum) / (Maximum - Minimum);
        var thumbX = Position.X + VisualOffset.X + (normalizedValue * (ElementSize.Width - thumbRadius * 2)) + thumbRadius;

        // Check if point is within thumb circle
        var deltaX = point.X - thumbX;
        var deltaY = point.Y - centerY;
        var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

        // Only return this control if clicking on the thumb
        if (distance <= thumbRadius)
        {
            return this;
        }

        return null;
    }

    #region IDraggableControl Implementation
    public bool IsDragging { get; set; }

    public void HandleDrag(float deltaX, float deltaY)
    {
        // Calculate how much the value should change based on pixel movement
        var pixelRange = ElementSize.Width - 28; // Account for thumb size
        var valueRange = Maximum - Minimum;
        var valuePerPixel = valueRange / pixelRange;

        // deltaX is positive when dragging right, negative when dragging left
        var valueDelta = deltaX * valuePerPixel;

        Value = Math.Clamp(Value + valueDelta, Minimum, Maximum);

        // Notify setters
        if (_setter.TryGetValue(nameof(Value), out var setters))
        {
            foreach (var setter in setters)
            {
                setter(Value);
            }
        }
    }
    #endregion
}
