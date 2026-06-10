using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class ActivityIndicatorPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "ActivityIndicator";

    protected override string Description =>
        "A spinning arc that indicates ongoing activity. Animates by default.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            Demo("Spinning (40x40)", new ActivityIndicator())),

        Section("Variations",
            Demo("Slow", new ActivityIndicator().SetSpeed(0.5f)),
            Demo("Thick stroke", new ActivityIndicator().SetStrokeThickness(6)),
            Demo("Custom color", new ActivityIndicator().SetColor(PlusUiDefaults.AccentSuccess))),
    ];
}
