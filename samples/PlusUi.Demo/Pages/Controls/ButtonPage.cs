using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class ButtonPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CountText))]
    private int _count;

    public string CountText => $"Total clicks: {Count}";

    [RelayCommand]
    private void Click() => Count++;
}

public class ButtonPage(ButtonPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Button";

    protected override string Description =>
        "A clickable button with text, command binding and a built-in hover state.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Click counter",
            Note("Every button on this page is wired to the same command - click any of them."),
            new HStack()
                .SetSpacing(8)
                .AddChild(new Button().SetText("One").SetCommand(vm.ClickCommand))
                .AddChild(new Button().SetText("Two").SetCommand(vm.ClickCommand))
                .AddChild(new Button().SetText("Three").SetCommand(vm.ClickCommand)),
            new Label().BindText(() => vm.CountText).SetFontWeight(FontWeight.Bold)),

        Section("Default",
            Demo("Bare button, no styling",
                new Button().SetText("Click me").SetCommand(vm.ClickCommand))),

        Section("Stretched",
            Note("A Stretch button fills the width of its container (here a 400px-wide box)."),
            new VStack()
                .SetDesiredWidth(400)
                .AddChild(new Button()
                    .SetText("Full width")
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .SetCommand(vm.ClickCommand))),
    ];
}
