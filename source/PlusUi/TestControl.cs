using PlusUi.core;
using SkiaSharp;

namespace PlusUi;

public class TestControl : UserControl
{
    public TestControl()
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






