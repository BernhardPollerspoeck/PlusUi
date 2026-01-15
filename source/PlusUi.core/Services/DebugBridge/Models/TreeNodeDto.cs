namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents a node in the UI tree for debug inspection.
/// </summary>
internal class TreeNodeDto
{
    /// <summary>
    /// Unique element ID for reference.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Element type name (e.g., "Button", "Label", "StackLayout").
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Whether the element is currently visible.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Element properties (name, type, value).
    /// </summary>
    public List<PropertyDto> Properties { get; set; } = [];

    /// <summary>
    /// Child elements.
    /// </summary>
    public List<TreeNodeDto> Children { get; set; } = [];

    public void MergeFrom(TreeNodeDto source)
    {
        Type = source.Type;
        IsVisible = source.IsVisible;
        Properties = source.Properties;

        var existingById = Children.ToDictionary(c => c.Id);
        var newChildren = new List<TreeNodeDto>();

        foreach (var sourceChild in source.Children)
        {
            if (existingById.TryGetValue(sourceChild.Id, out var existing))
            {
                existing.MergeFrom(sourceChild);
                newChildren.Add(existing);
                existingById.Remove(sourceChild.Id);
            }
            else
            {
                newChildren.Add(sourceChild);
            }
        }

        Children = newChildren;
    }
}
