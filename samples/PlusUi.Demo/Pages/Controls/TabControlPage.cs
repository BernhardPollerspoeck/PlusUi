using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class TabControlPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "TabControl";

    protected override string Description =>
        "Organizes content into selectable tabs. Each tab has a header and a content element.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            new TabControl()
                .AddTab(new TabItem()
                    .SetHeader("General")
                    .SetContent(new VStack()
                        .SetSpacing(8)
                        .SetMargin(new Margin(12))
                        .AddChild(new Label().SetText("General settings"))
                        .AddChild(new Label().SetText("Content of the first tab.").SetTextColor(PlusUiDefaults.TextSecondary))))
                .AddTab(new TabItem()
                    .SetHeader("Advanced")
                    .SetContent(new VStack()
                        .SetSpacing(8)
                        .SetMargin(new Margin(12))
                        .AddChild(new Label().SetText("Advanced settings"))
                        .AddChild(new HStack()
                            .SetSpacing(8)
                            .AddChild(new Checkbox().SetIsChecked(true))
                            .AddChild(new Label().SetText("Enable feature")))))
                .AddTab(new TabItem()
                    .SetHeader("About")
                    .SetContent(new Label().SetText("PlusUi TabControl demo").SetMargin(new Margin(12))))
                .SetSelectedIndex(0)),

        Section("Note",
            Note("The default active-tab accent is green (AccentSuccess), which differs from the blue accent used by most other controls.")),
    ];
}
