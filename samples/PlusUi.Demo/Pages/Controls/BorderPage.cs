using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class BorderPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Border";

    protected override string Description =>
        "A container that wraps a single child with a stroke, background and optional rounded corners.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            Note("A bare Border draws a 1px solid black outline with sharp corners - barely visible on the dark theme."),
            new Border()
                .AddChild(new Label().SetText("Default border").SetMargin(new Margin(12)))),

        Section("Styled",
            new Border()
                .SetBackground(PlusUiDefaults.BackgroundControl)
                .SetCornerRadius(8)
                .SetStrokeColor(PlusUiDefaults.BorderColor)
                .SetStrokeThickness(1)
                .AddChild(new Label().SetText("Background + rounded corners + themed stroke").SetMargin(new Margin(12)))),

        Section("Stroke types",
            new Border()
                .SetStrokeColor(PlusUiDefaults.BorderColor)
                .SetStrokeThickness(2)
                .SetStrokeType(StrokeType.Dashed)
                .AddChild(new Label().SetText("Dashed stroke").SetMargin(new Margin(12)))),
    ];
}
