using Microsoft.Extensions.Logging;

namespace PlusUi.core.Services.Rendering;

/// <summary>
/// Cross-platform render loop service that manages continuous 60 FPS rendering
/// when animations or other continuous invalidators are active.
/// </summary>
public class RenderLoopService : IDisposable
{
    private readonly InvalidationTracker _invalidationTracker;
    private readonly ILogger<RenderLoopService> _logger;
    private readonly SynchronizationContext? _synchronizationContext;
    private System.Threading.Timer? _renderTimer;
    private bool _isRunning;

    /// <summary>
    /// Fired every 16ms (60 FPS) when continuous rendering is needed.
    /// This event is always invoked on the main/UI thread to ensure thread safety for rendering operations.
    /// Platforms should subscribe to this event and call their platform-specific render method.
    /// </summary>
    public event EventHandler? RenderRequested;

    public RenderLoopService(
        InvalidationTracker invalidationTracker,
        ILogger<RenderLoopService> logger)
    {
        _invalidationTracker = invalidationTracker;
        _logger = logger;

        // Capture the synchronization context from the main thread (this constructor is called during DI setup on main thread)
        _synchronizationContext = SynchronizationContext.Current;

        // Subscribe to invalidation changes to start/stop the render loop
        _invalidationTracker.RenderingRequiredChanged += OnRenderingRequiredChanged;
    }

    private void OnRenderingRequiredChanged(object? sender, EventArgs e)
    {
        if (_invalidationTracker.NeedsRendering && !_isRunning)
        {
            StartRenderLoop();
        }
        else if (!_invalidationTracker.NeedsRendering && _isRunning)
        {
            StopRenderLoop();
        }
    }

    private void StartRenderLoop()
    {
        if (_isRunning) return;

        _isRunning = true;
        _logger.LogDebug("Starting continuous render loop (60 FPS)");

        // 60 FPS = ~16ms per frame
        _renderTimer = new System.Threading.Timer(_ =>
        {
            if (_invalidationTracker.NeedsRendering)
            {
                InvokeRenderRequested();
            }
        }, null, 0, 16);
    }

    private void InvokeRenderRequested()
    {
        if (_synchronizationContext != null)
        {
            // Marshal the event invocation to the main/UI thread
            _synchronizationContext.Post(_ => RenderRequested?.Invoke(this, EventArgs.Empty), null);
        }
        else
        {
            // Fallback: invoke directly if no synchronization context is available
            RenderRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    private void StopRenderLoop()
    {
        if (!_isRunning) return;

        _isRunning = false;
        _renderTimer?.Dispose();
        _renderTimer = null;

        _logger.LogDebug("Stopped continuous render loop");
    }

    public void Dispose()
    {
        StopRenderLoop();
        _invalidationTracker.RenderingRequiredChanged -= OnRenderingRequiredChanged;
    }
}
