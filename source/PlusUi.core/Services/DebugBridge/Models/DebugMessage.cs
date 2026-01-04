namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Base message type for debug bridge communication.
/// </summary>
internal class DebugMessage
{
    /// <summary>
    /// Message type: "ui_tree", "performance_frame", "log_entry", "invalidation", etc.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Message payload (type-specific data).
    /// </summary>
    public object? Data { get; set; }
}
