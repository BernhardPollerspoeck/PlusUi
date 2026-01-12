using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services;
using SkiaSharp;
using System.Linq.Expressions;

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
public partial class RadioButton : UiElement, IInputControl, IFocusable
{
    private static readonly float CircleSize = PlusUiDefaults.CheckboxSize;
    private static readonly float CircleTextSpacing = PlusUiDefaults.Spacing;

    private IRadioButtonManager? _manager;
    private SKFont _font;
    private SKPaint _textPaint;

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override bool InterceptsClicks => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.RadioButton;

    public RadioButton()
    {
        UpdatePaint();
        SetHighContrastForeground(PlusUiDefaults.HcForeground);
    }

    [MemberNotNull(nameof(_font), nameof(_textPaint))]
    private void UpdatePaint()
    {
        // Release old paint if exists (for property changes)
        if (_textPaint is not null && _font is not null)
        {
            PaintRegistry.Release(_textPaint, _font);
        }

        // Get or create from registry
        (_textPaint, _font) = PaintRegistry.GetOrCreate(
            color: TextColor,
            size: TextSize
        );
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? Text ?? "Radio button";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        return AccessibilityValue ?? (IsSelected ? "Selected" : "Not selected");
    }

    /// <inheritdoc />
    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = base.GetComputedAccessibilityTraits();
        if (IsSelected)
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

    public RadioButton BindIsSelected(Expression<Func<bool>> propertyExpression, Action<bool>? propertySetter = null)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsSelected = getter());
        if (propertySetter != null)
        {
            foreach (var segment in path)
            {
                RegisterSetter<bool>(segment, propertySetter);
            }
            RegisterSetter<bool>(nameof(IsSelected), propertySetter);
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

    public RadioButton BindGroup(Expression<Func<object?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Group = getter());
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

    public RadioButton BindValue(Expression<Func<object?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Value = getter());
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

    public RadioButton BindText(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Text = getter());
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
            UpdatePaint();
            InvalidateMeasure();
        }
    } = PlusUiDefaults.FontSize;

    public RadioButton SetTextSize(float size)
    {
        TextSize = size;
        return this;
    }

    public RadioButton BindTextSize(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TextSize = getter());
        return this;
    }
    #endregion

    #region TextColor
    internal Color TextColor
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            UpdatePaint();
        }
    } = PlusUiDefaults.TextPrimary;

    public RadioButton SetTextColor(Color color)
    {
        TextColor = color;
        return this;
    }

    public RadioButton BindTextColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TextColor = getter());
        return this;
    }
    #endregion

    #region CircleColor
    internal Color CircleColor
    {
        get => field;
        set => field = value;
    } = PlusUiDefaults.TextPrimary;

    public RadioButton SetCircleColor(Color color)
    {
        CircleColor = color;
        return this;
    }

    public RadioButton BindCircleColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => CircleColor = getter());
        return this;
    }
    #endregion

    #region SelectedColor
    internal Color SelectedColor
    {
        get => field;
        set => field = value;
    } = PlusUiDefaults.AccentSuccess;

    public RadioButton SetSelectedColor(Color color)
    {
        SelectedColor = color;
        return this;
    }

    public RadioButton BindSelectedColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedColor = getter());
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

            // Release paint from registry
            if (_textPaint is not null && _font is not null)
            {
                PaintRegistry.Release(_textPaint, _font);
            }
        }
        base.Dispose(disposing);
    }
}
