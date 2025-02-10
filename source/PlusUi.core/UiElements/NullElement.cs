using SkiaSharp;

namespace PlusUi.core.UiElements;

public class NullElement : UiElement
{
    public override void Render(SKCanvas canvas, SKPoint location)
    {
    }

    protected override Size MeasureInternal(Size availableSize)
    {
        return Size.Empty;
    }
}