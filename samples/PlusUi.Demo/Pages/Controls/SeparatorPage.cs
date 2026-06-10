using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class SeparatorPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Separator";

    protected override string Description =>
        "A thin divider line for separating sections, horizontal or vertical.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Horizontal (default)",
            new Label().SetText("Above"),
            new Separator(),
            new Label().SetText("Below")),

        Section("Custom thickness & color",
            new Label().SetText("Above"),
            new Separator().SetThickness(3).SetColor(PlusUiDefaults.AccentPrimary),
            new Label().SetText("Below")),

        Section("Vertical",
            new HStack()
                .SetSpacing(12)
                .AddChild(new Label().SetText("Left"))
                .AddChild(new Separator().SetOrientation(Orientation.Vertical))
                .AddChild(new Label().SetText("Right"))),
    ];
}
