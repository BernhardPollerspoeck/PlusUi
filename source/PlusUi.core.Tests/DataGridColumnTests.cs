using PlusUi.core;
using System.Windows.Input;

namespace PlusUi.core.Tests;

[TestClass]
public class DataGridColumnTests
{
    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public float Progress { get; set; } = 0.5f;
        public float Rating { get; set; } = 3f;
    }

    #region DataGrid Cell Layout Tests

    [TestMethod]
    public void ProgressBar_InDataGridCell_ShouldStretchToColumnWidth()
    {
        // Arrange - simulate how DataGrid measures cells
        var columnWidth = 120f;
        var rowHeight = 40f;

        var progressBar = new ProgressBar()
            .SetProgress(0.5f)
            .SetDesiredHeight(10f);

        // Act - DataGrid measures with dontStretch=true (see DataGrid.cs line 824)
        progressBar.Measure(new Size(columnWidth, rowHeight), dontStretch: true);
        progressBar.Arrange(new Rect(0, 0, columnWidth, rowHeight));

        // Assert - ProgressBar should use full column width
        Assert.AreEqual(columnWidth, progressBar.ElementSize.Width, 0.1f,
            $"ProgressBar width should be {columnWidth} (column width), but was {progressBar.ElementSize.Width}");
        Assert.AreEqual(10f, progressBar.ElementSize.Height, 0.1f,
            $"ProgressBar height should be 10 (desired), but was {progressBar.ElementSize.Height}");
    }

    [TestMethod]
    public void Slider_InDataGridCell_ShouldStretchToColumnWidth()
    {
        // Arrange - simulate how DataGrid measures cells
        var columnWidth = 120f;
        var rowHeight = 40f;

        var slider = new Slider()
            .SetMinimum(0)
            .SetMaximum(5)
            .SetValue(3)
            .SetDesiredHeight(24f);

        // Act - DataGrid measures with dontStretch=true
        slider.Measure(new Size(columnWidth, rowHeight), dontStretch: true);
        slider.Arrange(new Rect(0, 0, columnWidth, rowHeight));

        // Assert - Slider should use full column width
        Assert.AreEqual(columnWidth, slider.ElementSize.Width, 0.1f,
            $"Slider width should be {columnWidth} (column width), but was {slider.ElementSize.Width}");
    }

    [TestMethod]
    public void DataGridProgressColumn_CreateCell_ShouldStretchInDataGrid()
    {
        // Arrange
        var columnWidth = 120f;
        var rowHeight = 40f;

        var column = new DataGridProgressColumn<Person>()
            .SetHeader("Progress")
            .SetBinding(p => p.Progress)
            .SetWidth(DataGridColumnWidth.Absolute(columnWidth));

        var person = new Person { Progress = 0.75f };

        // Act - Create cell and measure like DataGrid does
        var cell = column.CreateCell(person, 0);
        cell.Measure(new Size(columnWidth, rowHeight), dontStretch: true);
        cell.Arrange(new Rect(0, 0, columnWidth, rowHeight));

        // Assert - Cell should fill the column width
        Assert.AreEqual(columnWidth, cell.ElementSize.Width, 0.1f,
            $"Progress cell width should be {columnWidth}, but was {cell.ElementSize.Width}");
    }

    [TestMethod]
    public void DataGridSliderColumn_CreateCell_ShouldStretchInDataGrid()
    {
        // Arrange
        var columnWidth = 120f;
        var rowHeight = 40f;

        var column = new DataGridSliderColumn<Person>()
            .SetHeader("Rating")
            .SetBinding(p => p.Rating, (p, v) => p.Rating = v)
            .SetRange(1, 5)
            .SetWidth(DataGridColumnWidth.Absolute(columnWidth));

        var person = new Person { Rating = 3f };

        // Act - Create cell and measure like DataGrid does
        var cell = column.CreateCell(person, 0);
        cell.Measure(new Size(columnWidth, rowHeight), dontStretch: true);
        cell.Arrange(new Rect(0, 0, columnWidth, rowHeight));

        // Assert - Cell should fill the column width
        Assert.AreEqual(columnWidth, cell.ElementSize.Width, 0.1f,
            $"Slider cell width should be {columnWidth}, but was {cell.ElementSize.Width}");
    }

    #endregion

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

    #region New Column Types Tests

    [TestMethod]
    public void DataGridComboBoxColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridComboBoxColumn<Person, string>();

        // Act
        var result = column.SetHeader("Status");

        // Assert
        Assert.AreEqual("Status", column.Header);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridComboBoxColumn_SetBinding_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridComboBoxColumn<Person, string>();
        Func<Person, string?> getter = p => p.Name;
        Action<Person, string?> setter = (p, v) => p.Name = v ?? "";

        // Act
        var result = column.SetBinding(getter, setter);

        // Assert
        Assert.IsNotNull(column.ValueGetter);
        Assert.IsNotNull(column.ValueSetter);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridComboBoxColumn_SetItemsSource_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridComboBoxColumn<Person, string>();
        var items = new[] { "Active", "Inactive" };

        // Act
        var result = column.SetItemsSource(items);

        // Assert
        Assert.IsNotNull(column.ItemsSource);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridComboBoxColumn_CreateCell_ReturnsComboBox()
    {
        // Arrange
        var column = new DataGridComboBoxColumn<Person, string>()
            .SetBinding(p => p.Name, (p, v) => p.Name = v ?? "")
            .SetItemsSource(new[] { "Alice", "Bob" });
        var person = new Person { Name = "Alice" };

        // Act
        var cell = column.CreateCell(person, 0);

        // Assert
        Assert.IsInstanceOfType<ComboBox<string>>(cell);
    }

    [TestMethod]
    public void DataGridDatePickerColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridDatePickerColumn<Person>();

        // Act
        var result = column.SetHeader("Birth Date");

        // Assert
        Assert.AreEqual("Birth Date", column.Header);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridDatePickerColumn_SetDisplayFormat_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridDatePickerColumn<Person>();

        // Act
        var result = column.SetDisplayFormat("yyyy-MM-dd");

        // Assert
        Assert.AreEqual("yyyy-MM-dd", column.DisplayFormat);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridDatePickerColumn_SetMinMaxDate_PropertiesAreSet()
    {
        // Arrange
        var column = new DataGridDatePickerColumn<Person>();
        var minDate = new DateOnly(2000, 1, 1);
        var maxDate = new DateOnly(2030, 12, 31);

        // Act
        column.SetMinDate(minDate).SetMaxDate(maxDate);

        // Assert
        Assert.AreEqual(minDate, column.MinDate);
        Assert.AreEqual(maxDate, column.MaxDate);
    }

    [TestMethod]
    public void DataGridTimePickerColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTimePickerColumn<Person>();

        // Act
        var result = column.SetHeader("Start Time");

        // Assert
        Assert.AreEqual("Start Time", column.Header);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridTimePickerColumn_SetMinuteIncrement_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTimePickerColumn<Person>();

        // Act
        var result = column.SetMinuteIncrement(15);

        // Assert
        Assert.AreEqual(15, column.MinuteIncrement);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridTimePickerColumn_Set24HourFormat_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridTimePickerColumn<Person>();

        // Act
        var result = column.Set24HourFormat(false);

        // Assert
        Assert.IsFalse(column.Is24HourFormat);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridImageColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridImageColumn<Person>();

        // Act
        var result = column.SetHeader("Avatar");

        // Assert
        Assert.AreEqual("Avatar", column.Header);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridImageColumn_SetAspect_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridImageColumn<Person>();

        // Act
        var result = column.SetAspect(Aspect.AspectFill);

        // Assert
        Assert.AreEqual(Aspect.AspectFill, column.Aspect);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridImageColumn_SetImageSize_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridImageColumn<Person>();
        var size = new Size(64, 64);

        // Act
        var result = column.SetImageSize(size);

        // Assert
        Assert.AreEqual(size, column.ImageSize);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridImageColumn_CreateCell_ReturnsImage()
    {
        // Arrange
        var column = new DataGridImageColumn<Person>()
            .SetBinding(p => p.Name); // Using Name as fake image URL
        var person = new Person { Name = "test.png" };

        // Act
        var cell = column.CreateCell(person, 0);

        // Assert
        Assert.IsInstanceOfType<Image>(cell);
    }

    [TestMethod]
    public void DataGridProgressColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridProgressColumn<Person>();

        // Act
        var result = column.SetHeader("Progress");

        // Assert
        Assert.AreEqual("Progress", column.Header);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridProgressColumn_SetProgressColor_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridProgressColumn<Person>();
        var color = new Color(0, 200, 100);

        // Act
        var result = column.SetProgressColor(color);

        // Assert
        Assert.AreEqual(color, column.ProgressColor);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridProgressColumn_CreateCell_ReturnsProgressBar()
    {
        // Arrange
        var column = new DataGridProgressColumn<Person>()
            .SetBinding(p => p.Age / 100f);
        var person = new Person { Age = 50 };

        // Act
        var cell = column.CreateCell(person, 0);

        // Assert
        Assert.IsInstanceOfType<ProgressBar>(cell);
    }

    [TestMethod]
    public void DataGridSliderColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridSliderColumn<Person>();

        // Act
        var result = column.SetHeader("Value");

        // Assert
        Assert.AreEqual("Value", column.Header);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridSliderColumn_SetRange_PropertiesAreSet()
    {
        // Arrange
        var column = new DataGridSliderColumn<Person>();

        // Act
        var result = column.SetRange(10, 200);

        // Assert
        Assert.AreEqual(10f, column.MinValue);
        Assert.AreEqual(200f, column.MaxValue);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridSliderColumn_CreateCell_ReturnsSlider()
    {
        // Arrange
        var column = new DataGridSliderColumn<Person>()
            .SetBinding(p => (float)p.Age, (p, v) => p.Age = (int)v);
        var person = new Person { Age = 30 };

        // Act
        var cell = column.CreateCell(person, 0);

        // Assert
        Assert.IsInstanceOfType<Slider>(cell);
    }

    [TestMethod]
    public void DataGridLinkColumn_SetHeader_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridLinkColumn<Person>();

        // Act
        var result = column.SetHeader("Link");

        // Assert
        Assert.AreEqual("Link", column.Header);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridLinkColumn_SetLinkColor_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridLinkColumn<Person>();
        var color = new Color(0, 100, 200);

        // Act
        var result = column.SetLinkColor(color);

        // Assert
        Assert.AreEqual(color, column.LinkColor);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridLinkColumn_SetCommand_PropertyIsSet()
    {
        // Arrange
        var column = new DataGridLinkColumn<Person>();
        var command = new TestCommand();

        // Act
        var result = column.SetCommand(command);

        // Assert
        Assert.AreSame(command, column.Command);
        Assert.AreSame(column, result);
    }

    [TestMethod]
    public void DataGridLinkColumn_CreateCell_ReturnsTapGestureDetector()
    {
        // Arrange
        var column = new DataGridLinkColumn<Person>()
            .SetBinding(p => p.Name);
        var person = new Person { Name = "Click Me" };

        // Act
        var cell = column.CreateCell(person, 0);

        // Assert
        Assert.IsInstanceOfType<TapGestureDetector>(cell);
    }

    #endregion
}
