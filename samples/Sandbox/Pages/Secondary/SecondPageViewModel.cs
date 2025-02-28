using PlusUi.core;
using Sandbox.Pages.Main;
using System.Windows.Input;

namespace Sandbox.Pages.Secondary;

internal class SecondPageViewModel : ViewModelBase
{
    public int RowHeight
    {
        get => field;
        set => SetProperty(ref field, value);
    } = 10;

    public ICommand NavCommand { get; }
    public ICommand IncrementCommand { get; }

    public SecondPageViewModel(INavigationService navigationService)
    {
        NavCommand = new SyncCommand(() => navigationService.NavigateTo<MainPage>());
        IncrementCommand = new SyncCommand(Increment);
    }

    private void Increment()
    {
        RowHeight += 10;
    }
}
