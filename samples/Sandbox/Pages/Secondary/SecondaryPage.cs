using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.Secondary;

internal class SecondaryPage(SecondPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new Grid()
            .AddColumn(Column.Star)
            .AddColumn(Column.Auto)
            .AddColumn(50)
            .AddRow(Row.Star)
            .AddRow(Row.Star, 2)
            .AddBoundRow(() => vm.RowHeight)

            .AddChild(new Solid().SetBackgroundColor(SKColors.Green))
            .AddChild(row: 1, child: new Solid().SetBackgroundColor(SKColors.Blue))
            .AddChild(column: 1, child: new Solid().SetBackgroundColor(SKColors.Yellow))
            .AddChild(row: 2, columnSpan: 2, child: new Solid().SetBackgroundColor(SKColors.Red))
            .AddChild(row: 1, column: 2, rowSpan: 2, child: new Solid().SetBackgroundColor(SKColors.Purple))

            .AddChild(row: 1, column: 1, child: new VStack(
                new Button()
                    .SetText("Increment")
                    .SetTextSize(20)
                    .SetCommand(vm.IncrementCommand)
                    .SetTextColor(SKColors.Black)
                    .SetBackgroundColor(SKColors.White),
                new Button()
                   .SetText("Back")
                   .SetTextSize(20)
                   .SetCommand(vm.NavCommand)
                   .SetTextColor(SKColors.Black)
                   .SetBackgroundColor(SKColors.White))
            );

    }
}
