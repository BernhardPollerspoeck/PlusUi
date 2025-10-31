using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

[GenerateGenericWrapper]
public abstract class RawUserControl : UiElement
{
    public abstract Size Size { get; }
    public abstract void RenderControl(SKBitmap bitmap);

    public override void BuildContent()
    {
        InvalidateMeasure();
    }
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        var bitmap = new SKBitmap((int)Size.Width, (int)Size.Height);
        RenderControl(bitmap);
        canvas.DrawBitmap(bitmap, Position.X, Position.Y);
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        return Size;
    }
}