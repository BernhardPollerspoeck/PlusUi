using PlusUi.core;
using Sandbox.Pages.Main;
using System.Windows.Input;

namespace Sandbox.Pages.ControlsGrid;

internal class ControlsGridPageViewModel : ViewModelBase
{
    public int RowHeight
    {
        get => field;
        set => SetProperty(ref field, value);
    } = 20;

    public ICommand NavCommand { get; }
    public ICommand IncrementCommand { get; }

    public ControlsGridPageViewModel(INavigationService navigationService)
    {
        NavCommand = new SyncCommand(() => navigationService.NavigateTo<MainPage>());
        IncrementCommand = new SyncCommand(Increment);
    }

    private void Increment()
    {
        RowHeight += 10;
    }
}
