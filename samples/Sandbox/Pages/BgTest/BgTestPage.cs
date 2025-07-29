using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.BgTest;
public class BgTestPage(BgTestPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        this.SetBackgroundColor(SKColors.White);

        return new VStack(
            new Label()
                .SetText("https://timelane.cloud")
                .SetTextSize(100)
                .SetTextColor(SKColors.Black),
            new Button()
                .SetText("Click Me")
                .SetPadding(new(10, 5))
                .SetCommand(vm.NavCommand)
        )
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
