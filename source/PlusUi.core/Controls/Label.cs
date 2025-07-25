using SkiaSharp;

namespace PlusUi.core;

public class Label : UiTextElement<Label>
{
    #region UiElement
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);

        canvas.DrawText(
            Text ?? string.Empty,
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y + TextSize,
            (SKTextAlign)HorizontalTextAlignment,
            Font,
            Paint);

    }

    #endregion
}
