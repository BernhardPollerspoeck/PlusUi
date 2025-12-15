using Foundation;
using PlusUi.core;
using PlusUi.core.Services.Accessibility;
using UIKit;

namespace PlusUi.ios.Accessibility;

/// <summary>
/// iOS accessibility bridge using UIAccessibility protocol for VoiceOver support.
/// Exposes the UI element tree as accessibility elements.
/// </summary>
public sealed class IosAccessibilityBridge : IAccessibilityBridge
{
    private Func<UiElement?>? _rootProvider;
    private UIView? _hostView;
    private bool _isEnabled;
    private bool _disposed;
    private PlusUiAccessibilityContainer? _accessibilityContainer;
    private NSObject? _voiceOverObserver;

    /// <inheritdoc />
    public bool IsEnabled => _isEnabled;

    /// <summary>
    /// Initializes the iOS accessibility bridge.
    /// </summary>
    public IosAccessibilityBridge()
    {
        _isEnabled = UIAccessibility.IsVoiceOverRunning;

        // Listen for VoiceOver status changes
        _voiceOverObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIView.VoiceOverStatusDidChangeNotification,
            notification => _isEnabled = UIAccessibility.IsVoiceOverRunning);
    }

    /// <summary>
    /// Sets the host view for accessibility support.
    /// </summary>
    /// <param name="hostView">The host UIView (typically the GLKView or main content view).</param>
    public void SetHostView(UIView hostView)
    {
        _hostView = hostView;

        if (_hostView != null)
        {
            _accessibilityContainer = new PlusUiAccessibilityContainer(
                _hostView,
                () => _rootProvider?.Invoke());

            // Make the host view an accessibility container
            _hostView.IsAccessibilityElement = false;
            _hostView.ShouldGroupAccessibilityChildren = true;
        }
    }

    /// <inheritdoc />
    public void Initialize(Func<UiElement?> rootProvider)
    {
        _rootProvider = rootProvider;
    }

    /// <inheritdoc />
    public void Announce(string message, bool interrupt = false)
    {
        if (string.IsNullOrEmpty(message) || !_isEnabled)
        {
            return;
        }

        try
        {
            // Use UIAccessibility.PostNotification for VoiceOver announcements
            var notification = interrupt
                ? UIAccessibilityPostNotification.ScreenChanged
                : UIAccessibilityPostNotification.Announcement;

            UIAccessibility.PostNotification(notification, new NSString(message));
        }
        catch
        {
            // Announcement failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyFocusChanged(UiElement? element)
    {
        if (!_isEnabled)
        {
            return;
        }

        try
        {
            if (element != null)
            {
                var accessibilityElement = GetAccessibilityElement(element);
                if (accessibilityElement != null)
                {
                    // Post notification to move VoiceOver cursor to this element
                    UIAccessibility.PostNotification(
                        UIAccessibilityPostNotification.LayoutChanged,
                        accessibilityElement);
                }
            }
        }
        catch
        {
            // Focus notification failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyValueChanged(UiElement element)
    {
        if (!_isEnabled)
        {
            return;
        }

        try
        {
            // Post screen changed notification to refresh accessibility info
            var accessibilityElement = GetAccessibilityElement(element);
            if (accessibilityElement != null)
            {
                // VoiceOver will re-read the element value
                UIAccessibility.PostNotification(
                    UIAccessibilityPostNotification.LayoutChanged,
                    accessibilityElement);
            }
        }
        catch
        {
            // Value change notification failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyStructureChanged()
    {
        if (!_isEnabled || _hostView == null)
        {
            return;
        }

        try
        {
            // Invalidate the accessibility tree
            _accessibilityContainer?.InvalidateAccessibilityElements();

            // Post screen changed notification
            UIAccessibility.PostNotification(
                UIAccessibilityPostNotification.ScreenChanged,
                _hostView);
        }
        catch
        {
            // Structure change notification failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyPropertyChanged(UiElement element)
    {
        if (!_isEnabled)
        {
            return;
        }

        try
        {
            var accessibilityElement = GetAccessibilityElement(element);
            if (accessibilityElement != null)
            {
                UIAccessibility.PostNotification(
                    UIAccessibilityPostNotification.LayoutChanged,
                    accessibilityElement);
            }
        }
        catch
        {
            // Property change notification failed - silently ignore
        }
    }

    /// <inheritdoc />
    public object? GetAccessibilityNode(UiElement element)
    {
        return GetAccessibilityElement(element);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            if (_voiceOverObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_voiceOverObserver);
                _voiceOverObserver = null;
            }
            _accessibilityContainer = null;
            _disposed = true;
        }
    }

    private UIAccessibilityElement? GetAccessibilityElement(UiElement element)
    {
        return _accessibilityContainer?.GetAccessibilityElement(element);
    }
}
