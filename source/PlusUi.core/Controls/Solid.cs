using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

[GenerateShadowMethods]
public partial class Solid : UiElement
{
    public Solid(float? width = null, float? height = null, SKColor? color = null)
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
        if (width is not null || height is not null)
        {
            SetDesiredSize(new Size(width ?? 0, height ?? 0));
        }
        if (color is not null)
        {
            SetBackground(new SolidColorBackground(color.Value));
        }
    }
}