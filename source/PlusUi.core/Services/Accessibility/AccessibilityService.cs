using PlusUi.core.Services.Focus;

namespace PlusUi.core.Services.Accessibility;

/// <summary>
/// Core accessibility service that coordinates accessibility features across the application.
/// Integrates with the focus manager and platform-specific accessibility bridges.
/// </summary>
public class AccessibilityService : IAccessibilityService, IDisposable
{
    private readonly IAccessibilityBridge _bridge;
    private readonly IFocusManager _focusManager;
    private bool _disposed;

    /// <inheritdoc />
    public bool IsEnabled => _bridge.IsEnabled;

    /// <inheritdoc />
    public IAccessibilityBridge Bridge => _bridge;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessibilityService"/> class.
    /// </summary>
    /// <param name="bridge">The platform-specific accessibility bridge.</param>
    /// <param name="focusManager">The focus manager service.</param>
    public AccessibilityService(IAccessibilityBridge bridge, IFocusManager focusManager)
    {
        _bridge = bridge;
        _focusManager = focusManager;

        // Subscribe to focus changes
        _focusManager.FocusChanged += OnFocusChanged;
    }

    /// <summary>
    /// Initializes the accessibility service with the root element provider.
    /// This should be called after the UI is ready.
    /// </summary>
    /// <param name="rootProvider">Function that returns the root UI element.</param>
    public void Initialize(Func<UiElement?> rootProvider)
    {
        _bridge.Initialize(rootProvider);
    }

    /// <inheritdoc />
    public void Announce(string message, bool interrupt = false)
    {
        _bridge.Announce(message, interrupt);
    }

    /// <inheritdoc />
    public void NotifyValueChanged(UiElement element)
    {
        _bridge.NotifyValueChanged(element);
    }

    /// <inheritdoc />
    public void NotifyStructureChanged()
    {
        _bridge.NotifyStructureChanged();
    }

    /// <inheritdoc />
    public void NotifyPropertyChanged(UiElement element)
    {
        _bridge.NotifyPropertyChanged(element);
    }

    private void OnFocusChanged(object? sender, FocusChangedEventArgs e)
    {
        // Get the UiElement from the focused element (IFocusable could be implemented by non-UiElement, but typically is)
        var focusedElement = e.NewElement as UiElement;
        _bridge.NotifyFocusChanged(focusedElement);

        // If accessibility is enabled, announce the focused element
        if (IsEnabled && focusedElement != null)
        {
            var label = focusedElement.GetComputedAccessibilityLabel();
            var value = focusedElement.GetComputedAccessibilityValue();
            var role = focusedElement.AccessibilityRole;

            // Build announcement
            var announcement = BuildFocusAnnouncement(label, value, role, focusedElement.AccessibilityHint);
            if (!string.IsNullOrEmpty(announcement))
            {
                Announce(announcement);
            }
        }
    }

    private static string BuildFocusAnnouncement(string? label, string? value, AccessibilityRole role, string? hint)
    {
        var parts = new List<string>();

        // Add label
        if (!string.IsNullOrEmpty(label))
        {
            parts.Add(label);
        }

        // Add role description
        var roleDescription = GetRoleDescription(role);
        if (!string.IsNullOrEmpty(roleDescription))
        {
            parts.Add(roleDescription);
        }

        // Add value
        if (!string.IsNullOrEmpty(value))
        {
            parts.Add(value);
        }

        // Add hint
        if (!string.IsNullOrEmpty(hint))
        {
            parts.Add(hint);
        }

        return string.Join(", ", parts);
    }

    private static string GetRoleDescription(AccessibilityRole role)
    {
        return role switch
        {
            AccessibilityRole.Button => "Button",
            AccessibilityRole.Checkbox => "Checkbox",
            AccessibilityRole.RadioButton => "Radio button",
            AccessibilityRole.TextInput => "Text field",
            AccessibilityRole.Label => "",  // Labels don't need role announcement
            AccessibilityRole.Heading => "Heading",
            AccessibilityRole.Link => "Link",
            AccessibilityRole.Image => "Image",
            AccessibilityRole.Slider => "Slider",
            AccessibilityRole.Toggle => "Toggle",
            AccessibilityRole.List => "List",
            AccessibilityRole.ListItem => "",  // List items typically announced as part of list context
            AccessibilityRole.ComboBox => "Combo box",
            AccessibilityRole.ProgressBar => "Progress",
            AccessibilityRole.ScrollView => "",  // Scroll views don't need announcement
            AccessibilityRole.Container => "",  // Containers don't need announcement
            AccessibilityRole.Dialog => "Dialog",
            AccessibilityRole.Alert => "Alert",
            AccessibilityRole.Toolbar => "Toolbar",
            AccessibilityRole.Menu => "Menu",
            AccessibilityRole.MenuItem => "Menu item",
            AccessibilityRole.Tab => "Tab",
            AccessibilityRole.TabPanel => "Tab panel",
            AccessibilityRole.DatePicker => "Date picker",
            AccessibilityRole.TimePicker => "Time picker",
            AccessibilityRole.Spinner => "Loading",
            AccessibilityRole.Tooltip => "",  // Tooltips are typically read as part of focus context
            AccessibilityRole.Page => "",  // Pages don't need role announcement
            AccessibilityRole.Navigation => "Navigation",
            _ => ""
        };
    }

    /// <summary>
    /// Disposes of the accessibility service and unsubscribes from events.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _focusManager.FocusChanged -= OnFocusChanged;
                _bridge.Dispose();
            }
            _disposed = true;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
