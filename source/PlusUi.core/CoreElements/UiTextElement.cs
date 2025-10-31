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
        var text = Text ?? string.Empty;
        Font.GetFontMetrics(out var fontMetrics);
        var lineHeight = fontMetrics.Descent - fontMetrics.Ascent;

        // Calculate the effective width constraint for text wrapping
        // This should match what will actually be used for rendering
        var effectiveWidth = availableSize.Width;
        if (DesiredSize?.Width >= 0)
        {
            effectiveWidth = Math.Min(DesiredSize.Value.Width, availableSize.Width);
        }

        if (TextWrapping == TextWrapping.NoWrap)
        {
            // No wrapping - single line
            var textWidth = Font.MeasureText(text);
            return new Size(
                Math.Min(textWidth, availableSize.Width),
                Math.Min(lineHeight, availableSize.Height));
        }
        else
        {
            // Text wrapping enabled - use effective width for wrapping
            var lines = WrapText(text, effectiveWidth);
            
            // Apply MaxLines if set
            if (MaxLines.HasValue && lines.Count > MaxLines.Value)
            {
                lines = lines.Take(MaxLines.Value).ToList();
            }

            var maxWidth = lines.Count > 0 
                ? lines.Max(line => Font.MeasureText(line)) 
                : 0f;
            var totalHeight = lineHeight * lines.Count;

            return new Size(
                Math.Min(maxWidth, availableSize.Width),
                Math.Min(totalHeight, availableSize.Height));
        }
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

    #region Text wrapping and truncation helpers
    protected List<string> WrapText(string text, float maxWidth)
    {
        var lines = new List<string>();
        if (string.IsNullOrEmpty(text) || maxWidth <= 0)
        {
            lines.Add(text);
            return lines;
        }

        if (TextWrapping == TextWrapping.WordWrap)
        {
            // Word wrap - break at word boundaries
            var words = text.Split(' ');
            var currentLine = "";

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                var testWidth = Font.MeasureText(testLine);

                if (testWidth > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }
        }
        else // TextWrapping.Wrap
        {
            // Character wrap - break at any character
            var currentLine = "";

            foreach (var ch in text)
            {
                var testLine = currentLine + ch;
                var testWidth = Font.MeasureText(testLine);

                if (testWidth > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = ch.ToString();
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }
        }

        return lines.Count > 0 ? lines : new List<string> { text };
    }

    protected string ApplyTruncation(string text, float maxWidth)
    {
        if (TextTruncation == TextTruncation.None || string.IsNullOrEmpty(text))
        {
            return text;
        }

        var textWidth = Font.MeasureText(text);
        if (textWidth <= maxWidth)
        {
            return text;
        }

        const string ellipsis = " ... ";
        var ellipsisWidth = Font.MeasureText(ellipsis);

        return TextTruncation switch
        {
            TextTruncation.Start => TruncateStart(text, maxWidth, ellipsis, ellipsisWidth),
            TextTruncation.Middle => TruncateMiddle(text, maxWidth, ellipsis, ellipsisWidth),
            TextTruncation.End => TruncateEnd(text, maxWidth, ellipsis, ellipsisWidth),
            _ => text,
        };
    }

    private string TruncateEnd(string text, float maxWidth, string ellipsis, float ellipsisWidth)
    {
        var availableWidth = maxWidth - ellipsisWidth;
        if (availableWidth <= 0)
        {
            return ellipsis;
        }

        for (int i = text.Length - 1; i >= 0; i--)
        {
            var substring = text.Substring(0, i);
            if (Font.MeasureText(substring) <= availableWidth)
            {
                return substring + ellipsis;
            }
        }

        return ellipsis;
    }

    private string TruncateStart(string text, float maxWidth, string ellipsis, float ellipsisWidth)
    {
        var availableWidth = maxWidth - ellipsisWidth;
        if (availableWidth <= 0)
        {
            return ellipsis;
        }

        for (int i = 0; i < text.Length; i++)
        {
            var substring = text.Substring(i);
            if (Font.MeasureText(substring) <= availableWidth)
            {
                return ellipsis + substring;
            }
        }

        return ellipsis;
    }

    private string TruncateMiddle(string text, float maxWidth, string ellipsis, float ellipsisWidth)
    {
        var availableWidth = maxWidth - ellipsisWidth;
        if (availableWidth <= 0)
        {
            return ellipsis;
        }

        var halfWidth = availableWidth / 2;
        var startChars = 0;
        var endChars = 0;

        // Find how many characters fit at the start
        for (int i = 1; i <= text.Length; i++)
        {
            if (Font.MeasureText(text.Substring(0, i)) <= halfWidth)
            {
                startChars = i;
            }
            else
            {
                break;
            }
        }

        // Find how many characters fit at the end
        for (int i = 1; i <= text.Length; i++)
        {
            if (Font.MeasureText(text.Substring(text.Length - i)) <= halfWidth)
            {
                endChars = i;
            }
            else
            {
                break;
            }
        }

        if (startChars + endChars >= text.Length)
        {
            return text;
        }

        return text.Substring(0, startChars) + ellipsis + text.Substring(text.Length - endChars);
    }
    #endregion
}