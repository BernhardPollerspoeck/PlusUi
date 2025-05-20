namespace PlusUi.core;

/// <summary>
/// Specifies the template type to use for a DataGrid cell.
/// </summary>
public enum DataGridCellTemplate
{
    /// <summary>
    /// Uses the default template based on data type.
    /// </summary>
    Default,
    
    /// <summary>
    /// Uses a Label to display text.
    /// </summary>
    Label,
    
    /// <summary>
    /// Uses an Entry for editable text.
    /// </summary>
    Entry,
    
    /// <summary>
    /// Uses an Image control to display an image.
    /// </summary>
    Image,
    
    /// <summary>
    /// Uses a Checkbox for boolean values.
    /// </summary>
    Checkbox
}