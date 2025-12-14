namespace PlusUi.core.Services;

/// <summary>
/// Service for monitoring application performance metrics.
/// Intended for debug/development builds to track rendering performance.
/// </summary>
public interface IAppMonitor
{
    /// <summary>
    /// Reports the total frame rendering time
    /// </summary>
    /// <param name="frameTimeMs">Frame time in milliseconds</param>
    void ReportFrameTime(double frameTimeMs);

    /// <summary>
    /// Reports the time spent in the measure phase
    /// </summary>
    /// <param name="measureTimeMs">Measure time in milliseconds</param>
    void ReportMeasureTime(double measureTimeMs);

    /// <summary>
    /// Reports the time spent in the arrange phase
    /// </summary>
    /// <param name="arrangeTimeMs">Arrange time in milliseconds</param>
    void ReportArrangeTime(double arrangeTimeMs);

    /// <summary>
    /// Reports the time spent in the render phase
    /// </summary>
    /// <param name="renderTimeMs">Render time in milliseconds</param>
    void ReportRenderTime(double renderTimeMs);

    /// <summary>
    /// Reports when a property update was skipped due to value equality
    /// </summary>
    /// <param name="propertyName">Name of the property that was skipped</param>
    void ReportPropertyUpdateSkipped(string propertyName);

    /// <summary>
    /// Reports when an arrange was skipped due to dirty flag
    /// </summary>
    void ReportArrangeSkipped();

    /// <summary>
    /// Reports when a measure was skipped due to dirty flag
    /// </summary>
    void ReportMeasureSkipped();
}
