using SkiaSharp;

namespace PlusUi.core.UiElements;

public abstract class UiTextElement<T> : UiTextElement where T : UiTextElement<T>
{
    public new T SetText(string text)
    {
        base.SetText(text);
        return (T)this;
    }
    public new T BindText(string propertyName, Func<string> propertyGetter)
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
    public new T SetHorizontalAlignment(SKTextAlign alignment)
    {
        base.SetHorizontalAlignment(alignment);
        return (T)this;
    }
    public new T BindHorizontalAlignment(string propertyName, Func<SKTextAlign> propertyGetter)
    {
        base.BindHorizontalAlignment(propertyName, propertyGetter);
        return (T)this;
    }
}
public abstract class UiTextElement : UiElement
{
    #region Text
    public string? Text
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
    public UiTextElement BindText(string propertyName, Func<string> propertyGetter)
    {
        RegisterBinding(propertyName, () => Text = propertyGetter());
        return this;
    }
    #endregion

    #region TextSize
    public float TextSize
    {
        get => field;
        set
        {
            field = value;
            Font = CreateFont();
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
    public SKColor TextColor
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

    #region HorizontalAlignment
    public SKTextAlign HorizontalAlignment
    {
        get => field;
        set
        {
            field = value;

        }
    } = SKTextAlign.Left;
    public UiTextElement SetHorizontalAlignment(SKTextAlign alignment)
    {
        HorizontalAlignment = alignment;
        return this;
    }
    public UiTextElement BindHorizontalAlignment(string propertyName, Func<SKTextAlign> propertyGetter)
    {
        RegisterBinding(propertyName, () => HorizontalAlignment = propertyGetter());
        return this;
    }
    #endregion

    public UiTextElement()
    {
        Paint = CreatePaint();
        Font = CreateFont();
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