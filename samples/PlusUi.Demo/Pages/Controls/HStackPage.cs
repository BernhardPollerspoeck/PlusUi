using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class HStackPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "HStack";

    protected override string Description =>
        "Stacks children horizontally, left to right. Optional spacing and row wrapping.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default (no spacing)",
            new HStack()
                .AddChild(Tile("A"))
                .AddChild(Tile("B"))
                .AddChild(Tile("C"))),

        Section("With spacing",
            new HStack()
                .SetSpacing(8)
                .AddChild(Tile("A"))
                .AddChild(Tile("B"))
                .AddChild(Tile("C"))),

        Section("Wrap into rows (width-limited)",
            Note("With SetWrap(true) and a limited width, items flow onto the next row."),
            new HStack()
                .SetWrap(true)
                .SetSpacing(8)
                .SetDesiredWidth(260)
                .AddChild(Tile("One"))
                .AddChild(Tile("Two"))
                .AddChild(Tile("Three"))
                .AddChild(Tile("Four"))
                .AddChild(Tile("Five"))
                .AddChild(Tile("Six"))),
    ];
}
