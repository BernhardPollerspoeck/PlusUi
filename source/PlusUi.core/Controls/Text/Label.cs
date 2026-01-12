using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A non-editable text label control for displaying static or dynamic text.
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
    public Label()
    {
        SetHighContrastForeground(PlusUiDefaults.HcForeground);
    }

    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Label;

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? Text;
    }

    #region UiElement
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        // Clip to element bounds to prevent text from spilling into other controls
        var clipRect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);
        canvas.Save();
        canvas.ClipRect(clipRect);

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

        canvas.Restore();
    }

    #endregion
}
