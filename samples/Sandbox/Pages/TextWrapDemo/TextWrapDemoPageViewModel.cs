using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;

namespace Sandbox.Pages.TextWrapDemo;

public partial class TextWrapDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    public string SampleText => "This is a long sample text that demonstrates text wrapping and truncation features in PlusUI. It will wrap across multiple lines or truncate based on the settings you choose.";
    
    public string ShortText => "Short text";
    
    public string LongText => "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
    
    [RelayCommand]
    private void Nav()
    {
        navigationService.NavigateTo<MainPage>();
    }
}
