using PlusUi.core;
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

    public ICommand SetColorCommand { get; }

    public MainViewModel()
    {
        SetColorCommand = new SyncCommand(SetColor);
    }

    private void SetColor()
    {
        Color = new SKColor((uint)Random.Shared.Next(0xFF0000, 0xFFFFFF) | 0xFF000000);
    }

}






