using PlusUi.core.CoreElements;
using PlusUi.core.Structures;
using SkiaSharp;

namespace PlusUi.core.Controls;

public class Solid : UiElement<Solid>
{
    public Solid(float? width = null, float? height = null, SKColor? color = null)
    {
        if (width is not null || height is not null)
        {
            SetDesiredSize(new Size(width ?? 0, height ?? 0));
        }
        if (color is not null)
        {
            SetBackgroundColor(color.Value);
        }
    }
}