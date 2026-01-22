using PlusUi.core;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PlusUi.core.Tests;

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
        var result2 = itemsList.BindOrientation(() => propertyValue);
        itemsList.UpdateBindings("propertyValue");

        // Verify binding
        Assert.AreEqual(Orientation.Vertical, itemsList.Orientation, "Orientation should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");

        // Change the bound property and update bindings
        propertyValue = Orientation.Horizontal;
        itemsList.UpdateBindings("propertyValue");

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
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 }
        };

        // Act - Test setter
        var result1 = itemsList.SetItemsSource(items);

        // Assert
        Assert.AreSame(items, itemsList.ItemsSource, "ItemsSource should be set to the provided collection");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");

        // Act - Test binding
        IEnumerable<TestItem>? propertyValue = [new() { Text = "Item 3", Value = 3 }];
        var result2 = itemsList.BindItemsSource(() => propertyValue);
        itemsList.UpdateBindings("propertyValue");

        // Verify binding
        Assert.AreSame(propertyValue, itemsList.ItemsSource, "ItemsSource should be bound to the property value");
        Assert.AreSame(itemsList, result2, "Method should return the ItemsList for chaining");
    }

    [TestMethod]
    public void TestItemsList_ItemTemplate_SetterAndBinder()
    {
        // Arrange
        var itemsList = new ItemsList<TestItem>();
        static UiElement template(TestItem item, int index)
        {
            return new Label().SetText(item.Text);
        }

        // Act - Test setter
        var result1 = itemsList.SetItemTemplate(template);

        // Assert
        Assert.AreSame(template, itemsList.ItemTemplate, "ItemTemplate should be set to the provided template");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");

        // Act - Test binding
        Func<TestItem, int, UiElement> propertyValue = (item, index) =>
            new Label().SetText(item.Value.ToString());

        var result2 = itemsList.BindItemTemplate(() => propertyValue);
        itemsList.UpdateBindings("propertyValue");

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
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 },
            new() { Text = "Item 3", Value = 3 }
        };

        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate((item, index) => new Solid(100, 50))
            .SetOrientation(Orientation.Vertical);

        var availableSize = new Size(200, 200);

        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));

        // Assert - Vertical list: width = widest child, height = min of content and available
        Assert.AreEqual(100, itemsList.ElementSize.Width, "ItemsList width should match widest child");
        Assert.AreEqual(150, itemsList.ElementSize.Height, "ItemsList height should be content height (3 items * 50)");
        Assert.IsNotEmpty(itemsList.Children, "ItemsList should have children");
    }

    [TestMethod]
    public void TestItemsList_HorizontalOrientation_MeasureAndArrange()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 },
            new() { Text = "Item 3", Value = 3 }
        };

        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items)
            .SetItemTemplate((item, index) => new Solid(50, 100))
            .SetOrientation(Orientation.Horizontal);

        var availableSize = new Size(200, 200);

        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));

        // Assert - Horizontal list: width = min of content and available, height = tallest child
        Assert.AreEqual(150, itemsList.ElementSize.Width, "ItemsList width should be content width (3 items * 50)");
        Assert.AreEqual(100, itemsList.ElementSize.Height, "ItemsList height should match tallest child");
        Assert.IsNotEmpty(itemsList.Children, "ItemsList should have children");
    }

    [TestMethod]
    public void TestItemsList_EmptyItemsSource_NoChildren()
    {
        // Arrange
        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource([])
            .SetItemTemplate((item, index) => new Label().SetText(item.Text));

        var availableSize = new Size(200, 200);

        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));

        // Assert
        Assert.IsEmpty(itemsList.Children, "ItemsList with empty source should have no children");
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
        Assert.IsEmpty(itemsList.Children, "ItemsList with null source should have no children");
    }

    [TestMethod]
    public void TestItemsList_NullItemTemplate_NoChildren()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 }
        };

        var itemsList = new ItemsList<TestItem>()
            .SetItemsSource(items);

        var availableSize = new Size(200, 200);

        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 200));

        // Assert
        Assert.IsEmpty(itemsList.Children, "ItemsList with null template should have no children");
    }

    [TestMethod]
    public void TestItemsList_VerticalScrolling_UpdatesOffset()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 },
            new() { Text = "Item 3", Value = 3 },
            new() { Text = "Item 4", Value = 4 },
            new() { Text = "Item 5", Value = 5 }
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
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 },
            new() { Text = "Item 3", Value = 3 },
            new() { Text = "Item 4", Value = 4 },
            new() { Text = "Item 5", Value = 5 }
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
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 }
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
    }

    [TestMethod]
    public void TestItemsList_HitTest_ReturnsItemsList()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 }
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
            new() { Text = "Item 1", Value = 1 }
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
        for (var i = 0; i < 100; i++)
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
        Assert.IsLessThan(items.Count, itemsList.Children.Count, $"Virtualization should render fewer items than total. Rendered: {itemsList.Children.Count}, Total: {items.Count}");
        Assert.IsNotEmpty(itemsList.Children, "Should have at least some visible items");
    }


    [TestMethod]
    public void TestItemsList_ScrollOffset_SetterAndBinder()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 }
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
        Assert.IsGreaterThanOrEqualTo(0, itemsList.ScrollOffset, "ScrollOffset should be non-negative");
        Assert.AreSame(itemsList, result1, "Method should return the ItemsList for chaining");

        // Act - Test binding
        float propertyValue = 25;
        var result2 = itemsList.BindScrollOffset(() => propertyValue);
        itemsList.UpdateBindings("propertyValue");

        // Verify binding
        Assert.IsGreaterThanOrEqualTo(0, itemsList.ScrollOffset, "ScrollOffset should be bound to the property value");
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
            new() { Text = "Item 1", Value = 1 },
            new() { Text = "Item 2", Value = 2 },
            new() { Text = "Item 3", Value = 3 }
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
            .FirstOrDefault(b => b.Position.Y is >= 0 and < 100);

        if (visibleButton != null)
        {
            // Hit test in the middle of the visible button
            var hitPoint = new Point(
                visibleButton.Position.X + (visibleButton.ElementSize.Width / 2),
                visibleButton.Position.Y + (visibleButton.ElementSize.Height / 2)
            );
            var hit = itemsList.HitTest(hitPoint);

            // Assert
            Assert.IsNotNull(hit, $"HitTest should return a result for button at ({visibleButton.Position.X}, {visibleButton.Position.Y})");
            Assert.AreSame(visibleButton, hit, $"HitTest should return the button after scrolling, but got {hit?.GetType().Name}");
        }
    }

    [TestMethod]
    public void TestItemsList_StretchButtons_ShouldRespectMargins()
    {
        // Arrange - Simulate the MainPage scenario with stretch buttons
        var items = new List<string> { "Short", "ActivityIndicator", "Mid" };

        var itemsList = new ItemsList<string>()
            .SetItemsSource(items)
            .SetItemTemplate((name, _) =>
                new Button()
                    .SetText(name)
                    .SetMargin(new Margin(4))
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch))
            .SetOrientation(Orientation.Vertical);

        var availableSize = new Size(500, 500);

        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 500, 500));

        // Get the children (buttons)
        var buttons = itemsList.Children.OfType<Button>().ToList();
        Assert.IsGreaterThanOrEqualTo(buttons.Count, 1, "Should have at least one button");

        // Debug info
        var debugInfo = $"ItemsList: Position=({itemsList.Position.X},{itemsList.Position.Y}), " +
                        $"Size=({itemsList.ElementSize.Width},{itemsList.ElementSize.Height})\n";
        foreach (var btn in buttons)
        {
            debugInfo += $"Button '{btn.Text}': Position=({btn.Position.X},{btn.Position.Y}), " +
                         $"Size=({btn.ElementSize.Width},{btn.ElementSize.Height}), " +
                         $"Margin=({btn.Margin.Left},{btn.Margin.Top},{btn.Margin.Right},{btn.Margin.Bottom}), " +
                         $"Margin.Horizontal={btn.Margin.Horizontal}\n";
        }

        // The button should NOT extend to the full ItemsList width
        // There should be margin space on the right
        foreach (var button in buttons)
        {
            var buttonRightEdge = button.Position.X + button.ElementSize.Width;
            var itemsListRightEdge = itemsList.Position.X + itemsList.ElementSize.Width;
            var rightMargin = itemsListRightEdge - buttonRightEdge;

            Assert.AreEqual(4, rightMargin, 0.1f,
                $"Button '{button.Text}' should have 4px right margin. " +
                $"Button ends at {buttonRightEdge}, ItemsList ends at {itemsListRightEdge}, " +
                $"actual right margin: {rightMargin}\n{debugInfo}");
        }

        // Also verify left margin
        foreach (var button in buttons)
        {
            var leftMargin = button.Position.X - itemsList.Position.X;

            Assert.AreEqual(4, leftMargin, 0.1f,
                $"Button '{button.Text}' should have 4px left margin. " +
                $"Button starts at {button.Position.X}, ItemsList starts at {itemsList.Position.X}, " +
                $"actual left margin: {leftMargin}\n{debugInfo}");
        }
    }

    private class TestCommand : ICommand
    {
        public bool WasExecuted { get; private set; }

        public event EventHandler? CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            WasExecuted = true;
        }
    }

    /// <summary>
    /// Test that proves the layout bug: When ItemsList is inside an HStack,
    /// the second child (right side) should be visible and have space.
    /// Currently ItemsList takes the full available width, leaving no room for siblings.
    /// </summary>
    [TestMethod]
    public void TestItemsList_InHStack_ShouldNotTakeFullWidth()
    {
        // Arrange - Simulate MainPage layout:
        // HStack
        //   ├── Border (with ItemsList inside)
        //   └── VStack (content on right side)
        var items = new[] { "Button", "Label", "Entry" };

        var itemsList = new ItemsList<string>()
            .SetItemsSource(items)
            .SetItemTemplate((name, _) => new Button().SetText(name));

        var leftSide = new Border().AddChild(
            new VStack(
                new Label().SetText("Controls"),
                itemsList
            )
        );

        var rightSide = new VStack(
            new Label().SetText("Welcome to PlusUi")
        )
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center);

        var hstack = new HStack(leftSide, rightSide);

        // Act - Measure and arrange with typical screen size
        var availableSize = new Size(800, 600);
        hstack.Measure(availableSize);
        hstack.Arrange(new Rect(0, 0, 800, 600));

        // Assert - Both children should have reasonable sizes
        // The left side (Border with ItemsList) should NOT take the full 800px width
        Assert.IsLessThan(availableSize.Width, leftSide.ElementSize.Width,
            $"Left side (Border with ItemsList) should not take full width. Actual width: {leftSide.ElementSize.Width}");

        // The right side should have positive width
        Assert.IsGreaterThan(0, rightSide.ElementSize.Width,
            $"Right side (VStack) should have positive width. Actual width: {rightSide.ElementSize.Width}");

        // The right side should be positioned at or after the left side (not overlapping)
        Assert.IsGreaterThanOrEqualTo(leftSide.Position.X + leftSide.ElementSize.Width, rightSide.Position.X,
            $"Right side should be positioned after left side. Left ends at: {leftSide.Position.X + leftSide.ElementSize.Width}, Right starts at: {rightSide.Position.X}");

        // Both children together should fit within the HStack
        var totalChildrenWidth = leftSide.ElementSize.Width + rightSide.ElementSize.Width;
        Assert.IsLessThanOrEqualTo(availableSize.Width, totalChildrenWidth,
            $"Total children width ({totalChildrenWidth}) should not exceed available width ({availableSize.Width})");
    }

    /// <summary>
    /// Test that proves layout bug: When a VStack contains a child with HorizontalAlignment.Stretch,
    /// the VStack should not expand to full available width - it should use its natural content width.
    /// </summary>
    [TestMethod]
    public void TestItemsList_WithStretchAccentLine_ShouldNotExpandParent()
    {
        // Arrange - Simulate the exact MainPage header layout:
        // HStack
        //   ├── Border (Left aligned)
        //   │     └── VStack
        //   │           ├── VStack (header)
        //   │           │     ├── Label "Controls"
        //   │           │     └── Border (accent line, Stretch)
        //   │           └── ItemsList
        //   └── VStack (content on right side)
        var items = new[] { "ActivityIndicator", "Border", "Button" };

        var accentLine = new Border()
            .SetDesiredHeight(2)
            .SetBackground(PlusUiDefaults.AccentPrimary)
            .SetStrokeThickness(0)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch);

        var headerVStack = new VStack()
            .SetSpacing(0)
            .AddChild(new Label().SetText("Controls").SetMargin(new Margin(16, 12)))
            .AddChild(accentLine);

        var itemsList = new ItemsList<string>()
            .SetItemsSource(items)
            .SetItemTemplate((name, _) =>
                new Button()
                    .SetText(name)
                    .SetMargin(new Margin(4))
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch));

        var leftSide = new Border()
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetStrokeThickness(0)
            .AddChild(
                new VStack()
                    .AddChild(headerVStack)
                    .AddChild(itemsList));

        var rightSide = new VStack(
            new Label().SetText("Welcome to PlusUi")
        )
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center);

        var hstack = new HStack(leftSide, rightSide);

        // Act - Measure and arrange with typical screen size
        var availableSize = new Size(800, 600);
        hstack.Measure(availableSize);
        hstack.Arrange(new Rect(0, 0, 800, 600));

        // Debug output
        var debugInfo = $"\nLayout Debug:\n" +
                        $"HStack: Size=({hstack.ElementSize.Width}, {hstack.ElementSize.Height})\n" +
                        $"LeftSide (Border): Position=({leftSide.Position.X}, {leftSide.Position.Y}), Size=({leftSide.ElementSize.Width}, {leftSide.ElementSize.Height})\n" +
                        $"RightSide (VStack): Position=({rightSide.Position.X}, {rightSide.Position.Y}), Size=({rightSide.ElementSize.Width}, {rightSide.ElementSize.Height})\n" +
                        $"AccentLine: Size=({accentLine.ElementSize.Width}, {accentLine.ElementSize.Height})\n" +
                        $"ItemsList: Size=({itemsList.ElementSize.Width}, {itemsList.ElementSize.Height})\n";

        // Assert - The left side should NOT take the full width
        Assert.IsLessThan(availableSize.Width * 0.5f, leftSide.ElementSize.Width,
            $"Left side should not take more than half the width.{debugInfo}");

        // The right side should have positive width and be visible
        Assert.IsGreaterThan(100, rightSide.ElementSize.Width,
            $"Right side should have reasonable width (at least 100px).{debugInfo}");

        // The right side should start after the left side
        var leftSideEnd = leftSide.Position.X + leftSide.ElementSize.Width;
        Assert.IsGreaterThanOrEqualTo(leftSideEnd, rightSide.Position.X,
            $"Right side should start after left side ends.{debugInfo}");
    }

    /// <summary>
    /// Test that exactly matches the MainPage layout to prove the button width bug.
    /// In the real app, some buttons are narrow and some are stretched.
    /// </summary>
    [TestMethod]
    public void TestItemsList_InMainPageLayout_AllButtonsShouldHaveSameWidth()
    {
        // Arrange - EXACT MainPage structure
        var controls = new List<string>
        {
            "Border", "Button", "Checkbox", "ComboBox", "ContextMenu",
            "DataGrid", "DatePicker", "Entry", "Gestures", "Grid",
            "HStack", "Image", "ItemsList", "Label", "LineGraph",
            "Link", "Menu", "ProgressBar"
        };

        var itemsList = new ItemsList<string>()
            .BindItemsSource(() => controls)
            .SetItemTemplate((name, _) =>
                new Button()
                    .SetText(name)
                    .SetMargin(new Margin(4))
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch));

        // Build the EXACT MainPage left side structure
        var leftSide = new Border()
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetBackground(PlusUiDefaults.BackgroundPrimary)
            .SetStrokeThickness(0)
            .AddChild(
                new VStack()
                    .AddChild(
                        new VStack()
                            .SetSpacing(0)
                            .AddChild(
                                new Label()
                                    .SetText("Controls")
                                    .SetTextSize(PlusUiDefaults.FontSizeLarge)
                                    .SetFontWeight(FontWeight.SemiBold)
                                    .SetMargin(new Margin(16, 12)))
                            .AddChild(
                                new Border()
                                    .SetDesiredHeight(2)
                                    .SetBackground(PlusUiDefaults.AccentPrimary)
                                    .SetStrokeThickness(0)
                                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)))
                    .AddChild(itemsList));

        var rightSide = new VStack()
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .AddChild(new Label().SetText("Welcome to PlusUi"));

        var hstack = new HStack()
            .AddChild(leftSide)
            .AddChild(rightSide);

        // Act - Measure and arrange with typical screen size
        var availableSize = new Size(800, 600);
        hstack.Measure(availableSize);
        hstack.Arrange(new Rect(0, 0, 800, 600));

        // Get all buttons
        var buttons = itemsList.Children.OfType<Button>().ToList();
        Assert.IsGreaterThanOrEqualTo(buttons.Count, 2, $"Should have at least 2 visible buttons, got {buttons.Count}");

        // Build debug info
        var debugInfo = $"\nLeftSide: Size=({leftSide.ElementSize.Width}, {leftSide.ElementSize.Height})\n" +
                        $"ItemsList: Size=({itemsList.ElementSize.Width}, {itemsList.ElementSize.Height})\n" +
                        $"Buttons:\n";
        foreach (var btn in buttons)
        {
            debugInfo += $"  '{btn.Text}': Width={btn.ElementSize.Width}, HAlign={btn.HorizontalAlignment}\n";
        }

        // Assert - All buttons should have the same width (since they all have Stretch alignment)
        var firstButtonWidth = buttons[0].ElementSize.Width;
        foreach (var button in buttons)
        {
            Assert.AreEqual(firstButtonWidth, button.ElementSize.Width, 0.1f,
                $"Button '{button.Text}' width ({button.ElementSize.Width}) should match " +
                $"first button '{buttons[0].Text}' width ({firstButtonWidth}).{debugInfo}");
        }
    }

    /// <summary>
    /// Test that proves the bug: All stretch buttons in ItemsList should have the same width.
    /// Currently some buttons are narrow (natural size) and some are stretched (full width).
    /// This appears to be related to virtualization - items measured at different times get different widths.
    /// </summary>
    [TestMethod]
    public void TestItemsList_AllStretchButtonsShouldHaveSameWidth()
    {
        // Arrange - Create many items to trigger virtualization
        var items = new List<string>
        {
            "Border", "Button", "Checkbox", "ComboBox", "ContextMenu",
            "DataGrid", "DatePicker", "Entry", "Gestures", "Grid",
            "HStack", "Image", "ItemsList", "Label", "LineGraph",
            "Link", "Menu", "ProgressBar"
        };

        var itemsList = new ItemsList<string>()
            .SetItemsSource(items)
            .SetItemTemplate((name, _) =>
                new Button()
                    .SetText(name)
                    .SetMargin(new Margin(4))
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch))
            .SetOrientation(Orientation.Vertical);

        // Use a size that shows only some items (to test virtualization)
        var availableSize = new Size(200, 400);

        // Act - Initial measure/arrange
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 400));

        // Get all visible buttons
        var buttons = itemsList.Children.OfType<Button>().ToList();
        Assert.IsGreaterThanOrEqualTo(buttons.Count, 2, $"Should have at least 2 visible buttons, got {buttons.Count}");

        // Build debug info
        var debugInfo = $"\nItemsList: Size=({itemsList.ElementSize.Width}, {itemsList.ElementSize.Height})\n";
        foreach (var btn in buttons)
        {
            debugInfo += $"Button '{btn.Text}': Width={btn.ElementSize.Width}, " +
                         $"Position=({btn.Position.X}, {btn.Position.Y})\n";
        }

        // Assert - All buttons should have the same width
        var firstButtonWidth = buttons[0].ElementSize.Width;
        foreach (var button in buttons)
        {
            Assert.AreEqual(firstButtonWidth, button.ElementSize.Width, 0.1f,
                $"Button '{button.Text}' width ({button.ElementSize.Width}) should match first button width ({firstButtonWidth}).{debugInfo}");
        }
    }

    /// <summary>
    /// Test that proves the bug: When scrolling down and back up, the initially visible items
    /// should still have the correct stretched width (not their natural size).
    /// This tests the scenario where virtualization recycles items.
    /// </summary>
    [TestMethod]
    public void TestItemsList_ButtonWidthShouldBeConsistentAfterScrollingDownAndBackUp()
    {
        // Arrange - EXACT MainPage layout
        var controls = new List<string>
        {
            "Border", "Button", "Checkbox", "ComboBox", "ContextMenu",
            "DataGrid", "DatePicker", "Entry", "Gestures", "Grid",
            "HStack", "Image", "ItemsList", "Label", "LineGraph",
            "Link", "Menu", "ProgressBar"
        };

        var itemsList = new ItemsList<string>()
            .SetItemsSource(controls)
            .SetItemTemplate((name, _) =>
                new Button()
                    .SetText(name)
                    .SetMargin(new Margin(4))
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch));

        // Build the EXACT MainPage left side structure
        var leftSide = new Border()
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetBackground(PlusUiDefaults.BackgroundPrimary)
            .SetStrokeThickness(0)
            .AddChild(
                new VStack()
                    .AddChild(
                        new VStack()
                            .SetSpacing(0)
                            .AddChild(
                                new Label()
                                    .SetText("Controls")
                                    .SetTextSize(PlusUiDefaults.FontSizeLarge)
                                    .SetFontWeight(FontWeight.SemiBold)
                                    .SetMargin(new Margin(16, 12)))
                            .AddChild(
                                new Border()
                                    .SetDesiredHeight(2)
                                    .SetBackground(PlusUiDefaults.AccentPrimary)
                                    .SetStrokeThickness(0)
                                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)))
                    .AddChild(itemsList));

        var rightSide = new VStack()
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .AddChild(new Label().SetText("Welcome to PlusUi"));

        var hstack = new HStack()
            .AddChild(leftSide)
            .AddChild(rightSide);

        // Use a small viewport to force virtualization
        var availableSize = new Size(800, 300);

        // Act 1: Initial layout
        hstack.Measure(availableSize);
        hstack.Arrange(new Rect(0, 0, 800, 300));

        var initialButtons = itemsList.Children.OfType<Button>().ToList();
        Assert.IsGreaterThanOrEqualTo(initialButtons.Count, 1, "Should have at least 1 visible button initially");
        var expectedWidth = initialButtons[0].ElementSize.Width;

        // Act 2: Scroll down (to show bottom items)
        itemsList.SetScrollOffset(300);
        hstack.Measure(availableSize);
        hstack.Arrange(new Rect(0, 0, 800, 300));

        // Act 3: Scroll back up (to show top items again)
        itemsList.SetScrollOffset(0);
        hstack.Measure(availableSize);
        hstack.Arrange(new Rect(0, 0, 800, 300));

        // Get buttons after scrolling back
        var buttonsAfterScroll = itemsList.Children.OfType<Button>().ToList();
        Assert.IsGreaterThanOrEqualTo(buttonsAfterScroll.Count, 1, "Should have at least 1 visible button after scrolling");

        // Build debug info
        var debugInfo = $"\nExpected width: {expectedWidth}\n" +
                        $"ItemsList: Size=({itemsList.ElementSize.Width}, {itemsList.ElementSize.Height})\n" +
                        $"Buttons after scroll back:\n";
        foreach (var btn in buttonsAfterScroll)
        {
            debugInfo += $"  '{btn.Text}': Width={btn.ElementSize.Width}, HAlign={btn.HorizontalAlignment}\n";
        }

        // Assert - Buttons after scrolling back should still have the expected width
        foreach (var button in buttonsAfterScroll)
        {
            Assert.AreEqual(expectedWidth, button.ElementSize.Width, 0.1f,
                $"Button '{button.Text}' width after scrolling back ({button.ElementSize.Width}) " +
                $"should match expected width ({expectedWidth}).{debugInfo}");
        }
    }

    /// <summary>
    /// Test that proves the bug: After scrolling, newly visible buttons should have the same width
    /// as the initially visible buttons. Currently they may have different widths due to
    /// being measured at different times.
    /// </summary>
    [TestMethod]
    public void TestItemsList_ButtonWidthShouldBeConsistentAfterScrolling()
    {
        // Arrange
        var items = new List<string>
        {
            "Border", "Button", "Checkbox", "ComboBox", "ContextMenu",
            "DataGrid", "DatePicker", "Entry", "Gestures", "Grid",
            "HStack", "Image", "ItemsList", "Label", "LineGraph"
        };

        var itemsList = new ItemsList<string>()
            .SetItemsSource(items)
            .SetItemTemplate((name, _) =>
                new Button()
                    .SetText(name)
                    .SetMargin(new Margin(4))
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch))
            .SetOrientation(Orientation.Vertical);

        // Use small height to force virtualization
        var availableSize = new Size(200, 150);

        // Act - Initial measure/arrange
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 150));

        var initialButtons = itemsList.Children.OfType<Button>().ToList();
        Assert.IsGreaterThanOrEqualTo(initialButtons.Count, 1, "Should have at least 1 visible button initially");
        var initialButtonWidth = initialButtons[0].ElementSize.Width;

        // Scroll down to show different items
        itemsList.SetScrollOffset(200);
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 150));

        var scrolledButtons = itemsList.Children.OfType<Button>().ToList();
        Assert.IsGreaterThanOrEqualTo(scrolledButtons.Count, 1, "Should have at least 1 visible button after scrolling");

        // Build debug info
        var debugInfo = $"\nInitial button width: {initialButtonWidth}\n" +
                        $"After scrolling:\n";
        foreach (var btn in scrolledButtons)
        {
            debugInfo += $"Button '{btn.Text}': Width={btn.ElementSize.Width}\n";
        }

        // Assert - Buttons after scrolling should have same width as initial buttons
        foreach (var button in scrolledButtons)
        {
            Assert.AreEqual(initialButtonWidth, button.ElementSize.Width, 0.1f,
                $"Button '{button.Text}' width after scrolling ({button.ElementSize.Width}) " +
                $"should match initial button width ({initialButtonWidth}).{debugInfo}");
        }
    }

    /// <summary>
    /// Test that proves the bug: Scrollbar height should match the ItemsList viewport height,
    /// not some other value.
    /// </summary>
    [TestMethod]
    public void TestItemsList_ScrollbarHeightShouldMatchViewport()
    {
        // Arrange - Create items that exceed the viewport height
        var items = new List<string>();
        for (int i = 0; i < 20; i++)
        {
            items.Add($"Item {i}");
        }

        var itemsList = new ItemsList<string>()
            .SetItemsSource(items)
            .SetItemTemplate((name, _) =>
                new Button()
                    .SetText(name)
                    .SetMargin(new Margin(4)))
            .SetShowScrollbar(true)
            .SetOrientation(Orientation.Vertical);

        var availableSize = new Size(200, 300);

        // Act
        itemsList.Measure(availableSize);
        itemsList.Arrange(new Rect(0, 0, 200, 300));

        // Get the scrollbar
        var scrollbar = itemsList.Scrollbar;

        // Build debug info
        var debugInfo = $"\nItemsList: Size=({itemsList.ElementSize.Width}, {itemsList.ElementSize.Height})\n" +
                        $"Scrollbar: Position=({scrollbar.Position.X}, {scrollbar.Position.Y}), " +
                        $"Size=({scrollbar.ElementSize.Width}, {scrollbar.ElementSize.Height})\n";

        // Assert - Scrollbar should have same height as ItemsList viewport
        Assert.AreEqual(itemsList.ElementSize.Height, scrollbar.ElementSize.Height, 0.1f,
            $"Scrollbar height ({scrollbar.ElementSize.Height}) should match ItemsList height ({itemsList.ElementSize.Height}).{debugInfo}");

        // Scrollbar should be positioned at the right edge of ItemsList
        var expectedScrollbarX = itemsList.Position.X + itemsList.ElementSize.Width - scrollbar.ElementSize.Width;
        Assert.AreEqual(expectedScrollbarX, scrollbar.Position.X, 0.1f,
            $"Scrollbar X position ({scrollbar.Position.X}) should be at right edge ({expectedScrollbarX}).{debugInfo}");

        // Scrollbar should start at the same Y as ItemsList
        Assert.AreEqual(itemsList.Position.Y, scrollbar.Position.Y, 0.1f,
            $"Scrollbar Y position ({scrollbar.Position.Y}) should match ItemsList Y ({itemsList.Position.Y}).{debugInfo}");
    }
}
