using SkiaSharp;
using System.Collections;
using System.Reflection;

namespace PlusUi.core;

/// <summary>
/// A control that displays data in a customizable grid.
/// </summary>
public class DataGrid : Grid
{
    private readonly List<DataGridColumn> _columns = [];

    /// <summary>
    /// Gets or sets whether columns are automatically generated from the data source.
    /// </summary>
    public bool AutoGenerateColumns
    {
        get;
        set
        {
            field = value;
            if (ItemsSource != null)
            {
                RefreshData();
            }
        }
    } = true;

    /// <summary>
    /// Gets the collection of columns in the DataGrid.
    /// </summary>
    public new IReadOnlyList<DataGridColumn> Columns => _columns;
    
    /// <summary>
    /// Gets or sets the data source for the DataGrid.
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get;
        set
        {
            field = value;
            RefreshData();
        }
    }
    
    /// <summary>
    /// Creates a new instance of the DataGrid.
    /// </summary>
    public DataGrid()
    {
        // Add default styling
        SetBackgroundColor(SKColors.White);
    }
    
    /// <summary>
    /// Sets the data source for the DataGrid.
    /// </summary>
    /// <param name="itemsSource">The data source.</param>
    /// <returns>The DataGrid instance.</returns>
    public DataGrid SetItemsSource(IEnumerable itemsSource)
    {
        ItemsSource = itemsSource;
        return this;
    }
    
    /// <summary>
    /// Binds the data source for the DataGrid.
    /// </summary>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">A function that returns the data source.</param>
    /// <returns>The DataGrid instance.</returns>
    public DataGrid BindItemsSource(string propertyName, Func<IEnumerable?> propertyGetter)
    {
        RegisterBinding(propertyName, () => ItemsSource = propertyGetter());
        return this;
    }
    
    /// <summary>
    /// Sets whether columns are automatically generated from the data source.
    /// </summary>
    /// <param name="autoGenerate">Whether to auto-generate columns.</param>
    /// <returns>The DataGrid instance.</returns>
    public DataGrid SetAutoGenerateColumns(bool autoGenerate)
    {
        AutoGenerateColumns = autoGenerate;
        return this;
    }
    
    /// <summary>
    /// Binds whether columns are automatically generated from the data source.
    /// </summary>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="propertyGetter">A function that returns whether to auto-generate columns.</param>
    /// <returns>The DataGrid instance.</returns>
    public DataGrid BindAutoGenerateColumns(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => AutoGenerateColumns = propertyGetter());
        return this;
    }
    
    /// <summary>
    /// Adds a column to the DataGrid.
    /// </summary>
    /// <param name="column">The column to add.</param>
    /// <returns>The DataGrid instance.</returns>
    public DataGrid AddColumn(DataGridColumn column)
    {
        _columns.Add(column);
        if (ItemsSource != null)
        {
            RefreshData();
        }
        return this;
    }
    
    /// <summary>
    /// Adds a column to the DataGrid.
    /// </summary>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="header">The header text for the column.</param>
    /// <returns>The DataGrid instance.</returns>
    public DataGrid AddColumn(string propertyName, string header)
    {
        return AddColumn(new DataGridColumn(propertyName, header));
    }
    
    /// <summary>
    /// Adds a column to the DataGrid with a specified template.
    /// </summary>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="header">The header text for the column.</param>
    /// <param name="cellTemplate">The cell template to use for this column.</param>
    /// <returns>The DataGrid instance.</returns>
    public DataGrid AddColumn(string propertyName, string header, DataGridCellTemplate cellTemplate)
    {
        return AddColumn(new DataGridColumn(propertyName, header, cellTemplate));
    }
    
    /// <summary>
    /// Adds a column to the DataGrid with a custom template selector.
    /// </summary>
    /// <param name="propertyName">The name of the property to bind to.</param>
    /// <param name="header">The header text for the column.</param>
    /// <param name="cellTemplateSelector">A function that returns a UI element for a cell.</param>
    /// <returns>The DataGrid instance.</returns>
    public DataGrid AddColumn(string propertyName, string header, Func<object?, UiElement> cellTemplateSelector)
    {
        return AddColumn(new DataGridColumn(propertyName, header, cellTemplateSelector));
    }
    
    /// <summary>
    /// Refreshes the data display in the DataGrid.
    /// </summary>
    private void RefreshData()
    {
        // Clear existing grid content
        ClearChildren();
        
        // If there's no data source, nothing more to do
        if (ItemsSource == null)
        {
            return;
        }
        
        // Get the item type from the first item in the collection (if any)
        Type? itemType = null;
        foreach (var item in ItemsSource)
        {
            itemType = item.GetType();
            break;
        }
        
        if (itemType == null)
        {
            return; // No items to display
        }
        
        // Auto-generate or use existing columns
        if (AutoGenerateColumns && _columns.Count == 0)
        {
            GenerateColumns(itemType);
        }
        
        // Set up the column definitions
        // First, clear any existing columns and add a new one for each column
        SetColumns([]); // Clear all columns
        
        for (var i = 0; i < _columns.Count; i++) 
        {
            AddColumn(Column.Auto);
        }
        
        // Add column spacing
        SetColumnSpacing(2);
        
        // Add a header row
        SetRows([]); // Clear all rows
        AddRow(Row.Auto);
        
        // Add row spacing
        SetRowSpacing(1);
        
        // Add headers
        for (var i = 0; i < _columns.Count; i++)
        {
            var headerLabel = new Label()
                .SetText(_columns[i].Header)
                .SetTextColor(SKColors.Black)
                .SetTextSize(14)
                .SetPadding(new Margin(5))
                .SetBackgroundColor(new SKColor(240, 240, 240));
            
            AddChild(headerLabel, 0, i);
        }
        
        // Add data rows
        var rowIndex = 1;
        foreach (var item in ItemsSource)
        {
            // Add a new row for each item
            AddRow(Row.Auto);
            
            // Add cells for each column
            for (var i = 0; i < _columns.Count; i++)
            {
                var column = _columns[i];
                var cell = CreateCell(item, column);
                if (cell != null)
                {
                    AddChild(cell, rowIndex, i);
                }
            }
            
            rowIndex++;
        }
    }
    
    /// <summary>
    /// Generates columns based on the properties of the data item type.
    /// </summary>
    /// <param name="itemType">The type of the data items.</param>
    private void GenerateColumns(Type itemType)
    {
        _columns.Clear();
        
        // Get all properties
        var properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        // Find properties with the DataGridColumn attribute
        var attributedColumns = new List<(PropertyInfo Property, DataGridColumnAttribute Attribute)>();
        foreach (var prop in properties)
        {
            var attr = prop.GetCustomAttribute<DataGridColumnAttribute>();
            if (attr != null)
            {
                attributedColumns.Add((prop, attr));
            }
        }
        
        // If we found attributes, only use those properties
        if (attributedColumns.Count > 0)
        {
            // Sort by the Order property of the attribute
            foreach (var (prop, attr) in attributedColumns.OrderBy(x => x.Attribute.Order))
            {
                var column = new DataGridColumn
                {
                    PropertyName = prop.Name,
                    Header = attr.Header ?? prop.Name,
                    CellTemplate = attr.CellTemplate,
                    PropertyInfo = prop
                };
                
                _columns.Add(column);
            }
        }
        else
        {
            // No attributes found, use all public properties
            foreach (var prop in properties)
            {
                var column = new DataGridColumn
                {
                    PropertyName = prop.Name,
                    Header = prop.Name,
                    PropertyInfo = prop,
                    // Set an appropriate template based on the property type
                    CellTemplate = GetDefaultCellTemplate(prop.PropertyType)
                };

                _columns.Add(column);
            }
        }
    }
    
    /// <summary>
    /// Gets the default cell template for a property type.
    /// </summary>
    /// <param name="propertyType">The property type.</param>
    /// <returns>The appropriate cell template for the type.</returns>
    private static DataGridCellTemplate GetDefaultCellTemplate(Type propertyType)
    {
        if (propertyType == typeof(bool) || propertyType == typeof(bool?))
        {
            return DataGridCellTemplate.Checkbox;
        }
        else if (propertyType == typeof(string) || 
                 propertyType == typeof(int) || propertyType == typeof(int?) ||
                 propertyType == typeof(double) || propertyType == typeof(double?) ||
                 propertyType == typeof(float) || propertyType == typeof(float?) ||
                 propertyType == typeof(decimal) || propertyType == typeof(decimal?))
        {
            return DataGridCellTemplate.Label;
        }
        else if (propertyType == typeof(Uri) || 
                 propertyType == typeof(string)) // Strings might be image paths
        {
            return DataGridCellTemplate.Image;
        }
        
        return DataGridCellTemplate.Label; // Default to Label for most types
    }
    
    /// <summary>
    /// Creates a cell UI element based on the item and column definition.
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <param name="column">The column definition.</param>
    /// <returns>A UI element representing the cell.</returns>
    private static UiElement? CreateCell(object item, DataGridColumn column)
    {
        // If there's a custom template selector, use that
        if (column.CellTemplateSelector != null)
        {
            var propertyValue = GetPropertyValue(item, column);
            return column.CellTemplateSelector(propertyValue);
        }
        
        // Otherwise, use the template specified or default for the type
        var cellTemplate = column.CellTemplate;
        var value = GetPropertyValue(item, column);
        
        // Apply appropriate template
        return cellTemplate switch
        {
            DataGridCellTemplate.Label => CreateLabelCell(value),
            DataGridCellTemplate.Entry => CreateEntryCell(item, column, value),
            DataGridCellTemplate.Image => CreateImageCell(value),
            DataGridCellTemplate.Checkbox => CreateCheckboxCell(item, column, value),
            _ => CreateLabelCell(value)
        };
    }
    
    /// <summary>
    /// Gets the value of a property from an object.
    /// </summary>
    /// <param name="item">The object.</param>
    /// <param name="column">The column definition.</param>
    /// <returns>The property value.</returns>
    private static object? GetPropertyValue(object item, DataGridColumn column)
    {
        // Use PropertyInfo if available
        if (column.PropertyInfo != null)
        {
            return column.PropertyInfo.GetValue(item);
        }
        
        // Otherwise try to get the property by name
        var property = item.GetType().GetProperty(column.PropertyName);
        return property?.GetValue(item);
    }
    
    /// <summary>
    /// Creates a Label cell.
    /// </summary>
    /// <param name="value">The cell value.</param>
    /// <returns>A Label element.</returns>
    private static Label CreateLabelCell(object? value)
    {
        return new Label()
            .SetText(value?.ToString() ?? string.Empty)
            .SetTextColor(SKColors.Black)
            .SetPadding(new Margin(5, 3));
    }
    
    /// <summary>
    /// Creates an Entry cell.
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <param name="column">The column definition.</param>
    /// <param name="value">The cell value.</param>
    /// <returns>An Entry element.</returns>
    private static Entry CreateEntryCell(object item, DataGridColumn column, object? value)
    {
        var entry = new Entry()
            .SetText(value?.ToString() ?? string.Empty)
            .SetTextColor(SKColors.Black)
            .SetPadding(new Margin(5, 3));
            
        // TODO: Implement two-way binding to update the source property when the entry changes
        
        return entry;
    }
    
    /// <summary>
    /// Creates an Image cell.
    /// </summary>
    /// <param name="value">The cell value.</param>
    /// <returns>An Image element.</returns>
    private static UiElement CreateImageCell(object? value)
    {
        var imageSource = value?.ToString();
        if (string.IsNullOrEmpty(imageSource))
        {
            return new Solid();
        }
        
        return new Image()
            .SetAspect(Aspect.AspectFit)
            .SetImageSource(imageSource)
            .SetDesiredHeight(32)
            .SetMargin(new Margin(2));
    }
    
    /// <summary>
    /// Creates a Checkbox cell.
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <param name="column">The column definition.</param>
    /// <param name="value">The cell value.</param>
    /// <returns>A Checkbox element.</returns>
    private static Checkbox CreateCheckboxCell(object item, DataGridColumn column, object? value)
    {
        var isChecked = value is bool boolValue && boolValue;
        
        var checkbox = new Checkbox()
            .SetIsChecked(isChecked);
            
        // TODO: Implement two-way binding to update the source property when the checkbox changes
        
        return checkbox;
    }
}