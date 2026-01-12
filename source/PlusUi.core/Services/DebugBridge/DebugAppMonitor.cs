using System.Diagnostics;
using PlusUi.core.Services;
using PlusUi.core.Services.DebugBridge.Models;

namespace PlusUi.core.Services.DebugBridge;

/// <summary>
/// IAppMonitor implementation that aggregates performance data and sends it to the debug server.
/// Sends aggregated metrics every second to avoid overwhelming the connection.
/// </summary>
internal class DebugAppMonitor : IAppMonitor
{
    private readonly DebugBridgeClient _client;
    private readonly object _lock = new();

    // Current frame data
    private double _currentFrameTimeMs;
    private double _currentMeasureTimeMs;
    private double _currentArrangeTimeMs;
    private double _currentRenderTimeMs;

    // Aggregated data for the current second
    private readonly List<double> _frameTimesMs = [];
    private readonly List<double> _measureTimesMs = [];
    private readonly List<double> _arrangeTimesMs = [];
    private readonly List<double> _renderTimesMs = [];
    private int _measureSkipped;
    private int _arrangeSkipped;
    private int _propertySkipped;
    private int _frameCount;

    // Timer for periodic sending
    private readonly Timer _sendTimer;
    private readonly Stopwatch _uptimeStopwatch;

    public DebugAppMonitor(DebugBridgeClient client)
    {
        _client = client;
        _uptimeStopwatch = Stopwatch.StartNew();

        // Send aggregated data every second
        _sendTimer = new Timer(SendAggregatedData, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    public void ReportFrameTime(double frameTimeMs)
    {
        lock (_lock)
        {
            _currentFrameTimeMs = frameTimeMs;
            _frameTimesMs.Add(frameTimeMs);
            _frameCount++;

            // Store current frame metrics
            if (_currentMeasureTimeMs > 0) _measureTimesMs.Add(_currentMeasureTimeMs);
            if (_currentArrangeTimeMs > 0) _arrangeTimesMs.Add(_currentArrangeTimeMs);
            if (_currentRenderTimeMs > 0) _renderTimesMs.Add(_currentRenderTimeMs);

            // Reset for next frame
            _currentMeasureTimeMs = 0;
            _currentArrangeTimeMs = 0;
            _currentRenderTimeMs = 0;
        }
    }

    public void ReportMeasureTime(double measureTimeMs)
    {
        lock (_lock)
        {
            _currentMeasureTimeMs = measureTimeMs;
        }
    }

    public void ReportArrangeTime(double arrangeTimeMs)
    {
        lock (_lock)
        {
            _currentArrangeTimeMs = arrangeTimeMs;
        }
    }

    public void ReportRenderTime(double renderTimeMs)
    {
        lock (_lock)
        {
            _currentRenderTimeMs = renderTimeMs;
        }
    }

    public void ReportMeasureSkipped()
    {
        lock (_lock)
        {
            _measureSkipped++;
        }
    }

    public void ReportArrangeSkipped()
    {
        lock (_lock)
        {
            _arrangeSkipped++;
        }
    }

    public void ReportPropertyUpdateSkipped(string propertyName)
    {
        lock (_lock)
        {
            _propertySkipped++;
        }
    }

    private void SendAggregatedData(object? state)
    {
        PerformanceFrameDto data;

        lock (_lock)
        {
            if (_frameCount == 0)
            {
                // No frames rendered - send empty data to indicate idle
                data = new PerformanceFrameDto
                {
                    Timestamp = DateTimeOffset.Now,
                    Fps = 0,
                    FrameTimeMs = 0,
                    UtilizationPercent = 0,
                    DidRender = false,
                    MemoryBytes = GC.GetTotalMemory(false)
                };
            }
            else
            {
                var avgFrameTime = _frameTimesMs.Count > 0 ? _frameTimesMs.Average() : 0;
                var avgMeasureTime = _measureTimesMs.Count > 0 ? _measureTimesMs.Average() : 0;
                var avgArrangeTime = _arrangeTimesMs.Count > 0 ? _arrangeTimesMs.Average() : 0;
                var avgRenderTime = _renderTimesMs.Count > 0 ? _renderTimesMs.Average() : 0;

                // Calculate FPS from frame count (we're sending every second)
                var fps = _frameCount;

                // Calculate utilization: how much of the 16.67ms budget is used
                // 16.67ms = 60fps budget
                var utilization = avgFrameTime > 0 ? (avgFrameTime / 16.67) * 100 : 0;

                data = new PerformanceFrameDto
                {
                    Timestamp = DateTimeOffset.Now,
                    FrameTimeMs = Math.Round(avgFrameTime, 2),
                    Fps = fps,
                    MeasureTimeMs = Math.Round(avgMeasureTime, 2),
                    ArrangeTimeMs = Math.Round(avgArrangeTime, 2),
                    RenderTimeMs = Math.Round(avgRenderTime, 2),
                    MeasureSkipped = _measureSkipped,
                    ArrangeSkipped = _arrangeSkipped,
                    PropertySkipped = _propertySkipped,
                    UtilizationPercent = Math.Round(utilization, 1),
                    MemoryBytes = GC.GetTotalMemory(false),
                    DidRender = true
                };
            }

            // Reset for next second
            _frameTimesMs.Clear();
            _measureTimesMs.Clear();
            _arrangeTimesMs.Clear();
            _renderTimesMs.Clear();
            _measureSkipped = 0;
            _arrangeSkipped = 0;
            _propertySkipped = 0;
            _frameCount = 0;
        }

        // Send async (fire and forget)
        _ = _client.SendAsync(new DebugMessage
        {
            Type = "performance",
            Data = data
        });
    }

    public void Dispose()
    {
        _sendTimer.Dispose();
    }
}
