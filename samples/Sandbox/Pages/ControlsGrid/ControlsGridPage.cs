using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.ControlsGrid;

internal class ControlsGridPage(ControlsGridPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new Grid()
            .AddColumn(Column.Star)
            .AddColumn(Column.Auto)
            .AddColumn(50)
            .AddRow(Row.Star)
            .AddRow(Row.Star, 2)
            .AddBoundRow(nameof(vm.RowHeight), () => vm.RowHeight)

            .AddChild(new Solid().SetBackground(new SolidColorBackground(SKColors.Green)).IgnoreStyling())
            .AddChild(column: 1, child: new Solid().SetBackground(new SolidColorBackground(SKColors.Yellow)).IgnoreStyling())
            .AddChild(row: 1, child: new Solid().SetBackground(new SolidColorBackground(SKColors.Blue)).IgnoreStyling())
            .AddChild(row: 2, columnSpan: 2, child: new Solid().SetBackground(new SolidColorBackground(SKColors.Red)).IgnoreStyling())
            .AddChild(row: 1, column: 2, rowSpan: 2, child: new Solid().SetBackground(new SolidColorBackground(SKColors.Purple)).IgnoreStyling())

            .AddChild(row: 1, column: 1, child: new VStack(
                new Border()
                    .AddChild(new Button()
                        .SetText("Increment")
                        .SetTextSize(20)
                        .SetCommand(vm.IncrementCommand)
                        .SetTextColor(SKColors.Black)
                        .SetBackground(new SolidColorBackground(SKColors.White)))
                    .SetStrokeColor(SKColors.White)
                    .SetStrokeThickness(2f)
                    .SetStrokeType(StrokeType.Solid)
                    .SetBackground(new SolidColorBackground(new SKColor(0, 0, 0, 150))),
                new Border()
                    .AddChild(new Button()
                        .SetText("Back")
                        .SetTextSize(20)
                        .SetCommand(vm.NavCommand)
                        .SetTextColor(SKColors.Black)
                        .SetBackground(new SolidColorBackground(SKColors.White)))
                    .SetStrokeColor(SKColors.Cyan)
                    .SetStrokeThickness(3f)
                    .SetStrokeType(StrokeType.Dashed)
                    .SetCornerRadius(5)
                    )
            );

    }
}
