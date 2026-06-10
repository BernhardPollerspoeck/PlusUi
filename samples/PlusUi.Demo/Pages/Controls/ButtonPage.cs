using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class ButtonPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Button";

    protected override string Description =>
        "A clickable button with text, command binding and a built-in hover state.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            Demo("Bare button, no styling",
                new Button().SetText("Click me"))),

        Section("In a row",
            new HStack()
                .SetSpacing(8)
                .AddChild(new Button().SetText("One"))
                .AddChild(new Button().SetText("Two"))
                .AddChild(new Button().SetText("Three"))),

        Section("Stretched",
            new Button()
                .SetText("Full width")
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)),
    ];
}
