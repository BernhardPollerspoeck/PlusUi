using SkiaSharp;

namespace PlusUi.core;

public class Label : UiTextElement<Label>
{
    #region UiElement
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        canvas.DrawText(
            Text,
            Position.X,
            Position.Y + TextSize,
            (SKTextAlign)TextAlignment,
            Font,
            Paint);

    }

    protected override Size MeasureInternal(Size availableSize)
    {
        var textWidth = Font.MeasureText(Text);
        Font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        //we need to cut or wrap if the text is too long
        return new Size(
            Math.Min(textWidth, availableSize.Width),
            Math.Min(textHeight, availableSize.Height));
    }
    #endregion
}
