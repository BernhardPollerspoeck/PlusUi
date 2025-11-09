using CommunityToolkit.Mvvm.ComponentModel;

namespace Sandbox.Pages.StyleDemo;

public partial class StyleDemoPageViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isToggled = false;

    [ObservableProperty]
    private bool _isChecked = false;

    [ObservableProperty]
    private double _sliderValue = 0.5;

    [ObservableProperty]
    private double _progressValue = 0.7;

    [ObservableProperty]
    private string _entryText = "";
}
