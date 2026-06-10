using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class VStackPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "VStack";

    protected override string Description =>
        "Stacks children vertically, top to bottom. Optional spacing and column wrapping.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default (no spacing)",
            new VStack()
                .AddChild(Tile("First"))
                .AddChild(Tile("Second"))
                .AddChild(Tile("Third"))),

        Section("With spacing",
            new VStack()
                .SetSpacing(8)
                .AddChild(Tile("First"))
                .AddChild(Tile("Second"))
                .AddChild(Tile("Third"))),

        Section("Wrap into columns (height-limited)",
            Note("With SetWrap(true) and a limited height, items flow into the next column."),
            new VStack()
                .SetWrap(true)
                .SetSpacing(8)
                .SetDesiredHeight(150)
                .AddChild(Tile("1"))
                .AddChild(Tile("2"))
                .AddChild(Tile("3"))
                .AddChild(Tile("4"))
                .AddChild(Tile("5"))
                .AddChild(Tile("6"))),
    ];
}
