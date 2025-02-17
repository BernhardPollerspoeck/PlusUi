using SkiaSharp;

namespace PlusUi.core;

public class NullElement : UiElement
{
    public override void Render(SKCanvas canvas)
    {
    }

    protected override Size MeasureInternal(Size availableSize)
    {
        return Size.Empty;
    }
}