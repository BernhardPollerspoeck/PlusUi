using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.ControlsGrid;

internal class ControlsGridPage(ControlsGridPageViewModel vm) : UiPageElement(vm)
{
    public override void OnNavigatedTo(object? parameter)
    {
        base.OnNavigatedTo(parameter);
        vm.Title = parameter as string;
    }

    protected override UiElement Build()
    {
        return new Grid()
            .AddColumn(Column.Star)
            .AddColumn(Column.Auto)
            .AddColumn(50)
            .AddRow(Row.Star)
            .AddRow(Row.Star, 2)
            .AddBoundRow(nameof(vm.RowHeight), () => vm.RowHeight)

            .AddChild(new Solid().SetBackground(new SolidColorBackground(Colors.Green)).IgnoreStyling())
            .AddChild(column: 1, child: new Solid().SetBackground(new SolidColorBackground(Colors.Yellow)).IgnoreStyling())
            .AddChild(row: 1, child: new Solid().SetBackground(new SolidColorBackground(Colors.Blue)).IgnoreStyling())
            .AddChild(row: 2, columnSpan: 2, child: new Solid().SetBackground(new SolidColorBackground(Colors.Red)).IgnoreStyling())
            .AddChild(row: 1, column: 2, rowSpan: 2, child: new Solid().SetBackground(new SolidColorBackground(Colors.Purple)).IgnoreStyling())

            .AddChild(row: 1, column: 1, child: new VStack(
                new Border()
                    .AddChild(new Button()
                        .BindText(() => $"Increment {vm.Title}")
                        .SetTextSize(20)
                        .SetCommand(vm.IncrementCommand)
                        .SetTextColor(Colors.Black)
                        .SetBackground(new SolidColorBackground(Colors.White)))
                    .SetStrokeColor(Colors.White)
                    .SetStrokeThickness(2f)
                    .SetStrokeType(StrokeType.Solid)
                    .SetBackground(new SolidColorBackground(new Color(0, 0, 0, 150))),
                new Border()
                    .AddChild(new Button()
                        .SetText("Back")
                        .SetTextSize(20)
                        .SetCommand(vm.NavCommand)
                        .SetTextColor(Colors.Black)
                        .SetBackground(new SolidColorBackground(Colors.White)))
                    .SetStrokeColor(Colors.Cyan)
                    .SetStrokeThickness(3f)
                    .SetStrokeType(StrokeType.Dashed)
                    .SetCornerRadius(5)
                    )
            );

    }
}
