using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class LinkPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Link";

    protected override string Description =>
        "A clickable text link that opens a URL. Has an underline but no link color by default.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default",
            Note("A bare Link renders as white underlined text - it has no distinct link color out of the box."),
            new Link().SetText("Plain link").SetUrl("https://github.com")),

        Section("Styled as a link",
            new Link()
                .SetText("github.com")
                .SetUrl("https://github.com")
                .SetTextColor(PlusUiDefaults.AccentPrimary)),

        Section("In a sentence",
            new HStack()
                .SetSpacing(4)
                .AddChild(new Label().SetText("Read the"))
                .AddChild(new Link()
                    .SetText("documentation")
                    .SetUrl("https://example.com")
                    .SetTextColor(PlusUiDefaults.AccentPrimary))
                .AddChild(new Label().SetText("for more details."))),
    ];
}
