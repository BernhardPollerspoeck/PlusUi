using PlusUi.core;
using Sandbox.Pages.Secondary;
using SkiaSharp;
using System.Windows.Input;

namespace Sandbox.Pages.Main;

internal class MainViewModel : ViewModelBase
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

    public MainViewModel(INavigationService navigationService)
    {
        SetColorCommand = new SyncCommand(SetColor);
        NavigateCommand = new SyncCommand(() => navigationService.NavigateTo<SecondaryPage>());
    }

    private void SetColor()
    {
        Color = new SKColor((uint)Random.Shared.Next(0xFF0000, 0xFFFFFF) | 0xFF000000);
    }

}






