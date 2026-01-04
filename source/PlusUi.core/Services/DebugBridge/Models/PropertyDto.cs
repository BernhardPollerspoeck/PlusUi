namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents a property of a UI element for debug inspection.
/// </summary>
public class PropertyDto
{
    /// <summary>
    /// Property name (e.g., "Text", "BackgroundColor", "Width").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Property type name (e.g., "String", "SKColor", "Double").
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Property value as string (formatted for display).
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// Whether the property can be modified.
    /// </summary>
    public bool CanWrite { get; set; }

    /// <summary>
    /// Whether this is an internal property (not part of public API).
    /// </summary>
    public bool IsInternal { get; set; }
}
