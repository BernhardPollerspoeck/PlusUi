using PlusUi.core;
using SkiaSharp;
using System.Windows.Input;

namespace PlusUi;

public class SecondPage(SecondPageViewModel vm) : UiPageElement(vm)
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

public class SecondPageViewModel(INavigationService navigationService) : ViewModelBase
{
    public ICommand NavCommand { get; } = new SyncCommand(navigationService.NavigateTo<MainPage>);
}





