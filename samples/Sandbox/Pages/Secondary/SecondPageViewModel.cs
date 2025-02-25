using PlusUi.core;
using Sandbox.Pages.Main;
using System.Windows.Input;

namespace Sandbox.Pages.Secondary;

internal class SecondPageViewModel(INavigationService navigationService) : ViewModelBase
{
    public ICommand NavCommand { get; } = new SyncCommand(navigationService.NavigateTo<MainPage>);
}
