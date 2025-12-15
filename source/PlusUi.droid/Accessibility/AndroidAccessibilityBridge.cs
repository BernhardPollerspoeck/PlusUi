using Android.Content;
using Android.Views.Accessibility;
using PlusUi.core;
using PlusUi.core.Services.Accessibility;

namespace PlusUi.droid.Accessibility;

/// <summary>
/// Android accessibility bridge using AccessibilityNodeProvider for TalkBack support.
/// Exposes the UI element tree as virtual accessibility nodes.
/// </summary>
public sealed class AndroidAccessibilityBridge : IAccessibilityBridge
{
    private Func<UiElement?>? _rootProvider;
    private Context? _context;
    private Android.Views.View? _hostView;
    private AccessibilityManager? _accessibilityManager;
    private bool _isEnabled;
    private bool _disposed;
    private PlusUiAccessibilityNodeProvider? _nodeProvider;

    /// <inheritdoc />
    public bool IsEnabled => _isEnabled;

    /// <summary>
    /// Initializes the Android accessibility bridge.
    /// </summary>
    public AndroidAccessibilityBridge()
    {
    }

    /// <summary>
    /// Sets the Android context and host view for accessibility support.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="hostView">The host view (typically the GLSurfaceView or main content view).</param>
    public void SetContext(Context context, Android.Views.View hostView)
    {
        _context = context;
        _hostView = hostView;
        _accessibilityManager = context.GetSystemService(Context.AccessibilityService) as AccessibilityManager;
        _isEnabled = _accessibilityManager?.IsEnabled == true && _accessibilityManager?.IsTouchExplorationEnabled == true;

        // Set up accessibility node provider
        if (_hostView != null)
        {
            _nodeProvider = new PlusUiAccessibilityNodeProvider(_hostView, () => _rootProvider?.Invoke());
            _hostView.SetAccessibilityDelegate(new PlusUiAccessibilityDelegate(_nodeProvider));
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
        if (string.IsNullOrEmpty(message) || !_isEnabled || _hostView == null)
        {
            return;
        }

        try
        {
            // Use AccessibilityEvent.TYPE_ANNOUNCEMENT for TalkBack announcements
            var announcement = AccessibilityEvent.Obtain(EventTypes.Announcement);
            if (announcement != null)
            {
                announcement.Text?.Add(new Java.Lang.String(message));
                announcement.PackageName = _context?.PackageName;

                _accessibilityManager?.SendAccessibilityEvent(announcement);
            }
        }
        catch
        {
            // Announcement failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyFocusChanged(UiElement? element)
    {
        if (!_isEnabled || _hostView == null)
        {
            return;
        }

        try
        {
            if (element != null)
            {
                var virtualViewId = GetVirtualViewId(element);
                _hostView.SendAccessibilityEvent(EventTypes.ViewFocused);

                // Also notify the node provider to update focus
                _nodeProvider?.NotifyFocusChanged(virtualViewId);
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
        if (!_isEnabled || _hostView == null)
        {
            return;
        }

        try
        {
            var virtualViewId = GetVirtualViewId(element);

            // Send content changed event
            var evt = AccessibilityEvent.Obtain(EventTypes.WindowContentChanged);
            if (evt != null)
            {
                evt.ContentChangeTypes = ContentChangeTypes.Text;
                evt.PackageName = _context?.PackageName;
                _accessibilityManager?.SendAccessibilityEvent(evt);
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
            // Invalidate the virtual view hierarchy
            _nodeProvider?.InvalidateVirtualView();

            var evt = AccessibilityEvent.Obtain(EventTypes.WindowContentChanged);
            if (evt != null)
            {
                evt.ContentChangeTypes = ContentChangeTypes.Subtree;
                evt.PackageName = _context?.PackageName;
                _accessibilityManager?.SendAccessibilityEvent(evt);
            }
        }
        catch
        {
            // Structure change notification failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyPropertyChanged(UiElement element)
    {
        if (!_isEnabled || _hostView == null)
        {
            return;
        }

        try
        {
            var virtualViewId = GetVirtualViewId(element);

            var evt = AccessibilityEvent.Obtain(EventTypes.WindowContentChanged);
            if (evt != null)
            {
                evt.ContentChangeTypes = ContentChangeTypes.ContentDescription;
                evt.PackageName = _context?.PackageName;
                _accessibilityManager?.SendAccessibilityEvent(evt);
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
        return _nodeProvider?.CreateAccessibilityNodeInfo(element);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _nodeProvider = null;
            _disposed = true;
        }
    }

    private static int GetVirtualViewId(UiElement element)
    {
        // Use element's hash code as virtual view ID
        // In a full implementation, you'd maintain a mapping
        return element.GetHashCode();
    }
}
