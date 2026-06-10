using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class ToolbarPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Toolbar";

    protected override string Description =>
        "A horizontal app bar with a title and left/right aligned content.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            Note("A bare Toolbar has no background or height of its own, so it is nearly invisible. Here a background and height are set so it reads as a bar."),
            new Toolbar()
                .SetTitle("My App")
                .SetBackground(PlusUiDefaults.BackgroundControl)
                .SetDesiredHeight(48)
                .AddLeft(new Button().SetText("Menu"))
                .AddRight(new Button().SetText("Search"))),

        Section("Title alignment",
            new Toolbar()
                .SetTitle("Centered title")
                .SetTitleAlignment(TitleAlignment.Center)
                .SetBackground(PlusUiDefaults.BackgroundControl)
                .SetDesiredHeight(48)
                .AddLeft(new Button().SetText("Back"))),
    ];
}
