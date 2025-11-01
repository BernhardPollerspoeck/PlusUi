using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;

namespace Sandbox.Pages.FontDemo;

public partial class FontDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    [ObservableProperty]
    private string _selectedFontFamily = "System Default";

    [ObservableProperty]
    private FontWeight _selectedFontWeight = FontWeight.Regular;

    [ObservableProperty]
    private FontStyle _selectedFontStyle = FontStyle.Normal;

    [RelayCommand]
    private void Nav()
    {
        navigationService.NavigateTo<MainPage>();
    }

    [RelayCommand]
    private void SetRegularWeight()
    {
        SelectedFontWeight = FontWeight.Regular;
    }

    [RelayCommand]
    private void SetBoldWeight()
    {
        SelectedFontWeight = FontWeight.Bold;
    }

    [RelayCommand]
    private void SetLightWeight()
    {
        SelectedFontWeight = FontWeight.Light;
    }

    [RelayCommand]
    private void ToggleItalic()
    {
        SelectedFontStyle = SelectedFontStyle == FontStyle.Normal ? FontStyle.Italic : FontStyle.Normal;
    }
}
