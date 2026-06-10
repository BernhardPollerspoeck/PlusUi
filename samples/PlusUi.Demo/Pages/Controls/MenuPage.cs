using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class MenuPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    private string _lastAction = "No action yet";

    [RelayCommand] private void New() => LastAction = "File > New";
    [RelayCommand] private void Open() => LastAction = "File > Open";
    [RelayCommand] private void Save() => LastAction = "File > Save";
    [RelayCommand] private void Cut() => LastAction = "Edit > Cut";
    [RelayCommand] private void Copy() => LastAction = "Edit > Copy";
}

public class MenuPage(MenuPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Menu";

    protected override string Description =>
        "A horizontal menu bar with nested items, separators, shortcuts and commands.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Menu bar",
            new Menu()
                .AddItem(new MenuItem()
                    .SetText("File")
                    .AddItem(new MenuItem().SetText("New").SetShortcut("Ctrl+N").SetCommand(vm.NewCommand))
                    .AddItem(new MenuItem().SetText("Open").SetShortcut("Ctrl+O").SetCommand(vm.OpenCommand))
                    .AddSeparator()
                    .AddItem(new MenuItem().SetText("Save").SetShortcut("Ctrl+S").SetCommand(vm.SaveCommand)))
                .AddItem(new MenuItem()
                    .SetText("Edit")
                    .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X").SetCommand(vm.CutCommand))
                    .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C").SetCommand(vm.CopyCommand)))),

        Section("Last action",
            new Label().BindText(() => vm.LastAction)),
    ];
}
