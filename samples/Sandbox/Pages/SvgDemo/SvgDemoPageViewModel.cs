using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.SvgDemo;

public partial class SvgDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void GoBack() => navigationService.GoBack();
}
