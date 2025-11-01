using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.ShadowDemo;

public partial class ShadowDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    public float ShadowBlur
    {
        get => field = 8f;
        set => SetProperty(ref field, value);
    }

    public float ShadowOffsetX
    {
        get => field = 0f;
        set => SetProperty(ref field, value);
    }

    public float ShadowOffsetY
    {
        get => field = 4f;
        set => SetProperty(ref field, value);
    }

    public float ShadowSpread
    {
        get => field = 0f;
        set => SetProperty(ref field, value);
    }

    public byte ShadowAlpha
    {
        get => field = 128;
        set => SetProperty(ref field, value);
    }

    public float CornerRadius
    {
        get => field = 8f;
        set => SetProperty(ref field, value);
    }

    public bool IsHovered
    {
        get => field = false;
        set => SetProperty(ref field, value);
    }

    public int Elevation
    {
        get => field = 2;
        set => SetProperty(ref field, value);
    }

    [RelayCommand]
    private void ToggleHover()
    {
        IsHovered = !IsHovered;
    }

    [RelayCommand]
    private void IncreaseElevation()
    {
        if (Elevation < 6)
            Elevation++;
    }

    [RelayCommand]
    private void DecreaseElevation()
    {
        if (Elevation > 0)
            Elevation--;
    }

    [RelayCommand]
    private void Nav()
    {
        navigationService.NavigateTo<Main.MainPage>();
    }
}
