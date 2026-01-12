namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Represents performance metrics for a single frame.
/// </summary>
public class PerformanceFrameDto
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

    /// <summary>
    /// Render utilization percentage (FrameTime / 16.67ms * 100).
    /// 100% means using full 60fps budget.
    /// </summary>
    public double UtilizationPercent { get; set; }

    /// <summary>
    /// Memory usage in bytes.
    /// </summary>
    public long MemoryBytes { get; set; }

    /// <summary>
    /// Whether a render actually occurred this frame.
    /// </summary>
    public bool DidRender { get; set; }
}
