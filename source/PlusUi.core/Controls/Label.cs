using PlusUi.core.CoreElements;
using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.Controls;

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
        //we need to cut or wrap if the text is too long
        return new Size(
            Math.Min(Font.MeasureText(Text), availableSize.Width),
            Math.Min(TextSize, availableSize.Height));
    }
    #endregion
}
