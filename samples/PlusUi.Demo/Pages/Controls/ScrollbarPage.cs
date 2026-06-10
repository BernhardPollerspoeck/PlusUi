using System.Linq;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class ScrollbarPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Scrollbar";

    protected override string Description =>
        "The draggable scrollbar used by scrolling controls. It is normally created and driven internally rather than placed directly.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Vertical scrollbar",
            Note("A height-limited list shows the vertical Scrollbar on its right edge. Hover and drag the thumb."),
            new ItemsList<string>()
                .SetItemsSource(Enumerable.Range(1, 40).Select(i => $"Item {i}").ToList())
                .SetItemTemplate((item, _) => new Label().SetText(item).SetMargin(new Margin(8, 4)))
                .SetDesiredHeight(180)),

        Section("Horizontal scrollbar",
            Note("A width-limited horizontal list shows the horizontal Scrollbar along its bottom edge."),
            new ItemsList<string>()
                .SetOrientation(Orientation.Horizontal)
                .SetItemsSource(Enumerable.Range(1, 30).Select(i => $"Col {i}").ToList())
                .SetItemTemplate((item, _) => new Label().SetText(item).SetMargin(new Margin(12, 10)))
                .SetDesiredWidth(380)
                .SetDesiredHeight(56)),
    ];
}
