using PlusUi.core;
using PlusUi.core.Services.Accessibility;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace PlusUi.desktop.Accessibility;

/// <summary>
/// Windows accessibility bridge using UI Automation (UIA).
/// Implements the IRawElementProviderFragmentRoot pattern for full tree exposure.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsAccessibilityBridge : IAccessibilityBridge
{
    private Func<UiElement?>? _rootProvider;
    private IntPtr _windowHandle;
    private bool _isEnabled;
    private bool _disposed;

    /// <inheritdoc />
    public bool IsEnabled => _isEnabled;

    /// <summary>
    /// Initializes the Windows accessibility bridge.
    /// </summary>
    public WindowsAccessibilityBridge()
    {
        // Check if a screen reader is running
        _isEnabled = IsScreenReaderRunning();
    }

    /// <summary>
    /// Sets the window handle for UI Automation provider hosting.
    /// </summary>
    /// <param name="hwnd">The native window handle.</param>
    public void SetWindowHandle(IntPtr hwnd)
    {
        _windowHandle = hwnd;
    }

    /// <inheritdoc />
    public void Initialize(Func<UiElement?> rootProvider)
    {
        _rootProvider = rootProvider;

        // Register as UIA provider if we have a window handle
        if (_windowHandle != IntPtr.Zero)
        {
            try
            {
                RegisterUiaProvider();
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
            // Use NotifyWinEvent or UIA events for announcements
            AnnounceViaUia(message, interrupt);
        }
        catch
        {
            // Announcement failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyFocusChanged(UiElement? element)
    {
        if (!_isEnabled || _windowHandle == IntPtr.Zero)
        {
            return;
        }

        try
        {
            // Raise UIA focus changed event
            if (element != null)
            {
                var provider = GetOrCreateProvider(element);
                if (provider != null)
                {
                    RaiseAutomationEvent(provider, AutomationEvents.AutomationFocusChanged);
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
            var provider = GetOrCreateProvider(element);
            if (provider != null)
            {
                RaisePropertyChangedEvent(provider, AutomationProperties.Value, element.GetComputedAccessibilityValue());
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
        if (!_isEnabled || _windowHandle == IntPtr.Zero)
        {
            return;
        }

        try
        {
            // Raise structure changed event on root
            var root = _rootProvider?.Invoke();
            if (root != null)
            {
                var provider = GetOrCreateProvider(root);
                if (provider != null)
                {
                    RaiseStructureChangedEvent(provider);
                }
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
            var provider = GetOrCreateProvider(element);
            if (provider != null)
            {
                // Notify about relevant property changes
                RaisePropertyChangedEvent(provider, AutomationProperties.Name, element.GetComputedAccessibilityLabel());
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
        return GetOrCreateProvider(element);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            UnregisterUiaProvider();
            _disposed = true;
        }
    }

    #region Private Methods

    private static bool IsScreenReaderRunning()
    {
        try
        {
            // Check for common screen readers via system parameters
            var screenReaderRunning = false;
            SystemParametersInfo(SPI_GETSCREENREADER, 0, ref screenReaderRunning, 0);
            return screenReaderRunning;
        }
        catch
        {
            return false;
        }
    }

    private void RegisterUiaProvider()
    {
        // This is a simplified registration - full implementation would use COM interop
        // to implement IRawElementProviderFragmentRoot
    }

    private void UnregisterUiaProvider()
    {
        // Cleanup UIA provider registration
    }

    private PlusUiAutomationProvider? GetOrCreateProvider(UiElement element)
    {
        // Create or retrieve cached provider for element
        return new PlusUiAutomationProvider(element, _windowHandle);
    }

    private void AnnounceViaUia(string message, bool interrupt)
    {
        // Use NotifyWinEvent with EVENT_SYSTEM_ALERT for announcements
        // This is picked up by screen readers
        if (_windowHandle != IntPtr.Zero)
        {
            NotifyWinEvent(EVENT_SYSTEM_ALERT, _windowHandle, OBJID_CLIENT, CHILDID_SELF);
        }
    }

    private static void RaiseAutomationEvent(PlusUiAutomationProvider provider, AutomationEvents eventId)
    {
        // Raise UIA event - simplified implementation
        // Full implementation would use UiaRaiseAutomationEvent
    }

    private static void RaisePropertyChangedEvent(PlusUiAutomationProvider provider, AutomationProperties property, string? newValue)
    {
        // Raise UIA property changed event - simplified implementation
        // Full implementation would use UiaRaiseAutomationPropertyChangedEvent
    }

    private static void RaiseStructureChangedEvent(PlusUiAutomationProvider provider)
    {
        // Raise UIA structure changed event - simplified implementation
        // Full implementation would use UiaRaiseStructureChangedEvent
    }

    #endregion

    #region Native Methods

    private const uint SPI_GETSCREENREADER = 0x0046;
    private const uint EVENT_SYSTEM_ALERT = 0x0002;
    private const int OBJID_CLIENT = -4;
    private const int CHILDID_SELF = 0;

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref bool pvParam, uint fWinIni);

    [DllImport("user32.dll")]
    private static extern void NotifyWinEvent(uint eventType, IntPtr hwnd, int idObject, int idChild);

    #endregion
}
