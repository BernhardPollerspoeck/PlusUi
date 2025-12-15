using PlusUi.core.Attributes;
using SkiaSharp;

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

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

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
    }
    public Entry SetPadding(Margin padding)
    {
        Padding = padding;
        return this;
    }
    public Entry BindPadding(string propertyName, Func<Margin> propertyGetter)
    {
        RegisterBinding(propertyName, () => Padding = propertyGetter());
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
    public Entry BindIsPassword(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => IsPassword = propertyGetter());
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
    public Entry BindPasswordChar(string propertyName, Func<char> propertyGetter)
    {
        RegisterBinding(propertyName, () => PasswordChar = propertyGetter());
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
    public Entry BindPlaceholder(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Placeholder = propertyGetter());
        return this;
    }
    #endregion

    #region PlaceholderColor
    internal SKColor PlaceholderColor { get; set; } = new SKColor(180, 180, 180);
    public Entry SetPlaceholderColor(SKColor color)
    {
        PlaceholderColor = color;
        return this;
    }
    public Entry BindPlaceholderColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => PlaceholderColor = propertyGetter());
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
    public Entry BindMaxLength(string propertyName, Func<int> propertyGetter)
    {
        RegisterBinding(propertyName, () => MaxLength = propertyGetter());
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
    public Entry BindKeyboard(string propertyName, Func<KeyboardType> propertyGetter)
    {
        RegisterBinding(propertyName, () => Keyboard = propertyGetter());
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
    public Entry BindReturnKey(string propertyName, Func<ReturnKeyType> propertyGetter)
    {
        RegisterBinding(propertyName, () => ReturnKey = propertyGetter());
        return this;
    }
    #endregion

    public Entry BindText(string propertyName, Func<string?> propertyGetter, Action<string> propertySetter)
    {
        base.BindText(propertyName, propertyGetter);
        RegisterSetter(nameof(Text), propertySetter);
        return this;
    }

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

        // Save original color and temporarily change if showing placeholder
        var originalColor = Paint.Color;
        var showingPlaceholder = !hasText && !string.IsNullOrEmpty(Placeholder);

        if (showingPlaceholder)
        {
            Paint.Color = PlaceholderColor;
        }

        canvas.DrawText(
            displayText,
            Position.X + VisualOffset.X + Padding.Left,
            Position.Y + VisualOffset.Y + textHeight,
            (SKTextAlign)HorizontalTextAlignment,
            Font,
            Paint);

        // Restore original color
        if (showingPlaceholder)
        {
            Paint.Color = originalColor;
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
        if (_setter.TryGetValue(nameof(Text), out var textSetter))
        {
            foreach (var setter in textSetter)
            {
                setter(Text);
            }
        }
    }

}
