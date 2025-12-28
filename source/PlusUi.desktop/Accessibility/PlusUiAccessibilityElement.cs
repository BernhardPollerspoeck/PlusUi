using PlusUi.core;
using System.Runtime.Versioning;

namespace PlusUi.desktop.Accessibility;

/// <summary>
/// NSAccessibility element wrapper for a PlusUi element.
/// </summary>
[SupportedOSPlatform("macos")]
internal sealed class PlusUiAccessibilityElement
{
    public UiElement Element { get; }

    public PlusUiAccessibilityElement(UiElement element)
    {
        Element = element;
    }

    /// <summary>
    /// Gets the NSAccessibility role for this element.
    /// </summary>
    public string GetAccessibilityRole()
    {
        return Element.AccessibilityRole switch
        {
            AccessibilityRole.Button => "AXButton",
            AccessibilityRole.Checkbox => "AXCheckBox",
            AccessibilityRole.RadioButton => "AXRadioButton",
            AccessibilityRole.TextInput => "AXTextField",
            AccessibilityRole.Label => "AXStaticText",
            AccessibilityRole.Heading => "AXHeading",
            AccessibilityRole.Link => "AXLink",
            AccessibilityRole.Image => "AXImage",
            AccessibilityRole.Slider => "AXSlider",
            AccessibilityRole.Toggle => "AXCheckBox",
            AccessibilityRole.List => "AXList",
            AccessibilityRole.ListItem => "AXCell",
            AccessibilityRole.ComboBox => "AXComboBox",
            AccessibilityRole.ProgressBar => "AXProgressIndicator",
            AccessibilityRole.ScrollView => "AXScrollArea",
            AccessibilityRole.Container => "AXGroup",
            AccessibilityRole.Dialog => "AXSheet",
            AccessibilityRole.Alert => "AXSheet",
            AccessibilityRole.Toolbar => "AXToolbar",
            AccessibilityRole.Menu => "AXMenu",
            AccessibilityRole.MenuItem => "AXMenuItem",
            AccessibilityRole.Tab => "AXRadioButton",
            AccessibilityRole.TabPanel => "AXTabGroup",
            AccessibilityRole.DatePicker => "AXDateField",
            AccessibilityRole.TimePicker => "AXTimeField",
            AccessibilityRole.Spinner => "AXBusyIndicator",
            AccessibilityRole.Tooltip => "AXHelpTag",
            AccessibilityRole.Page => "AXGroup",
            AccessibilityRole.Navigation => "AXGroup",
            _ => "AXUnknown"
        };
    }

    /// <summary>
    /// Gets the accessibility label.
    /// </summary>
    public string? GetAccessibilityLabel()
    {
        return Element.GetComputedAccessibilityLabel();
    }

    /// <summary>
    /// Gets the accessibility value.
    /// </summary>
    public string? GetAccessibilityValue()
    {
        return Element.GetComputedAccessibilityValue();
    }

    /// <summary>
    /// Gets whether the element is enabled.
    /// </summary>
    public bool IsAccessibilityEnabled()
    {
        var traits = Element.GetComputedAccessibilityTraits();
        return !traits.HasFlag(AccessibilityTrait.Disabled);
    }

    /// <summary>
    /// Gets whether the element has focus.
    /// </summary>
    public bool IsAccessibilityFocused()
    {
        return Element.IsFocused;
    }
}
