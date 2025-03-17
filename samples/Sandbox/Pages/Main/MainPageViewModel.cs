using PlusUi.core;
using Sandbox.Popups;
using SkiaSharp;
using System.Windows.Input;

namespace Sandbox.Pages.Main;

public class MainPageViewModel : ViewModelBase
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
    public ICommand PopupCommand { get; }

    private readonly INavigationService _navigationService;
    private readonly IPopupService _popupService;

    public MainPageViewModel(INavigationService navigationService, IPopupService popupService)
    {
        _navigationService = navigationService;
        _popupService = popupService;
        SetColorCommand = new SyncCommand(SetColor);
        NavigateCommand = new SyncCommand(Navigate);
        PopupCommand = new SyncCommand(Popup);
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

    private void Popup()
    {
        _popupService.ShowPopup<TestPopup, string>(
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

