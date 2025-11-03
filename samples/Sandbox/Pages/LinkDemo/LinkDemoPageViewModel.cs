using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using PlusUi.core;
using Sandbox.Pages.Main;

namespace Sandbox.Pages.LinkDemo;

public partial class LinkDemoPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public ICommand NavCommand { get; }

    public LinkDemoPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavCommand = new RelayCommand(Nav);
    }

    private void Nav()
    {
        _navigationService.NavigateTo<MainPage>();
    }
}
