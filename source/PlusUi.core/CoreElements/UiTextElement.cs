using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

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

    #region TextWrapping
    internal TextWrapping TextWrapping
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = TextWrapping.NoWrap;
    public UiTextElement SetTextWrapping(TextWrapping textWrapping)
    {
        TextWrapping = textWrapping;
        return this;
    }
    public UiTextElement BindTextWrapping(string propertyName, Func<TextWrapping> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextWrapping = propertyGetter());
        return this;
    }
    #endregion

    #region MaxLines
    internal int? MaxLines
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }
    public UiTextElement SetMaxLines(int maxLines)
    {
        MaxLines = maxLines;
        return this;
    }
    public UiTextElement BindMaxLines(string propertyName, Func<int> propertyGetter)
    {
        RegisterBinding(propertyName, () => MaxLines = propertyGetter());
        return this;
    }
    #endregion

    #region TextTruncation
    internal TextTruncation TextTruncation
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = TextTruncation.None;
    public UiTextElement SetTextTruncation(TextTruncation textTruncation)
    {
        TextTruncation = textTruncation;
        return this;
    }
    public UiTextElement BindTextTruncation(string propertyName, Func<TextTruncation> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextTruncation = propertyGetter());
        return this;
    }
    #endregion

    public UiTextElement()
    {
        Paint = CreatePaint();
        Font = CreateFont();
    }


    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
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