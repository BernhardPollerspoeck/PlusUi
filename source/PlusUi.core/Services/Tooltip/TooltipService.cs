namespace PlusUi.core;

/// <summary>
/// Service that manages tooltip display, timing, and lifecycle.
/// </summary>
public class TooltipService(IOverlayService overlayService) : ITooltipService, IDisposable
{
    private readonly object _lock = new();

    private Timer? _showTimer;
    private Timer? _hideTimer;
    private TooltipOverlay? _currentOverlay;
    private UiElement? _currentTargetElement;
    private bool _disposed;

    public void OnHoverEnter(UiElement? element)
    {
        if (_disposed || element == null)
        {
            return;
        }

        // Check if element has a tooltip
        var tooltip = element.Tooltip;
        if (tooltip?.Content == null)
        {
            return;
        }

        // If already showing tooltip for this element, do nothing
        if (_currentTargetElement == element && _currentOverlay != null)
        {
            return;
        }

        // Hide any existing tooltip first
        HideTooltipImmediate();

        // Update bindings
        tooltip.UpdateBindings();

        // Start show timer
        _currentTargetElement = element;

        lock (_lock)
        {
            CancelTimers();

            if (tooltip.ShowDelay <= 0)
            {
                ShowTooltipImmediate(element, tooltip);
            }
            else
            {
                _showTimer = new Timer(
                    _ => ShowTooltipCallback(element, tooltip),
                    null,
                    tooltip.ShowDelay,
                    Timeout.Infinite);
            }
        }
    }

    public void OnHoverLeave(UiElement? element)
    {
        if (_disposed)
        {
            return;
        }

        // Only hide if we're leaving the element that has the current tooltip
        if (element == null || element != _currentTargetElement)
        {
            return;
        }

        var tooltip = element.Tooltip;
        var hideDelay = tooltip?.HideDelay ?? 0;

        lock (_lock)
        {
            CancelTimers();

            if (hideDelay <= 0)
            {
                HideTooltipImmediate();
            }
            else
            {
                _hideTimer = new Timer(
                    _ => HideTooltipCallback(),
                    null,
                    hideDelay,
                    Timeout.Infinite);
            }
        }
    }

    private void ShowTooltipCallback(UiElement element, TooltipAttachment tooltip)
    {
        // All verification now happens inside ShowTooltipImmediate under lock
        ShowTooltipImmediate(element, tooltip);
    }

    private void ShowTooltipImmediate(UiElement element, TooltipAttachment tooltip)
    {
        TooltipOverlay? newOverlay = null;
        try
        {
            lock (_lock)
            {
                // Verify element is still the target (under lock to prevent race condition)
                if (_disposed || _currentTargetElement != element)
                {
                    return;
                }

                // Create and register overlay
                newOverlay = new TooltipOverlay(element, tooltip);
                _currentOverlay = newOverlay;
                overlayService.RegisterOverlay(_currentOverlay);
            }
        }
        catch
        {
            // Clean up on failure to prevent memory leak
            lock (_lock)
            {
                newOverlay?.Dispose();
                if (_currentOverlay == newOverlay)
                {
                    _currentOverlay = null;
                }
            }
        }
    }

    private void HideTooltipCallback()
    {
        HideTooltipImmediate();
    }

    private void HideTooltipImmediate()
    {
        lock (_lock)
        {
            if (_currentOverlay != null)
            {
                try
                {
                    overlayService.UnregisterOverlay(_currentOverlay);
                }
                finally
                {
                    // Always dispose to prevent memory leak
                    _currentOverlay.Dispose();
                    _currentOverlay = null;
                }
            }
            _currentTargetElement = null;
        }
    }

    private void CancelTimers()
    {
        _showTimer?.Dispose();
        _showTimer = null;
        _hideTimer?.Dispose();
        _hideTimer = null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        lock (_lock)
        {
            CancelTimers();
            HideTooltipImmediate();
        }

        GC.SuppressFinalize(this);
    }
}
