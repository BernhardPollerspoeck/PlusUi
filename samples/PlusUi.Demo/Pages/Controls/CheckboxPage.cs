using CommunityToolkit.Mvvm.ComponentModel;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class CheckboxPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AcceptedText))]
    private bool _accepted;

    public string AcceptedText => Accepted ? "Accepted" : "Not accepted";
}

public class CheckboxPage(CheckboxPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Checkbox";

    protected override string Description =>
        "A binary on/off control with a checkmark indicator. It has no caption of its own - pair it with a Label.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("States",
            Demo("Unchecked (default)", new Checkbox()),
            Demo("Checked", new Checkbox().SetIsChecked(true))),

        Section("Interactive",
            new HStack()
                .SetSpacing(8)
                .AddChild(new Checkbox().BindIsChecked(() => vm.Accepted, v => vm.Accepted = v))
                .AddChild(new Label().BindText(() => vm.AcceptedText))),
    ];
}
