using System.Reflection;

namespace PlusUi.core;

/// <summary>
/// Represents a column in a DataGrid.
/// </summary>
public class DataGridColumn
{
    /// <summary>
    /// Gets or sets the header text for the column.
    /// </summary>
    public string Header { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the name of the property to bind to.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the property info for the column.
    /// </summary>
    public PropertyInfo? PropertyInfo { get; set; }
    
    /// <summary>
    /// Gets or sets the cell template to use for this column.
    /// </summary>
    public DataGridCellTemplate CellTemplate { get; set; } = DataGridCellTemplate.Default;
    
    /// <summary>
    /// Gets or sets a custom template selector function that returns a UI element for a cell.
    /// </summary>
    public Func<object?, UiElement>? CellTemplateSelector { get; set; }
    
    /// <summary>
    /// Creates a new instance of the DataGridColumn.
    /// </summary>
    public DataGridColumn()
    {
    }
    
    /// <summary>
    /// Creates a new instance of the DataGridColumn with the specified property name and header.
    /// </summary>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="header">The header text for the column.</param>
    public DataGridColumn(string propertyName, string header)
    {
        PropertyName = propertyName;
        Header = header;
    }
    
    /// <summary>
    /// Creates a new instance of the DataGridColumn with the specified property name, header, and cell template.
    /// </summary>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="header">The header text for the column.</param>
    /// <param name="cellTemplate">The cell template to use for this column.</param>
    public DataGridColumn(string propertyName, string header, DataGridCellTemplate cellTemplate)
    {
        PropertyName = propertyName;
        Header = header;
        CellTemplate = cellTemplate;
    }
    
    /// <summary>
    /// Creates a new instance of the DataGridColumn with a custom template selector.
    /// </summary>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="header">The header text for the column.</param>
    /// <param name="cellTemplateSelector">A function that returns a UI element for a cell.</param>
    public DataGridColumn(string propertyName, string header, Func<object?, UiElement> cellTemplateSelector)
    {
        PropertyName = propertyName;
        Header = header;
        CellTemplateSelector = cellTemplateSelector;
    }
}