using PlusUi.core;
using System.Runtime.Versioning;

namespace PlusUi.desktop.Accessibility;

/// <summary>
/// Automation provider for a PlusUi element.
/// Implements the UIA provider pattern for individual elements.
/// </summary>
[SupportedOSPlatform("windows")]
internal sealed class PlusUiAutomationProvider
{
    public UiElement Element { get; }
    public IntPtr WindowHandle { get; }

    public PlusUiAutomationProvider(UiElement element, IntPtr windowHandle)
    {
        Element = element;
        WindowHandle = windowHandle;
    }

    /// <summary>
    /// Gets the UIA control type for this element's accessibility role.
    /// </summary>
    public int GetControlType()
    {
        return Element.AccessibilityRole switch
        {
            AccessibilityRole.Button => UIA_ButtonControlTypeId,
            AccessibilityRole.Checkbox => UIA_CheckBoxControlTypeId,
            AccessibilityRole.RadioButton => UIA_RadioButtonControlTypeId,
            AccessibilityRole.TextInput => UIA_EditControlTypeId,
            AccessibilityRole.Label => UIA_TextControlTypeId,
            AccessibilityRole.Heading => UIA_TextControlTypeId,
            AccessibilityRole.Link => UIA_HyperlinkControlTypeId,
            AccessibilityRole.Image => UIA_ImageControlTypeId,
            AccessibilityRole.Slider => UIA_SliderControlTypeId,
            AccessibilityRole.Toggle => UIA_CheckBoxControlTypeId,
            AccessibilityRole.List => UIA_ListControlTypeId,
            AccessibilityRole.ListItem => UIA_ListItemControlTypeId,
            AccessibilityRole.ComboBox => UIA_ComboBoxControlTypeId,
            AccessibilityRole.ProgressBar => UIA_ProgressBarControlTypeId,
            AccessibilityRole.ScrollView => UIA_ScrollBarControlTypeId,
            AccessibilityRole.Container => UIA_GroupControlTypeId,
            AccessibilityRole.Dialog => UIA_WindowControlTypeId,
            AccessibilityRole.Alert => UIA_PaneControlTypeId,
            AccessibilityRole.Toolbar => UIA_ToolBarControlTypeId,
            AccessibilityRole.Menu => UIA_MenuControlTypeId,
            AccessibilityRole.MenuItem => UIA_MenuItemControlTypeId,
            AccessibilityRole.Tab => UIA_TabItemControlTypeId,
            AccessibilityRole.TabPanel => UIA_TabControlTypeId,
            AccessibilityRole.DatePicker => UIA_CalendarControlTypeId,
            AccessibilityRole.TimePicker => UIA_SpinnerControlTypeId,
            AccessibilityRole.Spinner => UIA_SpinnerControlTypeId,
            AccessibilityRole.Tooltip => UIA_ToolTipControlTypeId,
            AccessibilityRole.Page => UIA_PaneControlTypeId,
            AccessibilityRole.Navigation => UIA_PaneControlTypeId,
            _ => UIA_CustomControlTypeId
        };
    }

    // UIA Control Type IDs
    private const int UIA_ButtonControlTypeId = 50000;
    private const int UIA_CalendarControlTypeId = 50001;
    private const int UIA_CheckBoxControlTypeId = 50002;
    private const int UIA_ComboBoxControlTypeId = 50003;
    private const int UIA_EditControlTypeId = 50004;
    private const int UIA_HyperlinkControlTypeId = 50005;
    private const int UIA_ImageControlTypeId = 50006;
    private const int UIA_ListItemControlTypeId = 50007;
    private const int UIA_ListControlTypeId = 50008;
    private const int UIA_MenuControlTypeId = 50009;
    private const int UIA_MenuBarControlTypeId = 50010;
    private const int UIA_MenuItemControlTypeId = 50011;
    private const int UIA_ProgressBarControlTypeId = 50012;
    private const int UIA_RadioButtonControlTypeId = 50013;
    private const int UIA_ScrollBarControlTypeId = 50014;
    private const int UIA_SliderControlTypeId = 50015;
    private const int UIA_SpinnerControlTypeId = 50016;
    private const int UIA_TabControlTypeId = 50018;
    private const int UIA_TabItemControlTypeId = 50019;
    private const int UIA_TextControlTypeId = 50020;
    private const int UIA_ToolBarControlTypeId = 50021;
    private const int UIA_ToolTipControlTypeId = 50022;
    private const int UIA_TreeControlTypeId = 50023;
    private const int UIA_TreeItemControlTypeId = 50024;
    private const int UIA_CustomControlTypeId = 50025;
    private const int UIA_GroupControlTypeId = 50026;
    private const int UIA_PaneControlTypeId = 50033;
    private const int UIA_WindowControlTypeId = 50032;
}
