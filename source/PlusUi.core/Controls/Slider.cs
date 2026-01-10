using PlusUi.core.Attributes;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A slider control for selecting a value within a range.
/// Supports horizontal orientation with customizable min/max values.
/// </summary>
[GenerateShadowMethods]
public partial class Slider : UiElement, IDraggableControl, IFocusable, IKeyboardInputHandler
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Slider;

    public Slider()
    {
        SetDesiredHeight(30);
        HorizontalAlignment = HorizontalAlignment.Stretch;
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? "Slider";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        return AccessibilityValue ?? $"{Value:F0} ({Minimum:F0} to {Maximum:F0})";
    }

    /// <inheritdoc />
    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = base.GetComputedAccessibilityTraits();
        if (IsDragging)
        {
            traits |= AccessibilityTrait.Selected;
        }
        return traits;
    }

    #region IFocusable
    bool IFocusable.IsFocusable => IsFocusable;
    int? IFocusable.TabIndex => TabIndex;
    bool IFocusable.TabStop => TabStop;
    bool IFocusable.IsFocused
    {
        get => IsFocused;
        set => IsFocused = value;
    }
    void IFocusable.OnFocus() => OnFocus();
    void IFocusable.OnBlur() => OnBlur();
    #endregion

    #region IKeyboardInputHandler
    /// <inheritdoc />
    public bool HandleKeyboardInput(PlusKey key)
    {
        var step = (Maximum - Minimum) / 20f; // 5% increments
        switch (key)
        {
            case PlusKey.ArrowLeft:
            case PlusKey.ArrowDown:
                Value -= step;
                NotifyValueSetters();
                return true;
            case PlusKey.ArrowRight:
            case PlusKey.ArrowUp:
                Value += step;
                NotifyValueSetters();
                return true;
            default:
                return false;
        }
    }

    private void NotifyValueSetters()
    {
        _onValueChanged?.Invoke(Value);
        if (_setter.TryGetValue(nameof(Value), out var setters))
        {
            foreach (var setter in setters)
            {
                setter(Value);
            }
        }
    }
    #endregion

    #region Value
    private Action<float>? _onValueChanged;

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

    /// <summary>
    /// Sets a callback that is invoked when Value changes.
    /// </summary>
    public Slider SetOnValueChanged(Action<float> callback)
    {
        _onValueChanged = callback;
        return this;
    }

    public Slider BindValue(Expression<Func<float>> propertyExpression, Action<float> propertySetter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Value = getter());
        foreach (var segment in path)
        {
            RegisterSetter<float>(segment, propertySetter);
        }
        RegisterSetter<float>(nameof(Value), propertySetter);
        return this;
    }
    public Slider BindValue<T>(
        Expression<Func<T>> propertyExpression,
        Action<T> propertySetter,
        Func<T, float>? toControl = null,
        Func<float, T>? toSource = null)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Value = toControl != null ? toControl(getter()) : (float)(object)getter()!);
        Action<float> wrappedSetter = controlValue => propertySetter(toSource != null ? toSource(controlValue) : (T)(object)controlValue);
        foreach (var segment in path)
        {
            RegisterSetter<float>(segment, wrappedSetter);
        }
        RegisterSetter<float>(nameof(Value), wrappedSetter);
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

    public Slider BindMinimum(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Minimum = getter());
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

    public Slider BindMaximum(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Maximum = getter());
        return this;
    }
    #endregion

    #region MinimumTrackColor
    internal Color MinimumTrackColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = new Color(0, 122, 255); // iOS blue

    public Slider SetMinimumTrackColor(Color color)
    {
        MinimumTrackColor = color;
        return this;
    }

    public Slider BindMinimumTrackColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MinimumTrackColor = getter());
        return this;
    }
    #endregion

    #region MaximumTrackColor
    internal Color MaximumTrackColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = new Color(230, 230, 230);

    public Slider SetMaximumTrackColor(Color color)
    {
        MaximumTrackColor = color;
        return this;
    }

    public Slider BindMaximumTrackColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MaximumTrackColor = getter());
        return this;
    }
    #endregion

    #region ThumbColor
    internal Color ThumbColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = Colors.White;

    public Slider SetThumbColor(Color color)
    {
        ThumbColor = color;
        return this;
    }

    public Slider BindThumbColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ThumbColor = getter());
        return this;
    }
    #endregion

    /// <inheritdoc />
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // Return available width (minus margin) and desired height so the control stretches properly
        var height = DesiredSize?.Height ?? 30f;
        var width = Math.Max(0, availableSize.Width - Margin.Horizontal);
        return new Size(width, height);
    }

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

        // Notify callback and setters
        _onValueChanged?.Invoke(Value);
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
