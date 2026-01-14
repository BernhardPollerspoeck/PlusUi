using PlusUi.core;

namespace PlusUi.core.Tests;

/// <summary>
/// This class tests the grid column and row layouting engine
/// </summary>
[TestClass]
public sealed class GridTests
{
    [TestMethod]
    public void TestGridColumnSizing_WithMixedColumns_ReturnsCorrectWidths()
    {
        //Arrange
        var grid = new Grid()
            .AddColumn(Column.Absolute, 30)      // Fixed 30px
            .AddColumn(Column.Star)              // Takes remaining space (1*)
            .AddColumn(Column.Star, 2)           // Takes twice as much remaining space (2*)
            .AddRow(Row.Auto);
        var availableSize = new Size(160, 100);  // 160px - 30px = 130px for Star columns
                                                 // 1* = 130px / 3 = 43.33px, 2* = 86.67px

        //Act
        grid.Measure(availableSize);
        grid.Arrange(new Rect(0, 0, 160, 100));

        //Assert
        Assert.AreEqual(160, grid.ElementSize.Width);
    }
    [TestMethod]
    public void TestGridRowSizing_WithMixedRows_ReturnsCorrectHeights()
    {
        //Arrange
        var grid = new Grid()
            .AddColumn(Column.Auto)
            .AddRow(Row.Absolute, 40)    // Fixed 40px
            .AddRow(Row.Star)            // Takes remaining space (1*)
            .AddRow(Row.Star, 2);        // Takes twice as much remaining space (2*)
        var availableSize = new Size(100, 180);  // 180px - 40px = 140px for Star rows
                                                 // 1* = 140px / 3 = 46.67px, 2* = 93.33px

        //Act
        grid.Measure(availableSize);
        grid.Arrange(new Rect(0, 0, 100, 180));

        //Assert
        Assert.AreEqual(180, grid.ElementSize.Height);
    }
    [TestMethod]
    public void TestGridItemPositioning_WithMultipleItems_CorrectPositions()
    {
        //Arrange
        var item1 = new Solid().SetDesiredSize(new Size(30, 30));
        var item2 = new Solid().SetDesiredSize(new Size(30, 30));
        var item3 = new Solid().SetDesiredSize(new Size(30, 30));

        var grid = new Grid()
            .AddColumn(Column.Absolute, 50)
            .AddColumn(Column.Absolute, 50)
            .AddRow(Row.Absolute, 40)
            .AddRow(Row.Absolute, 40)
            .AddChild(item1, 0, 0)           // Top-left
            .AddChild(item2, 0, 1)           // Top-right
            .AddChild(item3, 1, 0)           // Bottom-left
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 100, 80));

        //Assert
        Assert.AreEqual(0, item1.Position.X);
        Assert.AreEqual(0, item1.Position.Y);

        Assert.AreEqual(50, item2.Position.X);
        Assert.AreEqual(0, item2.Position.Y);

        Assert.AreEqual(0, item3.Position.X);
        Assert.AreEqual(40, item3.Position.Y);
    }
    [TestMethod]
    public void TestGridItemSizing_WithSpans_CorrectSizing()
    {
        //Arrange
        var itemNormal = new Solid();
        var itemColSpan = new Solid();
        var itemRowSpan = new Solid();
        var itemBothSpan = new Solid();

        var grid = new Grid()
            .AddColumn(Column.Absolute, 50)
            .AddColumn(Column.Absolute, 50)
            .AddColumn(Column.Absolute, 50)
            .AddRow(Row.Absolute, 40)
            .AddRow(Row.Absolute, 40)
            .AddRow(Row.Absolute, 40)
            .AddChild(itemNormal, 0, 0)                  // Normal item
            .AddChild(itemColSpan, 0, 1, 1, 2)           // Spans 2 columns
            .AddChild(itemRowSpan, 1, 0, 2, 1)           // Spans 2 rows
            .AddChild(itemBothSpan, 1, 1, 2, 2)         // Spans 2 rows and 2 columns
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 150, 120));

        //Assert
        Assert.AreEqual(50, itemNormal.ElementSize.Width);
        Assert.AreEqual(40, itemNormal.ElementSize.Height);

        Assert.AreEqual(100, itemColSpan.ElementSize.Width);  // 50px + 50px
        Assert.AreEqual(40, itemColSpan.ElementSize.Height);

        Assert.AreEqual(50, itemRowSpan.ElementSize.Width);
        Assert.AreEqual(80, itemRowSpan.ElementSize.Height);  // 40px + 40px

        Assert.AreEqual(100, itemBothSpan.ElementSize.Width); // 50px + 50px
        Assert.AreEqual(80, itemBothSpan.ElementSize.Height); // 40px + 40px
    }
    [TestMethod]
    public void TestGridAutoSizing_WithChildContent_AdjustsCorrectly()
    {
        //Arrange
        var item1 = new Solid().SetDesiredSize(new Size(70, 30));
        var item2 = new Solid().SetDesiredSize(new Size(50, 50));

        var grid = new Grid()
            .AddColumn(Column.Auto)
            .AddColumn(Column.Auto)
            .AddRow(Row.Auto)
            .AddChild(item1, 0, 0)
            .AddChild(item2, 0, 1)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 200, 200));

        //Assert
        Assert.AreEqual(120, grid.ElementSize.Width);  // 70px + 50px
        Assert.AreEqual(50, grid.ElementSize.Height);  // Max height of row
    }
    [TestMethod]
    public void TestGridBoundSizing_ReturnsCorrectSize()
    {
        //Arrange
        var dynamicRowHeight = 60;
        var dynamicColumnWidth = 80;

        var grid = new Grid()
            .AddBoundColumn(nameof(dynamicColumnWidth), () => dynamicColumnWidth)
            .AddColumn(Column.Auto)
            .AddBoundRow(nameof(dynamicRowHeight), () => dynamicRowHeight)
            .AddRow(Row.Auto)
            .AddChild(new Solid().SetDesiredSize(new Size(30, 30)), 0, 0)
            .AddChild(new Solid().SetDesiredSize(new Size(50, 40)), 1, 1)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 200, 200));

        //Assert
        Assert.AreEqual(130, grid.ElementSize.Width);  // 80px + 50px
        Assert.AreEqual(100, grid.ElementSize.Height); // 60px + 40px
    }
    [TestMethod]
    public void TestGridChildPositioning_WithVaryingCellSizes_CorrectPositions()
    {
        //Arrange
        var item1 = new Solid().SetDesiredSize(new Size(20, 20));
        var item2 = new Solid().SetDesiredSize(new Size(20, 20));
        var item3 = new Solid().SetDesiredSize(new Size(20, 20));
        var item4 = new Solid().SetDesiredSize(new Size(20, 20));

        var grid = new Grid()
            .AddColumn(Column.Absolute, 40)    // 40px fixed
            .AddColumn(Column.Absolute, 60)    // 60px fixed
            .AddRow(Row.Absolute, 30)          // 30px fixed
            .AddRow(Row.Absolute, 50)          // 50px fixed
            .AddChild(item1, 0, 0)             // Top-left
            .AddChild(item2, 0, 1)             // Top-right
            .AddChild(item3, 1, 0)             // Bottom-left
            .AddChild(item4, 1, 1)             // Bottom-right
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 100, 80));

        //Assert
        Assert.AreEqual(0, item1.Position.X);
        Assert.AreEqual(0, item1.Position.Y);

        Assert.AreEqual(40, item2.Position.X);
        Assert.AreEqual(0, item2.Position.Y);

        Assert.AreEqual(0, item3.Position.X);
        Assert.AreEqual(30, item3.Position.Y);

        Assert.AreEqual(40, item4.Position.X);
        Assert.AreEqual(30, item4.Position.Y);
    }
    [TestMethod]
    public void TestGridChildPositioning_WithStarColumns_CorrectPositions()
    {
        //Arrange
        var item1 = new Solid().SetDesiredSize(new Size(20, 20));
        var item2 = new Solid().SetDesiredSize(new Size(20, 20));
        var item3 = new Solid().SetDesiredSize(new Size(20, 20));

        var grid = new Grid()
            .AddColumn(Column.Star, 1)         // 1* of available space
            .AddColumn(Column.Star, 2)         // 2* of available space
            .AddRow(Row.Absolute, 40)
            .AddChild(item1, 0, 0)             // Left column
            .AddChild(item2, 0, 1)             // Right column
            .AddChild(item3, 0, 1)             // Also right column
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(300, 100));
        grid.Arrange(new Rect(0, 0, 300, 40)); // Width: 300px (1* = 100px, 2* = 200px)

        //Assert
        Assert.AreEqual(0, item1.Position.X);
        Assert.AreEqual(0, item1.Position.Y);

        Assert.AreEqual(100, item2.Position.X);
        Assert.AreEqual(0, item2.Position.Y);

        Assert.AreEqual(100, item3.Position.X);
        Assert.AreEqual(0, item3.Position.Y);
    }
    [TestMethod]
    public void TestGridChildPositioning_WithMixedColumns_CorrectPositions()
    {
        //Arrange
        var item1 = new Solid().SetDesiredSize(new Size(20, 20));
        var item2 = new Solid().SetDesiredSize(new Size(20, 20));
        var item3 = new Solid().SetDesiredSize(new Size(60, 20)); // Auto-sized item

        var grid = new Grid()
            .AddColumn(Column.Absolute, 50)
            .AddColumn(Column.Auto)            // Auto-sized based on content
            .AddColumn(Column.Star)            // Remainder of space
            .AddRow(Row.Absolute, 40)
            .AddChild(item1, 0, 0)             // First column
            .AddChild(item2, 0, 2)             // Third column
            .AddChild(item3, 0, 1)             // Second column (auto-sized)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 100));
        grid.Arrange(new Rect(0, 0, 200, 40));

        //Assert
        Assert.AreEqual(0, item1.Position.X);
        Assert.AreEqual(0, item1.Position.Y);

        Assert.AreEqual(50, item3.Position.X); // Auto column starts after first column
        Assert.AreEqual(0, item3.Position.Y);

        Assert.AreEqual(110, item2.Position.X); // Star column starts after auto column (50px + 60px)
        Assert.AreEqual(0, item2.Position.Y);
    }
    [TestMethod]
    public void TestGridChildPositioning_WithBoundSizes_CorrectPositions()
    {
        //Arrange
        var dynamicWidth = 70;
        var item1 = new Solid().SetDesiredSize(new Size(20, 20));
        var item2 = new Solid().SetDesiredSize(new Size(20, 20));

        var grid = new Grid()
            .AddBoundColumn(nameof(dynamicWidth), () => dynamicWidth)  // Dynamic width column
            .AddColumn(Column.Star)              // Remaining space
            .AddRow(Row.Absolute, 40)
            .AddChild(item1, 0, 0)               // First column
            .AddChild(item2, 0, 1)               // Second column
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 100));
        grid.Arrange(new Rect(0, 0, 200, 40));

        //Assert
        Assert.AreEqual(0, item1.Position.X);
        Assert.AreEqual(0, item1.Position.Y);

        Assert.AreEqual(70, item2.Position.X); // Star column starts after bound width column
        Assert.AreEqual(0, item2.Position.Y);

        // Change the dynamic width and verify positioning updates
        dynamicWidth = 100;
        grid.InvalidateMeasure();
        grid.Measure(new Size(200, 100));
        grid.Arrange(new Rect(0, 0, 200, 40));

        Assert.AreEqual(100, item2.Position.X); // Position should update with bound width
        Assert.AreEqual(0, item2.Position.Y);
    }
    [TestMethod]
    public void TestGridChildPositioning_WithSpanning_CorrectPositions()
    {
        //Arrange
        var item1 = new Solid(); // Regular item
        var item2 = new Solid(); // Spans 2 columns
        var item3 = new Solid(); // Spans 2 rows
        var item4 = new Solid(); // Regular item in bottom-right

        var grid = new Grid()
            .AddColumn(Column.Absolute, 40)
            .AddColumn(Column.Absolute, 40)
            .AddRow(Row.Absolute, 30)
            .AddRow(Row.Absolute, 30)
            .AddChild(item1, 0, 0)               // Top-left cell
            .AddChild(item2, 0, 0, 1, 2)         // Top row, spans both columns
            .AddChild(item3, 0, 1, 2, 1)         // Right column, spans both rows
            .AddChild(item4, 1, 1)               // Bottom-right cell
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 80, 60));

        //Assert
        // item1 is under item2 (due to adding order), but position should still be correct
        Assert.AreEqual(0, item1.Position.X);
        Assert.AreEqual(0, item1.Position.Y);

        // item2 spans both columns (total width: 80px)
        Assert.AreEqual(0, item2.Position.X);
        Assert.AreEqual(0, item2.Position.Y);
        Assert.AreEqual(80, item2.ElementSize.Width);
        Assert.AreEqual(30, item2.ElementSize.Height);

        // item3 spans both rows (total height: 60px)
        Assert.AreEqual(40, item3.Position.X);
        Assert.AreEqual(0, item3.Position.Y);
        Assert.AreEqual(40, item3.ElementSize.Width);
        Assert.AreEqual(60, item3.ElementSize.Height);

        // item4 is in the bottom-right cell
        Assert.AreEqual(40, item4.Position.X);
        Assert.AreEqual(30, item4.Position.Y);
    }
    [TestMethod]
    public void TestGridWithMultipleChildrenWithMargins_PositionsCorrectly()
    {
        //Arrange
        var child1 = new Solid()
            .SetDesiredSize(new(30, 30))
            .SetMargin(new(5, 5, 5, 5));

        var child2 = new Solid()
            .SetDesiredSize(new(30, 30))
            .SetMargin(new(10, 10, 10, 10));

        var grid = new Grid()
            .AddColumn(Column.Auto)
            .AddColumn(Column.Auto)
            .AddRow(Row.Auto)
            .AddChild(child1, 0, 0)
            .AddChild(child2, 0, 1)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 200, 200));

        //Assert
        // First column width includes child1 + margins
        Assert.AreEqual(5, child1.Position.X);  // Left margin
        Assert.AreEqual(5, child1.Position.Y);  // Top margin

        // Second column starts after first column (30 + 5 + 5 = 40)
        // Then add the left margin of child2 (10)
        Assert.AreEqual(40 + 10, child2.Position.X);
        Assert.AreEqual(10, child2.Position.Y);  // Top margin

        // Total grid size accounts for all content + margins
        Assert.AreEqual(40 + 50, grid.ElementSize.Width);  // (30+5+5) + (30+10+10)
        Assert.AreEqual(50, grid.ElementSize.Height);      // Max(30+5+5, 30+10+10)
    }
    [TestMethod]
    public void TestGridRowColumnSpanWithMargins_SizesCorrectly()
    {
        //Arrange
        var spannedChild = new Solid()
            .SetDesiredSize(new(50, 40))
            .SetMargin(new(5, 8, 10, 12));

        var grid = new Grid()
            .AddColumn(Column.Absolute, 60)
            .AddColumn(Column.Absolute, 60)
            .AddRow(Row.Absolute, 50)
            .AddRow(Row.Absolute, 50)
            .AddChild(spannedChild, 0, 0, 2, 2)  // Span 2 rows, 2 columns
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 120, 100));

        //Assert
        // Child position includes its margins
        Assert.AreEqual(5, spannedChild.Position.X);
        Assert.AreEqual(8, spannedChild.Position.Y);

        // Element should get the full size of the spanned area minus margins
        Assert.AreEqual(50, spannedChild.ElementSize.Width);  // Original width
        Assert.AreEqual(40, spannedChild.ElementSize.Height); // Original height

        // Child's total allocated space is 120x100, minus margins
        Assert.AreEqual(120, grid.ElementSize.Width);
        Assert.AreEqual(100, grid.ElementSize.Height);
    }
    [TestMethod]
    public void TestEmptyGrid_MeasuresAndArrangesCorrectly()
    {
        //Arrange
        var grid = new Grid()
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);
        var availableSize = new Size(100, 100);

        //Act
        grid.Measure(availableSize);
        grid.Arrange(new Rect(0, 0, 100, 100));

        //Assert
        Assert.AreEqual(0, grid.ElementSize.Width);
        Assert.AreEqual(0, grid.ElementSize.Height);
    }
    [TestMethod]
    public void TestAutoColumns_WithDifferentSizedChildren_UsesLargest()
    {
        //Arrange
        var smallChild = new Solid().SetDesiredSize(new Size(30, 20));
        var largeChild = new Solid().SetDesiredSize(new Size(70, 20));

        var grid = new Grid()
            .AddColumn(Column.Auto)
            .AddRow(Row.Auto)
            .AddChild(smallChild, 0, 0)
            .AddChild(largeChild, 0, 0) // Two children in the same cell
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);
        var availableSize = new Size(200, 100);

        //Act
        grid.Measure(availableSize);
        grid.Arrange(new Rect(0, 0, 200, 100));

        //Assert
        // Column should size to the largest child
        Assert.AreEqual(70, grid.ElementSize.Width);
        Assert.AreEqual(20, grid.ElementSize.Height);
    }
    [TestMethod]
    public void TestGridWithOutOfBoundsIndices_HandlesGracefully()
    {
        //Arrange
        var item1 = new Solid().SetDesiredSize(new Size(50, 30));
        var item2 = new Solid().SetDesiredSize(new Size(40, 20));
        var item3 = new Solid().SetDesiredSize(new Size(30, 10));

        var grid = new Grid()
            .AddColumn(Column.Auto)
            .AddRow(Row.Auto)
            .AddChild(item1, 0, 0)             // Normal placement
            .AddChild(item2, 5, 0)             // Out of bounds row
            .AddChild(item3, 0, 5)             // Out of bounds column
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);
        var availableSize = new Size(200, 100);

        //Act
        grid.Measure(availableSize);
        grid.Arrange(new Rect(0, 0, 200, 100));

        //Assert
        // Grid should still handle the normal item correctly
        Assert.AreEqual(50, grid.ElementSize.Width);
        Assert.AreEqual(30, grid.ElementSize.Height);

        // Out of bounds items should still be positioned somewhere reasonable
        Assert.IsGreaterThanOrEqualTo(0, item2.Position.Y);
        Assert.IsGreaterThanOrEqualTo(0, item3.Position.X);
    }
    [TestMethod]
    public void TestGridWithZeroSizeColumns_HandlesGracefully()
    {
        //Arrange
        var child1 = new Solid().SetDesiredSize(new Size(30, 20));
        var child2 = new Solid().SetDesiredSize(new Size(40, 20));

        var grid = new Grid()
            .AddColumn(Column.Absolute, 0)     // Zero-width column
            .AddColumn(Column.Absolute, 50)
            .AddRow(Row.Absolute, 30)
            .AddChild(child1, 0, 0)            // In zero-width column
            .AddChild(child2, 0, 1)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 100));
        grid.Arrange(new Rect(0, 0, 50, 30));

        //Assert
        Assert.AreEqual(0, child1.Position.X);
        Assert.AreEqual(0, child1.Position.Y);
        Assert.AreEqual(0, child1.ElementSize.Width);  // Zero width
        Assert.AreEqual(20, child1.ElementSize.Height);

        Assert.AreEqual(0, child2.Position.X);
        Assert.AreEqual(0, child2.Position.Y);
        Assert.AreEqual(40, child2.ElementSize.Width);
        Assert.AreEqual(20, child2.ElementSize.Height);
    }
    [TestMethod]
    public void TestGridWithLargeChildInAutoColumn_SizesCorrectly()
    {
        //Arrange
        var largeChild = new Solid().SetDesiredSize(new Size(200, 100));

        var grid = new Grid()
            .AddColumn(Column.Auto)        // Auto column with large child
            .AddColumn(Column.Star)        // Star column should get minimal space
            .AddRow(Row.Auto)
            .AddChild(largeChild, 0, 0)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(250, 150));  // Available size is larger than child
        grid.Arrange(new Rect(0, 0, 250, 150));

        //Assert
        // Auto column should size to child's width
        Assert.AreEqual(200, largeChild.ElementSize.Width);
        Assert.AreEqual(100, largeChild.ElementSize.Height);

        // Grid should use available space, with star column getting minimal space
        Assert.IsGreaterThanOrEqualTo(200, grid.ElementSize.Width);
    }
    [TestMethod]
    public void TestRemoveChildFromGrid_LayoutUpdates()
    {
        //Arrange
        var child1 = new Solid().SetDesiredSize(new Size(50, 30));
        var child2 = new Solid().SetDesiredSize(new Size(70, 40));

        var grid = (Grid)(new Grid()
            .AddColumn(Column.Auto)
            .AddRow(Row.Auto)
            .AddChild(child1, 0, 0)
            .AddChild(child2, 0, 0)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top));

        // Initial measure
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 200, 200));

        // With both children, should size to largest (child2)
        Assert.AreEqual(70, grid.ElementSize.Width);
        Assert.AreEqual(40, grid.ElementSize.Height);

        //Act
        grid.RemoveChild(child2);
        grid.InvalidateMeasure();
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 200, 200));

        //Assert
        // After removal, should size to remaining child
        Assert.AreEqual(50, grid.ElementSize.Width);
        Assert.AreEqual(30, grid.ElementSize.Height);
    }
    [TestMethod]
    public void TestGridRowColumnResizingWithEvents_UpdatesLayout()
    {
        //Arrange
        var dynamicWidth = 50f;
        var dynamicHeight = 40f;

        var grid = new Grid()
            .AddBoundColumn(nameof(dynamicWidth), () => dynamicWidth)
            .AddBoundRow(nameof(dynamicHeight), () => dynamicHeight)
            .AddChild(new Solid())
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        // Initial measure
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 200, 200));

        Assert.AreEqual(50, grid.ElementSize.Width);
        Assert.AreEqual(40, grid.ElementSize.Height);

        //Act
        dynamicWidth = 100f;
        dynamicHeight = 80f;

        // This should trigger size change events internally
        grid.InvalidateMeasure();
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 200, 200));

        //Assert
        Assert.AreEqual(100, grid.ElementSize.Width);
        Assert.AreEqual(80, grid.ElementSize.Height);
    }

    [TestMethod]
    public void TestGridInBorder_WithAbsoluteStarAutoColumns_ShouldNotExceedAvailableWidth()
    {
        // Arrange - simulates the ScreenshotsView item layout
        // Grid has: 80px absolute | Star (flex) | Auto (buttons ~108px)
        var thumbnail = new Solid().SetDesiredSize(new Size(80, 50));
        var info = new Solid().SetDesiredSize(new Size(100, 50));  // Info VStack
        var buttons = new Solid().SetDesiredSize(new Size(108, 32)); // 3 buttons * 32px + spacing

        var grid = new Grid()
            .AddColumn(Column.Absolute, 80)
            .AddColumn(Column.Star, 1)
            .AddColumn(Column.Auto)
            .AddChild(thumbnail, row: 0, column: 0)
            .AddChild(info, row: 0, column: 1)
            .AddChild(buttons, row: 0, column: 2);

        var border = new Border()
            .SetDesiredHeight(66)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
            .AddChild(grid);

        var containerWidth = 400f;

        // Act
        border.Measure(new Size(containerWidth, 100));
        border.Arrange(new Rect(0, 0, containerWidth, 100));

        // Assert - Grid should not exceed the container width
        Assert.IsTrue(grid.ElementSize.Width <= containerWidth,
            $"Grid width ({grid.ElementSize.Width}) should not exceed container width ({containerWidth})");
        Assert.AreEqual(containerWidth, border.ElementSize.Width,
            $"Border width should match container width");
    }

    [TestMethod]
    public void TestGridInItemsList_WithMixedColumns_ShouldRespectContainerWidth()
    {
        // Arrange - simulates ItemsList measuring items
        var thumbnail = new Solid().SetDesiredSize(new Size(80, 50));
        var info = new Solid().SetDesiredSize(new Size(100, 50));
        var buttons = new Solid().SetDesiredSize(new Size(108, 32));

        var grid = new Grid()
            .AddColumn(Column.Absolute, 80)
            .AddColumn(Column.Star, 1)
            .AddColumn(Column.Auto)
            .AddChild(thumbnail, row: 0, column: 0)
            .AddChild(info, row: 0, column: 1)
            .AddChild(buttons, row: 0, column: 2);

        var border = new Border()
            .SetDesiredHeight(66)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
            .AddChild(grid);

        // Act - ItemsList measures with float.MaxValue (the problem!)
        border.Measure(new Size(float.MaxValue, float.MaxValue));

        // This is likely what ItemsList does
        var containerWidth = 400f;
        border.Arrange(new Rect(0, 0, containerWidth, 100));

        // Assert - After arrange, grid should respect container bounds
        Assert.IsTrue(grid.ElementSize.Width <= containerWidth,
            $"Grid width ({grid.ElementSize.Width}) should not exceed container width ({containerWidth}) even when measured with MaxValue");
    }

    [TestMethod]
    public void TestGridInRealItemsList_ShouldRespectContainerWidth()
    {
        // Arrange - real ItemsList scenario
        var containerWidth = 400f;

        var itemsList = new ItemsList<int>()
            .SetItemsSource([1])
            .SetItemTemplate((item, index) =>
            {
                var grid = new Grid()
                    .AddColumn(Column.Absolute, 80)
                    .AddColumn(Column.Star, 1)
                    .AddColumn(Column.Auto)
                    .AddChild(new Solid().SetDesiredSize(new Size(80, 50)), row: 0, column: 0)
                    .AddChild(new Solid().SetDesiredSize(new Size(100, 50)), row: 0, column: 1)
                    .AddChild(new Solid().SetDesiredSize(new Size(108, 32)), row: 0, column: 2);

                return new Border()
                    .SetDesiredHeight(66)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .AddChild(grid);
            });

        // Act
        itemsList.Measure(new Size(containerWidth, 500));
        itemsList.Arrange(new Rect(0, 0, containerWidth, 500));

        // Assert - The item should not exceed container width
        var firstItem = itemsList.Children.FirstOrDefault();
        Assert.IsNotNull(firstItem, "ItemsList should have at least one child");
        Assert.IsTrue(firstItem.ElementSize.Width <= containerWidth,
            $"Item width ({firstItem.ElementSize.Width}) should not exceed container width ({containerWidth})");
    }

    [TestMethod]
    public void TestGridInItemsListInVStack_ShouldRespectContainerWidth()
    {
        // Arrange - simulates ScreenshotsView: VStack > ItemsList > Border > Grid
        var containerWidth = 400f;
        Grid? capturedGrid = null;

        var itemsList = new ItemsList<int>()
            .SetItemsSource([1])
            .SetItemTemplate((item, index) =>
            {
                capturedGrid = new Grid()
                    .AddColumn(Column.Absolute, 80)
                    .AddColumn(Column.Star, 1)
                    .AddColumn(Column.Auto)
                    .AddChild(new Solid().SetDesiredSize(new Size(80, 50)), row: 0, column: 0)
                    .AddChild(new Solid().SetDesiredSize(new Size(100, 50)), row: 0, column: 1)
                    .AddChild(new Solid().SetDesiredSize(new Size(108, 32)), row: 0, column: 2);

                return new Border()
                    .SetDesiredHeight(66)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .AddChild(capturedGrid);
            });

        var vstack = new VStack()
            .AddChild(new Label().SetText("Header"))
            .AddChild(itemsList);

        // Act
        vstack.Measure(new Size(containerWidth, 500));
        vstack.Arrange(new Rect(0, 0, containerWidth, 500));

        // Assert
        Assert.IsNotNull(capturedGrid, "Grid should have been created");
        var firstItem = itemsList.Children.FirstOrDefault();
        Assert.IsNotNull(firstItem, "ItemsList should have at least one child");

        Assert.IsTrue(capturedGrid.ElementSize.Width <= containerWidth,
            $"Grid width ({capturedGrid.ElementSize.Width}) should not exceed container width ({containerWidth})");
        Assert.IsTrue(firstItem.ElementSize.Width <= containerWidth,
            $"Item width ({firstItem.ElementSize.Width}) should not exceed container width ({containerWidth})");
    }

    [TestMethod]
    public void TestGridInItemsListInTabControl_ShouldRespectContainerWidth()
    {
        // Arrange - simulates real scenario: TabControl > VStack > ItemsList > Border > Grid
        var containerWidth = 400f;
        Grid? capturedGrid = null;
        Border? capturedBorder = null;

        var itemsList = new ItemsList<int>()
            .SetItemsSource([1])
            .SetItemTemplate((item, index) =>
            {
                capturedGrid = new Grid()
                    .AddColumn(Column.Absolute, 80)
                    .AddColumn(Column.Star, 1)
                    .AddColumn(Column.Auto)
                    .AddChild(new Solid().SetDesiredSize(new Size(80, 50)), row: 0, column: 0)
                    .AddChild(new Solid().SetDesiredSize(new Size(100, 50)), row: 0, column: 1)
                    .AddChild(new Solid().SetDesiredSize(new Size(108, 32)), row: 0, column: 2);

                capturedBorder = new Border()
                    .SetDesiredHeight(66)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .AddChild(capturedGrid);

                return capturedBorder;
            });

        var vstack = new VStack()
            .AddChild(new Label().SetText("Screenshots (1)"))
            .AddChild(itemsList);

        var tabControl = new TabControl()
            .AddTab(new TabItem { Header = "Screenshots", Content = vstack });

        // Act
        tabControl.Measure(new Size(containerWidth, 500));
        tabControl.Arrange(new Rect(0, 0, containerWidth, 500));

        // Assert
        Assert.IsNotNull(capturedGrid, "Grid should have been created");
        Assert.IsNotNull(capturedBorder, "Border should have been created");

        // Check each level of nesting
        Assert.IsTrue(itemsList.ElementSize.Width <= containerWidth,
            $"ItemsList width ({itemsList.ElementSize.Width}) should not exceed container width ({containerWidth})");
        Assert.IsTrue(capturedBorder.ElementSize.Width <= containerWidth,
            $"Border width ({capturedBorder.ElementSize.Width}) should not exceed container width ({containerWidth})");
        Assert.IsTrue(capturedGrid.ElementSize.Width <= containerWidth,
            $"Grid width ({capturedGrid.ElementSize.Width}) should not exceed container width ({containerWidth})");
    }

    [TestMethod]
    public void TestGridInFullLayoutHierarchy_ShouldRespectContainerWidth()
    {
        // Arrange - exact simulation: Grid(Star) > TabControl > VStack > ItemsList > Border > Grid
        var containerWidth = 400f;
        Grid? capturedGrid = null;
        Border? capturedBorder = null;
        ItemsList<int>? capturedItemsList = null;

        capturedItemsList = new ItemsList<int>()
            .SetItemsSource([1])
            .SetItemTemplate((item, index) =>
            {
                capturedGrid = new Grid()
                    .AddColumn(Column.Absolute, 80)
                    .AddColumn(Column.Star, 1)
                    .AddColumn(Column.Auto)
                    .AddChild(new Solid().SetDesiredSize(new Size(80, 50)), row: 0, column: 0)
                    .AddChild(new Solid().SetDesiredSize(new Size(100, 50)), row: 0, column: 1)
                    .AddChild(new Solid().SetDesiredSize(new Size(108, 32)), row: 0, column: 2);

                capturedBorder = new Border()
                    .SetDesiredHeight(66)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .AddChild(capturedGrid);

                return capturedBorder;
            });

        var vstack = new VStack()
            .AddChild(new Label().SetText("Screenshots (1)"))
            .AddChild(capturedItemsList);

        var tabControl = new TabControl()
            .AddTab(new TabItem { Header = "Screenshots", Content = vstack });

        // Outer grid like in AppContentView
        var outerGrid = new Grid()
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddChild(tabControl, row: 0, column: 0);

        // BuildContent like real app
        outerGrid.BuildContent();

        // Act
        outerGrid.Measure(new Size(containerWidth, 500));
        outerGrid.Arrange(new Rect(0, 0, containerWidth, 500));

        // Assert
        Assert.IsNotNull(capturedGrid, "Grid should have been created");
        Assert.IsNotNull(capturedBorder, "Border should have been created");

        // Debug output
        System.Diagnostics.Debug.WriteLine($"OuterGrid: {outerGrid.ElementSize.Width}");
        System.Diagnostics.Debug.WriteLine($"TabControl: {tabControl.ElementSize.Width}");
        System.Diagnostics.Debug.WriteLine($"VStack: {vstack.ElementSize.Width}");
        System.Diagnostics.Debug.WriteLine($"ItemsList: {capturedItemsList.ElementSize.Width}");
        System.Diagnostics.Debug.WriteLine($"Border: {capturedBorder.ElementSize.Width}");
        System.Diagnostics.Debug.WriteLine($"Grid: {capturedGrid.ElementSize.Width}");

        // Check each level of nesting
        Assert.IsTrue(capturedItemsList.ElementSize.Width <= containerWidth,
            $"ItemsList width ({capturedItemsList.ElementSize.Width}) should not exceed container width ({containerWidth})");
        Assert.IsTrue(capturedBorder.ElementSize.Width <= containerWidth,
            $"Border width ({capturedBorder.ElementSize.Width}) should not exceed container width ({containerWidth})");
        Assert.IsTrue(capturedGrid.ElementSize.Width <= containerWidth,
            $"Grid width ({capturedGrid.ElementSize.Width}) should not exceed container width ({containerWidth})");
    }

    [TestMethod]
    public void TestExactDebugServerLayout_ShouldRespectContainerWidth()
    {
        // EXACT replica of DebugServer layout:
        // MainPage: Grid(Star/Star) > TabControl(AppTabs)
        //   AppContentView: Grid(Star/Star) > TabControl(FeatureTabs)
        //     ScreenshotsView: VStack > [Header, ItemsList(Margin 8)]
        //       Item: Border(Margin 4, Height 66, HStretch) > Grid(Margin 8, Cols: 80/Star/Auto)
        //         Col0: Thumbnail 80x50
        //         Col1: VStack(Margin 12,0,0,0) with labels
        //         Col2: HStack(Spacing 4) with 3 buttons 32x32

        var windowWidth = 400f;
        var windowHeight = 600f;

        Grid? itemGrid = null;
        Border? itemBorder = null;
        ItemsList<int>? itemsList = null;

        // ScreenshotsView content (simulated UserControl.Build)
        itemsList = new ItemsList<int>();
        itemsList.SetItemsSource([1]);
        itemsList.SetMargin(new Margin(8));
        itemsList.SetItemTemplate((item, index) =>
            {
                // Buttons: 3x 32px + 2x 4px spacing = 104px
                var buttons = new HStack()
                    .AddChild(new Solid().SetDesiredSize(new Size(32, 32)))
                    .AddChild(new Solid().SetDesiredSize(new Size(32, 32)))
                    .AddChild(new Solid().SetDesiredSize(new Size(32, 32)))
                    .SetSpacing(4)
                    .SetVerticalAlignment(VerticalAlignment.Center);

                // Info VStack
                var info = new VStack()
                    .AddChild(new Label().SetText("Full Page"))
                    .AddChild(new Label().SetText("1200x800"))
                    .AddChild(new Label().SetText("18:54:32"))
                    .SetSpacing(2)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .SetMargin(new Margin(12, 0, 0, 0));

                // Thumbnail
                var thumbnail = new Solid()
                    .SetDesiredSize(new Size(80, 50))
                    .SetVerticalAlignment(VerticalAlignment.Center);

                itemGrid = new Grid()
                    .AddColumn(Column.Absolute, 80)
                    .AddColumn(Column.Star, 1)
                    .AddColumn(Column.Auto)
                    .SetMargin(new Margin(8))
                    .AddChild(thumbnail, row: 0, column: 0)
                    .AddChild(info, row: 0, column: 1)
                    .AddChild(buttons, row: 0, column: 2);

                itemBorder = new Border()
                    .SetMargin(new Margin(4))
                    .SetDesiredHeight(66)
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .AddChild(itemGrid);

                return itemBorder;
            });

        // ScreenshotsView: VStack with header and list
        var screenshotsContent = new VStack()
            .AddChild(
                new HStack()
                    .AddChild(new Label().SetText("Screenshots"))
                    .AddChild(new Label().SetText("(1)"))
                    .SetSpacing(8)
                    .SetMargin(new Margin(12, 8)))
            .AddChild(itemsList);

        // AppContentView: Grid(Star/Star) > TabControl
        var featureTabControl = new TabControl()
            .AddTab(new TabItem { Header = "Screenshots", Content = screenshotsContent });

        var appContentGrid = new Grid()
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddChild(featureTabControl, row: 0, column: 0);

        // MainPage: Grid(Star/Star) > TabControl
        var appTabControl = new TabControl()
            .AddTab(new TabItem { Header = "App-123", Content = appContentGrid });

        var mainPageGrid = new Grid()
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddChild(appTabControl, row: 0, column: 0);

        // Build like real app
        mainPageGrid.BuildContent();

        // Act - measure and arrange
        mainPageGrid.Measure(new Size(windowWidth, windowHeight));
        mainPageGrid.Arrange(new Rect(0, 0, windowWidth, windowHeight));

        // Assert
        Assert.IsNotNull(itemGrid, "Item Grid should have been created");
        Assert.IsNotNull(itemBorder, "Item Border should have been created");
        Assert.IsNotNull(itemsList, "ItemsList should exist");

        // Calculate expected max width for item
        // Window: 400
        // ItemsList margin: 8 left + 8 right = 16 -> available: 384
        // Border margin: 4 left + 4 right = 8 -> available: 376
        // Grid margin: 8 left + 8 right = 16 -> available for columns: 360
        // Columns: 80 (abs) + Star + Auto(~104)
        // Star should be: 360 - 80 - 104 = 176
        // Total grid content: 80 + 176 + 104 = 360
        // Grid with margin: 360 + 16 = 376
        // Border with margin: 376 + 8 = 384

        var expectedMaxItemWidth = windowWidth - 16; // 384 (ItemsList margin)

        Console.WriteLine($"=== Layout Debug ===");
        Console.WriteLine($"Window: {windowWidth}");
        Console.WriteLine($"MainPageGrid: {mainPageGrid.ElementSize.Width}");
        Console.WriteLine($"AppTabControl: {appTabControl.ElementSize.Width}");
        Console.WriteLine($"AppContentGrid: {appContentGrid.ElementSize.Width}");
        Console.WriteLine($"FeatureTabControl: {featureTabControl.ElementSize.Width}");
        Console.WriteLine($"ScreenshotsVStack: {screenshotsContent.ElementSize.Width}");
        Console.WriteLine($"ItemsList: {itemsList.ElementSize.Width}");
        Console.WriteLine($"ItemBorder: {itemBorder.ElementSize.Width}");
        Console.WriteLine($"ItemGrid: {itemGrid.ElementSize.Width}");

        Assert.IsTrue(itemBorder.ElementSize.Width <= expectedMaxItemWidth,
            $"Item Border width ({itemBorder.ElementSize.Width}) should not exceed {expectedMaxItemWidth} (window {windowWidth} - ItemsList margin 16)");
        Assert.IsTrue(itemGrid.ElementSize.Width <= expectedMaxItemWidth - 8,
            $"Item Grid width ({itemGrid.ElementSize.Width}) should not exceed {expectedMaxItemWidth - 8} (available after Border margin)");
    }

    [TestMethod]
    public void TestImageVerticalStretch_ShouldCalculateWidthFromAspectRatio()
    {
        // Arrange: Container with fixed height, Image with VerticalStretch + HorizontalLeft
        // Image aspect ratio 1.5 (width:height = 3:2, e.g. 1200x800)
        var containerWidth = 400f;
        var containerHeight = 100f;
        var imageAspect = 1.5f; // 1200/800 = 1.5

        // Create a Border that stretches vertically
        var border = new Border()
            .SetVerticalAlignment(VerticalAlignment.Stretch)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetStrokeThickness(1);

        // Simulate an element with aspect ratio (using a simple UiElement for testing)
        var imageElement = new TestImageElement(imageAspect)
            .SetVerticalAlignment(VerticalAlignment.Stretch)
            .SetHorizontalAlignment(HorizontalAlignment.Left);

        border.AddChild(imageElement);

        var container = new Grid()
            .AddRow(Row.Absolute, (int)containerHeight)
            .AddColumn(Column.Absolute, (int)containerWidth)
            .AddChild(border, row: 0, column: 0);

        // Act
        container.Measure(new Size(containerWidth, containerHeight));
        container.Arrange(new Rect(0, 0, containerWidth, containerHeight));

        // Debug output
        Console.WriteLine($"=== Image Vertical Stretch Test ===");
        Console.WriteLine($"Container: {containerWidth}x{containerHeight}");
        Console.WriteLine($"Border size: {border.ElementSize.Width}x{border.ElementSize.Height}");
        Console.WriteLine($"Border position: {border.Position.X}, {border.Position.Y}");
        Console.WriteLine($"Image size: {imageElement.ElementSize.Width}x{imageElement.ElementSize.Height}");
        Console.WriteLine($"Image position: {imageElement.Position.X}, {imageElement.Position.Y}");
        Console.WriteLine($"Expected image width: {containerHeight * imageAspect} (height {containerHeight} * aspect {imageAspect})");

        // Assert
        // Border should stretch to full height
        Assert.AreEqual(containerHeight, border.ElementSize.Height, 0.1f,
            "Border should stretch to container height");

        // Image should stretch to full height
        Assert.AreEqual(containerHeight - 2, imageElement.ElementSize.Height, 0.1f,
            "Image should stretch to border content height (minus stroke)");

        // Image width should be calculated from aspect ratio
        var expectedImageWidth = (containerHeight - 2) * imageAspect;
        Assert.AreEqual(expectedImageWidth, imageElement.ElementSize.Width, 0.1f,
            $"Image width should be height * aspect ratio = {expectedImageWidth}");

        // Border width should match image width (plus stroke)
        Assert.AreEqual(expectedImageWidth + 2, border.ElementSize.Width, 0.1f,
            "Border width should wrap image width");
    }

    [TestMethod]
    public void TestVerticalCentering_InGridRow_ShouldBePerfectlyCentered()
    {
        // Arrange: Replicate ScreenshotsView layout
        // Item: Border(height 116, margin 4) > Grid(margin 8) > [Image(VStretch), Info(VCenter), Buttons(VCenter)]

        var itemHeight = 116f;
        var borderStroke = 1f; // Border default StrokeThickness
        var gridMargin = 8f;
        // Available height: 116 (item) - 2 (stroke) - 16 (margin) = 98px
        var availableHeightForContent = itemHeight - (borderStroke * 2) - (gridMargin * 2);

        // Create elements similar to ScreenshotsView
        var imageElement = new TestImageElement(1.5f) // 1200x800 aspect
            .SetVerticalAlignment(VerticalAlignment.Stretch)
            .SetHorizontalAlignment(HorizontalAlignment.Left);

        var imageBorder = new Border()
            .SetStrokeThickness(1)
            .SetVerticalAlignment(VerticalAlignment.Stretch)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .AddChild(imageElement);

        // Info VStack with 3 labels (simulated with fixed height)
        // Matches real ScreenshotsView: SetMargin(new Margin(12, 0, 0, 0))
        var infoStack = new VStack()
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetSpacing(2)
            .SetMargin(new Margin(12, 0, 0, 0)) // Left margin like real code
            .AddChild(new Label().SetText("Full Page").SetTextSize(13))
            .AddChild(new Label().SetText("1200x800").SetTextSize(11))
            .AddChild(new Label().SetText("19:32:10").SetTextSize(11));

        // Buttons HStack (3 buttons, 32x32 each)
        var buttonsStack = new HStack()
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetSpacing(4)
            .AddChild(new Border().SetDesiredWidth(32).SetDesiredHeight(32))
            .AddChild(new Border().SetDesiredWidth(32).SetDesiredHeight(32))
            .AddChild(new Border().SetDesiredWidth(32).SetDesiredHeight(32));

        // No explicit Row definition - Grid creates implicit row (like real ScreenshotsView)
        var contentGrid = new Grid()
            .AddColumn(Column.Star, 1)
            .AddColumn(Column.Star, 1)
            .AddColumn(Column.Auto)
            .SetMargin(new Margin(gridMargin))
            .AddChild(imageBorder, row: 0, column: 0)
            .AddChild(infoStack, row: 0, column: 1)
            .AddChild(buttonsStack, row: 0, column: 2);

        // Matches real ScreenshotsView: Margin(4), no explicit stroke (default 1.0)
        var itemBorderMargin = 4f;
        var itemBorder = new Border()
            .SetMargin(new Margin(itemBorderMargin))
            .SetDesiredHeight(itemHeight)
            .SetDesiredWidth(600)
            .AddChild(contentGrid);

        // Act - Arrange with space for margin
        var totalHeight = itemHeight + itemBorderMargin * 2; // 116 + 8 = 124
        itemBorder.Measure(new Size(600, totalHeight));
        itemBorder.Arrange(new Rect(0, 0, 600, totalHeight));

        // Debug output
        Console.WriteLine($"=== Vertical Centering Test ===");
        Console.WriteLine($"Item height: {itemHeight}");
        Console.WriteLine($"Item border margin: {itemBorderMargin}");
        Console.WriteLine($"Border stroke: {borderStroke}");
        Console.WriteLine($"Grid margin: {gridMargin}");
        Console.WriteLine($"Available for content: {availableHeightForContent}");
        Console.WriteLine();
        Console.WriteLine($"ItemBorder position: Y={itemBorder.Position.Y}");
        Console.WriteLine($"ItemBorder size: {itemBorder.ElementSize.Width}x{itemBorder.ElementSize.Height}");
        Console.WriteLine();
        Console.WriteLine($"ContentGrid position: Y={contentGrid.Position.Y}");
        Console.WriteLine($"ContentGrid size: {contentGrid.ElementSize.Width}x{contentGrid.ElementSize.Height}");
        Console.WriteLine();
        Console.WriteLine($"ImageBorder position: Y={imageBorder.Position.Y}");
        Console.WriteLine($"ImageBorder size: {imageBorder.ElementSize.Width}x{imageBorder.ElementSize.Height}");
        Console.WriteLine($"Image position: Y={imageElement.Position.Y}");
        Console.WriteLine($"Image size: {imageElement.ElementSize.Width}x{imageElement.ElementSize.Height}");
        Console.WriteLine();
        Console.WriteLine($"InfoStack position: Y={infoStack.Position.Y}");
        Console.WriteLine($"InfoStack size: {infoStack.ElementSize.Width}x{infoStack.ElementSize.Height}");
        Console.WriteLine();
        Console.WriteLine($"ButtonsStack position: Y={buttonsStack.Position.Y}");
        Console.WriteLine($"ButtonsStack size: {buttonsStack.ElementSize.Width}x{buttonsStack.ElementSize.Height}");

        // Calculate expected centered positions
        // Grid starts at: itemMargin + borderStroke + gridMargin offset from top
        var contentAreaTop = itemBorderMargin + borderStroke + gridMargin;
        var contentAreaHeight = availableHeightForContent;
        var contentAreaCenterY = contentAreaTop + (contentAreaHeight / 2);

        // InfoStack should be centered
        var infoExpectedY = contentAreaTop + (contentAreaHeight - infoStack.ElementSize.Height) / 2;
        var infoActualCenterY = infoStack.Position.Y + (infoStack.ElementSize.Height / 2);

        // ButtonsStack should be centered
        var buttonsExpectedY = contentAreaTop + (contentAreaHeight - buttonsStack.ElementSize.Height) / 2;
        var buttonsActualCenterY = buttonsStack.Position.Y + (buttonsStack.ElementSize.Height / 2);

        Console.WriteLine();
        Console.WriteLine($"Content area: top={contentAreaTop}, height={contentAreaHeight}, centerY={contentAreaCenterY}");
        Console.WriteLine($"InfoStack: expectedY={infoExpectedY}, actualY={infoStack.Position.Y}, actualCenterY={infoActualCenterY}");
        Console.WriteLine($"ButtonsStack: expectedY={buttonsExpectedY}, actualY={buttonsStack.Position.Y}, actualCenterY={buttonsActualCenterY}");

        // Assert: Elements with VCenter should be perfectly centered
        Assert.AreEqual(contentAreaCenterY, infoActualCenterY, 1f,
            $"InfoStack center ({infoActualCenterY}) should be at content area center ({contentAreaCenterY})");

        Assert.AreEqual(contentAreaCenterY, buttonsActualCenterY, 1f,
            $"ButtonsStack center ({buttonsActualCenterY}) should be at content area center ({contentAreaCenterY})");
    }
}

/// <summary>
/// Test helper that simulates an Image with aspect ratio behavior.
/// When stretched vertically, calculates width from aspect ratio.
/// </summary>
internal class TestImageElement : UiElement
{
    private readonly float _aspectRatio;

    public TestImageElement(float aspectRatio)
    {
        _aspectRatio = aspectRatio;
    }

    protected internal override bool IsFocusable => false;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Image;

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var desiredW = DesiredSize?.Width ?? -1;
        var desiredH = DesiredSize?.Height ?? -1;

        // Both dimensions set
        if (desiredW > 0 && desiredH > 0)
            return new Size(desiredW, desiredH);

        // Only height set - calculate width from aspect ratio
        if (desiredH > 0)
            return new Size(desiredH * _aspectRatio, desiredH);

        // Only width set - calculate height from aspect ratio
        if (desiredW > 0)
            return new Size(desiredW, desiredW / _aspectRatio);

        // Stretching vertically - use available height to calculate width
        if (VerticalAlignment == VerticalAlignment.Stretch && availableSize.Height < 1e6f)
            return new Size(availableSize.Height * _aspectRatio, availableSize.Height);

        // Stretching horizontally - use available width to calculate height
        if (HorizontalAlignment == HorizontalAlignment.Stretch && availableSize.Width < 1e6f)
            return new Size(availableSize.Width, availableSize.Width / _aspectRatio);

        // Default: return some nominal size
        return new Size(100 * _aspectRatio, 100);
    }
}
