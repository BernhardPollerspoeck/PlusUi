using CommunityToolkit.Mvvm.ComponentModel;

namespace PlusUi.Demo.Pages.Main;

public partial class MainPageViewModel : ObservableObject
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
}
