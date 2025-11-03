using CommunityToolkit.Mvvm.ComponentModel;

namespace Sandbox.Pages.NewControlsDemo;

public partial class NewControlsDemoPageViewModel : ObservableObject
{
    // Toggle state
    public bool IsToggled
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    // Progress value (0.0 to 1.0)
    public float Progress
    {
        get => field = 0.65f;
        set => SetProperty(ref field, value);
    }

    // Slider value
    public float SliderValue
    {
        get => field = 50f;
        set => SetProperty(ref field, value);
    }

    // Entry text for placeholder demo
    public string EntryText
    {
        get => field = string.Empty;
        set => SetProperty(ref field, value);
    }

    // Entry text for MaxLength demo
    public string MaxLengthText
    {
        get => field = string.Empty;
        set => SetProperty(ref field, value);
    }

    // ActivityIndicator running state
    public bool IsLoading
    {
        get => field = true;
        set => SetProperty(ref field, value);
    }
}
