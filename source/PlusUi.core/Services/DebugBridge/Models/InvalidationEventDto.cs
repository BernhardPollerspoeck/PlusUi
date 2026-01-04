namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents an invalidation event for tracking render triggers.
/// </summary>
internal class InvalidationEventDto
{
    /// <summary>
    /// Timestamp when the invalidation occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Reason for the invalidation (e.g., property name, "Manual").
    /// </summary>
    public required string Reason { get; set; }

    /// <summary>
    /// Stack trace showing where the invalidation was triggered from.
    /// </summary>
    public required string StackTrace { get; set; }
}
