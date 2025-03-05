using PlusUi.core;
using Sandbox.Pages.Main;
using System.Windows.Input;

namespace Sandbox.Pages.TextRendering;

public class TextRenderPageViewModel(INavigationService navigationService) : ViewModelBase
{
    public ICommand NavCommand { get; } = new SyncCommand(() => navigationService.NavigateTo<MainPage>());
}
