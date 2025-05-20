using PlusUi.core;
using System.Reflection;

namespace UiPlus.core.Tests;

/// <summary>
/// Tests for DataGrid functionality
/// </summary>
[TestClass]
public sealed class DataGridTests
{
    private class TestModel
    {
        [DataGridColumn("ID")]
        public int Id { get; set; }
        
        [DataGridColumn("Name")]
        public string Name { get; set; } = string.Empty;
        
        [DataGridColumn("Active", CellTemplate = DataGridCellTemplate.Checkbox)]
        public bool IsActive { get; set; }
        
        [DataGridColumn("Image", CellTemplate = DataGridCellTemplate.Image)]
        public string ImageUrl { get; set; } = string.Empty;
        
        public string NotMapped { get; set; } = string.Empty;
    }
    
    [TestMethod]
    public void DataGrid_InitialState_HasEmptyColumnsCollection()
    {
        // Arrange
        var dataGrid = new DataGrid();
        
        // Assert
        Assert.AreEqual(0, dataGrid.Columns.Count);
        Assert.IsTrue(dataGrid.AutoGenerateColumns);
        Assert.IsNull(dataGrid.ItemsSource);
    }
    
    [TestMethod]
    public void SetItemsSource_SetsItemsSource_AndRefreshesData()
    {
        // Arrange
        var dataGrid = new DataGrid();
        var items = new List<TestModel> 
        { 
            new TestModel { Id = 1, Name = "Test" } 
        };
        
        // Act
        dataGrid.SetItemsSource(items);
        
        // Assert
        Assert.AreEqual(items, dataGrid.ItemsSource);
    }
    
    [TestMethod]
    public void AddColumn_AddsColumnToCollection()
    {
        // Arrange
        var dataGrid = new DataGrid();
        
        // Act
        dataGrid.AddColumn("Id", "ID");
        
        // Assert
        Assert.AreEqual(1, dataGrid.Columns.Count);
        Assert.AreEqual("Id", dataGrid.Columns[0].PropertyName);
        Assert.AreEqual("ID", dataGrid.Columns[0].Header);
    }
    
    [TestMethod]
    public void AddColumn_WithCellTemplate_SetsCellTemplate()
    {
        // Arrange
        var dataGrid = new DataGrid();
        
        // Act
        dataGrid.AddColumn("IsActive", "Active", DataGridCellTemplate.Checkbox);
        
        // Assert
        Assert.AreEqual(1, dataGrid.Columns.Count);
        Assert.AreEqual(DataGridCellTemplate.Checkbox, dataGrid.Columns[0].CellTemplate);
    }
    
    [TestMethod]
    public void AddColumn_WithTemplateSelector_SetsCellTemplateSelector()
    {
        // Arrange
        var dataGrid = new DataGrid();
        Func<object?, UiElement> selector = (value) => new Label();
        
        // Act
        dataGrid.AddColumn("Name", "Name", selector);
        
        // Assert
        Assert.AreEqual(1, dataGrid.Columns.Count);
        Assert.AreEqual(selector, dataGrid.Columns[0].CellTemplateSelector);
    }
    
    [TestMethod]
    public void AutoGenerateColumns_WithAttributedProperties_GeneratesColumns()
    {
        // Arrange
        var dataGrid = new DataGrid()
            .SetAutoGenerateColumns(true);
        var items = new List<TestModel> { new TestModel { Id = 1, Name = "Test" } };
        
        // Act
        dataGrid.SetItemsSource(items);
        
        // Assert
        Assert.AreEqual(4, dataGrid.Columns.Count); // Only the properties with DataGridColumn attribute
        Assert.AreEqual("Id", dataGrid.Columns[0].PropertyName);
        Assert.AreEqual("ID", dataGrid.Columns[0].Header);
        
        // Check that cell templates are set correctly
        var checkboxColumn = dataGrid.Columns.FirstOrDefault(c => c.PropertyName == "IsActive");
        Assert.IsNotNull(checkboxColumn);
        Assert.AreEqual(DataGridCellTemplate.Checkbox, checkboxColumn.CellTemplate);
        
        var imageColumn = dataGrid.Columns.FirstOrDefault(c => c.PropertyName == "ImageUrl");
        Assert.IsNotNull(imageColumn);
        Assert.AreEqual(DataGridCellTemplate.Image, imageColumn.CellTemplate);
    }
    
    [TestMethod]
    public void DefaultCellTemplate_ForBooleanProperty_IsCheckbox()
    {
        // Arrange
        var dataGrid = new DataGrid()
            .SetAutoGenerateColumns(true);
        
        // Create a test type with boolean property but no attribute
        var testType = new
        {
            BoolProperty = true
        }.GetType();
        
        // Use reflection to access private method
        var methodInfo = typeof(DataGrid).GetMethod("GetDefaultCellTemplate", 
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(methodInfo, "Method GetDefaultCellTemplate not found");
        
        // Act
        var template = (DataGridCellTemplate)methodInfo.Invoke(null, new object[] { typeof(bool) });
        
        // Assert
        Assert.AreEqual(DataGridCellTemplate.Checkbox, template);
    }
    
    [TestMethod]
    public void GetPropertyValue_WithValidProperty_ReturnsValue()
    {
        // Arrange
        var testModel = new TestModel { Id = 42, Name = "Test Name" };
        var column = new DataGridColumn("Name", "Name");
        
        // Find the private static method using reflection
        var methodInfo = typeof(DataGrid).GetMethod("GetPropertyValue", 
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(methodInfo, "Method GetPropertyValue not found");
        
        // Act
        var result = methodInfo.Invoke(null, new object[] { testModel, column });
        
        // Assert
        Assert.AreEqual("Test Name", result);
    }
    
    [TestMethod]
    public void CreateCell_ForLabelTemplate_ReturnsLabelWithText()
    {
        // Arrange
        var testModel = new TestModel { Name = "Test Name" };
        var column = new DataGridColumn("Name", "Name", DataGridCellTemplate.Label);
        
        // Find the private static method using reflection
        var createCellMethod = typeof(DataGrid).GetMethod("CreateCell", 
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(createCellMethod, "Method CreateCell not found");
        
        // Act
        var cell = createCellMethod.Invoke(null, new object[] { testModel, column }) as UiElement;
        
        // Assert
        Assert.IsNotNull(cell);
        Assert.IsInstanceOfType(cell, typeof(Label));
    }
    
    [TestMethod]
    public void CreateCell_ForCheckboxTemplate_ReturnsCheckbox()
    {
        // Arrange
        var testModel = new TestModel { IsActive = true };
        var column = new DataGridColumn("IsActive", "Active", DataGridCellTemplate.Checkbox);
        
        // Find the private static method using reflection
        var createCellMethod = typeof(DataGrid).GetMethod("CreateCell", 
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(createCellMethod, "Method CreateCell not found");
        
        // Act
        var cell = createCellMethod.Invoke(null, new object[] { testModel, column }) as UiElement;
        
        // Assert
        Assert.IsNotNull(cell);
        Assert.IsInstanceOfType(cell, typeof(Checkbox));
    }
    
    [TestMethod]
    public void BindItemsSource_RegistersBinding()
    {
        // Arrange
        var dataGrid = new DataGrid();
        var items = new List<TestModel> { new TestModel { Id = 1, Name = "Test" } };
        
        // Act
        dataGrid.BindItemsSource("Items", () => items);
        
        // Assert
        // Ensure binding was registered (indirect test as we can't directly access private fields)
        var boundItems = dataGrid.ItemsSource;
        Assert.AreEqual(items, boundItems);
    }

    [TestMethod]
    public void BindAutoGenerateColumns_RegistersBinding()
    {
        // Arrange
        var dataGrid = new DataGrid();
        bool autoGenerate = true;
        
        // Act
        dataGrid.BindAutoGenerateColumns("AutoGenerateColumns", () => autoGenerate);
        
        // Assert
        Assert.IsTrue(dataGrid.AutoGenerateColumns);
        
        // Change the bound value
        autoGenerate = false;
        dataGrid.InvokeBindings(); // Simulate a change notification
        
        // Check that the property was updated
        Assert.IsFalse(dataGrid.AutoGenerateColumns);
    }
    
    [TestMethod]
    public void DemoPageGrid_HasCorrectLayout()
    {
        // Arrange - Recreate the grid from the demo page
        var people = new List<TestModel> 
        { 
            new TestModel { Id = 1, Name = "John Doe", IsActive = true, ImageUrl = "test.png" },
            new TestModel { Id = 2, Name = "Jane Smith", IsActive = false, ImageUrl = "test.png" }
        };
        
        var dataGrid = new DataGrid()
            .SetItemsSource(people)
            .SetAutoGenerateColumns(true);
        
        // Get the private RefreshData method using reflection to simulate rebuilding the grid
        var refreshDataMethod = typeof(DataGrid).GetMethod("RefreshData", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(refreshDataMethod, "Method RefreshData not found");
        
        // Act
        refreshDataMethod.Invoke(dataGrid, Array.Empty<object>());
        
        // Assert
        // Check that there are columns for all mapped properties
        Assert.AreEqual(4, dataGrid.Columns.Count);
        
        // Check that the grid has the correct number of rows (header + data rows)
        // We have to infer this from the structure
        // Since we can't directly access the UI elements, we'll verify through column property info
        Assert.IsNotNull(dataGrid.Columns[0].PropertyInfo);
        
        // Check that rows aren't overlapping (test that proper row spacing is applied)
        // Indirect test as we can't access the rows directly
        var rowSpacing = dataGrid.GetType().GetProperty("RowSpacing", 
            BindingFlags.Public | BindingFlags.Instance)?.GetValue(dataGrid);
        Assert.IsNotNull(rowSpacing);
        Assert.IsTrue((int)rowSpacing > 0);
        
        // Check column spacing
        var columnSpacing = dataGrid.GetType().GetProperty("ColumnSpacing", 
            BindingFlags.Public | BindingFlags.Instance)?.GetValue(dataGrid);
        Assert.IsNotNull(columnSpacing);
        Assert.IsTrue((int)columnSpacing > 0);
    }
}