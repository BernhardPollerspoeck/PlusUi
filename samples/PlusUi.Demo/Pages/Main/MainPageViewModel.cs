using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.Demo.Pages.Controls;

namespace PlusUi.Demo.Pages.Main;

/// <summary>A row in the grouped sidebar: either a group header or a control entry.</summary>
public abstract record SidebarRow;
public sealed record SidebarHeader(string Title) : SidebarRow;
public sealed record SidebarItem(string Name, bool Available) : SidebarRow;

public partial class MainPageViewModel : ObservableObject
{
    private readonly Dictionary<string, Action> _navigators;

    /// <summary>Flattened group headers + control items for the sidebar list.</summary>
    public List<SidebarRow> Rows { get; } = [];

    public MainPageViewModel(INavigationService navigation)
    {
        _navigators = new()
        {
            ["Label"] = () => navigation.NavigateTo<LabelPage>(),
            ["RichTextLabel"] = () => navigation.NavigateTo<RichTextLabelPage>(),
            ["Entry"] = () => navigation.NavigateTo<EntryPage>(),
            ["CodeEditor"] = () => navigation.NavigateTo<CodeEditorPage>(),
            ["Button"] = () => navigation.NavigateTo<ButtonPage>(),
            ["Checkbox"] = () => navigation.NavigateTo<CheckboxPage>(),
            ["RadioButton"] = () => navigation.NavigateTo<RadioButtonPage>(),
            ["Toggle"] = () => navigation.NavigateTo<TogglePage>(),
            ["Slider"] = () => navigation.NavigateTo<SliderPage>(),
            ["Separator"] = () => navigation.NavigateTo<SeparatorPage>(),
            ["ProgressBar"] = () => navigation.NavigateTo<ProgressBarPage>(),
            ["ActivityIndicator"] = () => navigation.NavigateTo<ActivityIndicatorPage>(),
            ["Border"] = () => navigation.NavigateTo<BorderPage>(),
            ["Image"] = () => navigation.NavigateTo<ImagePage>(),
            ["Solid"] = () => navigation.NavigateTo<SolidPage>(),
            ["GameCanvas"] = () => navigation.NavigateTo<GameCanvasPage>(),
            ["Link"] = () => navigation.NavigateTo<LinkPage>(),
            ["TabControl"] = () => navigation.NavigateTo<TabControlPage>(),
            ["Menu"] = () => navigation.NavigateTo<MenuPage>(),
            ["ContextMenu"] = () => navigation.NavigateTo<ContextMenuPage>(),
            ["Toolbar"] = () => navigation.NavigateTo<ToolbarPage>(),
            ["Tooltip"] = () => navigation.NavigateTo<TooltipPage>(),
            ["LineGraph"] = () => navigation.NavigateTo<LineGraphPage>(),
            ["Scrollbar"] = () => navigation.NavigateTo<ScrollbarPage>(),
            ["Gestures"] = () => navigation.NavigateTo<GesturesPage>(),
            ["Hover & Cursor"] = () => navigation.NavigateTo<HoverPage>(),
            ["UserControl"] = () => navigation.NavigateTo<UserControlPage>(),
            ["Grid"] = () => navigation.NavigateTo<GridPage>(),
            ["HStack"] = () => navigation.NavigateTo<HStackPage>(),
            ["VStack"] = () => navigation.NavigateTo<VStackPage>(),
            ["UniformGrid"] = () => navigation.NavigateTo<UniformGridPage>(),
            ["ScrollView"] = () => navigation.NavigateTo<ScrollViewPage>(),
            ["ComboBox"] = () => navigation.NavigateTo<ComboBoxPage>(),
            ["DatePicker"] = () => navigation.NavigateTo<DatePickerPage>(),
            ["TimePicker"] = () => navigation.NavigateTo<TimePickerPage>(),
            ["ItemsList"] = () => navigation.NavigateTo<ItemsListPage>(),
            ["TreeView"] = () => navigation.NavigateTo<TreeViewPage>(),
            ["DataGrid"] = () => navigation.NavigateTo<DataGridPage>(),
        };

        AddGroup("Text", "Label", "RichTextLabel", "Link", "Entry", "CodeEditor");
        AddGroup("Buttons & Selection", "Button", "Checkbox", "RadioButton", "Toggle", "Slider");
        AddGroup("Pickers & Lists", "ComboBox", "DatePicker", "TimePicker", "ItemsList", "TreeView", "DataGrid");
        AddGroup("Layout", "Border", "Grid", "HStack", "VStack", "UniformGrid", "ScrollView", "Separator");
        AddGroup("Navigation & Menus", "TabControl", "Menu", "ContextMenu", "Toolbar");
        AddGroup("Indicators", "ActivityIndicator", "ProgressBar", "LineGraph", "Tooltip");
        AddGroup("Graphics & Media", "Image", "Solid", "GameCanvas");
        AddGroup("Advanced", "Gestures", "Hover & Cursor", "UserControl", "Scrollbar");
    }

    private void AddGroup(string title, params string[] names)
    {
        Rows.Add(new SidebarHeader(title));
        foreach (var name in names)
            Rows.Add(new SidebarItem(name, _navigators.ContainsKey(name)));
    }

    [RelayCommand]
    private void NavigateToDemo(string? controlName)
    {
        if (controlName is not null && _navigators.TryGetValue(controlName, out var navigate))
            navigate();
    }
}
