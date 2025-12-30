using PlusUi.core;

namespace PlusUiApp.Pages;

public class MainPage(MainPageViewModel viewModel) : UiPage<MainPageViewModel>(viewModel)
{
    protected override UiElement Build(MainPageViewModel vm) =>
        new VStack()
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .AddChildren(
                new Label()
                    .SetText("Welcome to PlusUi!")
                    .SetTextSize(32)
                    .SetFontWeight(FontWeight.Bold)
                    .SetTextColor(Colors.White),
                new Label()
                    .SetText("Start building your cross-platform app")
                    .SetTextSize(16)
                    .SetTextColor(new Color(180, 180, 180))
                    .SetMargin(new Margin(0, 8, 0, 24)),
                new Button()
                    .SetText("Click Me!")
                    .SetCommand(vm.ClickCommand)
                    .SetPadding(new Margin(24, 12)),
                new Label()
                    .BindText(nameof(vm.ClickCount), () => $"Clicked {vm.ClickCount} times")
                    .SetTextSize(14)
                    .SetTextColor(new Color(150, 150, 150))
                    .SetMargin(new Margin(0, 16, 0, 0))
            );
}
