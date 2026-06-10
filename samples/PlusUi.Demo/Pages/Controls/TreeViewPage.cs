using System.Linq;
using SkiaSharp;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public class TreeNode(string name, params TreeNode[] children)
{
    public string Name { get; } = name;
    public List<TreeNode> Children { get; } = children.ToList();
}

public class TreeViewPage(DemoPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "TreeView";

    protected override string Description =>
        "A hierarchical, expandable list. Children are provided per item type via a children selector.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Two-level tree",
            Note("Click the triangles to expand or collapse nodes."),
            new TreeView()
                .SetItemsSource(new List<object>
                {
                    new TreeNode("Fruits",
                        new TreeNode("Apple"),
                        new TreeNode("Banana"),
                        new TreeNode("Cherry")),
                    new TreeNode("Vegetables",
                        new TreeNode("Carrot"),
                        new TreeNode("Potato")),
                })
                .SetChildrenSelector<TreeNode>(n => n.Children.Cast<object>())
                .SetItemTemplate((item, _) => new Label()
                    .SetText(((TreeNode)item).Name)
                    .SetVerticalAlignment(VerticalAlignment.Center))
                .SetAutoExpandInitialLevels(true)
                .SetItemHeight(30)
                .SetIndentation(28)
                .SetShowLines(true)
                .SetLineColor(new SKColor(120, 120, 120))
                .SetDesiredHeight(260)),
    ];
}
