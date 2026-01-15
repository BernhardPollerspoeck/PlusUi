using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.DebugServer.Pages;

namespace PlusUi.DebugServer.Components;

/// <summary>
/// Displays UI element hierarchy as a tree with screenshot buttons.
/// </summary>
internal class ElementTreeView : UserControl
{
    private readonly MainViewModel _viewModel;
    private TreeView? _treeView;

    public ElementTreeView(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.TreeRefreshRequested += OnTreeRefreshRequested;
    }

    private void OnTreeRefreshRequested(object? sender, EventArgs e)
    {
        _treeView?.Refresh();
    }

    protected override UiElement Build()
    {
        _treeView = CreateTreeView();
        return new VStack()
            .AddChild(
                // Capture Page Button at top
                new HStack(
                    new Button()
                        .SetIcon("camera.svg")
                        .SetIconTintColor(Colors.White)
                        .SetText("Capture Page")
                        .SetTextColor(Colors.White)
                        .SetTextSize(12)
                        .SetBackground(new Color(60, 60, 60))
                        .SetCornerRadius(4)
                        .SetPadding(new Margin(8, 4))
                        .SetCommand(_viewModel.CapturePageScreenshotCommand))
                .SetMargin(new Margin(8))
                .SetHorizontalAlignment(HorizontalAlignment.Left))
            .AddChild(_treeView);
    }

    private TreeView CreateTreeView()
    {
        var treeView = new TreeView();
        treeView.BindItemsSource(() => _viewModel.RootItems);
        treeView.SetChildrenSelector<TreeNodeDto>(node => node.Children);
        treeView.SetExpandedKeys<TreeNodeDto>(_viewModel.ExpandedTreeIds, node => node.Id);
        treeView.SetItemTemplate((item, depth) => item switch
        {
            TreeNodeDto node => CreateTreeItem(node),
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
        treeView.SetAutoExpandInitialLevels(true, 2);
        treeView.BindSelectedItem(() => _viewModel.SelectedNode,
            item => _viewModel.SelectedNode = item as TreeNodeDto);
        return treeView;
    }

    private UiElement CreateTreeItem(TreeNodeDto node)
    {
        return new HStack(
            new Label()
                .SetText($"{node.Type} ({node.Id})")
                .SetTextColor(Colors.LightBlue)
                .SetTextSize(13)
                .SetVerticalAlignment(VerticalAlignment.Center),
            new Button()
                .SetIcon("camera.svg")
                .SetIconTintColor(new Color(150, 150, 150))
                .SetBackground(Colors.Transparent)
                .SetDesiredWidth(20)
                .SetDesiredHeight(20)
                .SetCommand(new RelayCommand(() => _viewModel.CaptureElementScreenshotCommand.Execute(node.Id))))
        .SetSpacing(4)
        .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
