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
            .AddRow(Row.Star)
            .AddColumn(Column.Star)

            // App Tabs - content is AppContentView with feature tabs + status bar
            .AddChild(
                row: 0,
                column: 0,
                child: new TabControl()
                    .BindTabs(() => vm.AppTabs)
                    .BindSelectedIndex(() => vm.SelectedAppTabIndex,
                        index => vm.SelectedAppTabIndex = index)
                    .SetHeaderBackgroundColor(new SKColor(35, 35, 35))
                    .SetActiveTabBackgroundColor(new SKColor(50, 50, 50))
                    .SetHeaderTextSize(13))

            // Waiting overlay - visible when no apps connected
            .AddChild(
                row: 0,
                column: 0,
                child: new WaitingOverlay(vm));
    }
}
