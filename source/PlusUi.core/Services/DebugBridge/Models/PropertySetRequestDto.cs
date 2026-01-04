namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents a request to set a property value on a UI element (from debug server).
/// </summary>
internal class PropertySetRequestDto
{
    /// <summary>
    /// Element ID to modify.
    /// </summary>
    public required string ElementId { get; set; }

    /// <summary>
    /// Property name to set.
    /// </summary>
    public required string PropertyName { get; set; }

    /// <summary>
    /// New value as string (will be converted to appropriate type).
    /// </summary>
    public required string Value { get; set; }
}
