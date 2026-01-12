namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents a property of a UI element for debug inspection.
/// </summary>
internal class PropertyDto
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Value { get; set; }
    public bool CanWrite { get; set; }
    public bool IsInternal { get; set; }
    public List<PropertyDto> Children { get; set; } = [];
    public bool IsExpanded { get; set; }
    public bool HasChildren => Children.Count > 0;
    public string Path { get; set; } = "";
    public string ElementId { get; set; } = "";
}
