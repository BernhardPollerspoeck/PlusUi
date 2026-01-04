namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents performance metrics for a single frame.
/// </summary>
internal class PerformanceFrameDto
{
    /// <summary>
    /// Timestamp when the frame was captured.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Total frame time in milliseconds.
    /// </summary>
    public double FrameTimeMs { get; set; }

    /// <summary>
    /// Frames per second (calculated from frame time).
    /// </summary>
    public double Fps { get; set; }

    /// <summary>
    /// Time spent in measure phase (ms).
    /// </summary>
    public double MeasureTimeMs { get; set; }

    /// <summary>
    /// Time spent in arrange phase (ms).
    /// </summary>
    public double ArrangeTimeMs { get; set; }

    /// <summary>
    /// Time spent in render phase (ms).
    /// </summary>
    public double RenderTimeMs { get; set; }

    /// <summary>
    /// Number of elements that skipped measure.
    /// </summary>
    public int MeasureSkipped { get; set; }

    /// <summary>
    /// Number of elements that skipped arrange.
    /// </summary>
    public int ArrangeSkipped { get; set; }

    /// <summary>
    /// Number of property changes that were skipped.
    /// </summary>
    public int PropertySkipped { get; set; }
}
