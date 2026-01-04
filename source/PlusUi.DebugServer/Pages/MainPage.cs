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

            .AddChild(
                row: 0,
                column: 0,
                columnSpan: 2,
                child: new HStack()
                    .SetMargin(new Margin(8))
                    .SetBackground(new Color(45, 45, 45))
                    .AddChild(new Label()
                        .BindText(nameof(vm.StatusText), () => vm.StatusText)
                        .SetTextSize(14)
                        .SetTextColor(Colors.White)
                        .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                        .SetMargin(new Margin(0, 0, 8, 0)))
                    .AddChild(new Button()
                        .SetText("Refresh Tree")
                        .SetCommand(vm.RefreshTreeCommand)
                        .SetBackground(new Color(60, 60, 60))
                        .SetTextColor(Colors.White)
                        .SetPadding(new Margin(12, 6))))

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
        treeView.SetItemsSource(vm.RootItems);
        treeView.SetChildrenSelector<TreeNodeDto>(node => node.Children);
        treeView.SetItemTemplate((item, depth) => item switch
        {
            TreeNodeDto node => new Label()
                .SetText($"{node.Type} ({node.Id})")
                .SetTextColor(Colors.LightBlue)
                .SetTextSize(13)
                .SetVerticalAlignment(VerticalAlignment.Center),
            _ => new Label()
                .SetText(item?.ToString() ?? "")
                .SetTextColor(Colors.Gray)
                .SetTextSize(13)
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

    private TreeView BuildPropertyGrid()
    {
        var treeView = new TreeView();
        treeView.SetItemsSource(vm.SelectedProperties);
        treeView.SetChildrenSelector<PropertyDto>(prop => prop.Children);
        treeView.SetItemTemplate((item, depth) =>
        {
            if (item is not PropertyDto prop)
                return new Label()
                    .SetText("")
                    .SetTextColor(Colors.Gray)
                    .SetTextSize(12);

            var valueControl = prop.CanWrite && !prop.HasChildren
                ? (UiElement)new Entry()
                    .SetText(prop.Value)
                    .SetTextColor(Colors.LightGray)
                    .SetTextSize(12)
                    .SetBackground(new Color(40, 40, 40))
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .SetMargin(new Margin(0, 2, 16, 2))
                    .SetPadding(new Margin(4, 2))
                    .BindText($"Property_{prop.Path}", () => prop.Value, newValue => vm.UpdatePropertyValue(prop, newValue))
                : new Label()
                    .SetText(prop.Value)
                    .SetTextColor(Colors.LightGray)
                    .SetTextSize(12)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .SetMargin(new Margin(0, 0, 16, 0));

            return new HStack()
                .SetVerticalAlignment(VerticalAlignment.Center)
                .AddChild(new Label()
                    .SetText(prop.Name)
                    .SetTextColor(prop.HasChildren ? Colors.LightBlue : Colors.White)
                    .SetTextSize(12)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetMargin(new Margin(8, 0, 16, 0)))
                .AddChild(valueControl)
                .AddChild(new Label()
                    .SetText(prop.Type)
                    .SetTextColor(Colors.Gray)
                    .SetTextSize(11)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetMargin(new Margin(0, 0, 8, 0)));
        });
        treeView.SetItemHeight(28);
        treeView.SetIndentation(24);
        treeView.SetExpanderSize(16);
        treeView.SetShowLines(true);
        treeView.SetLineColor(new Color(60, 60, 60));
        treeView.SetBackground(new Color(30, 30, 30));
        return treeView;
    }
}
