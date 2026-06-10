using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class SolidPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Solid";

    protected override string Description =>
        "A simple solid-colored rectangle. Useful as a colored block, spacer or divider.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Colored blocks",
            new HStack()
                .SetSpacing(12)
                .AddChild(new Solid(60, 60, PlusUiDefaults.AccentError))
                .AddChild(new Solid(60, 60, PlusUiDefaults.AccentSuccess))
                .AddChild(new Solid(60, 60, PlusUiDefaults.AccentPrimary))),

        Section("As a vertical divider",
            new HStack()
                .SetSpacing(12)
                .AddChild(new Label().SetText("Left"))
                .AddChild(new Solid(2, 24, PlusUiDefaults.BorderColor))
                .AddChild(new Label().SetText("Right"))),

        Section("Note",
            Note("A bare new Solid() has no color and stretches to fill, so it renders nothing until you give it a color or size.")),
    ];
}
