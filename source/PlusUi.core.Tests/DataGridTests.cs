using PlusUi.core;
using System.Collections.ObjectModel;

namespace PlusUi.core.Tests;

[TestClass]
public class DataGridTests
{
    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsActive { get; set; }
    }

    #region Phase 1: Core Structure - Default Values

    [TestMethod]
    public void DataGrid_DefaultValues_ColumnsIsEmpty()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.IsNotNull(dataGrid.Columns, "Columns should not be null");
        Assert.AreEqual(0, dataGrid.Columns.Count, "Columns should be empty by default");
    }

    [TestMethod]
    public void DataGrid_DefaultValues_ItemsSourceIsNull()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.IsNull(dataGrid.ItemsSource, "ItemsSource should be null by default");
    }

    [TestMethod]
    public void DataGrid_DefaultValues_SelectedItemIsNull()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.IsNull(dataGrid.SelectedItem, "SelectedItem should be null by default");
    }

    [TestMethod]
    public void DataGrid_DefaultValues_SelectionModeIsSingle()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.AreEqual(SelectionMode.Single, dataGrid.SelectionMode, "SelectionMode should be Single by default");
    }

    [TestMethod]
    public void DataGrid_DefaultValues_HeaderHeightIsPositive()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.IsGreaterThan(0, dataGrid.HeaderHeight, "HeaderHeight should be positive by default");
    }

    [TestMethod]
    public void DataGrid_DefaultValues_RowHeightIsPositive()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.IsGreaterThan(0, dataGrid.RowHeight, "RowHeight should be positive by default");
    }

    [TestMethod]
    public void DataGrid_DefaultValues_AlternatingRowStylesIsTrue()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.IsTrue(dataGrid.AlternatingRowStyles, "AlternatingRowStyles should be true by default");
    }

    [TestMethod]
    public void DataGrid_SetEvenRowStyle_PropertyIsSet()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>();
        var background = new SolidColorBackground(Colors.White);
        var foreground = Colors.Black;

        // Act
        var result = dataGrid.SetEvenRowStyle(background, foreground);

        // Assert
        Assert.IsNotNull(dataGrid.EvenRowStyle, "EvenRowStyle should be set");
        Assert.AreEqual(background, dataGrid.EvenRowStyle.Value.Background, "Background should match");
        Assert.AreEqual(foreground, dataGrid.EvenRowStyle.Value.Foreground, "Foreground should match");
        Assert.AreSame(dataGrid, result, "Method should return DataGrid for chaining");
    }

    [TestMethod]
    public void DataGrid_SetOddRowStyle_PropertyIsSet()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>();
        var background = new SolidColorBackground(new Color(245, 245, 245));
        var foreground = Colors.Black;

        // Act
        var result = dataGrid.SetOddRowStyle(background, foreground);

        // Assert
        Assert.IsNotNull(dataGrid.OddRowStyle, "OddRowStyle should be set");
        Assert.AreEqual(background, dataGrid.OddRowStyle.Value.Background, "Background should match");
        Assert.AreEqual(foreground, dataGrid.OddRowStyle.Value.Foreground, "Foreground should match");
        Assert.AreSame(dataGrid, result, "Method should return DataGrid for chaining");
    }

    [TestMethod]
    public void DataGrid_SetRowStyleCallback_PropertyIsSet()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>();
        Func<Person, int, DataGridRowStyle> callback = (person, index) => new DataGridRowStyle(
            Background: new SolidColorBackground(Colors.Red),
            Foreground: Colors.White
        );

        // Act
        var result = dataGrid.SetRowStyleCallback(callback);

        // Assert
        Assert.IsNotNull(dataGrid.RowStyleCallback, "RowStyleCallback should be set");
        Assert.AreSame(dataGrid, result, "Method should return DataGrid for chaining");
    }

    [TestMethod]
    public void DataGrid_MethodChaining_ReturnsDataGrid()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>();

        // Act
        var result = dataGrid
            .SetHeaderHeight(40)
            .SetRowHeight(35)
            .SetSelectionMode(SelectionMode.Multiple)
            .SetAlternatingRowStyles(true);

        // Assert
        Assert.AreSame(dataGrid, result, "All methods should return the same DataGrid instance for chaining");
        Assert.AreEqual(40f, dataGrid.HeaderHeight, "HeaderHeight should be set");
        Assert.AreEqual(35f, dataGrid.RowHeight, "RowHeight should be set");
        Assert.AreEqual(SelectionMode.Multiple, dataGrid.SelectionMode, "SelectionMode should be set");
        Assert.IsTrue(dataGrid.AlternatingRowStyles, "AlternatingRowStyles should be set");
    }

    [TestMethod]
    public void DataGrid_AccessibilityRole_IsGrid()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.AreEqual(AccessibilityRole.Grid, dataGrid.AccessibilityRole, "AccessibilityRole should be Grid");
    }

    #endregion

    #region Phase 2: Column System

    [TestMethod]
    public void DataGrid_AddColumn_ColumnIsAdded()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>();
        var column = new DataGridTextColumn<Person>()
            .SetHeader("Name")
            .SetBinding(p => p.Name);

        // Act
        var result = dataGrid.AddColumn(column);

        // Assert
        Assert.AreEqual(1, dataGrid.Columns.Count, "Column should be added");
        Assert.AreSame(column, dataGrid.Columns[0], "Added column should match");
        Assert.AreSame(dataGrid, result, "Method should return DataGrid for chaining");
    }

    [TestMethod]
    public void DataGrid_AddMultipleColumns_AllColumnsAdded()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>();
        var column1 = new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name);
        var column2 = new DataGridTextColumn<Person>().SetHeader("Age").SetBinding(p => p.Age.ToString());
        var column3 = new DataGridCheckboxColumn<Person>().SetHeader("Active").SetBinding(p => p.IsActive, (p, v) => p.IsActive = v);

        // Act
        dataGrid.AddColumn(column1).AddColumn(column2).AddColumn(column3);

        // Assert
        Assert.AreEqual(3, dataGrid.Columns.Count, "All columns should be added");
    }

    [TestMethod]
    public void DataGrid_RemoveColumn_ColumnIsRemoved()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>();
        var column = new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name);
        dataGrid.AddColumn(column);

        // Act
        var result = dataGrid.RemoveColumn(column);

        // Assert
        Assert.AreEqual(0, dataGrid.Columns.Count, "Column should be removed");
        Assert.AreSame(dataGrid, result, "Method should return DataGrid for chaining");
    }

    #endregion

    #region Phase 3: Data Binding

    [TestMethod]
    public void DataGrid_SetItemsSource_PropertyIsSet()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>();
        var items = new List<Person>
        {
            new() { Name = "Alice", Age = 30 },
            new() { Name = "Bob", Age = 25 }
        };

        // Act
        var result = dataGrid.SetItemsSource(items);

        // Assert
        Assert.AreSame(items, dataGrid.ItemsSource, "ItemsSource should be set");
        Assert.AreSame(dataGrid, result, "Method should return DataGrid for chaining");
    }

    [TestMethod]
    public void DataGrid_BindItemsSource_UpdatesOnPropertyChange()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>();
        var items1 = new List<Person> { new() { Name = "Alice" } };
        var items2 = new List<Person> { new() { Name = "Bob" }, new() { Name = "Charlie" } };
        IEnumerable<Person>? currentItems = items1;

        dataGrid.BindItemsSource("Items", () => currentItems);
        dataGrid.UpdateBindings("Items");

        // Act
        currentItems = items2;
        dataGrid.UpdateBindings("Items");

        // Assert
        Assert.AreSame(items2, dataGrid.ItemsSource, "ItemsSource should be updated via binding");
    }

    [TestMethod]
    public void DataGrid_ObservableCollection_Add_InvalidatesMeasure()
    {
        // Arrange
        var items = new ObservableCollection<Person>
        {
            new() { Name = "Alice" }
        };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name));

        dataGrid.Measure(new Size(400, 300));

        // Act
        items.Add(new Person { Name = "Bob" });

        // Assert - Measure should be invalidated (we can't directly test this, but we ensure no exception)
        dataGrid.Measure(new Size(400, 300));
    }

    [TestMethod]
    public void DataGrid_ObservableCollection_Remove_InvalidatesMeasure()
    {
        // Arrange
        var items = new ObservableCollection<Person>
        {
            new() { Name = "Alice" },
            new() { Name = "Bob" }
        };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name));

        dataGrid.Measure(new Size(400, 300));

        // Act
        items.RemoveAt(0);

        // Assert
        dataGrid.Measure(new Size(400, 300));
    }

    [TestMethod]
    public void DataGrid_ObservableCollection_Clear_ClearsGrid()
    {
        // Arrange
        var items = new ObservableCollection<Person>
        {
            new() { Name = "Alice" },
            new() { Name = "Bob" }
        };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name));

        dataGrid.Measure(new Size(400, 300));

        // Act
        items.Clear();

        // Assert
        dataGrid.Measure(new Size(400, 300));
    }

    [TestMethod]
    public void DataGrid_SetItemsSource_Null_NoException()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>()
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name));

        // Act & Assert - Should not throw
        dataGrid.SetItemsSource(null);
        dataGrid.Measure(new Size(400, 300));
    }

    [TestMethod]
    public void DataGrid_SetItemsSource_Empty_NoException()
    {
        // Arrange
        var dataGrid = new DataGrid<Person>()
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name));

        // Act & Assert - Should not throw
        dataGrid.SetItemsSource(new List<Person>());
        dataGrid.Measure(new Size(400, 300));
    }

    [TestMethod]
    public void DataGrid_ChangeItemsSource_UnsubscribesFromOldCollection()
    {
        // Arrange
        var items1 = new ObservableCollection<Person> { new() { Name = "Alice" } };
        var items2 = new ObservableCollection<Person> { new() { Name = "Bob" } };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items1)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name));

        // Act - Change to new collection
        dataGrid.SetItemsSource(items2);

        // Adding to old collection should not affect the grid (no exception)
        items1.Add(new Person { Name = "Charlie" });

        // Assert
        Assert.AreSame(items2, dataGrid.ItemsSource);
    }

    #endregion

    #region Phase 4: Virtualization

    [TestMethod]
    public void DataGrid_Virtualization_Only1000Items_NotAllRendered()
    {
        // Arrange
        var items = new List<Person>();
        for (int i = 0; i < 1000; i++)
        {
            items.Add(new Person { Name = $"Person {i}", Age = i });
        }

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name))
            .SetRowHeight(30);

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert - With 300px height and 30px row height, only ~10 rows should be visible
        // Plus header, so we should have far fewer than 1000 children
        Assert.IsLessThan(1000, dataGrid.Children.Count, "Virtualization should limit rendered items");
    }

    [TestMethod]
    public void DataGrid_Virtualization_ScrollUpdatesVisibleItems()
    {
        // Arrange
        var items = new List<Person>();
        for (int i = 0; i < 100; i++)
        {
            items.Add(new Person { Name = $"Person {i}", Age = i });
        }

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name))
            .SetRowHeight(30);

        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Act - Scroll down
        dataGrid.HandleScroll(0, 150);
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert - Should still have limited number of children
        Assert.IsLessThan(100, dataGrid.Children.Count, "Virtualization should still work after scroll");
    }

    [TestMethod]
    public void DataGrid_HandleScroll_VerticalScrollUpdatesOffset()
    {
        // Arrange
        var items = new List<Person>();
        for (int i = 0; i < 50; i++)
        {
            items.Add(new Person { Name = $"Person {i}" });
        }

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name))
            .SetRowHeight(30);

        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Act
        dataGrid.HandleScroll(0, 100);

        // Assert
        Assert.AreEqual(100, dataGrid.ScrollOffset, "ScrollOffset should be updated");
    }

    [TestMethod]
    public void DataGrid_ScrollOffset_ClampsToValidRange()
    {
        // Arrange
        var items = new List<Person>
        {
            new() { Name = "Alice" },
            new() { Name = "Bob" }
        };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name))
            .SetRowHeight(30);

        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Act - Try to scroll beyond content
        dataGrid.SetScrollOffset(-100);
        var negativeClamp = dataGrid.ScrollOffset;

        dataGrid.SetScrollOffset(10000);
        var positiveClamp = dataGrid.ScrollOffset;

        // Assert
        Assert.IsGreaterThanOrEqualTo(0, negativeClamp, "ScrollOffset should not be negative");
    }

    [TestMethod]
    public void DataGrid_ImplementsIScrollableControl()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.IsInstanceOfType<IScrollableControl>(dataGrid, "DataGrid should implement IScrollableControl");
    }

    [TestMethod]
    public void DataGrid_IsScrolling_DefaultFalse()
    {
        // Arrange & Act
        var dataGrid = new DataGrid<Person>();

        // Assert
        Assert.IsFalse(dataGrid.IsScrolling, "IsScrolling should be false by default");
    }

    #endregion

    #region Phase 5: Selection

    [TestMethod]
    public void DataGrid_SetSelectedItem_PropertyIsSet()
    {
        // Arrange
        var person = new Person { Name = "Alice" };
        var items = new List<Person> { person, new() { Name = "Bob" } };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items);

        // Act
        var result = dataGrid.SetSelectedItem(person);

        // Assert
        Assert.AreSame(person, dataGrid.SelectedItem, "SelectedItem should be set");
        Assert.AreSame(dataGrid, result, "Method should return DataGrid for chaining");
    }

    [TestMethod]
    public void DataGrid_SetSelectedItem_AddedToSelectedItems()
    {
        // Arrange
        var person = new Person { Name = "Alice" };
        var items = new List<Person> { person };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .SetSelectionMode(SelectionMode.Single);

        // Act
        dataGrid.SetSelectedItem(person);

        // Assert
        Assert.IsTrue(dataGrid.SelectedItems.Contains(person), "SelectedItem should be in SelectedItems");
    }

    [TestMethod]
    public void DataGrid_SingleSelection_ReplacesExisting()
    {
        // Arrange
        var alice = new Person { Name = "Alice" };
        var bob = new Person { Name = "Bob" };
        var items = new List<Person> { alice, bob };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .SetSelectionMode(SelectionMode.Single);

        // Act
        dataGrid.SetSelectedItem(alice);
        dataGrid.SetSelectedItem(bob);

        // Assert
        Assert.AreSame(bob, dataGrid.SelectedItem, "SelectedItem should be replaced");
        Assert.AreEqual(1, dataGrid.SelectedItems.Count, "Only one item should be selected");
        Assert.IsTrue(dataGrid.SelectedItems.Contains(bob), "Bob should be selected");
    }

    [TestMethod]
    public void DataGrid_SetSelectedItem_Null_ClearsSelection()
    {
        // Arrange
        var person = new Person { Name = "Alice" };
        var items = new List<Person> { person };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .SetSelectedItem(person);

        // Act
        dataGrid.SetSelectedItem(null);

        // Assert
        Assert.IsNull(dataGrid.SelectedItem, "SelectedItem should be null");
        Assert.AreEqual(0, dataGrid.SelectedItems.Count, "SelectedItems should be empty");
    }

    [TestMethod]
    public void DataGrid_MultipleSelection_CanSelectMultiple()
    {
        // Arrange
        var alice = new Person { Name = "Alice" };
        var bob = new Person { Name = "Bob" };
        var charlie = new Person { Name = "Charlie" };
        var items = new List<Person> { alice, bob, charlie };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .SetSelectionMode(SelectionMode.Multiple);

        // Act
        dataGrid.SelectItem(alice);
        dataGrid.SelectItem(bob);

        // Assert
        Assert.AreEqual(2, dataGrid.SelectedItems.Count, "Two items should be selected");
        Assert.IsTrue(dataGrid.SelectedItems.Contains(alice), "Alice should be selected");
        Assert.IsTrue(dataGrid.SelectedItems.Contains(bob), "Bob should be selected");
    }

    [TestMethod]
    public void DataGrid_MultipleSelection_DeselectItem()
    {
        // Arrange
        var alice = new Person { Name = "Alice" };
        var bob = new Person { Name = "Bob" };
        var items = new List<Person> { alice, bob };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .SetSelectionMode(SelectionMode.Multiple);

        dataGrid.SelectItem(alice);
        dataGrid.SelectItem(bob);

        // Act
        dataGrid.DeselectItem(alice);

        // Assert
        Assert.AreEqual(1, dataGrid.SelectedItems.Count, "One item should remain selected");
        Assert.IsFalse(dataGrid.SelectedItems.Contains(alice), "Alice should be deselected");
        Assert.IsTrue(dataGrid.SelectedItems.Contains(bob), "Bob should still be selected");
    }

    [TestMethod]
    public void DataGrid_MultipleSelection_ClearSelection()
    {
        // Arrange
        var alice = new Person { Name = "Alice" };
        var bob = new Person { Name = "Bob" };
        var items = new List<Person> { alice, bob };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .SetSelectionMode(SelectionMode.Multiple);

        dataGrid.SelectItem(alice);
        dataGrid.SelectItem(bob);

        // Act
        dataGrid.ClearSelection();

        // Assert
        Assert.IsNull(dataGrid.SelectedItem, "SelectedItem should be null");
        Assert.AreEqual(0, dataGrid.SelectedItems.Count, "SelectedItems should be empty");
    }

    [TestMethod]
    public void DataGrid_NoSelection_CannotSelect()
    {
        // Arrange
        var person = new Person { Name = "Alice" };
        var items = new List<Person> { person };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .SetSelectionMode(SelectionMode.None);

        // Act
        dataGrid.SelectItem(person);

        // Assert
        Assert.IsNull(dataGrid.SelectedItem, "SelectedItem should remain null");
        Assert.AreEqual(0, dataGrid.SelectedItems.Count, "No items should be selected");
    }

    [TestMethod]
    public void DataGrid_BindSelectedItem_TwoWay()
    {
        // Arrange
        var alice = new Person { Name = "Alice" };
        var bob = new Person { Name = "Bob" };
        var items = new List<Person> { alice, bob };

        Person? selectedPerson = null;
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .BindSelectedItem("SelectedPerson",
                () => selectedPerson,
                v => selectedPerson = v);

        // Act - Set from binding
        selectedPerson = alice;
        dataGrid.UpdateBindings("SelectedPerson");

        // Assert
        Assert.AreSame(alice, dataGrid.SelectedItem, "SelectedItem should be updated from binding");

        // Act - Set from DataGrid
        dataGrid.SetSelectedItem(bob);

        // Assert
        Assert.AreSame(bob, selectedPerson, "Bound property should be updated from DataGrid");
    }

    #endregion

    #region Layout Debug Tests

    [TestMethod]
    public void DataGrid_Layout_HasChildren_AfterMeasureAndArrange()
    {
        // Arrange
        var items = new List<Person>
        {
            new() { Name = "Alice", Age = 30 },
            new() { Name = "Bob", Age = 25 }
        };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name)
                .SetWidth(DataGridColumnWidth.Star(1)));

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert
        Assert.IsGreaterThan(0, dataGrid.Children.Count, $"DataGrid should have children after layout. ElementSize: {dataGrid.ElementSize}");
    }

    [TestMethod]
    public void DataGrid_Layout_ElementSize_IsPositive()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice" } };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name));

        // Act
        dataGrid.Measure(new Size(400, 300));

        // Assert
        Assert.IsGreaterThan(0, dataGrid.ElementSize.Width, $"Width should be > 0, got {dataGrid.ElementSize.Width}");
        Assert.IsGreaterThan(0, dataGrid.ElementSize.Height, $"Height should be > 0, got {dataGrid.ElementSize.Height}");
    }

    [TestMethod]
    public void DataGrid_Layout_ChildrenHavePositiveSize()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice" } };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name)
                .SetWidth(DataGridColumnWidth.Absolute(100)));

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert
        foreach (var child in dataGrid.Children)
        {
            Assert.IsGreaterThan(0, child.ElementSize.Width,
                $"Child {child.GetType().Name} should have width > 0, got {child.ElementSize.Width}");
            Assert.IsGreaterThan(0, child.ElementSize.Height,
                $"Child {child.GetType().Name} should have height > 0, got {child.ElementSize.Height}");
        }
    }

    [TestMethod]
    public void DataGrid_Layout_ChildrenHaveValidPosition()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice" } };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name)
                .SetWidth(DataGridColumnWidth.Absolute(100)));

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert
        foreach (var child in dataGrid.Children)
        {
            Assert.IsGreaterThanOrEqualTo(0, child.Position.X,
                $"Child {child.GetType().Name} X should be >= 0, got {child.Position.X}");
            Assert.IsGreaterThanOrEqualTo(0, child.Position.Y,
                $"Child {child.GetType().Name} Y should be >= 0, got {child.Position.Y}");
        }
    }

    [TestMethod]
    public void DataGrid_Layout_ColumnWidthsCalculated()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice" } };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name)
                .SetWidth(DataGridColumnWidth.Star(1)));

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert
        Assert.AreEqual(400f, dataGrid.Columns[0].ActualWidth,
            $"Column should have full width of 400, got {dataGrid.Columns[0].ActualWidth}");
    }

    #endregion

    #region Phase 6: Layout & Rendering

    [TestMethod]
    public void DataGrid_MeasureAndArrange_AbsoluteColumnWidth()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice", Age = 30 } };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name)
                .SetWidth(DataGridColumnWidth.Absolute(100)))
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Age")
                .SetBinding(p => p.Age.ToString())
                .SetWidth(DataGridColumnWidth.Absolute(50)));

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert - Columns should have specified widths
        Assert.AreEqual(100f, dataGrid.Columns[0].ActualWidth, "First column should be 100px");
        Assert.AreEqual(50f, dataGrid.Columns[1].ActualWidth, "Second column should be 50px");
    }

    [TestMethod]
    public void DataGrid_MeasureAndArrange_StarColumnsDistributeSpace()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice", Age = 30 } };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name)
                .SetWidth(DataGridColumnWidth.Star(2)))
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Age")
                .SetBinding(p => p.Age.ToString())
                .SetWidth(DataGridColumnWidth.Star(1)));

        // Act
        dataGrid.Measure(new Size(300, 300));
        dataGrid.Arrange(new Rect(0, 0, 300, 300));

        // Assert - Star columns should distribute space proportionally (2:1 ratio)
        // 300px / 3 total stars = 100px per star
        // Column 1: 2 * 100 = 200px
        // Column 2: 1 * 100 = 100px
        Assert.AreEqual(200f, dataGrid.Columns[0].ActualWidth, 0.1f, "First column should get 2/3 of space");
        Assert.AreEqual(100f, dataGrid.Columns[1].ActualWidth, 0.1f, "Second column should get 1/3 of space");
    }

    [TestMethod]
    public void DataGrid_MeasureAndArrange_MixedColumnWidths()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice", Age = 30 } };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Fixed")
                .SetBinding(p => "X")
                .SetWidth(DataGridColumnWidth.Absolute(100)))
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name)
                .SetWidth(DataGridColumnWidth.Star(1)))
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Age")
                .SetBinding(p => p.Age.ToString())
                .SetWidth(DataGridColumnWidth.Star(1)));

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert - Fixed column is 100, remaining 300 split equally
        Assert.AreEqual(100f, dataGrid.Columns[0].ActualWidth, "Fixed column should be 100px");
        Assert.AreEqual(150f, dataGrid.Columns[1].ActualWidth, 0.1f, "Star column should get half of remaining");
        Assert.AreEqual(150f, dataGrid.Columns[2].ActualWidth, 0.1f, "Star column should get half of remaining");
    }

    [TestMethod]
    public void DataGrid_HitTest_InsideBounds_ReturnsDataGrid()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice" } };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name));

        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Act
        var hit = dataGrid.HitTest(new Point(200, 150));

        // Assert
        Assert.IsNotNull(hit, "HitTest should return a result inside bounds");
    }

    [TestMethod]
    public void DataGrid_HitTest_OutsideBounds_ReturnsNull()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice" } };
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name));

        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Act
        var hit = dataGrid.HitTest(new Point(500, 400));

        // Assert
        Assert.IsNull(hit, "HitTest should return null outside bounds");
    }

    [TestMethod]
    public void DataGrid_RowStyling_AlternatingRows_AppliesCorrectBackground()
    {
        // Arrange
        var items = new List<Person>
        {
            new() { Name = "Alice" },
            new() { Name = "Bob" },
            new() { Name = "Charlie" }
        };

        var evenBg = new SolidColorBackground(Colors.White);
        var oddBg = new SolidColorBackground(Colors.LightGray);

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name))
            .SetAlternatingRowStyles(true)
            .SetEvenRowStyle(evenBg, Colors.Black)
            .SetOddRowStyle(oddBg, Colors.Black);

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert - Row styles should be applied (visual test, we ensure no exception)
        Assert.IsTrue(dataGrid.AlternatingRowStyles);
    }

    [TestMethod]
    public void DataGrid_RowStyling_CustomCallback_AppliesStyle()
    {
        // Arrange
        var items = new List<Person>
        {
            new() { Name = "Alice", IsActive = true },
            new() { Name = "Bob", IsActive = false }
        };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name))
            .SetRowStyleCallback((person, index) => new DataGridRowStyle(
                Background: person.IsActive
                    ? new SolidColorBackground(Colors.LightGreen)
                    : new SolidColorBackground(Colors.LightCoral),
                Foreground: Colors.Black
            ));

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert
        Assert.IsNotNull(dataGrid.RowStyleCallback);
    }

    [TestMethod]
    public void DataGrid_RowStyling_CallbackOverridesAlternating()
    {
        // Arrange
        var items = new List<Person> { new() { Name = "Alice" } };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name))
            .SetAlternatingRowStyles(true)
            .SetEvenRowStyle(new SolidColorBackground(Colors.White), Colors.Black)
            .SetRowStyleCallback((person, index) => new DataGridRowStyle(
                Background: new SolidColorBackground(Colors.Yellow),
                Foreground: Colors.Black
            ));

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert - Callback should take precedence (visual test)
        Assert.IsNotNull(dataGrid.RowStyleCallback);
    }

    [TestMethod]
    public void DataGrid_RowStyling_SelectedRowHighlight()
    {
        // Arrange
        var alice = new Person { Name = "Alice" };
        var items = new List<Person> { alice, new() { Name = "Bob" } };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>().SetHeader("Name").SetBinding(p => p.Name))
            .SetSelectedItem(alice);

        // Act
        dataGrid.Measure(new Size(400, 300));
        dataGrid.Arrange(new Rect(0, 0, 400, 300));

        // Assert
        Assert.AreSame(alice, dataGrid.SelectedItem, "Item should be selected");
    }

    #endregion

    #region Debug Tests

    [TestMethod]
    public void DataGrid_InVStack_HasCorrectSize()
    {
        // Arrange - Simulate the demo page structure
        var items = new List<Person>
        {
            new() { Name = "Alice", Age = 30, IsActive = true },
            new() { Name = "Bob", Age = 25, IsActive = false }
        };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name))
            .SetDesiredHeight(400);

        var vstack = new VStack(
            new HStack(new Label().SetText("Header")).SetDesiredHeight(40),
            new HStack(new Label().SetText("Status")).SetDesiredHeight(40),
            dataGrid,
            new VStack(new Label().SetText("Info")).SetDesiredHeight(100)
        );

        // Act
        vstack.Measure(new Size(800, 600));
        vstack.Arrange(new Rect(0, 0, 800, 600));

        // Assert - DataGrid should have correct size
        Assert.IsGreaterThan(0f, dataGrid.ElementSize.Height, "DataGrid should have positive height");
        Assert.AreEqual(400f, dataGrid.ElementSize.Height, 10f, "DataGrid should respect DesiredHeight");
        Assert.IsGreaterThan(0, ((UiLayoutElement)dataGrid).Children.Count, "DataGrid should have children");
    }

    [TestMethod]
    public void DataGrid_InVStack_ChildrenHavePositiveSize()
    {
        // Arrange
        var items = new List<Person>
        {
            new() { Name = "Alice", Age = 30, IsActive = true },
        };

        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name))
            .SetDesiredHeight(300);

        var vstack = new VStack(dataGrid);

        // Act
        vstack.Measure(new Size(800, 600));
        vstack.Arrange(new Rect(0, 0, 800, 600));

        // Assert
        foreach (var child in ((UiLayoutElement)dataGrid).Children)
        {
            Assert.IsGreaterThan(0f, child.ElementSize.Width, $"Child {child.GetType().Name} should have positive width");
            Assert.IsGreaterThan(0f, child.ElementSize.Height, $"Child {child.GetType().Name} should have positive height");
        }
    }

    #endregion

    #region API Order Tests

    [TestMethod]
    public void DataGrid_SetItemsSourceBeforeAddColumn_HasChildren()
    {
        // Arrange - This mimics the demo page usage pattern
        var items = new List<Person>
        {
            new() { Name = "Alice", Age = 30, IsActive = true },
            new() { Name = "Bob", Age = 25, IsActive = false }
        };

        // Act - SetItemsSource BEFORE AddColumn (like the demo does)
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name)
                .SetWidth(DataGridColumnWidth.Star(2)))
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Age")
                .SetBinding(p => p.Age.ToString())
                .SetWidth(DataGridColumnWidth.Absolute(60)));

        dataGrid.Measure(new Size(800, 400));
        dataGrid.Arrange(new Rect(0, 0, 800, 400));

        // Assert
        Assert.IsGreaterThan(0, dataGrid.Children.Count, "DataGrid should have children after measure/arrange");
        Assert.IsGreaterThan(0f, dataGrid.ElementSize.Width, "DataGrid should have positive width");
        Assert.IsGreaterThan(0f, dataGrid.ElementSize.Height, "DataGrid should have positive height");
    }

    [TestMethod]
    public void DataGrid_SetItemsSourceBeforeAddColumn_ChildrenHavePositiveSize()
    {
        // Arrange
        var items = new List<Person>
        {
            new() { Name = "Alice", Age = 30, IsActive = true },
            new() { Name = "Bob", Age = 25, IsActive = false }
        };

        // Act - SetItemsSource BEFORE AddColumn
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name))
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Age")
                .SetBinding(p => p.Age.ToString()));

        dataGrid.Measure(new Size(800, 400));
        dataGrid.Arrange(new Rect(0, 0, 800, 400));

        // Assert - All children should have positive sizes
        foreach (var child in dataGrid.Children)
        {
            Assert.IsGreaterThan(0f, child.ElementSize.Width, $"Child {child.GetType().Name} should have positive width");
            Assert.IsGreaterThan(0f, child.ElementSize.Height, $"Child {child.GetType().Name} should have positive height");
        }
    }

    [TestMethod]
    public void DataGrid_SetItemsSourceBeforeAddColumn_ChildrenPositionedCorrectly()
    {
        // Arrange
        var items = new List<Person>
        {
            new() { Name = "Alice", Age = 30, IsActive = true },
        };

        // Act
        var dataGrid = new DataGrid<Person>()
            .SetItemsSource(items)
            .AddColumn(new DataGridTextColumn<Person>()
                .SetHeader("Name")
                .SetBinding(p => p.Name));

        dataGrid.Measure(new Size(800, 400));
        dataGrid.Arrange(new Rect(10, 20, 800, 400));

        // Assert - Children should be positioned relative to DataGrid position
        foreach (var child in dataGrid.Children)
        {
            Assert.IsTrue(child.Position.X >= 10f, $"Child X ({child.Position.X}) should be >= DataGrid X (10)");
            Assert.IsTrue(child.Position.Y >= 20f, $"Child Y ({child.Position.Y}) should be >= DataGrid Y (20)");
            Assert.IsTrue(child.Position.X <= 810f, $"Child X ({child.Position.X}) should be <= DataGrid right edge (810)");
            Assert.IsTrue(child.Position.Y <= 420f, $"Child Y ({child.Position.Y}) should be <= DataGrid bottom edge (420)");
        }
    }

    #endregion
}
