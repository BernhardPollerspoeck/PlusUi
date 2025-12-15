using PlusUi.core;
using PlusUi.core.Services.Accessibility;
using System.Runtime.Versioning;

namespace PlusUi.desktop.Accessibility;

/// <summary>
/// Linux accessibility bridge using AT-SPI2 (Assistive Technology Service Provider Interface).
/// Communicates via D-Bus to expose the UI element tree to Orca and other screen readers.
/// </summary>
[SupportedOSPlatform("linux")]
public sealed class LinuxAccessibilityBridge : IAccessibilityBridge
{
    private Func<UiElement?>? _rootProvider;
    private bool _isEnabled;
    private bool _disposed;
    private AtSpiConnection? _connection;

    /// <inheritdoc />
    public bool IsEnabled => _isEnabled;

    /// <summary>
    /// Initializes the Linux accessibility bridge.
    /// </summary>
    public LinuxAccessibilityBridge()
    {
        _isEnabled = IsAtSpiAvailable();
    }

    /// <inheritdoc />
    public void Initialize(Func<UiElement?> rootProvider)
    {
        _rootProvider = rootProvider;

        if (_isEnabled)
        {
            try
            {
                InitializeAtSpiConnection();
            }
            catch
            {
                // Failed to initialize AT-SPI connection
                _isEnabled = false;
            }
        }
    }

    /// <inheritdoc />
    public void Announce(string message, bool interrupt = false)
    {
        if (string.IsNullOrEmpty(message) || !_isEnabled || _connection == null)
        {
            return;
        }

        try
        {
            _connection.SendAnnouncement(message, interrupt);
        }
        catch
        {
            // Announcement failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyFocusChanged(UiElement? element)
    {
        if (!_isEnabled || _connection == null)
        {
            return;
        }

        try
        {
            if (element != null)
            {
                var atSpiObject = GetOrCreateAtSpiObject(element);
                _connection.EmitFocusEvent(atSpiObject);
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
        if (!_isEnabled || _connection == null)
        {
            return;
        }

        try
        {
            var atSpiObject = GetOrCreateAtSpiObject(element);
            _connection.EmitPropertyChangeEvent(atSpiObject, "accessible-value", element.GetComputedAccessibilityValue());
        }
        catch
        {
            // Value change notification failed - silently ignore
        }
    }

    /// <inheritdoc />
    public void NotifyStructureChanged()
    {
        if (!_isEnabled || _connection == null)
        {
            return;
        }

        try
        {
            var root = _rootProvider?.Invoke();
            if (root != null)
            {
                var atSpiObject = GetOrCreateAtSpiObject(root);
                _connection.EmitChildrenChangedEvent(atSpiObject);
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
        if (!_isEnabled || _connection == null)
        {
            return;
        }

        try
        {
            var atSpiObject = GetOrCreateAtSpiObject(element);
            _connection.EmitPropertyChangeEvent(atSpiObject, "accessible-name", element.GetComputedAccessibilityLabel());
        }
        catch
        {
            // Property change notification failed - silently ignore
        }
    }

    /// <inheritdoc />
    public object? GetAccessibilityNode(UiElement element)
    {
        return GetOrCreateAtSpiObject(element);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _connection?.Dispose();
            _connection = null;
            _disposed = true;
        }
    }

    #region Private Methods

    private static bool IsAtSpiAvailable()
    {
        try
        {
            // Check if AT-SPI2 is available via D-Bus
            // Look for the AT-SPI registry on the session bus
            var atSpiAddress = Environment.GetEnvironmentVariable("AT_SPI_BUS_ADDRESS");
            if (!string.IsNullOrEmpty(atSpiAddress))
            {
                return true;
            }

            // Also check if accessibility is enabled via gsettings
            return CheckGSettingsAccessibility();
        }
        catch
        {
            return false;
        }
    }

    private static bool CheckGSettingsAccessibility()
    {
        try
        {
            // This would typically use D-Bus to query GSettings
            // For now, we assume it's available if running on Linux
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void InitializeAtSpiConnection()
    {
        _connection = new AtSpiConnection();
        _connection.Connect();

        // Register the application with AT-SPI registry
        if (_rootProvider != null)
        {
            var root = _rootProvider();
            if (root != null)
            {
                var rootObject = GetOrCreateAtSpiObject(root);
                _connection.RegisterApplication(rootObject);
            }
        }
    }

    private static AtSpiAccessibleObject GetOrCreateAtSpiObject(UiElement element)
    {
        return new AtSpiAccessibleObject(element);
    }

    #endregion
}
