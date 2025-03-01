﻿using PlusUi.core;

namespace UiPlus.core.Tests;

/// <summary>
/// This class tests the grid column and row layouting engine
/// </summary>
[TestClass]
public sealed class GridTests
{
    [TestMethod]
    public void TestGridSelfSizing_WithDefaultColumns_ReturnsCorrectSize()
    {
        //Arrange
        var grid = new Grid()
            .AddColumn(Column.Auto)
            .AddRow(Row.Auto)
            .AddChild(new Solid().SetDesiredSize(new Size(50, 30)))
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);
        var availableSize = new Size(100, 100);

        //Act
        grid.Measure(availableSize);
        grid.Arrange(new Rect(0, 0, 100, 100));

        //Assert
        Assert.AreEqual(50, grid.ElementSize.Width);
        Assert.AreEqual(30, grid.ElementSize.Height);
    }

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
            .AddBoundColumn(() => dynamicColumnWidth)
            .AddColumn(Column.Auto)
            .AddBoundRow(() => dynamicRowHeight)
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
            .AddBoundColumn(() => dynamicWidth)  // Dynamic width column
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
    public void TestGridWithMargin_MeasuresAndArrangesCorrectly()
    {
        //Arrange
        var grid = new Grid()
            .AddColumn(Column.Auto)
            .AddRow(Row.Auto)
            .AddChild(new Solid().SetDesiredSize(new(50, 30)))
            .SetMargin(new(10, 15, 20, 25)) // Left, Top, Right, Bottom
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        var availableSize = new Size(200, 200);

        //Act
        grid.Measure(availableSize);
        grid.Arrange(new Rect(0, 0, 200, 200));

        //Assert
        Assert.AreEqual(50 + 10 + 20, grid.ElementSize.Width);  // Content + left margin + right margin
        Assert.AreEqual(30 + 15 + 25, grid.ElementSize.Height); // Content + top margin + bottom margin
        Assert.AreEqual(10, grid.Position.X);  // Left margin is the starting position
        Assert.AreEqual(15, grid.Position.Y);  // Top margin is the starting position
    }

    [TestMethod]
    public void TestGridChildWithMargin_PositionsCorrectly()
    {
        //Arrange
        var child = new Solid()
            .SetDesiredSize(new(40, 20))
            .SetMargin(new(5, 10, 15, 20)); // Left, Top, Right, Bottom

        var grid = new Grid()
            .AddColumn(Column.Absolute, 100)
            .AddRow(Row.Absolute, 100)
            .AddChild(child)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 100, 100));

        //Assert
        Assert.AreEqual(5, child.Position.X);  // Left margin
        Assert.AreEqual(10, child.Position.Y); // Top margin
        Assert.AreEqual(40, child.ElementSize.Width);  // Original width
        Assert.AreEqual(20, child.ElementSize.Height); // Original height
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
    public void TestGridAndChildWithMargins_NestedPositioning()
    {
        //Arrange
        var child = new Solid()
            .SetDesiredSize(new(40, 30))
            .SetMargin(new(5, 10, 15, 20));

        var grid = new Grid()
            .AddColumn(Column.Auto)
            .AddRow(Row.Auto)
            .AddChild(child)
            .SetMargin(new(8, 12, 16, 20))
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .SetVerticalAlignment(VerticalAlignment.Top);

        //Act
        grid.Measure(new Size(200, 200));
        grid.Arrange(new Rect(0, 0, 200, 200));

        //Assert
        // Grid position is its own margin
        Assert.AreEqual(8, grid.Position.X);
        Assert.AreEqual(12, grid.Position.Y);

        // Child position is relative to grid + its own margin
        Assert.AreEqual(5, child.Position.X);  // Left margin within grid
        Assert.AreEqual(10, child.Position.Y); // Top margin within grid

        // Grid size includes content + child margins + grid margins
        Assert.AreEqual(40 + 5 + 15 + 8 + 16, grid.ElementSize.Width);  // Child + child margins + grid margins
        Assert.AreEqual(30 + 10 + 20 + 12 + 20, grid.ElementSize.Height);
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
}
