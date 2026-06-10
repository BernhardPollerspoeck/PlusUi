using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class ScrollViewPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "ScrollView";

    protected override string Description =>
        "A scrollable container for content larger than the visible area. Scroll axes can be toggled independently.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Vertical scroll (height-limited)",
            Note("Content taller than the view scrolls with the mouse wheel or by dragging."),
            new ScrollView(BuildRows(12))
                .SetCanScrollHorizontally(false)
                .SetDesiredHeight(200)),

        Section("Horizontal scroll",
            Note("A wide row scrolls sideways when vertical scrolling is disabled."),
            new ScrollView(BuildColumns(30))
                .SetCanScrollVertically(false)
                .SetDesiredHeight(80)),
    ];

    private static VStack BuildRows(int count)
    {
        var stack = new VStack().SetSpacing(8);
        for (var i = 1; i <= count; i++)
            stack.AddChild(Tile($"Row {i}"));
        return stack;
    }

    private static HStack BuildColumns(int count)
    {
        var stack = new HStack().SetSpacing(8);
        for (var i = 1; i <= count; i++)
            stack.AddChild(Tile($"Column {i}"));
        return stack;
    }
}
