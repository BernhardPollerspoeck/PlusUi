using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Represents a non-editable text label control for displaying static or dynamic text.
/// Inherits from <see cref="UiTextElement"/> and supports all text formatting options.
/// </summary>
/// <example>
/// <code>
/// // Simple label
/// new Label().SetText("Hello World");
///
/// // Styled label
/// new Label()
///     .SetText("Title")
///     .SetFontSize(24)
///     .SetFontWeight(FontWeight.Bold)
///     .SetTextColor(Colors.Blue);
///
/// // Data-bound label
/// new Label().BindText(nameof(viewModel.UserName), () => viewModel.UserName);
/// </code>
/// </example>
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
            
            // Calculate X position based on text alignment
            var x = HorizontalTextAlignment switch
            {
                HorizontalTextAlignment.Center => Position.X + VisualOffset.X + (ElementSize.Width / 2),
                HorizontalTextAlignment.Right => Position.X + VisualOffset.X + ElementSize.Width,
                _ => Position.X + VisualOffset.X
            };
            
            canvas.DrawText(
                truncatedText,
                x,
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
                // Calculate X position based on text alignment
                var x = HorizontalTextAlignment switch
                {
                    HorizontalTextAlignment.Center => Position.X + VisualOffset.X + (ElementSize.Width / 2),
                    HorizontalTextAlignment.Right => Position.X + VisualOffset.X + ElementSize.Width,
                    _ => Position.X + VisualOffset.X
                };
                
                canvas.DrawText(
                    line,
                    x,
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
