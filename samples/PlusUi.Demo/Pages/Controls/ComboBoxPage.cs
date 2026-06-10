using CommunityToolkit.Mvvm.ComponentModel;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class ComboBoxPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    private string _selected = "Nothing selected yet";
}

public class ComboBoxPage(ComboBoxPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "ComboBox";

    protected override string Description =>
        "A dropdown for selecting a single item from a list. Generic over the item type.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            Note("The closed box looks fine, but the open dropdown has a bright white border and an almost-invisible selection highlight (known issues)."),
            new ComboBox<string>()
                .SetItemsSource(new[] { "Red", "Green", "Blue", "Yellow" })
                .SetPlaceholder("Choose a color...")
                .SetOnSelectionChanged(v => vm.Selected = v ?? "none")),

        Section("Selected value",
            new Label().BindText(() => vm.Selected)),
    ];
}
