using PlusUi.core;
using SkiaSharp;

namespace Sandbox.h264;

public class ApplicationStyle : IApplicationStyle
{
    public void ConfigureStyle(Style style)
    {
        style.AddStyle<UiPageElement>(page =>
        {
            page.SetBackground(new SolidColorBackground(SKColors.Black));
        });

        style.AddStyle<Label>(label =>
        {
            label.SetTextColor(SKColors.White);
        });
    }
}
