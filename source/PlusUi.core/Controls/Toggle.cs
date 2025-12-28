using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A toggle/switch control for on/off states.
/// Provides a modern iOS/Android style switch UI.
/// </summary>
[GenerateShadowMethods]
public partial class Toggle : UiElement, IToggleButtonControl, IFocusable
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Toggle;

    public Toggle()
    {
        SetDesiredSize(new(50, 28));
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? "Toggle switch";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        return AccessibilityValue ?? (IsOn ? "On" : "Off");
    }

    /// <inheritdoc />
    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = base.GetComputedAccessibilityTraits();
        if (IsOn)
        {
            traits |= AccessibilityTrait.Checked;
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

    #region IsOn
    internal bool IsOn
    {
        get => field;
        set
        {
            field = value;
        }
    }

    public Toggle SetIsOn(bool isOn)
    {
        IsOn = isOn;
        return this;
    }

    public Toggle BindIsOn(string propertyName, Func<bool> propertyGetter, Action<bool> propertySetter)
    {
        RegisterBinding(propertyName, () => IsOn = propertyGetter());
        RegisterSetter(nameof(IsOn), propertySetter);
        return this;
    }
    #endregion

    #region OnColor
    internal SKColor OnColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = new SKColor(52, 199, 89); // iOS green

    public Toggle SetOnColor(SKColor color)
    {
        OnColor = color;
        return this;
    }

    public Toggle BindOnColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => OnColor = propertyGetter());
        return this;
    }
    #endregion

    #region OffColor
    internal SKColor OffColor
    {
        get => field;
        set
        {
            field = value;
        }
    } = new SKColor(120, 120, 128); // iOS gray

    public Toggle SetOffColor(SKColor color)
    {
        OffColor = color;
        return this;
    }

    public Toggle BindOffColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => OffColor = propertyGetter());
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

    public Toggle SetThumbColor(SKColor color)
    {
        ThumbColor = color;
        return this;
    }

    public Toggle BindThumbColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => ThumbColor = propertyGetter());
        return this;
    }
    #endregion

    void IToggleButtonControl.Toggle()
    {
        IsOn = !IsOn;
        if (_setter.TryGetValue(nameof(IsOn), out var setter))
        {
            foreach (var setterAction in setter)
            {
                setterAction(IsOn);
            }
        }
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        var trackColor = IsOn ? OnColor : OffColor;
        var trackRect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        // Draw track (background)
        using var trackPaint = new SKPaint
        {
            Color = trackColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRoundRect(trackRect, ElementSize.Height / 2, ElementSize.Height / 2, trackPaint);

        // Calculate thumb position
        var thumbRadius = (ElementSize.Height / 2) - 2;
        var thumbCenterY = Position.Y + VisualOffset.Y + (ElementSize.Height / 2);
        var thumbCenterX = IsOn
            ? Position.X + VisualOffset.X + ElementSize.Width - thumbRadius - 2
            : Position.X + VisualOffset.X + thumbRadius + 2;

        // Draw thumb (circle)
        using var thumbPaint = new SKPaint
        {
            Color = ThumbColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        // Add shadow to thumb
        using var thumbShadowPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 50),
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
        };
        canvas.DrawCircle(thumbCenterX, thumbCenterY + 1, thumbRadius, thumbShadowPaint);
        canvas.DrawCircle(thumbCenterX, thumbCenterY, thumbRadius, thumbPaint);
    }
}
