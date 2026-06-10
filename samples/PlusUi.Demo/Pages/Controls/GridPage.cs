using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class GridPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Grid";

    protected override string Description =>
        "Arranges children in rows and columns. Columns/rows can be Absolute, Auto (fit content) or Star (proportional).";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Star columns (proportional)",
            Note("Three columns 1* / 2* / 1* - the middle column is twice as wide."),
            new Grid()
                .SetVerticalAlignment(VerticalAlignment.Top)
                .AddColumn(Column.Star, 1)
                .AddColumn(Column.Star, 2)
                .AddColumn(Column.Star, 1)
                .AddRow(Row.Auto)
                .AddChild(Tile("1*"), row: 0, column: 0)
                .AddChild(Tile("2*"), row: 0, column: 1)
                .AddChild(Tile("1*"), row: 0, column: 2)),

        Section("Auto + Star",
            Note("First column sizes to its content (Auto), the second fills the rest (Star)."),
            new Grid()
                .SetVerticalAlignment(VerticalAlignment.Top)
                .AddColumn(Column.Auto)
                .AddColumn(Column.Star, 1)
                .AddRow(Row.Auto)
                .AddChild(Tile("Auto"), row: 0, column: 0)
                .AddChild(Tile("Star fills the remaining width"), row: 0, column: 1)),

        Section("Row & column span",
            new Grid()
                .SetVerticalAlignment(VerticalAlignment.Top)
                .AddColumn(Column.Star, 1)
                .AddColumn(Column.Star, 1)
                .AddRow(Row.Auto)
                .AddRow(Row.Auto)
                .AddChild(Tile("Spans 2 columns"), row: 0, column: 0, columnSpan: 2)
                .AddChild(Tile("R1 C0"), row: 1, column: 0)
                .AddChild(Tile("R1 C1"), row: 1, column: 1)),
    ];
}
