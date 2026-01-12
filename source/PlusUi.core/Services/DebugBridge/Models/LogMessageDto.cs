using Microsoft.Extensions.Logging;

namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents a log message sent from the debugged app to the debug server.
/// </summary>
internal class LogMessageDto
{
    /// <summary>
    /// Log level (Debug, Info, Warning, Error, Critical).
    /// </summary>
    public required LogLevel Level { get; set; }

    /// <summary>
    /// Timestamp when the log was created.
    /// </summary>
    public required DateTime Timestamp { get; set; }

    /// <summary>
    /// Log message text.
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Category name (usually the logger name/class).
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Exception details if available.
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// Event ID if available.
    /// </summary>
    public int? EventId { get; set; }
}
