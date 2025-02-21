using PlusUi.core;
using System.Windows.Input;

namespace PlusUi;



public class MainPage(MainViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("I have been clicked {vm.Count} times")
                .SetTextSize(20)
                .SetHorizontalAlignment(HorizontalAlignment.Center),
            new Button()
                .SetText("Light")
                .SetTextSize(20)
                .SetPadding(new(10, 5))
                .SetCommand(vm.ThemeCommand)
                .SetCommandParameter(Theme.Light)
                .SetHorizontalAlignment(HorizontalAlignment.Center),
            new Button()
                .SetText("Dark")
                .SetTextSize(20)
                .SetPadding(new(10, 5))
                .SetCommand(vm.ThemeCommand)
                .SetCommandParameter(Theme.Dark)
                .SetHorizontalAlignment(HorizontalAlignment.Center),
            new Button()
                .SetText("Blue")
                .SetTextSize(20)
                .SetPadding(new(10, 5))
                .SetCommand(vm.ThemeCommand)
                .SetCommandParameter("Blue")
                .SetHorizontalAlignment(HorizontalAlignment.Center))
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);
    }
}


public class MainViewModel : ViewModelBase
{

    public ICommand ThemeCommand { get; }

    public MainViewModel(IThemeService themeService)
    {
        ThemeCommand = new SyncCommand((p) =>
        {
            if (p is Theme theme)
            {
                themeService.SetTheme(theme);
            }
            else if (p is string color)
            {
                themeService.SetTheme(color);
            }
        });
    }
}






