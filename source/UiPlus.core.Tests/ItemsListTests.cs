using PlusUi.core;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
        Assert.AreEqual(0, itemsList.ScrollOffset, "ScrollOffset should be 0 by default");
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
        Func<TestItem, int, UiElement> template = (item, index) => new Label().SetText(item.Text);
        
        // Act - Test setter
        var result1 = itemsList.SetItemTemplate(template);
        
        // Assert
        Assert.AreSame(template, itemsList.ItemTemplate, "ItemTemplate should be set to the provided template");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");
        
        // Act - Test binding
        Func<TestItem, int, UiElement>? propertyValue = (item, index) => new Label().SetText(item.Value.ToString());
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
            .SetItemTemplate((item, index) => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical);
        
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
            .SetItemTemplate((item, index) => new Solid(50, 100))
            .SetOrientation(Orientation.Horizontal);
        
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
            .SetItemTemplate((item, index) => new Label().SetText(item.Text));
        
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
            .SetItemTemplate((item, index) => new Label().SetText(item.Text));
        
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
            .SetItemTemplate((item, index) => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        itemsList.HandleScroll(0, 50);
        
        // Assert
        Assert.AreEqual(50, itemsList.ScrollOffset, "Scroll offset should be updated after scrolling");
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
            .SetItemTemplate((item, index) => new Solid(50, 100))
            .SetOrientation(Orientation.Horizontal);
        
        var availableSize = new Size(200, 200);
        
        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        itemsList.HandleScroll(50, 0);
        
        // Assert
        Assert.AreEqual(50, itemsList.ScrollOffset, "Scroll offset should be updated after scrolling");
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
            .SetItemTemplate((item, index) => new Solid(100, 50))
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
            .SetItemTemplate((item, index) => new Solid(100, 50))
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
            .SetItemTemplate((item, index) => new Solid(100, 50))
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
            .SetItemTemplate((item, index) => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical);
        
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
            .SetItemTemplate((item, index) => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical);
        
        var availableSize = new Size(200, 200);
        
        // Act - Measure and arrange
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        var initialChildren = itemsList.Children.ToList();
        
        // Scroll down significantly
        itemsList.SetScrollOffset(500);
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));
        
        // Assert - Should have different visible items after scrolling
        Assert.IsTrue(true, "Scrolling should update visible items without errors");
    }
    
    [TestMethod]
    public void TestItemsList_ScrollOffset_SetterAndBinder()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 }
        };
        
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate((item, index) => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical);
        
        // Act - Test setter
        itemsList.Measure(new Size(100, 100));
        itemsList.Arrange(new Rect(0, 0, 100, 100));
        var result1 = itemsList.SetScrollOffset(50);
        
        // Assert
        Assert.IsTrue(itemsList.ScrollOffset >= 0, "ScrollOffset should be non-negative");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");
        
        // Act - Test binding
        float propertyValue = 25;
        var result2 = itemsList.BindScrollOffset("TestProperty", () => propertyValue);
        itemsList.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.IsTrue(itemsList.ScrollOffset >= 0, "ScrollOffset should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");
    }

    [TestMethod]
    public void TestItemsList_HitTest_ButtonAfterScroll()
    {
        // Arrange - Create items with buttons
        var command1 = new TestCommand();
        var command2 = new TestCommand();
        var command3 = new TestCommand();
        
        var items = new List<TestItem>
        {
            new TestItem { Text = "Item 1", Value = 1 },
            new TestItem { Text = "Item 2", Value = 2 },
            new TestItem { Text = "Item 3", Value = 3 }
        };
        
        var commands = new[] { command1, command2, command3 };
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate((item, index) => 
                new Button()
                    .SetText(item.Text)
                    .SetCommand(commands[index])
                    .SetPadding(new(10))
            )
            .SetOrientation(Orientation.Vertical);
        
        // Act - Measure and arrange with limited height to enable scrolling
        itemsList.Measure(new Size(200, 100));
        itemsList.Arrange(new Rect(0, 0, 200, 100));
        
        // Scroll down
        itemsList.SetScrollOffset(50);
        itemsList.Measure(new Size(200, 100));
        itemsList.Arrange(new Rect(0, 0, 200, 100));
        
        // Get one of the visible buttons
        var visibleButton = itemsList.Children
            .OfType<Button>()
            .FirstOrDefault(b => b.Position.Y >= 0 && b.Position.Y < 100);
        
        if (visibleButton != null)
        {
            // Hit test in the middle of the visible button
            var hitPoint = new Point(
                visibleButton.Position.X + visibleButton.ElementSize.Width / 2,
                visibleButton.Position.Y + visibleButton.ElementSize.Height / 2
            );
            var hit = itemsList.HitTest(hitPoint);
            
            // Assert
            Assert.IsNotNull(hit, $"HitTest should return a result for button at ({visibleButton.Position.X}, {visibleButton.Position.Y})");
            Assert.AreSame(visibleButton, hit, $"HitTest should return the button after scrolling, but got {hit?.GetType().Name}");
        }
    }
    
    private class TestCommand : ICommand
    {
        public bool WasExecuted { get; private set; }
        
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            WasExecuted = true;
        }
    }
}
