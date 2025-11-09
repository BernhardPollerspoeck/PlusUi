using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Popups;
using SkiaSharp;
using System.Windows.Input;

namespace Sandbox.Pages.Main;

public partial class MainPageViewModel(INavigationService navigationService, IPopupService popupService) : ObservableObject
{
    public string? Text
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public SKColor Color
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public bool Checked
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    [RelayCommand]
    private void SetColor()
    {
        Color = new SKColor((uint)Random.Shared.Next(0xFF0000, 0xFFFFFF) | 0xFF000000);
    }

    [RelayCommand]
    private void Navigate(object? arg)
    {
        if (arg is Type pageType)
        {
            // Use reflection to call the generic NavigateTo method with the provided type
            typeof(INavigationService)
                .GetMethod(nameof(INavigationService.NavigateTo))
                ?.MakeGenericMethod(pageType)
                .Invoke(navigationService, ["Tree is tall"]);
        }
    }

    [RelayCommand]
    private void Popup()
    {
        popupService.ShowPopup<TestPopup, string>(
            arg: "Some Argument",
            onClosed: () => Color = SKColors.Green,
            configure: cfg =>
            {
                cfg.CloseOnBackgroundClick = true;
                cfg.CloseOnEscape = true;
                cfg.BackgroundColor = new SKColor(0, 0, 0, 220);
            });
    }

}

