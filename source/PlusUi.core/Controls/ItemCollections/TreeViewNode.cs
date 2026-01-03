namespace PlusUi.core;

/// <summary>
/// Node representing an item in the tree hierarchy.
/// </summary>
public class TreeViewNode(object item, int depth, TreeViewNode? parent = null)
{
    public object Item { get; } = item;
    public int Depth { get; } = depth;
    public bool IsExpanded { get; set; } = false;
    public float ExpandedHeight { get; private set; }
    public List<TreeViewNode>? Children { get; set; }
    public TreeViewNode? Parent { get; } = parent;
    public bool HasChildren { get; set; } = false;

    /// <summary>
    /// Updates this node's ExpandedHeight and propagates the change to parents.
    /// </summary>
    public void UpdateExpandedHeight(float itemHeight)
    {
        float childrenHeight = 0;
        if (IsExpanded && Children != null)
        {
            foreach (var child in Children)
            {
                childrenHeight += child.ExpandedHeight;
            }
        }

        var newHeight = itemHeight + childrenHeight;
        var heightDelta = newHeight - ExpandedHeight;
        ExpandedHeight = newHeight;

        // Propagate to parent
        if (heightDelta != 0 && Parent != null)
        {
            Parent.UpdateExpandedHeight(itemHeight);
        }
    }
}
