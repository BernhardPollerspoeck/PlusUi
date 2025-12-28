using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Controls;

internal class HelloWorldControl : UserControl
{
    public HelloWorldControl()
    {
        SetBackground(new SolidColorBackground(Colors.Red));
    }
    protected override UiElement Build()
    {
        return new Label()
            .SetText("Hello World")
            .SetTextSize(20)
            .SetTextColor(Colors.White)
            .SetHorizontalAlignment(HorizontalAlignment.Center);
    }
}







