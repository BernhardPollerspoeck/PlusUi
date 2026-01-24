using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace PlusUi.Demo.Pages.RichTextLabelDemo;

public partial class RichTextLabelPageViewModel(INavigationService navigation) : ObservableObject
{
    [ObservableProperty]
    private string _dynamicText = "Dynamic";

    [ObservableProperty]
    private Color _highlightColor = Colors.Yellow;

    [RelayCommand]
    private void ToggleHighlight()
    {
        HighlightColor = HighlightColor == Colors.Yellow
            ? Colors.Cyan
            : Colors.Yellow;
    }

    [RelayCommand]
    private void UpdateText()
    {
        DynamicText = DynamicText == "Dynamic" ? "Updated" : "Dynamic";
    }

    [RelayCommand]
    private void GoBack()
    {
        navigation.GoBack();
    }
}
