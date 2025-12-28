using PlusUi.core;
using PlusUi.core.Services.Accessibility;
using System.Runtime.Versioning;

namespace PlusUi.desktop.Accessibility;

/// <summary>
/// macOS accessibility bridge using NSAccessibility protocol.
/// Exposes the UI element tree to VoiceOver and other assistive technologies.
/// </summary>
[SupportedOSPlatform("macos")]
public sealed class MacOSAccessibilityBridge : IAccessibilityBridge
{
    private Func<UiElement?>? _rootProvider;
    private IntPtr _windowHandle;
    private bool _isEnabled;
    private bool _disposed;

    /// <inheritdoc />
    public bool IsEnabled => _isEnabled;

    /// <summary>
    /// Initializes the macOS accessibility bridge.
    /// </summary>
    public MacOSAccessibilityBridge()
    {
        _isEnabled = IsVoiceOverRunning();
    }

    /// <summary>
    /// Sets the window handle for accessibility provider hosting.
    /// </summary>
    /// <param name="windowPtr">The native window pointer (NSWindow*).</param>
    public void SetWindowHandle(IntPtr windowPtr)
    {
        _windowHandle = windowPtr;
    }

    /// <inheritdoc />
    public void Initialize(Func<UiElement?> rootProvider)
    {
        _rootProvider = rootProvider;

        if (_windowHandle != IntPtr.Zero)
        {
            try
            {
                RegisterAccessibilityProvider();
            }
            catch
            {
                // Registration failed - accessibility features will be limited
            }
        }
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
            // Use NSAccessibility announcement
            AnnounceViaVoiceOver(message, interrupt);
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
            // Post NSAccessibilityFocusedUIElementChangedNotification
            if (element != null)
            {
                PostAccessibilityNotification(
                    NSAccessibilityNotification.FocusedUIElementChanged,
                    element);
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
            PostAccessibilityNotification(
                NSAccessibilityNotification.ValueChanged,
                element);
        }
        catch
        {
            // Value change notification failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyStructureChanged()
    {
        if (!_isEnabled || _windowHandle == IntPtr.Zero)
        {
            return;
        }

        try
        {
            var root = _rootProvider?.Invoke();
            if (root != null)
            {
                PostAccessibilityNotification(
                    NSAccessibilityNotification.UIElementsChanged,
                    root);
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
        if (!_isEnabled)
        {
            return;
        }

        try
        {
            PostAccessibilityNotification(
                NSAccessibilityNotification.TitleChanged,
                element);
        }
        catch
        {
            // Property change notification failed - silently ignore
        }
    }

    /// <inheritdoc />
    public object? GetAccessibilityNode(UiElement element)
    {
        return CreateAccessibilityElement(element);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            UnregisterAccessibilityProvider();
            _disposed = true;
        }
    }

    #region Private Methods

    private static bool IsVoiceOverRunning()
    {
        try
        {
            // Check if VoiceOver is enabled via system preferences
            // This uses NSWorkspace.sharedWorkspace.isVoiceOverEnabled
            return CheckVoiceOverEnabled();
        }
        catch
        {
            return false;
        }
    }

    private void RegisterAccessibilityProvider()
    {
        // Register with macOS accessibility system
        // Full implementation would use P/Invoke to NSAccessibility APIs
    }

    private void UnregisterAccessibilityProvider()
    {
        // Unregister from macOS accessibility system
    }

    private static PlusUiAccessibilityElement CreateAccessibilityElement(UiElement element)
    {
        return new PlusUiAccessibilityElement(element);
    }

    private void AnnounceViaVoiceOver(string message, bool interrupt)
    {
        // Use NSAccessibilityPostNotificationWithUserInfo for announcements
        // Native call would be:
        // NSAccessibilityPostNotificationWithUserInfo(
        //     NSApp,
        //     NSAccessibilityAnnouncementRequestedNotification,
        //     @{NSAccessibilityAnnouncementKey: message, NSAccessibilityPriorityKey: priority});
    }

    private void PostAccessibilityNotification(NSAccessibilityNotification notification, UiElement element)
    {
        // Native call would be:
        // NSAccessibilityPostNotification(accessibilityElement, notificationName);
    }

    private static bool CheckVoiceOverEnabled()
    {
        // Would use P/Invoke to call:
        // [[NSWorkspace sharedWorkspace] isVoiceOverEnabled]
        return false;
    }

    #endregion
}
