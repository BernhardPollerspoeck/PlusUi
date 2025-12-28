using PlusUi.core;
using System.Windows.Input;

namespace UiPlus.core.Tests;

[TestClass]
public class DataGridColumnTests
{
    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsActive { get; set; }
    }

    #region DataGridTextColumn Tests

    [TestMethod]
    public void DataGridTextColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTextColumn<Person>();

        // Act
        var result = column.SetHeader("Name");

        // Assert
        Assert.AreEqual("Name", column.Header, "Header should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");
    }

    [TestMethod]
    public void DataGridTextColumn_SetBinding_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTextColumn<Person>();
        Func<Person, string> binding = p => p.Name;

        // Act
        var result = column.SetBinding(binding);

        // Assert
        Assert.IsNotNull(column.ValueGetter, "ValueGetter should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");

        // Verify binding works
        var person = new Person { Name = "Alice" };
        Assert.AreEqual("Alice", column.ValueGetter(person), "ValueGetter should return correct value");
    }

    [TestMethod]
    public void DataGridTextColumn_SetWidth_Absolute_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTextColumn<Person>();

        // Act
        var result = column.SetWidth(DataGridColumnWidth.Absolute(150));

        // Assert
        Assert.AreEqual(DataGridColumnWidthType.Absolute, column.Width.Type, "Width type should be Absolute");
        Assert.AreEqual(150f, column.Width.Value, "Width value should be 150");
        Assert.AreSame(column, result, "Method should return column for chaining");
    }

    [TestMethod]
    public void DataGridTextColumn_SetWidth_Star_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTextColumn<Person>();

        // Act
        var result = column.SetWidth(DataGridColumnWidth.Star(2));

        // Assert
        Assert.AreEqual(DataGridColumnWidthType.Star, column.Width.Type, "Width type should be Star");
        Assert.AreEqual(2f, column.Width.Value, "Width value should be 2");
        Assert.AreSame(column, result, "Method should return column for chaining");
    }

    [TestMethod]
    public void DataGridTextColumn_SetWidth_Auto_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTextColumn<Person>();

        // Act
        var result = column.SetWidth(DataGridColumnWidth.Auto);

        // Assert
        Assert.AreEqual(DataGridColumnWidthType.Auto, column.Width.Type, "Width type should be Auto");
        Assert.AreSame(column, result, "Method should return column for chaining");
    }

    [TestMethod]
    public void DataGridTextColumn_DefaultWidth_IsStar1()
    {
        // Arrange & Act
        var column = new DataGridTextColumn<Person>();

        // Assert
        Assert.AreEqual(DataGridColumnWidthType.Star, column.Width.Type, "Default width type should be Star");
        Assert.AreEqual(1f, column.Width.Value, "Default width value should be 1");
    }

    #endregion

    #region DataGridCheckboxColumn Tests

    [TestMethod]
    public void DataGridCheckboxColumn_SetBinding_TwoWay_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridCheckboxColumn<Person>();
        Func<Person, bool> getter = p => p.IsActive;
        Action<Person, bool> setter = (p, v) => p.IsActive = v;

        // Act
        var result = column.SetBinding(getter, setter);

        // Assert
        Assert.IsNotNull(column.ValueGetter, "ValueGetter should be set");
        Assert.IsNotNull(column.ValueSetter, "ValueSetter should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");

        // Verify getter works
        var person = new Person { IsActive = true };
        Assert.IsTrue(column.ValueGetter(person), "ValueGetter should return correct value");

        // Verify setter works
        column.ValueSetter(person, false);
        Assert.IsFalse(person.IsActive, "ValueSetter should update the value");
    }

    [TestMethod]
    public void DataGridCheckboxColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridCheckboxColumn<Person>();

        // Act
        var result = column.SetHeader("Active");

        // Assert
        Assert.AreEqual("Active", column.Header, "Header should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");
    }

    #endregion

    #region DataGridButtonColumn Tests

    [TestMethod]
    public void DataGridButtonColumn_SetCommand_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridButtonColumn<Person>();
        var command = new TestCommand();

        // Act
        var result = column.SetCommand(command);

        // Assert
        Assert.AreSame(command, column.Command, "Command should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");
    }

    [TestMethod]
    public void DataGridButtonColumn_SetCommandWithParameter_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridButtonColumn<Person>();
        var command = new TestCommand();
        Func<Person, object?> parameterGetter = p => p.Name;

        // Act
        var result = column.SetCommand(command, parameterGetter);

        // Assert
        Assert.AreSame(command, column.Command, "Command should be set");
        Assert.IsNotNull(column.CommandParameterGetter, "CommandParameterGetter should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");
    }

    [TestMethod]
    public void DataGridButtonColumn_SetButtonText_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridButtonColumn<Person>();

        // Act
        var result = column.SetButtonText("Delete");

        // Assert
        Assert.AreEqual("Delete", column.ButtonText, "ButtonText should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");
    }

    [TestMethod]
    public void DataGridButtonColumn_SetButtonTextGetter_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridButtonColumn<Person>();
        Func<Person, string> textGetter = p => $"Edit {p.Name}";

        // Act
        var result = column.SetButtonTextGetter(textGetter);

        // Assert
        Assert.IsNotNull(column.ButtonTextGetter, "ButtonTextGetter should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");

        // Verify getter works
        var person = new Person { Name = "Alice" };
        Assert.AreEqual("Edit Alice", column.ButtonTextGetter(person), "ButtonTextGetter should return correct text");
    }

    #endregion

    #region DataGridTemplateColumn Tests

    [TestMethod]
    public void DataGridTemplateColumn_SetCellTemplate_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTemplateColumn<Person>();
        Func<Person, int, UiElement> template = (person, index) => new Label().SetText(person.Name);

        // Act
        var result = column.SetCellTemplate(template);

        // Assert
        Assert.IsNotNull(column.CellTemplate, "CellTemplate should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");

        // Verify template works
        var person = new Person { Name = "Alice" };
        var element = column.CellTemplate(person, 0);
        Assert.IsInstanceOfType<Label>(element, "Template should create a Label");
    }

    [TestMethod]
    public void DataGridTemplateColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTemplateColumn<Person>();

        // Act
        var result = column.SetHeader("Custom");

        // Assert
        Assert.AreEqual("Custom", column.Header, "Header should be set");
        Assert.AreSame(column, result, "Method should return column for chaining");
    }

    #endregion

    #region Column Width Tests

    [TestMethod]
    public void DataGridColumnWidth_Absolute_CreatesCorrectWidth()
    {
        // Act
        var width = DataGridColumnWidth.Absolute(200);

        // Assert
        Assert.AreEqual(DataGridColumnWidthType.Absolute, width.Type);
        Assert.AreEqual(200f, width.Value);
    }

    [TestMethod]
    public void DataGridColumnWidth_Star_CreatesCorrectWidth()
    {
        // Act
        var width = DataGridColumnWidth.Star(3);

        // Assert
        Assert.AreEqual(DataGridColumnWidthType.Star, width.Type);
        Assert.AreEqual(3f, width.Value);
    }

    [TestMethod]
    public void DataGridColumnWidth_Star_DefaultsTo1()
    {
        // Act
        var width = DataGridColumnWidth.Star();

        // Assert
        Assert.AreEqual(DataGridColumnWidthType.Star, width.Type);
        Assert.AreEqual(1f, width.Value);
    }

    [TestMethod]
    public void DataGridColumnWidth_Auto_CreatesCorrectWidth()
    {
        // Act
        var width = DataGridColumnWidth.Auto;

        // Assert
        Assert.AreEqual(DataGridColumnWidthType.Auto, width.Type);
    }

    #endregion

    private class TestCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) { }
    }
}
