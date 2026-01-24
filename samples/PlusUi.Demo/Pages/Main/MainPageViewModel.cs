using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.Demo.Pages.EntryDemo;
using PlusUi.Demo.Pages.RichTextLabelDemo;

namespace PlusUi.Demo.Pages.Main;

public partial class MainPageViewModel(INavigationService navigation) : ObservableObject
{
    public string[] Controls { get; } = [
        "ActivityIndicator",
        "Border",
        "Button",
        "Checkbox",
        "ComboBox",
        "ContextMenu",
        "DataGrid",
        "DatePicker",
        "Entry",
        "Gestures",
        "Grid",
        "HStack",
        "Image",
        "ItemsList",
        "Label",
        "LineGraph",
        "Link",
        "Menu",
        "ProgressBar",
        "RadioButton",
        "RichTextLabel",
        "ScrollView",
        "Separator",
        "Slider",
        "Solid",
        "TabControl",
        "TimePicker",
        "Toggle",
        "Toolbar",
        "TreeView",
        "UniformGrid",
        "UserControl",
        "VStack"
    ];

    [RelayCommand]
    private void NavigateToDemo(string? controlName)
    {
        switch (controlName)
        {
            case "Entry":
                navigation.NavigateTo<EntryPage>();
                break;
            case "RichTextLabel":
                navigation.NavigateTo<RichTextLabelPage>();
                break;
        }
    }
}
