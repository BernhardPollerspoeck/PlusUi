namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents a log entry for debug log viewer.
/// </summary>
internal class LogEntryDto
{
    /// <summary>
    /// Timestamp when the log was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Log level (Trace, Debug, Information, Warning, Error, Critical).
    /// </summary>
    public required string Level { get; set; }

    /// <summary>
    /// Log message.
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Exception details (if any).
    /// </summary>
    public string? Exception { get; set; }
}
