using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;

namespace Sandbox.Pages.BgTest;

public partial class BgTestPageViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void Nav()
    {
        navigationService.NavigateTo<MainPage>();
    }
}
