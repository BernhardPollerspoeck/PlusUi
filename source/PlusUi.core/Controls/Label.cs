using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

[GenerateShadowMethods]
public partial class Label : UiTextElement
{
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
            
            canvas.DrawText(
                truncatedText,
                Position.X + VisualOffset.X,
                Position.Y + VisualOffset.Y + TextSize,
                (SKTextAlign)HorizontalTextAlignment,
                Font,
                Paint);
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
                canvas.DrawText(
                    line,
                    Position.X + VisualOffset.X,
                    y,
                    (SKTextAlign)HorizontalTextAlignment,
                    Font,
                    Paint);
                y += lineHeight;
            }
        }
    }

    #endregion
}
