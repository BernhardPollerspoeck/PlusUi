namespace PlusUi.core;

[AttributeUsage(AttributeTargets.Property)]
public class DataGridColumnAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the header text for the column.
    /// </summary>
    public string? Header { get; set; }
    
    /// <summary>
    /// Gets or sets the order of the column.
    /// </summary>
    public int Order { get; set; }
    
    /// <summary>
    /// Gets or sets the cell template type to use for this column.
    /// </summary>
    public DataGridCellTemplate CellTemplate { get; set; } = DataGridCellTemplate.Default;
    
    /// <summary>
    /// Creates a new instance of the DataGridColumnAttribute.
    /// </summary>
    public DataGridColumnAttribute()
    {
    }
    
    /// <summary>
    /// Creates a new instance of the DataGridColumnAttribute with the specified header.
    /// </summary>
    /// <param name="header">The header text for the column.</param>
    public DataGridColumnAttribute(string header)
    {
        Header = header;
    }
}