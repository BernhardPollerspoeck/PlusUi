using PlusUi.core;
using PlusUi.core.Services.DebugBridge.Models;
using SkiaSharp;

namespace PlusUi.DebugServer.Pages;

public class MainPage(MainViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new Grid()
            .SetBackground(new Color(25, 25, 25))
            .AddRow(Row.Auto)      // Status bar
            .AddRow(Row.Star)      // Main content
            .AddColumn(Column.Star) // TreeView (left half)
            .AddColumn(Column.Star) // DataGrid (right half)

            // Status label - spans both columns
            .AddChild(
                row: 0,
                column: 0,
                columnSpan: 2,
                child: new Label()
                    .BindText(nameof(vm.StatusText), () => vm.StatusText)
                    .SetTextSize(14)
                    .SetTextColor(Colors.White)
                    .SetMargin(new Margin(8))
                    .SetBackground(new Color(45, 45, 45)))

            // TreeView for UI hierarchy - left column
            .AddChild(
                row: 1,
                column: 0,
                child: new Border()
                    .SetMargin(new Margin(8, 8, 4, 8))
                    .SetBackground(new Color(30, 30, 30))
                    .SetCornerRadius(4)
                    .AddChild(BuildTreeView()))

            // DataGrid for properties - right column
            .AddChild(
                row: 1,
                column: 1,
                child: new Border()
                    .SetMargin(new Margin(4, 8, 8, 8))
                    .SetCornerRadius(4)
                    .AddChild(BuildPropertyGrid()));
    }

    private TreeView BuildTreeView()
    {
        var treeView = new TreeView();
        treeView.SetBackground(Colors.Transparent);
        treeView.SetItemsSource(vm.RootItems);
        treeView.SetChildrenSelector<TreeNodeDto>(node => node.Children);
        treeView.SetItemTemplate((item, depth) => item switch
        {
            TreeNodeDto node => new Label()
                .SetText($"{node.Type} ({node.Id})")
                .SetTextColor(Colors.LightBlue)
                .SetVerticalAlignment(VerticalAlignment.Center),
            _ => new Label()
                .SetText(item?.ToString() ?? "")
                .SetTextColor(Colors.Gray)
                .SetVerticalAlignment(VerticalAlignment.Center)
        });
        treeView.SetItemHeight(28);
        treeView.SetIndentation(20);
        treeView.SetExpanderSize(14);
        treeView.SetShowLines(true);
        treeView.SetLineColor(new Color(60, 60, 60));
        treeView.BindSelectedItem(nameof(vm.SelectedNode),
            () => vm.SelectedNode,
            item => vm.SelectedNode = item as TreeNodeDto);
        return treeView;
    }

    private DataGrid<PropertyDto> BuildPropertyGrid()
    {
        var grid = new DataGrid<PropertyDto>();
        grid.SetItemsSource(vm.SelectedProperties);
        grid.SetAlternatingRowStyles(true);
        grid.SetEvenRowStyle(new SolidColorBackground(new Color(40, 40, 40)), Colors.White);
        grid.SetOddRowStyle(new SolidColorBackground(new Color(35, 35, 35)), Colors.White);
        grid.SetShowColumnSeparators(true);
        grid.SetHeaderSeparatorColor(new SKColor(80, 80, 80));
        grid.AddColumn(new DataGridTextColumn<PropertyDto>()
            .SetHeader("Property")
            .SetBinding(p => p.Name)
            .SetWidth(DataGridColumnWidth.Star(1)));
        grid.AddColumn(new DataGridTextColumn<PropertyDto>()
            .SetHeader("Value")
            .SetBinding(p => p.Value)
            .SetWidth(DataGridColumnWidth.Star(2)));
        grid.AddColumn(new DataGridTextColumn<PropertyDto>()
            .SetHeader("Type")
            .SetBinding(p => p.Type)
            .SetWidth(DataGridColumnWidth.Star(1)));
        grid.SetRowHeight(28);
        grid.SetHeaderHeight(36);
        return grid;
    }
}
