using CommunityToolkit.Mvvm.ComponentModel;
using PlusUi.core;
using Sandbox.Pages.Main;
using System.ComponentModel;
using System.Windows.Input;

namespace Sandbox.Pages.ControlsGrid;

internal class ControlsGridPageViewModel : ObservableObject
{
    [ObservableProperty]
    private int _rowHeight = 20;

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
