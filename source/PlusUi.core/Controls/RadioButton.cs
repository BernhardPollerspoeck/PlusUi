using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A radio button control for mutually exclusive selection within a group.
/// RadioButtons with the same Group value form a logical group where only one can be selected.
/// </summary>
/// <example>
/// <code>
/// // Simple radio buttons grouped by string
/// new RadioButton().SetText("Option A").SetGroup("myGroup").SetValue("A");
/// new RadioButton().SetText("Option B").SetGroup("myGroup").SetValue("B");
///
/// // Using enum for grouping
/// new RadioButton().SetText("Small").SetGroup(SizeGroup.Instance).SetValue(Size.Small);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class RadioButton : UiElement, IInputControl
{
    private const float CircleSize = 20f;
    private const float CircleTextSpacing = 8f;

    private IRadioButtonManager? _manager;
    private SKFont? _font;
    private SKPaint? _textPaint;

    public RadioButton()
    {
        _font = new SKFont(SKTypeface.Default) { Size = TextSize };
        _textPaint = new SKPaint { Color = TextColor, IsAntialias = true };
    }

    #region IsSelected
    private bool _isSelected;
    private bool _isUpdatingFromManager;

    internal bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            if (value && !_isUpdatingFromManager)
            {
                GetManager()?.NotifySelected(this);
            }
        }
    }

    public RadioButton SetIsSelected(bool isSelected)
    {
        IsSelected = isSelected;
        return this;
    }

    public RadioButton BindIsSelected(string propertyName, Func<bool> propertyGetter, Action<bool>? propertySetter = null)
    {
        RegisterBinding(propertyName, () => IsSelected = propertyGetter());
        if (propertySetter != null)
        {
            RegisterSetter(nameof(IsSelected), propertySetter);
        }
        return this;
    }
    #endregion

    #region Group
    internal object? Group
    {
        get => field;
        set
        {
            field = value;
            EnsureRegistered();
        }
    }

    public RadioButton SetGroup(object group)
    {
        Group = group;
        return this;
    }

    public RadioButton BindGroup(string propertyName, Func<object?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Group = propertyGetter());
        return this;
    }
    #endregion

    #region Value
    internal object? Value { get; set; }

    public RadioButton SetValue(object value)
    {
        Value = value;
        return this;
    }

    public RadioButton BindValue(string propertyName, Func<object?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Value = propertyGetter());
        return this;
    }
    #endregion

    #region Text
    internal string? Text
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    }

    public RadioButton SetText(string text)
    {
        Text = text;
        return this;
    }

    public RadioButton BindText(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Text = propertyGetter());
        return this;
    }
    #endregion

    #region TextSize
    internal float TextSize
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            _font?.Dispose();
            _font = new SKFont(SKTypeface.Default) { Size = value };
            InvalidateMeasure();
        }
    } = 14f;

    public RadioButton SetTextSize(float size)
    {
        TextSize = size;
        return this;
    }

    public RadioButton BindTextSize(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextSize = propertyGetter());
        return this;
    }
    #endregion

    #region TextColor
    internal SKColor TextColor
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            _textPaint?.Dispose();
            _textPaint = new SKPaint { Color = value, IsAntialias = true };
        }
    } = SKColors.White;

    public RadioButton SetTextColor(SKColor color)
    {
        TextColor = color;
        return this;
    }

    public RadioButton BindTextColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextColor = propertyGetter());
        return this;
    }
    #endregion

    #region CircleColor
    internal SKColor CircleColor
    {
        get => field;
        set => field = value;
    } = SKColors.White;

    public RadioButton SetCircleColor(SKColor color)
    {
        CircleColor = color;
        return this;
    }

    public RadioButton BindCircleColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => CircleColor = propertyGetter());
        return this;
    }
    #endregion

    #region SelectedColor
    internal SKColor SelectedColor
    {
        get => field;
        set => field = value;
    } = new SKColor(52, 199, 89); // iOS green

    public RadioButton SetSelectedColor(SKColor color)
    {
        SelectedColor = color;
        return this;
    }

    public RadioButton BindSelectedColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => SelectedColor = propertyGetter());
        return this;
    }
    #endregion

    /// <summary>
    /// Called by RadioButtonManager to deselect this button when another in the group is selected.
    /// </summary>
    internal void DeselectInternal()
    {
        if (!_isSelected) return;

        _isUpdatingFromManager = true;
        _isSelected = false;
        _isUpdatingFromManager = false;

        // Notify bound properties
        if (_setter.TryGetValue(nameof(IsSelected), out var setters))
        {
            foreach (var setter in setters)
            {
                setter(false);
            }
        }
    }

    private void Select()
    {
        if (IsSelected) return;

        IsSelected = true;

        // Notify bound properties
        if (_setter.TryGetValue(nameof(IsSelected), out var setters))
        {
            foreach (var setter in setters)
            {
                setter(true);
            }
        }
    }

    private IRadioButtonManager? GetManager()
    {
        _manager ??= ServiceProviderService.ServiceProvider?.GetService<IRadioButtonManager>();
        return _manager;
    }

    private void EnsureRegistered()
    {
        GetManager()?.Register(this);
    }

    #region IInputControl
    public void InvokeCommand()
    {
        Select();
    }
    #endregion

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var textWidth = 0f;
        var textHeight = CircleSize;

        if (!string.IsNullOrEmpty(Text) && _font != null)
        {
            textWidth = _font.MeasureText(Text);
            _font.GetFontMetrics(out var metrics);
            textHeight = Math.Max(CircleSize, metrics.Descent - metrics.Ascent);
        }

        var totalWidth = CircleSize + (textWidth > 0 ? CircleTextSpacing + textWidth : 0);

        return new Size(
            Math.Min(totalWidth, availableSize.Width),
            Math.Min(textHeight, availableSize.Height));
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible) return;

        var centerX = Position.X + VisualOffset.X + CircleSize / 2;
        var centerY = Position.Y + VisualOffset.Y + ElementSize.Height / 2;

        // Draw outer circle (ring)
        using var strokePaint = new SKPaint
        {
            Color = IsSelected ? SelectedColor : CircleColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true
        };
        canvas.DrawCircle(centerX, centerY, CircleSize / 2 - 1, strokePaint);

        // Draw inner filled circle when selected
        if (IsSelected)
        {
            using var fillPaint = new SKPaint
            {
                Color = SelectedColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            canvas.DrawCircle(centerX, centerY, CircleSize / 2 - 5, fillPaint);
        }

        // Draw text
        if (!string.IsNullOrEmpty(Text) && _font != null && _textPaint != null)
        {
            var textX = Position.X + VisualOffset.X + CircleSize + CircleTextSpacing;
            var textY = centerY + TextSize / 3; // Approximate vertical centering
            canvas.DrawText(Text, textX, textY, _font, _textPaint);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetManager()?.Unregister(this);
            _font?.Dispose();
            _textPaint?.Dispose();
        }
        base.Dispose(disposing);
    }
}
