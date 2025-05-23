﻿using PlusUi.core;
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
            .AddBoundRow(() => vm.RowHeight)

            .AddChild(new Solid().SetBackgroundColor(SKColors.Green).IgnoreStyling())
            .AddChild(column: 1, child: new Solid().SetBackgroundColor(SKColors.Yellow).IgnoreStyling())
            .AddChild(row: 1, child: new Solid().SetBackgroundColor(SKColors.Blue).IgnoreStyling())
            .AddChild(row: 2, columnSpan: 2, child: new Solid().SetBackgroundColor(SKColors.Red).IgnoreStyling())
            .AddChild(row: 1, column: 2, rowSpan: 2, child: new Solid().SetBackgroundColor(SKColors.Purple).IgnoreStyling())

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
