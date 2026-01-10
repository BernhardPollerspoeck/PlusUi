using PlusUi.core;
using PlusUi.core.Services.DebugBridge.Models;
using PlusUi.DebugServer.Pages;
using System.ComponentModel;

namespace PlusUi.DebugServer.Components;

/// <summary>
/// Displays UI element hierarchy as a tree.
/// </summary>
public class ElementTreeView : UserControl
{
    private readonly MainViewModel _viewModel;

    public ElementTreeView(MainViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        var treeView = new TreeView();
        treeView.BindItemsSource(() => _viewModel.RootItems);
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
        treeView.SetAutoExpandInitialLevels(true, 2);
        treeView.BindSelectedItem(() => _viewModel.SelectedNode,
            item => _viewModel.SelectedNode = item as TreeNodeDto);
        return treeView;
    }
}
