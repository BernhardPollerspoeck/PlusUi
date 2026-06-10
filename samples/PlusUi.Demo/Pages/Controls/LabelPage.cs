using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class LabelPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Label";

    protected override string Description =>
        "Displays text. Supports size, weight, color, alignment and wrapping.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            new Label().SetText("The quick brown fox jumps over the lazy dog.")),

        Section("Sizes",
            new Label().SetText("Small (12)").SetTextSize(12),
            new Label().SetText("Default (14)"),
            new Label().SetText("Large (18)").SetTextSize(18),
            new Label().SetText("Huge (28)").SetTextSize(28)),

        Section("Weights",
            new Label().SetText("Regular"),
            new Label().SetText("SemiBold").SetFontWeight(FontWeight.SemiBold),
            new Label().SetText("Bold").SetFontWeight(FontWeight.Bold)),

        Section("Colors",
            new Label().SetText("Primary text"),
            new Label().SetText("Secondary text").SetTextColor(PlusUiDefaults.TextSecondary),
            new Label().SetText("Accent").SetTextColor(PlusUiDefaults.AccentPrimary)),

        Section("Wrapping",
            new Label()
                .SetText("This is a long line of text that wraps across multiple lines when WordWrap is enabled and the available width is limited.")
                .SetTextWrapping(TextWrapping.WordWrap)
                .SetDesiredWidth(360)),
    ];
}
