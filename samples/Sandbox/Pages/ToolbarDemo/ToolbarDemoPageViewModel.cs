using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;

namespace Sandbox.Pages.ToolbarDemo;

public partial class ToolbarDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void GoBack() => navigationService.NavigateTo<MainPage>();

    [ObservableProperty]
    private int _clickCount;

    [ObservableProperty]
    private string _lastAction = "No action yet";

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private bool _isBookmarked;

    [RelayCommand]
    private void IncrementClick()
    {
        ClickCount++;
        LastAction = $"Clicked {ClickCount} times";
    }

    [RelayCommand]
    private void ToggleLike()
    {
        IsLiked = !IsLiked;
        LastAction = IsLiked ? "Liked!" : "Unliked";
    }

    [RelayCommand]
    private void ToggleBookmark()
    {
        IsBookmarked = !IsBookmarked;
        LastAction = IsBookmarked ? "Bookmarked!" : "Removed bookmark";
    }

    [RelayCommand]
    private void Search()
    {
        LastAction = "Search clicked!";
    }

    [RelayCommand]
    private void Share()
    {
        LastAction = "Share clicked!";
    }

    [RelayCommand]
    private void Settings()
    {
        LastAction = "Settings clicked!";
    }

    [RelayCommand]
    private void Menu()
    {
        LastAction = "Menu clicked!";
    }
}
