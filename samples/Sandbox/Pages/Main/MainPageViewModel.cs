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

    public Color Color
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
        var random = Random.Shared.Next(0xFF0000, 0xFFFFFF);
        Color = new Color((byte)((random >> 16) & 0xFF), (byte)((random >> 8) & 0xFF), (byte)(random & 0xFF));
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
            onClosed: () => Color = Colors.Green,
            configure: cfg =>
            {
                cfg.CloseOnBackgroundClick = true;
                cfg.CloseOnEscape = true;
                cfg.BackgroundColor = new Color(200, 0, 0, 220);
            });
    }

}

