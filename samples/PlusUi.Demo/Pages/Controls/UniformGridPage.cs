using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class UniformGridPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "UniformGrid";

    protected override string Description =>
        "A grid where every cell is the same size. Children fill cells left-to-right, top-to-bottom.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Auto layout (square-ish)",
            Note("With no rows or columns set, children fill a square-ish grid automatically."),
            new UniformGrid()
                .SetDesiredHeight(130)
                .AddChildren(Tile("1"), Tile("2"), Tile("3"), Tile("4"), Tile("5"))),

        Section("Fixed columns",
            Note("SetColumns(3): every cell is identical, filled row by row."),
            new UniformGrid()
                .SetColumns(3)
                .SetDesiredHeight(130)
                .AddChildren(Tile("1"), Tile("2"), Tile("3"), Tile("4"), Tile("5"), Tile("6"))),
    ];
}
