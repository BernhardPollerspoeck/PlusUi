using PlusUi.core.Attributes;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A checkbox control for binary on/off selection with a checkmark indicator.
/// Supports two-way data binding for MVVM patterns.
/// </summary>
/// <example>
/// <code>
/// // Simple checkbox
/// new Checkbox()
///     .SetIsChecked(true);
///
/// // Data-bound checkbox
/// new Checkbox()
///     .BindIsChecked(nameof(vm.AcceptTerms), () => vm.AcceptTerms, value => vm.AcceptTerms = value);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class Checkbox : UiElement, IToggleButtonControl, IFocusable
{

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Checkbox;

    public Checkbox()
    {
        SetDesiredSize(new(PlusUiDefaults.CheckboxSize + 2, PlusUiDefaults.CheckboxSize + 2));
        SetColor(PlusUiDefaults.TextPrimary);
        SetHighContrastForeground(PlusUiDefaults.HcForeground);
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? "Checkbox";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        return AccessibilityValue ?? (IsChecked ? "Checked" : "Unchecked");
    }

    /// <inheritdoc />
    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = base.GetComputedAccessibilityTraits();
        if (IsChecked)
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

    #region IsChecked
    private Action<bool>? _onIsCheckedChanged;

    internal bool IsChecked
    {
        get => field;
        set
        {
            field = value;
        }
    }
    public Checkbox SetIsChecked(bool isChecked)
    {
        IsChecked = isChecked;
        return this;
    }

    /// <summary>
    /// Sets a callback that is invoked when IsChecked changes.
    /// </summary>
    public Checkbox SetOnIsCheckedChanged(Action<bool> callback)
    {
        _onIsCheckedChanged = callback;
        return this;
    }
    public Checkbox BindIsChecked(Expression<Func<bool>> propertyExpression, Action<bool> propertySetter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsChecked = getter());
        foreach (var segment in path)
        {
            RegisterSetter<bool>(segment, propertySetter);
        }
        RegisterSetter<bool>(nameof(IsChecked), propertySetter);
        return this;
    }
    public Checkbox BindIsChecked<T>(
        Expression<Func<T>> propertyExpression,
        Action<T> propertySetter,
        Func<T, bool>? toControl = null,
        Func<bool, T>? toSource = null)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsChecked = toControl != null ? toControl(getter()) : (bool)(object)getter()!);
        Action<bool> wrappedSetter = controlValue => propertySetter(toSource != null ? toSource(controlValue) : (T)(object)controlValue);
        foreach (var segment in path)
        {
            RegisterSetter<bool>(segment, wrappedSetter);
        }
        RegisterSetter<bool>(nameof(IsChecked), wrappedSetter);
        return this;
    }

    #endregion

    #region Color
    internal Color Color
    {
        get => field;
        set
        {
            field = value;
        }
    }
    public Checkbox SetColor(Color color)
    {
        Color = color;
        return this;
    }
    public Checkbox BindColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Color = getter());
        return this;
    }
    #endregion

    public void Toggle()
    {
        IsChecked = !IsChecked;
        _onIsCheckedChanged?.Invoke(IsChecked);
        if (_setter.TryGetValue(nameof(IsChecked), out var textSetter))
        {
            foreach (var setter in textSetter)
            {
                setter(IsChecked);
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

        using var strokePaint = new SKPaint
        {
            StrokeWidth = 2,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = Color
        };

        var rect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        canvas.DrawRoundRect(rect, 5, 5, strokePaint);

        if (IsChecked)
        {
            var checkPath = new SKPath();
            checkPath.MoveTo(Position.X + VisualOffset.X + 4, Position.Y + VisualOffset.Y + (ElementSize.Height / 2));
            checkPath.LineTo(Position.X + VisualOffset.X + (ElementSize.Width / 3), Position.Y + VisualOffset.Y + ElementSize.Height - 4);
            checkPath.LineTo(Position.X + VisualOffset.X + ElementSize.Width - 4, Position.Y + VisualOffset.Y + 4);
            canvas.DrawPath(checkPath, strokePaint);
        }
    }

}
