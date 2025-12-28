using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.TabControlDemo;

public partial class TabControlDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void GoBack() => navigationService.GoBack();

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private int _verticalTabIndex;

    [ObservableProperty]
    private TabPosition _dynamicTabPosition = TabPosition.Top;

    [RelayCommand]
    private void CycleTabPosition()
    {
        DynamicTabPosition = DynamicTabPosition switch
        {
            TabPosition.Top => TabPosition.Right,
            TabPosition.Right => TabPosition.Bottom,
            TabPosition.Bottom => TabPosition.Left,
            TabPosition.Left => TabPosition.Top,
            _ => TabPosition.Top
        };
    }
}
