using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PlusUiApp.Pages;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private int _clickCount;

    [RelayCommand]
    private void Click()
    {
        ClickCount++;
    }
}
