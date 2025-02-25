using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.Secondary;

internal class SecondaryPage(SecondPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
           new Button()
               .SetText("Back")
               .SetTextSize(20)
               .SetCommand(vm.NavCommand)
               .SetTextColor(SKColors.Black)
               .SetBackgroundColor(SKColors.White))
           .SetHorizontalAlignment(HorizontalAlignment.Center)
           .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
