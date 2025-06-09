using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;
using System.Windows.Input;

namespace Sandbox.Pages.TextRendering;

public partial class TextRenderPageViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void Nav()
    {
        navigationService.NavigateTo<MainPage>();
    }

}
