using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;
using System.Windows.Input;

namespace Sandbox.Pages.ScrollViewDemo;

internal partial class ScrollViewExamplePageViewModel(INavigationService navigationService) : ObservableObject
{
    public string LongText
    {
        get => field;
        set => SetProperty(ref field, value);
    } = "This is a long text that should scroll both horizontally and vertically " +
        "to demonstrate the ScrollView capabilities. You can drag to scroll in both directions. " +
        "This text should be long enough to demonstrate vertical scrolling with multiple lines. " +
        "The ScrollView control supports both horizontal and vertical scrolling as needed.";

    public bool IsHorizontalScrollingEnabled
    {
        get => field;
        set => SetProperty(ref field, value);
    } = true;

    public bool IsVerticalScrollingEnabled
    {
        get => field;
        set => SetProperty(ref field, value);
    } = true;


    [RelayCommand]
    private void Nav()
    {
        navigationService.NavigateTo<MainPage>();
    }
}