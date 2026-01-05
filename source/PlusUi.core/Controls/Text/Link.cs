using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A clickable hyperlink control that opens URLs when clicked.
/// </summary>
/// <example>
/// <code>
/// // Simple link
/// new Link()
///     .SetText("Visit Website")
///     .SetUrl("https://example.com");
///
/// // Styled link
/// new Link()
///     .SetText("Documentation")
///     .SetUrl("https://docs.example.com")
///     .SetTextColor(Colors.Blue)
///     .SetFontWeight(FontWeight.SemiBold);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class Link : UiTextElement, IInputControl, IFocusable
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override bool InterceptsClicks => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Link;

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? Text;
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

    #region Url
    internal string? Url
    {
        get => field;
        set => field = value;
    }
    public Link SetUrl(string url)
    {
        Url = url;
        return this;
    }
    public Link BindUrl(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Url = propertyGetter());
        return this;
    }
    #endregion

    #region UnderlineThickness
    internal float UnderlineThickness { get; set; } = 1f;
    public Link SetUnderlineThickness(float thickness)
    {
        UnderlineThickness = thickness;
        return this;
    }
    public Link BindUnderlineThickness(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => UnderlineThickness = propertyGetter());
        return this;
    }
    #endregion

    #region IInputControl
    public void InvokeCommand()
    {
        if (!string.IsNullOrEmpty(Url))
        {
            OpenUrl(Url);
        }
    }

    private static void OpenUrl(string url)
    {
        // Try to get the platform service from DI
        var platformService = ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();

        if (platformService != null)
        {
            platformService.OpenUrl(url);
        }
    }
    #endregion

    #region UiElement
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        Font.GetFontMetrics(out var fontMetrics);
        var lineHeight = fontMetrics.Descent - fontMetrics.Ascent;
        var text = Text ?? string.Empty;

        if (TextWrapping == TextWrapping.NoWrap)
        {
            // Single line rendering with optional truncation
            var truncatedText = ApplyTruncation(text, ElementSize.Width);

            // Calculate X position based on text alignment
            var x = HorizontalTextAlignment switch
            {
                HorizontalTextAlignment.Center => Position.X + VisualOffset.X + (ElementSize.Width / 2),
                HorizontalTextAlignment.Right => Position.X + VisualOffset.X + ElementSize.Width,
                _ => Position.X + VisualOffset.X
            };

            var y = Position.Y + VisualOffset.Y + TextSize;

            // Draw text
            canvas.DrawText(
                truncatedText,
                x,
                y,
                (SKTextAlign)HorizontalTextAlignment,
                Font,
                Paint);

            // Draw underline
            DrawUnderline(canvas, truncatedText, x, y);
        }
        else
        {
            // Multi-line rendering with wrapping
            var lines = WrapText(text, ElementSize.Width);

            // Apply MaxLines if set
            if (MaxLines.HasValue && lines.Count > MaxLines.Value)
            {
                lines = lines.Take(MaxLines.Value).ToList();

                // Apply truncation to the last line if needed
                if (TextTruncation != TextTruncation.None && lines.Count > 0)
                {
                    var lastIndex = lines.Count - 1;
                    lines[lastIndex] = ApplyTruncation(lines[lastIndex], ElementSize.Width);
                }
            }

            var y = Position.Y + VisualOffset.Y + TextSize;
            foreach (var line in lines)
            {
                // Calculate X position based on text alignment
                var x = HorizontalTextAlignment switch
                {
                    HorizontalTextAlignment.Center => Position.X + VisualOffset.X + (ElementSize.Width / 2),
                    HorizontalTextAlignment.Right => Position.X + VisualOffset.X + ElementSize.Width,
                    _ => Position.X + VisualOffset.X
                };

                // Draw text
                canvas.DrawText(
                    line,
                    x,
                    y,
                    (SKTextAlign)HorizontalTextAlignment,
                    Font,
                    Paint);

                // Draw underline
                DrawUnderline(canvas, line, x, y);

                y += lineHeight;
            }
        }
    }

    private void DrawUnderline(SKCanvas canvas, string text, float x, float y)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var textWidth = Font.MeasureText(text);

        // Calculate underline start position based on alignment
        var underlineStartX = HorizontalTextAlignment switch
        {
            HorizontalTextAlignment.Center => x - (textWidth / 2),
            HorizontalTextAlignment.Right => x - textWidth,
            _ => x
        };

        var underlineEndX = underlineStartX + textWidth;

        // Position underline slightly below the text baseline
        Font.GetFontMetrics(out var fontMetrics);
        var underlineY = y + fontMetrics.UnderlinePosition.GetValueOrDefault(2f);

        using var underlinePaint = new SKPaint
        {
            Color = Paint.Color,
            StrokeWidth = UnderlineThickness,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        canvas.DrawLine(underlineStartX, underlineY, underlineEndX, underlineY, underlinePaint);
    }
    #endregion
}
