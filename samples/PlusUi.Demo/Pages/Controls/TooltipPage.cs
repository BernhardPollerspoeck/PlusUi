using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class TooltipPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Tooltip";

    protected override string Description =>
        "A hover popup attached to any element via the SetTooltip extension. Supports plain text or rich content.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Hover for a tooltip",
            new HStack()
                .SetSpacing(12)
                .AddChild(new Button().SetText("Save").SetTooltip("Saves the current document"))
                .AddChild(new Button().SetText("Delete")
                    .SetTooltip(t => t
                        .SetContent("This cannot be undone")
                        .SetPlacement(TooltipPlacement.Bottom)))),

        Section("Rich tooltip content",
            new Button().SetText("Info")
                .SetTooltip(new VStack()
                    .SetSpacing(2)
                    .AddChild(new Label().SetText("Keyboard shortcut").SetFontWeight(FontWeight.Bold))
                    .AddChild(new Label().SetText("Ctrl+S").SetTextColor(PlusUiDefaults.TextSecondary)))),
    ];
}
