using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusUi.core;
using System.Collections.ObjectModel;

namespace UiPlus.core.Tests;

[TestClass]
public class ComboBoxTests
{
    [TestMethod]
    public void ComboBox_SetItemsSource_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };

        // Act
        comboBox.SetItemsSource(items);

        // Assert
        Assert.AreEqual(items, comboBox.ItemsSource);
    }

    [TestMethod]
    public void ComboBox_SetSelectedItem_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);

        // Act
        comboBox.SetSelectedItem("Item 2");

        // Assert
        Assert.AreEqual("Item 2", comboBox.SelectedItem);
        Assert.AreEqual(1, comboBox.SelectedIndex);
    }

    [TestMethod]
    public void ComboBox_SetSelectedIndex_ShouldUpdateSelectedItem()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);

        // Act
        comboBox.SetSelectedIndex(2);

        // Assert
        Assert.AreEqual("Item 3", comboBox.SelectedItem);
        Assert.AreEqual(2, comboBox.SelectedIndex);
    }

    [TestMethod]
    public void ComboBox_SetSelectedItem_ShouldUpdateSelectedIndex()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);

        // Act
        comboBox.SetSelectedItem("Item 1");

        // Assert
        Assert.AreEqual(0, comboBox.SelectedIndex);
    }

    [TestMethod]
    public void ComboBox_DefaultSelectedIndex_ShouldBeMinusOne()
    {
        // Arrange & Act
        var comboBox = new ComboBox<string>();

        // Assert
        Assert.AreEqual(-1, comboBox.SelectedIndex);
    }

    [TestMethod]
    public void ComboBox_SetPlaceholder_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();

        // Act
        comboBox.SetPlaceholder("Select an option...");

        // Assert
        Assert.AreEqual("Select an option...", comboBox.Placeholder);
    }

    [TestMethod]
    public void ComboBox_SetIsOpen_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();

        // Act
        comboBox.SetIsOpen(true);

        // Assert
        Assert.IsTrue(comboBox.IsOpen);
    }

    [TestMethod]
    public void ComboBox_DefaultIsOpen_ShouldBeFalse()
    {
        // Arrange & Act
        var comboBox = new ComboBox<string>();

        // Assert
        Assert.IsFalse(comboBox.IsOpen);
    }

    [TestMethod]
    public void ComboBox_InvokeCommand_ShouldToggleIsOpen()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var initialState = comboBox.IsOpen;

        // Act
        comboBox.InvokeCommand();

        // Assert
        Assert.AreEqual(!initialState, comboBox.IsOpen);

        // Toggle again
        comboBox.InvokeCommand();
        Assert.AreEqual(initialState, comboBox.IsOpen);
    }

    [TestMethod]
    public void ComboBox_SetDisplayFunc_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<int>();
        Func<int, string> displayFunc = i => $"Number {i}";

        // Act
        comboBox.SetDisplayFunc(displayFunc);

        // Assert
        Assert.AreEqual(displayFunc, comboBox.DisplayFunc);
    }

    [TestMethod]
    public void ComboBox_DefaultDisplayFunc_ShouldUseToString()
    {
        // Arrange
        var comboBox = new ComboBox<int>();
        var items = new[] { 1, 2, 3 };
        comboBox.SetItemsSource(items);
        comboBox.SetSelectedItem(2);

        // Act
        var result = comboBox.DisplayFunc(comboBox.SelectedItem!);

        // Assert
        Assert.AreEqual("2", result);
    }

    [TestMethod]
    public void ComboBox_CustomDisplayFunc_ShouldWork()
    {
        // Arrange
        var comboBox = new ComboBox<int>();
        comboBox.SetDisplayFunc(i => $"Number {i}");
        var items = new[] { 1, 2, 3 };
        comboBox.SetItemsSource(items);
        comboBox.SetSelectedItem(2);

        // Act
        var result = comboBox.DisplayFunc(comboBox.SelectedItem!);

        // Assert
        Assert.AreEqual("Number 2", result);
    }

    [TestMethod]
    public void ComboBox_BindSelectedItem_ShouldBindProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        string? selectedItem = "Item 2";

        // Act
        comboBox.BindSelectedItem(nameof(selectedItem), () => selectedItem, v => selectedItem = v);
        comboBox.UpdateBindings();

        // Assert
        Assert.AreEqual("Item 2", comboBox.SelectedItem);
        Assert.AreEqual(1, comboBox.SelectedIndex);

        // Change value and update
        selectedItem = "Item 3";
        comboBox.UpdateBindings();

        // Assert
        Assert.AreEqual("Item 3", comboBox.SelectedItem);
        Assert.AreEqual(2, comboBox.SelectedIndex);
    }

    [TestMethod]
    public void ComboBox_BindSelectedIndex_ShouldBindProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        int selectedIndex = 1;

        // Act
        comboBox.BindSelectedIndex(nameof(selectedIndex), () => selectedIndex, v => selectedIndex = v);
        comboBox.UpdateBindings();

        // Assert
        Assert.AreEqual(1, comboBox.SelectedIndex);
        Assert.AreEqual("Item 2", comboBox.SelectedItem);

        // Change value and update
        selectedIndex = 0;
        comboBox.UpdateBindings();

        // Assert
        Assert.AreEqual(0, comboBox.SelectedIndex);
        Assert.AreEqual("Item 1", comboBox.SelectedItem);
    }

    [TestMethod]
    public void ComboBox_BindItemsSource_ShouldBindProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };

        // Act
        comboBox.BindItemsSource(nameof(items), () => items);
        comboBox.UpdateBindings();

        // Assert
        Assert.AreEqual(items, comboBox.ItemsSource);
    }

    [TestMethod]
    public void ComboBox_ObservableCollection_ShouldUpdateOnChange()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new ObservableCollection<string> { "Item 1", "Item 2" };
        comboBox.SetItemsSource(items);

        // Act - Add item
        items.Add("Item 3");

        // Assert - The combo box should have updated its cache
        // We can't directly check the cache, but we can test that setting selected index works
        comboBox.SetSelectedIndex(2);
        Assert.AreEqual("Item 3", comboBox.SelectedItem);
    }

    [TestMethod]
    public void ComboBox_MethodChaining_ShouldWork()
    {
        // Arrange
        var items = new[] { "Item 1", "Item 2", "Item 3" };

        // Act
        var comboBox = new ComboBox<string>()
            .SetItemsSource(items)
            .SetSelectedIndex(1)
            .SetPlaceholder("Select...")
            .SetIsOpen(true)
            .SetTextSize(16)
            .SetTextColor(SkiaSharp.SKColors.Red);

        // Assert
        Assert.AreEqual(items, comboBox.ItemsSource);
        Assert.AreEqual(1, comboBox.SelectedIndex);
        Assert.AreEqual("Item 2", comboBox.SelectedItem);
        Assert.AreEqual("Select...", comboBox.Placeholder);
        Assert.IsTrue(comboBox.IsOpen);
        Assert.AreEqual(16f, comboBox.TextSize);
        Assert.AreEqual(SkiaSharp.SKColors.Red, comboBox.TextColor);
    }

    [TestMethod]
    public void ComboBox_SetPadding_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var padding = new Margin(10, 5);

        // Act
        comboBox.SetPadding(padding);

        // Assert
        Assert.AreEqual(10, comboBox.Padding.Left);
        Assert.AreEqual(5, comboBox.Padding.Top);
    }

    [TestMethod]
    public void ComboBox_DefaultPadding_ShouldBe12_8()
    {
        // Arrange & Act
        var comboBox = new ComboBox<string>();

        // Assert
        Assert.AreEqual(12, comboBox.Padding.Left);
        Assert.AreEqual(8, comboBox.Padding.Top);
    }

    [TestMethod]
    public void ComboBox_SetTextColor_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();

        // Act
        comboBox.SetTextColor(SkiaSharp.SKColors.Blue);

        // Assert
        Assert.AreEqual(SkiaSharp.SKColors.Blue, comboBox.TextColor);
    }

    [TestMethod]
    public void ComboBox_SetTextSize_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();

        // Act
        comboBox.SetTextSize(18f);

        // Assert
        Assert.AreEqual(18f, comboBox.TextSize);
    }

    [TestMethod]
    public void ComboBox_DefaultTextSize_ShouldBe14()
    {
        // Arrange & Act
        var comboBox = new ComboBox<string>();

        // Assert
        Assert.AreEqual(14f, comboBox.TextSize);
    }

    [TestMethod]
    public void ComboBox_Measure_ShouldReturnValidSize()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        comboBox.SetSelectedItem("Item 1");

        // Act
        comboBox.Measure(new Size(300, 300));

        // Assert
        Assert.IsTrue(comboBox.ElementSize.Width > 0);
        Assert.IsTrue(comboBox.ElementSize.Height > 0);
    }

    [TestMethod]
    public void ComboBox_WithCustomObjects_ShouldWork()
    {
        // Arrange
        var person1 = new TestPerson { Name = "Alice", Age = 30 };
        var person2 = new TestPerson { Name = "Bob", Age = 25 };
        var items = new[] { person1, person2 };

        var comboBox = new ComboBox<TestPerson>()
            .SetItemsSource(items)
            .SetDisplayFunc(p => p.Name)
            .SetSelectedItem(person1);

        // Act
        var displayText = comboBox.DisplayFunc(comboBox.SelectedItem!);

        // Assert
        Assert.AreEqual("Alice", displayText);
        Assert.AreEqual(person1, comboBox.SelectedItem);
        Assert.AreEqual(0, comboBox.SelectedIndex);
    }

    [TestMethod]
    public void ComboBox_SetSelectedItemToNull_ShouldClearSelection()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        comboBox.SetSelectedItem("Item 2");

        // Act
        comboBox.SetSelectedItem(null);

        // Assert
        Assert.IsNull(comboBox.SelectedItem);
        Assert.AreEqual(-1, comboBox.SelectedIndex);
    }

    [TestMethod]
    public void ComboBox_SetSelectedIndexToMinusOne_ShouldClearSelection()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        comboBox.SetSelectedIndex(1);

        // Act
        comboBox.SetSelectedIndex(-1);

        // Assert
        Assert.AreEqual(-1, comboBox.SelectedIndex);
        Assert.IsNull(comboBox.SelectedItem);
    }

    [TestMethod]
    public void ComboBox_HitTest_OnComboBoxArea_ShouldReturnComboBox()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        comboBox.Measure(new Size(300, 300));
        comboBox.Arrange(new Rect(10, 10, 200, 40));

        // Act - click inside the combo box button area
        var result = comboBox.HitTest(new Point(50, 25));

        // Assert
        Assert.AreEqual(comboBox, result);
    }

    [TestMethod]
    public void ComboBox_HitTest_OutsideComboBox_ShouldReturnNull()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        comboBox.Measure(new Size(300, 300));
        comboBox.Arrange(new Rect(10, 10, 200, 40));

        // Act - click outside the combo box
        var result = comboBox.HitTest(new Point(300, 300));

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ComboBox_HitTest_OutsideWhenOpen_ShouldNotCloseDropdown()
    {
        // Arrange
        // Note: Closing dropdown on outside click is handled by InputService,
        // not by HitTest (HitTest should have no side effects)
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        comboBox.SetIsOpen(true);
        comboBox.Measure(new Size(300, 300));
        comboBox.Arrange(new Rect(10, 10, 200, 40));

        // Act - click outside the combo box when open
        comboBox.HitTest(new Point(300, 300));

        // Assert - HitTest should NOT close dropdown (no side effects)
        Assert.IsTrue(comboBox.IsOpen);
    }

    [TestMethod]
    public void ComboBox_InvokeSetters_ShouldCallSelectedItemSetter()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        string? capturedValue = null;
        comboBox.BindSelectedItem("test", () => null, v => capturedValue = v);
        comboBox.SetSelectedItem("Item 2");

        // Act
        comboBox.InvokeSetters();

        // Assert
        Assert.AreEqual("Item 2", capturedValue);
    }

    [TestMethod]
    public void ComboBox_InvokeSetters_ShouldCallSelectedIndexSetter()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        comboBox.SetItemsSource(items);
        int capturedIndex = -999;
        comboBox.BindSelectedIndex("test", () => -1, v => capturedIndex = v);
        comboBox.SetSelectedIndex(2);

        // Act
        comboBox.InvokeSetters();

        // Assert
        Assert.AreEqual(2, capturedIndex);
    }

    [TestMethod]
    public void ComboBox_SetDropdownBackground_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();

        // Act
        comboBox.SetDropdownBackground(SkiaSharp.SKColors.Navy);

        // Assert
        Assert.AreEqual(SkiaSharp.SKColors.Navy, comboBox.DropdownBackground);
    }

    [TestMethod]
    public void ComboBox_SetHoverBackground_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();

        // Act
        comboBox.SetHoverBackground(SkiaSharp.SKColors.Green);

        // Assert
        Assert.AreEqual(SkiaSharp.SKColors.Green, comboBox.HoverBackground);
    }

    [TestMethod]
    public void ComboBox_SetPlaceholderColor_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();

        // Act
        comboBox.SetPlaceholderColor(SkiaSharp.SKColors.Gray);

        // Assert
        Assert.AreEqual(SkiaSharp.SKColors.Gray, comboBox.PlaceholderColor);
    }

    [TestMethod]
    public void ComboBox_SetFontFamily_ShouldSetProperty()
    {
        // Arrange
        var comboBox = new ComboBox<string>();

        // Act
        comboBox.SetFontFamily("Arial");

        // Assert
        Assert.AreEqual("Arial", comboBox.FontFamily);
    }

    [TestMethod]
    public void ComboBox_Dispose_ShouldNotThrow()
    {
        // Arrange
        var comboBox = new ComboBox<string>();
        var items = new ObservableCollection<string> { "Item 1", "Item 2" };
        comboBox.SetItemsSource(items);
        comboBox.SetIsOpen(true);

        // Act & Assert - should not throw
        // Note: Dispose cleans up resources but doesn't change IsOpen state
        try
        {
            comboBox.Dispose();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Dispose should not throw: {ex.Message}");
        }
    }
}

// Helper class for testing custom objects
public class TestPerson
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}
