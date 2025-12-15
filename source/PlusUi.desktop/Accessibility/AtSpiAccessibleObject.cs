using PlusUi.core;
using System.Runtime.Versioning;

namespace PlusUi.desktop.Accessibility;

/// <summary>
/// AT-SPI accessible object wrapper for a PlusUi element.
/// Implements the org.a11y.atspi.Accessible interface.
/// </summary>
[SupportedOSPlatform("linux")]
internal sealed class AtSpiAccessibleObject
{
    public UiElement Element { get; }

    public AtSpiAccessibleObject(UiElement element)
    {
        Element = element;
    }

    /// <summary>
    /// Gets the accessible name.
    /// </summary>
    public string? Name => Element.GetComputedAccessibilityLabel();

    /// <summary>
    /// Gets the accessible description.
    /// </summary>
    public string? Description => Element.AccessibilityHint;

    /// <summary>
    /// Gets the AT-SPI role for this element.
    /// </summary>
    public AtSpiRole GetRole()
    {
        return Element.AccessibilityRole switch
        {
            AccessibilityRole.Button => AtSpiRole.PushButton,
            AccessibilityRole.Checkbox => AtSpiRole.CheckBox,
            AccessibilityRole.RadioButton => AtSpiRole.RadioButton,
            AccessibilityRole.TextInput => AtSpiRole.Entry,
            AccessibilityRole.Label => AtSpiRole.Label,
            AccessibilityRole.Heading => AtSpiRole.Heading,
            AccessibilityRole.Link => AtSpiRole.Link,
            AccessibilityRole.Image => AtSpiRole.Image,
            AccessibilityRole.Slider => AtSpiRole.Slider,
            AccessibilityRole.Toggle => AtSpiRole.ToggleButton,
            AccessibilityRole.List => AtSpiRole.List,
            AccessibilityRole.ListItem => AtSpiRole.ListItem,
            AccessibilityRole.ComboBox => AtSpiRole.ComboBox,
            AccessibilityRole.ProgressBar => AtSpiRole.ProgressBar,
            AccessibilityRole.ScrollView => AtSpiRole.ScrollPane,
            AccessibilityRole.Container => AtSpiRole.Panel,
            AccessibilityRole.Dialog => AtSpiRole.Dialog,
            AccessibilityRole.Alert => AtSpiRole.Alert,
            AccessibilityRole.Toolbar => AtSpiRole.ToolBar,
            AccessibilityRole.Menu => AtSpiRole.Menu,
            AccessibilityRole.MenuItem => AtSpiRole.MenuItem,
            AccessibilityRole.Tab => AtSpiRole.PageTab,
            AccessibilityRole.TabPanel => AtSpiRole.PageTabList,
            AccessibilityRole.DatePicker => AtSpiRole.DateEditor,
            AccessibilityRole.TimePicker => AtSpiRole.SpinButton,
            AccessibilityRole.Spinner => AtSpiRole.Animation,
            AccessibilityRole.Tooltip => AtSpiRole.ToolTip,
            AccessibilityRole.Page => AtSpiRole.Panel,
            AccessibilityRole.Navigation => AtSpiRole.Panel,
            _ => AtSpiRole.Unknown
        };
    }

    /// <summary>
    /// Gets the AT-SPI state set for this element.
    /// </summary>
    public AtSpiStateSet GetStateSet()
    {
        var states = AtSpiStateSet.None;
        var traits = Element.GetComputedAccessibilityTraits();

        if (Element.IsVisible)
        {
            states |= AtSpiStateSet.Visible | AtSpiStateSet.Showing;
        }

        if (!traits.HasFlag(AccessibilityTrait.Disabled))
        {
            states |= AtSpiStateSet.Enabled | AtSpiStateSet.Sensitive;
        }

        if (traits.HasFlag(AccessibilityTrait.Focusable))
        {
            states |= AtSpiStateSet.Focusable;
        }

        if (traits.HasFlag(AccessibilityTrait.Focused))
        {
            states |= AtSpiStateSet.Focused;
        }

        if (traits.HasFlag(AccessibilityTrait.Checked))
        {
            states |= AtSpiStateSet.Checked;
        }

        if (traits.HasFlag(AccessibilityTrait.Selected))
        {
            states |= AtSpiStateSet.Selected;
        }

        if (traits.HasFlag(AccessibilityTrait.Expanded))
        {
            states |= AtSpiStateSet.Expanded | AtSpiStateSet.Expandable;
        }

        if (traits.HasFlag(AccessibilityTrait.Pressed))
        {
            states |= AtSpiStateSet.Pressed;
        }

        if (traits.HasFlag(AccessibilityTrait.ReadOnly))
        {
            states |= AtSpiStateSet.ReadOnly;
        }

        if (traits.HasFlag(AccessibilityTrait.Required))
        {
            states |= AtSpiStateSet.Required;
        }

        if (traits.HasFlag(AccessibilityTrait.Modal))
        {
            states |= AtSpiStateSet.Modal;
        }

        if (traits.HasFlag(AccessibilityTrait.Busy))
        {
            states |= AtSpiStateSet.Busy;
        }

        return states;
    }
}
