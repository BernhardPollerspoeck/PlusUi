using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.TreeViewDemo;

public class TreeViewDemoPage(TreeViewDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Button()
                .SetText("<- Back")
                .SetTextSize(16)
                .SetCommand(vm.GoBackCommand)
                .SetTextColor(Colors.White)
                .SetPadding(new Margin(10, 5)),

            new Grid()
                .AddColumn(Column.Star)
                .AddColumn(Column.Star)
                .AddRow(Row.Star)
                .AddChild(
                    new Border()
                        .SetBackground(new Color(40, 40, 40))
                        .SetCornerRadius(4)
                        .SetMargin(new Margin(8))
                        .AddChild(new FileTreeUserControl(vm)),
                    row: 0, column: 0)
                .AddChild(
                    new Border()
                        .SetBackground(new Color(40, 40, 40))
                        .SetCornerRadius(4)
                        .SetMargin(new Margin(8))
                        .AddChild(new CategoryTreeUserControl(vm)),
                    row: 0, column: 1)
        );
    }
}
