using SkiaSharp;

namespace PlusUi.core;

public class NullElement : UiElement
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.None;

    public override void Render(SKCanvas canvas)
    {
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        return Size.Empty;
    }
}