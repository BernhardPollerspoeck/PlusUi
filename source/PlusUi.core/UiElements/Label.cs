using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.UiElements;



public class Label : UiTextElement<Label>
{
    #region UiElement
    public override void Render(SKCanvas canvas, SKPoint location)
    {
        base.Render(canvas, location);
        canvas.DrawText(
            Text,
            Margin.Left + location.X,
            Margin.Top + location.Y + TextSize,
            HorizontalAlignment,
            Font,
            Paint);

    }

    protected override Size MeasureInternal(Size availableSize)
    {//TODO: include availableSize
        //we need to cut or wrap if the text is too long
        return new Size(Font.MeasureText(Text), TextSize);
    }
    #endregion
}
