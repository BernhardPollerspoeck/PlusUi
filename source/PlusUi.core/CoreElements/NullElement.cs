using SkiaSharp;

namespace PlusUi.core;

public class NullElement : UiElement
{
    public override void Render(SKCanvas canvas)
    {
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        return Size.Empty;
    }
}