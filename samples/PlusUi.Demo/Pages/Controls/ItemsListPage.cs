using System.Linq;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public record Person(string Name, string Role);

public class ItemsListPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "ItemsList";

    protected override string Description =>
        "A virtualized list that renders only visible items from a collection using an item template.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Simple list (height-limited)",
            new ItemsList<string>()
                .SetItemsSource(Enumerable.Range(1, 8).Select(i => $"Item {i}").ToList())
                .SetItemTemplate((item, _) => new Label().SetText(item).SetMargin(new Margin(8, 6)))
                .SetDesiredHeight(180)),

        Section("Templated rows",
            new ItemsList<Person>()
                .SetItemsSource(new List<Person>
                {
                    new("Ada Lovelace", "Engineer"),
                    new("Alan Turing", "Mathematician"),
                    new("Grace Hopper", "Rear Admiral"),
                    new("Katherine Johnson", "Mathematician"),
                })
                .SetItemTemplate((p, _) => new HStack()
                    .SetSpacing(8)
                    .SetMargin(new Margin(8, 6))
                    .AddChild(new Label().SetText(p.Name).SetFontWeight(FontWeight.SemiBold))
                    .AddChild(new Label().SetText(p.Role).SetTextColor(PlusUiDefaults.TextSecondary)))
                .SetDesiredHeight(160)),
    ];
}
