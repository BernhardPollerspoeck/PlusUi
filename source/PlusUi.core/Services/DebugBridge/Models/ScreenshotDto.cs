namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents a screenshot captured from the debugged app.
/// </summary>
public class ScreenshotDto
{
    /// <summary>
    /// Element ID that was captured, or null for full page screenshot.
    /// </summary>
    public string? ElementId { get; set; }

    /// <summary>
    /// Base64 encoded PNG image data.
    /// </summary>
    public required string ImageBase64 { get; set; }

    /// <summary>
    /// Width of the captured image in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Height of the captured image in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Timestamp when the screenshot was captured.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}
