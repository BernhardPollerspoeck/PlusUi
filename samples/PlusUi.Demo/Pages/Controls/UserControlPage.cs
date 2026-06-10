using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

/// <summary>
/// A small reusable component built by composing other elements - the typical UserControl pattern.
/// </summary>
public class StatCard(string title, string value) : UserControl
{
    protected override UiElement Build() =>
        new Border()
            .SetBackground(PlusUiDefaults.BackgroundControl)
            .SetCornerRadius(8)
            .SetStrokeThickness(0)
            .AddChild(new VStack()
                .SetSpacing(4)
                .SetMargin(new Margin(16))
                .AddChild(new Label().SetText(value).SetTextSize(24).SetFontWeight(FontWeight.Bold))
                .AddChild(new Label().SetText(title).SetTextColor(PlusUiDefaults.TextSecondary).SetTextSize(12)));
}

public class UserControlPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "UserControl";

    protected override string Description =>
        "A base class for building reusable, composed components by overriding the Build() method.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Composed reusable control",
            Note("'StatCard' is a UserControl that composes a Border, VStack and two Labels. It is reused below with different data."),
            new HStack()
                .SetSpacing(12)
                .AddChild(new StatCard("Users", "1,204"))
                .AddChild(new StatCard("Revenue", "$8.3k"))
                .AddChild(new StatCard("Uptime", "99.9%"))),
    ];
}
