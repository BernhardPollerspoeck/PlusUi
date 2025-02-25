using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Controls;

internal class HelloWorldControl : UserControl
{
    public HelloWorldControl()
    {
        SetBackgroundColor(SKColors.Red);
        SetHorizontalAlignment(HorizontalAlignment.Stretch);
    }
    protected override UiElement Build()
    {
        return new Label()
            .SetText("Hello World")
            .SetTextSize(20)
            .SetTextColor(SKColors.White)
            .SetHorizontalAlignment(HorizontalAlignment.Center);
    }
}







