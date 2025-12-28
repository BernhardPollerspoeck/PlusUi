using System.Runtime.Versioning;

namespace PlusUi.desktop.Accessibility;

/// <summary>
/// AT-SPI state flags matching ATSPI_STATE_* constants.
/// </summary>
[Flags]
[SupportedOSPlatform("linux")]
internal enum AtSpiStateSet : long
{
    None = 0,
    Active = 1L << 1,
    Armed = 1L << 2,
    Busy = 1L << 3,
    Checked = 1L << 4,
    Collapsed = 1L << 5,
    Defunct = 1L << 6,
    Editable = 1L << 7,
    Enabled = 1L << 8,
    Expandable = 1L << 9,
    Expanded = 1L << 10,
    Focusable = 1L << 11,
    Focused = 1L << 12,
    HasToolTip = 1L << 13,
    Horizontal = 1L << 14,
    Iconified = 1L << 15,
    Modal = 1L << 16,
    MultiLine = 1L << 17,
    Multiselectable = 1L << 18,
    Opaque = 1L << 19,
    Pressed = 1L << 20,
    Resizable = 1L << 21,
    Selectable = 1L << 22,
    Selected = 1L << 23,
    Sensitive = 1L << 24,
    Showing = 1L << 25,
    SingleLine = 1L << 26,
    Stale = 1L << 27,
    Transient = 1L << 28,
    Vertical = 1L << 29,
    Visible = 1L << 30,
    ManagesDescendants = 1L << 31,
    Indeterminate = 1L << 32,
    Required = 1L << 33,
    Truncated = 1L << 34,
    Animated = 1L << 35,
    InvalidEntry = 1L << 36,
    SupportsAutocompletion = 1L << 37,
    SelectableText = 1L << 38,
    IsDefault = 1L << 39,
    Visited = 1L << 40,
    ReadOnly = 1L << 44
}
