namespace PlusUi.core;

/// <summary>
/// Specifies the selection behavior of a DataGrid or similar control.
/// </summary>
public enum SelectionMode
{
    /// <summary>
    /// No items can be selected.
    /// </summary>
    None,

    /// <summary>
    /// Only one item can be selected at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Multiple items can be selected simultaneously.
    /// </summary>
    Multiple
}
