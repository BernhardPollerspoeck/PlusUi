using PlusUi.core;
using PlusUi.DebugServer.Components;
using SkiaSharp;

namespace PlusUi.DebugServer.Pages;

public class MainPage(MainViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new Grid()
            .SetBackground(new Color(25, 25, 25))
            .AddRow(Row.Auto)      // App Tabs
            .AddRow(Row.Auto)      // Status bar
            .AddRow(Row.Star)      // Main content
            .AddColumn(Column.Star) // TreeView (left half)
            .AddColumn(Column.Star) // DataGrid (right half)

            // App Tabs using TabControl
            .AddChild(
                row: 0,
                column: 0,
                columnSpan: 2,
                child: new TabControl()
                    .BindTabs(nameof(vm.AppTabs), () => vm.AppTabs)
                    .BindSelectedIndex(nameof(vm.SelectedAppTabIndex),
                        () => vm.SelectedAppTabIndex,
                        index => vm.SelectedAppTabIndex = index)
                    .SetHeaderBackgroundColor(new SKColor(35, 35, 35))
                    .SetActiveTabBackgroundColor(new SKColor(50, 50, 50))
                    .SetHeaderTextSize(13))

            // Status Bar
            .AddChild(
                row: 1,
                column: 0,
                columnSpan: 2,
                child: new DebugStatusBar(vm))

            // Element Tree - left column
            .AddChild(
                row: 2,
                column: 0,
                child: new Border()
                    .SetMargin(new Margin(8, 8, 4, 8))
                    .SetBackground(new Color(30, 30, 30))
                    .SetCornerRadius(4)
                    .AddChild(new ElementTreeView(vm)))

            // Property Grid - right column
            .AddChild(
                row: 2,
                column: 1,
                child: new Border()
                    .SetMargin(new Margin(4, 8, 8, 8))
                    .SetCornerRadius(4)
                    .AddChild(new PropertyGridView(vm)))

            // Waiting overlay - spans all content rows when no apps connected
            .AddChild(
                row: 0,
                column: 0,
                rowSpan: 3,
                columnSpan: 2,
                child: new WaitingOverlay(vm));
    }
}
