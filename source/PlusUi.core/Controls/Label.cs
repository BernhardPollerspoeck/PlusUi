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
