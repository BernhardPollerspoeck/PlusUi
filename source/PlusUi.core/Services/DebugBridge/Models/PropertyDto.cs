namespace PlusUi.core.Services.DebugBridge.Models;

public class PropertyDto
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Value { get; set; }
    public bool CanWrite { get; set; }
    public bool IsInternal { get; set; }
    public List<PropertyDto> Children { get; set; } = new();
    public bool IsExpanded { get; set; }
    public bool HasChildren => Children.Count > 0;
    public string Path { get; set; } = "";
    public string ElementId { get; set; } = "";
}
