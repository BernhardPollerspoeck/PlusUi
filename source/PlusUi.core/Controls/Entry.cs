using SkiaSharp;

namespace PlusUi.core;

public class Entry : UiTextElement<Entry>, ITextInputControl
{
    private bool _isSelected;
    private DateTime _selectionTime;

    protected override bool SkipBackground => true;

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
        if (BackgroundPaint is not null)
        {
            var rect = new SKRect(
                Position.X + VisualOffset.X,
                Position.Y + VisualOffset.Y,
                Position.X + VisualOffset.X + ElementSize.Width,
                Position.Y + VisualOffset.Y + ElementSize.Height);
            if (CornerRadius > 0)
            {
                canvas.DrawRoundRect(rect, CornerRadius, CornerRadius, BackgroundPaint);
            }
            else
            {
                canvas.DrawRect(rect, BackgroundPaint);
            }
        }

        canvas.DrawText(
            IsPassword && !string.IsNullOrEmpty(Text) 
                ? new string(PasswordChar, Text.Length) 
                : Text ?? string.Empty,
            Position.X + VisualOffset.X + Padding.Left,
            Position.Y + VisualOffset.Y + textHeight,
            (SKTextAlign)HorizontalTextAlignment,
            Font,
            Paint);

        if (_isSelected)
        {
            var elapsedMilliseconds = (DateTime.Now - _selectionTime).TotalMilliseconds;
            if ((elapsedMilliseconds % 1600) < 800)
            {
                var displayText = IsPassword && !string.IsNullOrEmpty(Text) 
                    ? new string(PasswordChar, Text.Length) 
                    : Text ?? string.Empty;
                var cursorX = Position.X + VisualOffset.X + Padding.Left + Font.MeasureText(displayText) + 2;
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
