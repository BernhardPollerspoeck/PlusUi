using CommunityToolkit.Mvvm.ComponentModel;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class TogglePageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    private bool _enabled = true;

    public string StatusText => Enabled ? "Enabled" : "Disabled";
}

public class TogglePage(TogglePageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Toggle";

    protected override string Description =>
        "An on/off switch in the iOS/Android style with a sliding thumb.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("States",
            Demo("Off (default)", new Toggle()),
            Demo("On", new Toggle().SetIsOn(true))),

        Section("Interactive",
            new HStack()
                .SetSpacing(8)
                .AddChild(new Toggle().BindIsOn(() => vm.Enabled, v => vm.Enabled = v))
                .AddChild(new Label().BindText(() => vm.StatusText))),
    ];
}
