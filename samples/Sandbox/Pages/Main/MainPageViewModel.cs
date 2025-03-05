using PlusUi.core;
using Sandbox.Pages.ControlsGrid;
using SkiaSharp;
using System.Windows.Input;

namespace Sandbox.Pages.Main;

internal class MainPageViewModel : ViewModelBase
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

    public ICommand SetColorCommand { get; }
    public ICommand NavigateCommand { get; }

    private readonly INavigationService _navigationService;

    public MainPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        SetColorCommand = new SyncCommand(SetColor);
        NavigateCommand = new SyncCommand(Navigate);
    }

    private void SetColor()
    {
        Color = new SKColor((uint)Random.Shared.Next(0xFF0000, 0xFFFFFF) | 0xFF000000);
    }

    private void Navigate(object? arg)
    {
        if (arg is Type pageType)
        {
            // Use reflection to call the generic NavigateTo method with the provided type
            typeof(INavigationService)
                .GetMethod(nameof(INavigationService.NavigateTo))
                ?.MakeGenericMethod(pageType)
                .Invoke(_navigationService, null);
        }
    }
}






