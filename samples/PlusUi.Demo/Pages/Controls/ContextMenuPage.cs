using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class ContextMenuPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    private string _lastAction = "Right-click the box and pick an item";

    [RelayCommand] private void Cut() => LastAction = "Cut";
    [RelayCommand] private void Copy() => LastAction = "Copy";
    [RelayCommand] private void Delete() => LastAction = "Delete";
}

public class ContextMenuPage(ContextMenuPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "ContextMenu";

    protected override string Description =>
        "A right-click popup menu attached to any element via the SetContextMenu extension.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Right-click target",
            new Border()
                .SetBackground(PlusUiDefaults.BackgroundControl)
                .SetCornerRadius(8)
                .SetStrokeThickness(0)
                .AddChild(new Label().SetText("Right-click me").SetMargin(new Margin(24)))
                .SetContextMenu(new ContextMenu()
                    .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X").SetCommand(vm.CutCommand))
                    .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C").SetCommand(vm.CopyCommand))
                    .AddSeparator()
                    .AddItem(new MenuItem().SetText("Delete").SetCommand(vm.DeleteCommand)))),

        Section("Last action",
            new Label().BindText(() => vm.LastAction)),
    ];
}
