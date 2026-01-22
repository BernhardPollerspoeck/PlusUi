using PlusUi.core.Attributes;
using SkiaSharp;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A single-line text input control for user text entry.
/// Supports password masking, placeholders, keyboard types, and two-way data binding.
/// </summary>
/// <example>
/// <code>
/// // Simple text entry
/// new Entry()
///     .SetPlaceholder("Enter your name...")
///     .BindText(nameof(vm.Name), () => vm.Name, value => vm.Name = value);
///
/// // Password entry
/// new Entry()
///     .SetIsPassword(true)
///     .SetPlaceholder("Password");
///
/// // Email entry with mobile keyboard
/// new Entry()
///     .SetKeyboardType(KeyboardType.Email)
///     .SetReturnKeyType(ReturnKeyType.Done);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class Entry : UiTextElement, ITextInputControl, IFocusable
{
    private bool _isSelected;
    private DateTime _selectionTime;

    public Entry()
    {
        SetBackground(new SolidColorBackground(PlusUiDefaults.BackgroundInput));
        SetCornerRadius(PlusUiDefaults.CornerRadius);
        SetDesiredWidth(200);
        SetHighContrastBackground(PlusUiDefaults.HcInputBackground);
        SetHighContrastForeground(PlusUiDefaults.HcForeground);
    }

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override bool InterceptsClicks => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.TextInput;

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? Placeholder ?? "Text input";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        return AccessibilityValue ?? (IsPassword ? new string('*', Text?.Length ?? 0) : Text);
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

    #region Padding
    internal Margin Padding
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = new Margin(PlusUiDefaults.PaddingHorizontal, PlusUiDefaults.PaddingVertical);
    public Entry SetPadding(Margin padding)
    {
        Padding = padding;
        return this;
    }
    public Entry BindPadding(Expression<Func<Margin>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Padding = getter());
        return this;
    }
    #endregion

    #region IsPassword
    internal bool IsPassword { get; set; }
    public Entry SetIsPassword(bool isPassword)
    {
        IsPassword = isPassword;
        return this;
    }
    public Entry BindIsPassword(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsPassword = getter());
        return this;
    }
    #endregion

    #region PasswordChar
    internal char PasswordChar { get; set; } = '•';
    public Entry SetPasswordChar(char passwordChar)
    {
        PasswordChar = passwordChar;
        return this;
    }
    public Entry BindPasswordChar(Expression<Func<char>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => PasswordChar = getter());
        return this;
    }
    #endregion

    #region Placeholder
    internal string? Placeholder { get; set; }
    public Entry SetPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return this;
    }
    public Entry BindPlaceholder(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Placeholder = getter());
        return this;
    }
    #endregion

    #region PlaceholderColor
    internal Color PlaceholderColor { get; set; } = PlusUiDefaults.TextPlaceholder;
    public Entry SetPlaceholderColor(Color color)
    {
        PlaceholderColor = color;
        return this;
    }
    public Entry BindPlaceholderColor(Expression<Func<Color>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => PlaceholderColor = getter());
        return this;
    }
    #endregion

    #region MaxLength
    internal int? MaxLength { get; set; }
    public Entry SetMaxLength(int maxLength)
    {
        MaxLength = maxLength;
        return this;
    }
    public Entry BindMaxLength(Expression<Func<int>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => MaxLength = getter());
        return this;
    }
    #endregion

    #region Keyboard
    internal KeyboardType Keyboard { get; set; } = KeyboardType.Default;
    public Entry SetKeyboard(KeyboardType keyboard)
    {
        Keyboard = keyboard;
        return this;
    }
    public Entry BindKeyboard(Expression<Func<KeyboardType>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Keyboard = getter());
        return this;
    }
    #endregion

    #region ReturnKey
    internal ReturnKeyType ReturnKey { get; set; } = ReturnKeyType.Default;
    public Entry SetReturnKey(ReturnKeyType returnKey)
    {
        ReturnKey = returnKey;
        return this;
    }
    public Entry BindReturnKey(Expression<Func<ReturnKeyType>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ReturnKey = getter());
        return this;
    }
    #endregion

    private Action<string?>? _onTextChanged;

    /// <summary>
    /// Sets a callback that is invoked when Text changes.
    /// </summary>
    public Entry SetOnTextChanged(Action<string?> callback)
    {
        _onTextChanged = callback;
        return this;
    }

    public Entry BindText(Expression<Func<string?>> propertyExpression, Action<string> propertySetter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Text = getter());
        foreach (var segment in path)
        {
            RegisterSetter<string>(segment, propertySetter);
        }
        RegisterSetter<string>(nameof(Text), propertySetter);
        return this;
    }
    public Entry BindText<T>(
        Expression<Func<T>> propertyExpression,
        Action<T> propertySetter,
        Func<T, string?>? toControl = null,
        Func<string, T>? toSource = null)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Text = toControl != null ? toControl(getter()) : getter()?.ToString());
        Action<string> wrappedSetter = controlValue => propertySetter(toSource != null ? toSource(controlValue) : (T)(object)controlValue);
        foreach (var segment in path)
        {
            RegisterSetter<string>(segment, wrappedSetter);
        }
        RegisterSetter<string>(nameof(Text), wrappedSetter);
        return this;
    }

    protected override Margin? GetDebugPadding() => Padding;

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        // Determine what text to display
        var hasText = !string.IsNullOrEmpty(Text);
        var displayText = hasText
            ? (IsPassword ? new string(PasswordChar, Text!.Length) : Text!)
            : (Placeholder ?? string.Empty);

        // For placeholder, create a temporary paint instead of mutating the shared one
        var showingPlaceholder = !hasText && !string.IsNullOrEmpty(Placeholder);

        if (showingPlaceholder)
        {
            // Create temporary paint for placeholder (don't mutate shared Paint from registry!)
            using var placeholderPaint = new SKPaint
            {
                Color = PlaceholderColor,
                IsAntialias = Paint.IsAntialias
            };

            canvas.DrawText(
                displayText,
                Position.X + VisualOffset.X + Padding.Left,
                Position.Y + VisualOffset.Y + Padding.Top + textHeight,
                (SKTextAlign)HorizontalTextAlignment,
                Font,
                placeholderPaint);
        }
        else
        {
            canvas.DrawText(
                displayText,
                Position.X + VisualOffset.X + Padding.Left,
                Position.Y + VisualOffset.Y + Padding.Top + textHeight,
                (SKTextAlign)HorizontalTextAlignment,
                Font,
                Paint);
        }

        // Show cursor when selected
        if (_isSelected)
        {
            var elapsedMilliseconds = (DateTime.Now - _selectionTime).TotalMilliseconds;
            if ((elapsedMilliseconds % 1600) < 800)
            {
                // Cursor position: at end of actual text (not placeholder text)
                var textForCursor = hasText ? displayText : string.Empty;
                var cursorX = Position.X + VisualOffset.X + Padding.Left + Font.MeasureText(textForCursor) + 2;
                var cursorYStart = Position.Y + VisualOffset.Y + Padding.Top + (textHeight * 0.1f);
                var cursorYEnd = Position.Y + VisualOffset.Y + Padding.Top + textHeight - (textHeight * 0.1f);

                var strokeWidth = Paint.StrokeWidth;
                Paint.StrokeWidth = Math.Max(2, textHeight * 0.04f);
                canvas.DrawLine(cursorX, cursorYStart, cursorX, cursorYEnd, Paint);
                Paint.StrokeWidth = strokeWidth;
            }
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var textWidth = Font.MeasureText(Text ?? string.Empty);
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        //we need to cut or wrap if the text is too long
        return new Size(
            Math.Min(textWidth + Padding.Left + Padding.Right, availableSize.Width),
            Math.Min(textHeight + Padding.Top + Padding.Bottom, availableSize.Height));
    }
    public void SetSelectionStatus(bool isSelected)
    {
        if (!_isSelected && isSelected)
        {
            _selectionTime = TimeProvider.System.GetLocalNow().LocalDateTime;
        }
        _isSelected = isSelected;
    }
    public void HandleInput(PlusKey key)
    {
        if (key == PlusKey.Backspace)
        {
            if (Text?.Length > 0)
            {
                Text = Text[..^1];
                _onTextChanged?.Invoke(Text);
                if (_setter.TryGetValue(nameof(Text), out var textSetter))
                {
                    foreach (var setter in textSetter)
                    {
                        setter(Text);
                    }
                }
            }
        }
    }
    public void HandleInput(char chr)
    {
        // Check MaxLength before adding character
        if (MaxLength.HasValue && (Text?.Length ?? 0) >= MaxLength.Value)
        {
            return;
        }

        Text += chr;
        _onTextChanged?.Invoke(Text);
        if (_setter.TryGetValue(nameof(Text), out var textSetter))
        {
            foreach (var setter in textSetter)
            {
                setter(Text);
            }
        }
    }

}
