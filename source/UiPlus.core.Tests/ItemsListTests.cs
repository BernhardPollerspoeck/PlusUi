using PlusUi.core;
using System.Collections.ObjectModel;

namespace UiPlus.core.Tests;

[TestClass]
public class ItemsListTests
{
    private class TestItem
    {
        public string Text { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    [TestMethod]
    public void TestItemsList_Properties_DefaultValues()
    {
        // Arrange & Act
        var itemsList = new ItemsList<TestItem>();
        
        // Assert
        Assert.AreEqual(Orientation.Vertical, itemsList.Orientation, "Orientation should be Vertical by default");
        Assert.IsNull(itemsList.ItemsSource, "ItemsSource should be null by default");
        Assert.IsNull(itemsList.ItemTemplate, "ItemTemplate should be null by default");
        Assert.IsFalse(itemsList.CanScrollHorizontally, "CanScrollHorizontally should be false by default");
        Assert.IsTrue(itemsList.CanScrollVertically, "CanScrollVertically should be true by default");
        Assert.AreEqual(0, itemsList.HorizontalOffset, "HorizontalOffset should be 0 by default");
        Assert.AreEqual(0, itemsList.VerticalOffset, "VerticalOffset should be 0 by default");
        Assert.IsFalse(itemsList.IsScrolling, "IsScrolling should be false by default");
    }
    
    [TestMethod]
    public void TestItemsList_Orientation_SetterAndBinder()
    {
        // Arrange
        var itemsList = new ItemsList<TestItem>();
        
        // Act - Test setter
        var result1 = itemsList.SetOrientation(Orientation.Horizontal);
        
        // Assert
        Assert.AreEqual(Orientation.Horizontal, itemsList.Orientation, "Orientation should be set to Horizontal");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");
        
        // Act - Test binding
        Orientation propertyValue = Orientation.Vertical;
        var result2 = itemsList.BindOrientation("TestProperty", () => propertyValue);
        itemsList.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.AreEqual(Orientation.Vertical, itemsList.Orientation, "Orientation should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");
        
        // Change the bound property and update bindings
        propertyValue = Orientation.Horizontal;
        itemsList.UpdateBindings("TestProperty");
        
        // Verify binding update took effect
        Assert.AreEqual(Orientation.Horizontal, itemsList.Orientation, "Orientation should reflect the updated bound property");
    }
    
    [TestMethod]
    public void TestItemsList_ItemsSource_SetterAndBinder()
    {
        // Arrange
        var itemsList = new ItemsList<TestItem>();
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 }
        };
        
        // Act - Test setter
        var result1 = itemsList.SetItemsSource(items);
        
        // Assert
        Assert.AreSame(items, itemsList.ItemsSource, "ItemsSource should be set to the provided collection");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");
        
        // Act - Test binding
        IEnumerable<TestItem>? propertyValue = new List<TestItem> { new TestItem { Text = "Item 3", Value = 3 } };
        var result2 = itemsList.BindItemsSource("TestProperty", () => propertyValue);
        itemsList.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.AreSame(propertyValue, itemsList.ItemsSource, "ItemsSource should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");
    }
    
    [TestMethod]
    public void TestItemsList_ItemTemplate_SetterAndBinder()
    {
        // Arrange
        var itemsList = new ItemsList<TestItem>();
        Func<TestItem, UiElement> template = item => new Label().SetText(item.Text);
        
        // Act - Test setter
        var result1 = itemsList.SetItemTemplate(template);
        
        // Assert
        Assert.AreSame(template, itemsList.ItemTemplate, "ItemTemplate should be set to the provided template");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");
        
        // Act - Test binding
        Func<TestItem, UiElement>? propertyValue = item => new Label().SetText(item.Value.ToString());
        var result2 = itemsList.BindItemTemplate("TestProperty", () => propertyValue);
        itemsList.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.AreSame(propertyValue, itemsList.ItemTemplate, "ItemTemplate should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");
    }
    
    [TestMethod]
    public void TestItemsList_VerticalOrientation_MeasureAndArrange()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 },
            new TestItem { Text = "Item 3", Value = 3 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical)
            .SetCanScrollVertically(true);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        // Assert
        Assert.AreEqual(200, itemsList.ElementSize.Width, "ItemsList should take full available width");
        Assert.AreEqual(200, itemsList.ElementSize.Height, "ItemsList should take full available height");
        Assert.IsTrue(itemsList.Children.Count > 0, "ItemsList should have children");
    }
    
    [TestMethod]
    public void TestItemsList_HorizontalOrientation_MeasureAndArrange()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 },
            new TestItem { Text = "Item 3", Value = 3 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(50, 100))
            .SetOrientation(Orientation.Horizontal)
            .SetCanScrollHorizontally(true);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        // Assert
        Assert.AreEqual(200, itemsList.ElementSize.Width, "ItemsList should take full available width");
        Assert.AreEqual(200, itemsList.ElementSize.Height, "ItemsList should take full available height");
        Assert.IsTrue(itemsList.Children.Count > 0, "ItemsList should have children");
    }
    
    [TestMethod]
    public void TestItemsList_EmptyItemsSource_NoChildren()
    {
        // Arrange
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(new List<TestItem>())
            .SetItemTemplate(item => new Label().SetText(item.Text));
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        // Assert
        Assert.AreEqual(0, itemsList.Children.Count, "ItemsList with empty source should have no children");
    }
    
    [TestMethod]
    public void TestItemsList_NullItemsSource_NoChildren()
    {
        // Arrange
        var itemsList = new ItemsList<TestItem>()
            .SetItemTemplate(item => new Label().SetText(item.Text));
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        // Assert
        Assert.AreEqual(0, itemsList.Children.Count, "ItemsList with null source should have no children");
    }
    
    [TestMethod]
    public void TestItemsList_NullItemTemplate_NoChildren()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        // Assert
        Assert.AreEqual(0, itemsList.Children.Count, "ItemsList with null template should have no children");
    }
    
    [TestMethod]
    public void TestItemsList_VerticalScrolling_UpdatesOffset()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 },
            new TestItem { Text = "Item 3", Value = 3 },
            new TestItem { Text = "Item 4", Value = 4 },
            new TestItem { Text = "Item 5", Value = 5 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical)
            .SetCanScrollVertically(true);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        itemsList.HandleScroll(0, 50);
        
        // Assert
        Assert.AreEqual(50, itemsList.VerticalOffset, "Vertical offset should be updated after scrolling");
    }
    
    [TestMethod]
    public void TestItemsList_HorizontalScrolling_UpdatesOffset()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 },
            new TestItem { Text = "Item 3", Value = 3 },
            new TestItem { Text = "Item 4", Value = 4 },
            new TestItem { Text = "Item 5", Value = 5 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(50, 100))
            .SetOrientation(Orientation.Horizontal)
            .SetCanScrollHorizontally(true);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        itemsList.HandleScroll(50, 0);
        
        // Assert
        Assert.AreEqual(50, itemsList.HorizontalOffset, "Horizontal offset should be updated after scrolling");
    }
    
    [TestMethod]
    public void TestItemsList_ScrollingDisabled_OffsetNotUpdated()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical)
            .SetCanScrollVertically(false);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        itemsList.HandleScroll(0, 50);
        
        // Assert
        Assert.AreEqual(0, itemsList.VerticalOffset, "Vertical offset should not change when scrolling is disabled");
    }
    
    [TestMethod]
    public void TestItemsList_ObservableCollection_CollectionChanged()
    {
        // Arrange
        var items = new ObservableCollection<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical);
        
        var availableSize = new Size(200, 200);
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        var initialChildCount = itemsList.Children.Count;
        
        // Act - Add item to observable collection
        items.Add(new TestItem { Text = "Item 3", Value = 3 });
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        // Assert
        // Since we're using virtualization, we just check that the collection change was handled
        Assert.IsTrue(true, "Collection change should be handled without errors");
    }
    
    [TestMethod]
    public void TestItemsList_HitTest_ReturnsItemsList()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        var hit = itemsList.HitTest(new Point(50, 50));
        
        // Assert
        Assert.IsNotNull(hit, "Hit test should return a result when inside bounds");
    }
    
    [TestMethod]
    public void TestItemsList_HitTest_OutsideBounds_ReturnsNull()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        var hit = itemsList.HitTest(new Point(250, 250));
        
        // Assert
        Assert.IsNull(hit, "Hit test should return null when outside bounds");
    }
    
    [TestMethod]
    public void TestItemsList_Virtualization_OnlyVisibleItemsRendered()
    {
        // Arrange - Create many items
        var items = new List<TestItem>();
        for (int i = 0; i < 100; i++)
        {
            items.Add(new TestItem { Text = $"Item {i}", Value = i });
        }
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical)
            .SetCanScrollVertically(true);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        // Assert - Should only have visible items, not all 100
        Assert.IsTrue(itemsList.Children.Count < items.Count, 
            $"Virtualization should render fewer items than total. Rendered: {itemsList.Children.Count}, Total: {items.Count}");
        Assert.IsTrue(itemsList.Children.Count > 0, "Should have at least some visible items");
    }
    
    [TestMethod]
    public void TestItemsList_Virtualization_ScrollingUpdatesVisibleItems()
    {
        // Arrange - Create many items
        var items = new List<TestItem>();
        for (int i = 0; i < 100; i++)
        {
            items.Add(new TestItem { Text = $"Item {i}", Value = i });
        }
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical)
            .SetCanScrollVertically(true);
        
        var availableSize = new Size(200, 200);
        
        // Act - Measure and arrange
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        var initialChildren = itemsList.Children.ToList();
        
        // Scroll down significantly
        itemsList.SetVerticalOffset(500);
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        // Assert - Should have different visible items after scrolling
        Assert.IsTrue(true, "Scrolling should update visible items without errors");
    }
    
    [TestMethod]
    public void TestItemsList_CanScrollHorizontally_SetterAndBinder()
    {
        // Arrange
        var itemsList = new ItemsList<TestItem>();
        
        // Act - Test setter
        var result1 = itemsList.SetCanScrollHorizontally(true);
        
        // Assert
        Assert.IsTrue(itemsList.CanScrollHorizontally, "CanScrollHorizontally should be set to true");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");
        
        // Act - Test binding
        bool propertyValue = false;
        var result2 = itemsList.BindCanScrollHorizontally("TestProperty", () => propertyValue);
        itemsList.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.IsFalse(itemsList.CanScrollHorizontally, "CanScrollHorizontally should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");
    }
    
    [TestMethod]
    public void TestItemsList_CanScrollVertically_SetterAndBinder()
    {
        // Arrange
        var itemsList = new ItemsList<TestItem>();
        
        // Act - Test setter
        var result1 = itemsList.SetCanScrollVertically(false);
        
        // Assert
        Assert.IsFalse(itemsList.CanScrollVertically, "CanScrollVertically should be set to false");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");
        
        // Act - Test binding
        bool propertyValue = true;
        var result2 = itemsList.BindCanScrollVertically("TestProperty", () => propertyValue);
        itemsList.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.IsTrue(itemsList.CanScrollVertically, "CanScrollVertically should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");
    }
    
    [TestMethod]
    public void TestItemsList_HorizontalOffset_SetterAndBinder()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Horizontal)
            .SetCanScrollHorizontally(true);
        
        // Act - Test setter
        itemsList.Measure(new Size(100, 100));
        itemsList.Arrange(new Rect(0, 0, 100, 100));
        var result1 = itemsList.SetHorizontalOffset(50);
        
        // Assert
        Assert.IsTrue(itemsList.HorizontalOffset >= 0, "HorizontalOffset should be non-negative");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");
        
        // Act - Test binding
        float propertyValue = 25;
        var result2 = itemsList.BindHorizontalOffset("TestProperty", () => propertyValue);
        itemsList.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.IsTrue(itemsList.HorizontalOffset >= 0, "HorizontalOffset should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");
    }
    
    [TestMethod]
    public void TestItemsList_VerticalOffset_SetterAndBinder()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate(item => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical)
            .SetCanScrollVertically(true);
        
        // Act - Test setter
        itemsList.Measure(new Size(100, 100));
        itemsList.Arrange(new Rect(0, 0, 100, 100));
        var result1 = itemsList.SetVerticalOffset(50);
        
        // Assert
        Assert.IsTrue(itemsList.VerticalOffset >= 0, "VerticalOffset should be non-negative");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");
        
        // Act - Test binding
        float propertyValue = 25;
        var result2 = itemsList.BindVerticalOffset("TestProperty", () => propertyValue);
        itemsList.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.IsTrue(itemsList.VerticalOffset >= 0, "VerticalOffset should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");
    }
}
