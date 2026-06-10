using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace PlusUi.Demo.Pages.Shared;

/// <summary>
/// Base view model for the simple control demo pages. Provides the shared
/// "back to controls" navigation. Pages that need their own state derive from this.
/// </summary>
public partial class DemoPageViewModel(INavigationService navigation) : ObservableObject
{
    [RelayCommand]
    private void GoBack() => navigation.GoBack();
}
