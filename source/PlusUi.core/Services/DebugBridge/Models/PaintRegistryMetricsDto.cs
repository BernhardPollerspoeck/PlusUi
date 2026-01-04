namespace PlusUi.core.Services.DebugBridge.Models;

/// <summary>
/// Metrics snapshot from PaintRegistry for debugging.
/// </summary>
internal class PaintRegistryMetricsDto
{
    /// <summary>
    /// Timestamp when metrics were captured.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Total number of cached paint instances.
    /// </summary>
    public int TotalInstances { get; set; }

    /// <summary>
    /// Sum of all reference counts across all entries.
    /// </summary>
    public int TotalRefCount { get; set; }

    /// <summary>
    /// Individual entry details for debugging.
    /// </summary>
    public List<PaintEntryMetricsDto> Entries { get; set; } = [];
}

/// <summary>
/// Metrics for a single paint cache entry.
/// </summary>
internal class PaintEntryMetricsDto
{
    /// <summary>
    /// Paint color in hex format.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Font size.
    /// </summary>
    public float Size { get; set; }

    /// <summary>
    /// Current reference count for this entry.
    /// </summary>
    public int RefCount { get; set; }

    /// <summary>
    /// Font hinting setting.
    /// </summary>
    public string Hinting { get; set; } = string.Empty;

    /// <summary>
    /// Font edging setting.
    /// </summary>
    public string Edging { get; set; } = string.Empty;
}
