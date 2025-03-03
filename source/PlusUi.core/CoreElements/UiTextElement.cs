using SkiaSharp;

namespace PlusUi.core;

public abstract class UiTextElement<T> : UiTextElement where T : UiTextElement<T>
{
    public new T SetText(string text)
    {
        base.SetText(text);
        return (T)this;
    }
    public new T BindText(string propertyName, Func<string?> propertyGetter)
    {
        base.BindText(propertyName, propertyGetter);
        return (T)this;
    }
    public new T SetTextSize(float fontSize)
    {
        base.SetTextSize(fontSize);
        return (T)this;
    }
    public new T BindTextSize(string propertyName, Func<float> propertyGetter)
    {
        base.BindTextSize(propertyName, propertyGetter);
        return (T)this;
    }
    public new T SetTextColor(SKColor color)
    {
        base.SetTextColor(color);
        return (T)this;
    }
    public new T BindTextColor(string propertyName, Func<SKColor> propertyGetter)
    {
        base.BindTextColor(propertyName, propertyGetter);
        return (T)this;
    }
    public new T SetHorizontalTextAlignment(HorizontalTextAlignment alignment)
    {
        base.SetHorizontalTextAlignment(alignment);
        return (T)this;
    }
    public new T BindHorizontalTextAlignment(string propertyName, Func<HorizontalTextAlignment> propertyGetter)
    {
        base.BindHorizontalTextAlignment(propertyName, propertyGetter);
        return (T)this;
    }
}
public abstract class UiTextElement : UiElement
{
    #region Text
    internal string? Text
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }
    public UiTextElement SetText(string text)
    {
        Text = text;
        return this;
    }
    public UiTextElement BindText(string propertyName, Func<string?> propertyGetter)
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
            field = value;
            Font = CreateFont();
            InvalidateMeasure();
        }
    } = 12;
    public UiTextElement SetTextSize(float fontSize)
    {
        TextSize = fontSize;
        return this;
    }
    public UiTextElement BindTextSize(string propertyName, Func<float> propertyGetter)
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
            field = value;
            Paint = CreatePaint();
        }
    } = SKColors.White;
    public UiTextElement SetTextColor(SKColor color)
    {
        TextColor = color;
        return this;
    }
    public UiTextElement BindTextColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextColor = propertyGetter());
        return this;
    }
    #endregion

    #region HorizontalTextAlignment
    internal HorizontalTextAlignment HorizontalTextAlignment
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = HorizontalTextAlignment.Left;
    public UiTextElement SetHorizontalTextAlignment(HorizontalTextAlignment alignment)
    {
        HorizontalTextAlignment = alignment;
        return this;
    }
    public UiTextElement BindHorizontalTextAlignment(string propertyName, Func<HorizontalTextAlignment> propertyGetter)
    {
        RegisterBinding(propertyName, () => HorizontalTextAlignment = propertyGetter());
        return this;
    }
    #endregion

    public UiTextElement()
    {
        Paint = CreatePaint();
        Font = CreateFont();
    }


    public override Size MeasureInternal(Size availableSize)
    {
        var textWidth = Font.MeasureText(Text ?? string.Empty);
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        //we need to cut or wrap if the text is too long
        return new Size(
            Math.Min(textWidth, availableSize.Width),
            Math.Min(textHeight, availableSize.Height));
    }

    #region render cache
    protected SKPaint Paint { get; set; } = null!;
    private SKPaint CreatePaint()
    {
        return new SKPaint
        {
            Color = TextColor,
            IsAntialias = true,
        };
    }
    protected SKFont Font { get; set; } = null!;
    private SKFont CreateFont()
    {
        return new SKFont(SKTypeface.Default)
        {
            Size = TextSize,
        };
    }
    #endregion
}