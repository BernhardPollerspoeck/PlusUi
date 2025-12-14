using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;

namespace Sandbox.Pages.ControlsGrid;

internal partial class ControlsGridPageViewModel(INavigationService navigationService) : ObservableObject
{
    [ObservableProperty]
    private int _rowHeight = 20;

    [ObservableProperty]
    private string? _title;

    [RelayCommand]
    private void Increment()
    {
        RowHeight += 10;
    }

    [RelayCommand]
    private void Nav()
    {
        navigationService.GoBack();
    }
}
