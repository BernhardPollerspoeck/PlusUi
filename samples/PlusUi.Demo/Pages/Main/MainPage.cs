using PlusUi.core;

namespace PlusUi.Demo.Pages.Main;

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new Label()
            .SetText("Hello World")
            .SetTextSize(32)
            .SetTextColor(Colors.White)
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
