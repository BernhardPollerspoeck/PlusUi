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

        Section("Stroke colors & thickness",
            new Border()
                .SetStrokeColor(PlusUiDefaults.BorderColor)
                .SetStrokeThickness(1)
                .AddChild(new Label().SetText("1px themed border").SetMargin(new Margin(12))),
            new Border()
                .SetStrokeColor(PlusUiDefaults.AccentPrimary)
                .SetStrokeThickness(3)
                .AddChild(new Label().SetText("3px accent border").SetMargin(new Margin(12)))),

        Section("Stroke types",
            new Border()
                .SetStrokeColor(PlusUiDefaults.BorderColor)
                .SetStrokeThickness(2)
                .SetStrokeType(StrokeType.Dashed)
                .AddChild(new Label().SetText("Dashed").SetMargin(new Margin(12))),
            new Border()
                .SetStrokeColor(PlusUiDefaults.BorderColor)
                .SetStrokeThickness(2)
                .SetStrokeType(StrokeType.Dotted)
                .AddChild(new Label().SetText("Dotted").SetMargin(new Margin(12)))),

        Section("Corner radius",
            new Border()
                .SetBackground(PlusUiDefaults.BackgroundControl)
                .SetStrokeThickness(0)
                .SetCornerRadius(4)
                .AddChild(new Label().SetText("Radius 4").SetMargin(new Margin(12))),
            new Border()
                .SetBackground(PlusUiDefaults.BackgroundControl)
                .SetStrokeThickness(0)
                .SetCornerRadius(12)
                .AddChild(new Label().SetText("Radius 12").SetMargin(new Margin(12))),
            new Border()
                .SetBackground(PlusUiDefaults.BackgroundControl)
                .SetStrokeThickness(0)
                .SetCornerRadius(24)
                .AddChild(new Label().SetText("Radius 24 (pill-ish)").SetMargin(new Margin(16, 10)))),

        Section("Background + border combined",
            new Border()
                .SetBackground(PlusUiDefaults.BackgroundControl)
                .SetCornerRadius(8)
                .SetStrokeColor(PlusUiDefaults.AccentPrimary)
                .SetStrokeThickness(2)
                .AddChild(new Label().SetText("Filled card with accent outline").SetMargin(new Margin(16)))),
    ];
}
