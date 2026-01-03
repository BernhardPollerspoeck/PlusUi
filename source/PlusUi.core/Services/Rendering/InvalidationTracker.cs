using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace PlusUi.core.Services.Rendering;

/// <summary>
/// Tracks all components that require rendering and provides a centralized way
/// to determine if the platform render loop should be active.
/// This enables battery-efficient rendering by only rendering when necessary.
/// </summary>
public class InvalidationTracker
{
    private readonly HashSet<IInvalidator> _invalidators = [];
    private readonly ILogger<InvalidationTracker>? _logger;
    private bool _manualRenderRequested;
    private Timer? _debounceTimer;
    private bool _previousRenderingState;

    public InvalidationTracker(ILogger<InvalidationTracker>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Indicates whether rendering is currently needed.
    /// True if any registered invalidator needs rendering OR if a manual render was requested.
    /// </summary>
    public bool NeedsRendering
    {
        get
        {
            if (_manualRenderRequested)
                return true;

            return _invalidators.Any(i => i.NeedsRendering);
        }
    }

    /// <summary>
    /// Event fired when rendering state changes (needed → not needed, or vice versa).
    /// Platforms can subscribe to this to start/stop their render loops efficiently.
    /// </summary>
    public event EventHandler? RenderingRequiredChanged;

    /// <summary>
    /// Manually request a single render frame.
    /// Used for one-time updates that don't require continuous rendering
    /// (e.g., resize, single property change, input event).
    /// The flag is automatically cleared after rendering.
    /// </summary>
    /// <param name="reason">Optional reason for debugging (captured via CallerMemberName)</param>
    public void RequestRender([CallerMemberName] string? reason = null)
    {
        _logger?.LogTrace("Manual render requested: {Reason}", reason);

        var wasNeeded = NeedsRendering;
        _manualRenderRequested = true;

        if (!wasNeeded)
        {
            NotifyRenderingStateChanged();
        }
    }

    /// <summary>
    /// Called by platform after rendering a frame.
    /// Clears the manual render request flag.
    /// </summary>
    public void FrameRendered()
    {
        if (_manualRenderRequested)
        {
            _manualRenderRequested = false;

            // Check if we should notify state change (rendering → idle)
            if (!NeedsRendering && _previousRenderingState)
            {
                NotifyRenderingStateChanged();
            }
        }
    }

    /// <summary>
    /// Registers an invalidator that can request continuous rendering.
    /// The invalidator will be tracked until explicitly unregistered.
    /// </summary>
    /// <param name="invalidator">The component to track</param>
    /// <param name="caller">Automatically captured caller name for debugging</param>
    public void Register(IInvalidator invalidator, [CallerMemberName] string? caller = null)
    {
        _logger?.LogTrace("Invalidator registered: {Type} from {Caller}",
            invalidator.GetType().Name, caller);

        if (_invalidators.Add(invalidator))
        {
            invalidator.InvalidationChanged += OnInvalidatorChanged;
            CheckRenderingState();
        }
    }

    /// <summary>
    /// Unregisters a previously registered invalidator.
    /// Should be called when the component is disposed or no longer needs tracking.
    /// </summary>
    /// <param name="invalidator">The component to unregister</param>
    public void Unregister(IInvalidator invalidator)
    {
        _logger?.LogTrace("Invalidator unregistered: {Type}",
            invalidator.GetType().Name);

        if (_invalidators.Remove(invalidator))
        {
            invalidator.InvalidationChanged -= OnInvalidatorChanged;
            CheckRenderingState();
        }
    }

    private void OnInvalidatorChanged(object? sender, EventArgs e)
    {
        // Debounce to prevent excessive state change notifications
        // when multiple invalidators change state rapidly
        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(_ =>
        {
            CheckRenderingState();
        }, null, 16, Timeout.Infinite); // 16ms = ~1 frame @ 60fps
    }

    private void CheckRenderingState()
    {
        var needsRendering = NeedsRendering;

        if (needsRendering != _previousRenderingState)
        {
            _logger?.LogDebug("Rendering state changed: {NeedsRendering}. Active invalidators: {Count}",
                needsRendering, _invalidators.Count(i => i.NeedsRendering));

            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                foreach (var inv in _invalidators.Where(i => i.NeedsRendering))
                {
                    _logger.LogTrace("  - {Type} needs rendering", inv.GetType().Name);
                }
            }

            NotifyRenderingStateChanged();
        }

        _previousRenderingState = needsRendering;
    }

    private void NotifyRenderingStateChanged()
    {
        RenderingRequiredChanged?.Invoke(this, EventArgs.Empty);
    }
}
