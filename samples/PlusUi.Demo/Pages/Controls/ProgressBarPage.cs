using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class ProgressBarPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "ProgressBar";

    protected override string Description =>
        "Shows determinate progress as a filled capsule bar. Stretches to fill the available width.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Determinate",
            Demo("0%", new ProgressBar()),
            Demo("25%", new ProgressBar().SetProgress(0.25f)),
            Demo("50%", new ProgressBar().SetProgress(0.5f)),
            Demo("75%", new ProgressBar().SetProgress(0.75f)),
            Demo("100%", new ProgressBar().SetProgress(1f))),

        Section("Custom color",
            Demo("Success", new ProgressBar().SetProgress(0.6f).SetProgressColor(PlusUiDefaults.AccentSuccess))),
    ];
}
