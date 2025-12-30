using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public sealed class HStackWrapTests
{
    [TestMethod]
    public void HStack_Wrap_SingleRow_NoWrapping()
    {
        // Arrange: 3 items of 20px each = 60px total, fits in 100px
        var stack = new HStack(
            new Solid(20, 10),
            new Solid(20, 10),
            new Solid(20, 10)
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: All in one row
        Assert.AreEqual(60, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(20, stack.Children[1].Position.X);
        Assert.AreEqual(40, stack.Children[2].Position.X);
        // All on same Y
        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(0, stack.Children[1].Position.Y);
        Assert.AreEqual(0, stack.Children[2].Position.Y);
    }

    [TestMethod]
    public void HStack_Wrap_TwoRows()
    {
        // Arrange: 3 items of 40px each = 120px total, only 100px available
        // Should wrap: [40, 40] [40]
        var stack = new HStack(
            new Solid(40, 10),
            new Solid(40, 10),
            new Solid(40, 10)
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: Two rows
        Assert.AreEqual(80, stack.ElementSize.Width); // max row width
        Assert.AreEqual(20, stack.ElementSize.Height); // 2 rows * 10px

        // First row
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(40, stack.Children[1].Position.X);
        Assert.AreEqual(0, stack.Children[1].Position.Y);

        // Second row
        Assert.AreEqual(0, stack.Children[2].Position.X);
        Assert.AreEqual(10, stack.Children[2].Position.Y);
    }

    [TestMethod]
    public void HStack_Wrap_ThreeRows()
    {
        // Arrange: 5 items of 40px each, only 100px available
        // Should wrap: [40, 40] [40, 40] [40]
        var stack = new HStack(
            new Solid(40, 10),
            new Solid(40, 10),
            new Solid(40, 10),
            new Solid(40, 10),
            new Solid(40, 10)
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: Three rows
        Assert.AreEqual(80, stack.ElementSize.Width);
        Assert.AreEqual(30, stack.ElementSize.Height); // 3 rows * 10px

        // Row 1
        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(0, stack.Children[1].Position.Y);
        // Row 2
        Assert.AreEqual(10, stack.Children[2].Position.Y);
        Assert.AreEqual(10, stack.Children[3].Position.Y);
        // Row 3
        Assert.AreEqual(20, stack.Children[4].Position.Y);
    }

    [TestMethod]
    public void HStack_Wrap_DifferentHeights_RowHeightIsMax()
    {
        // Arrange: Items with different heights
        var stack = new HStack(
            new Solid(40, 10),
            new Solid(40, 20), // Taller
            new Solid(40, 15)  // Wraps to second row
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert
        // First row height = max(10, 20) = 20
        // Second row starts at Y=20
        Assert.AreEqual(80, stack.ElementSize.Width);
        Assert.AreEqual(35, stack.ElementSize.Height); // 20 + 15

        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(0, stack.Children[1].Position.Y);
        Assert.AreEqual(20, stack.Children[2].Position.Y); // Second row
    }

    [TestMethod]
    public void HStack_Wrap_WithMargins()
    {
        // Arrange: Items with margins
        var stack = new HStack(
            new Solid(30, 10).SetMargin(new Margin(5)),
            new Solid(30, 10).SetMargin(new Margin(5)),
            new Solid(30, 10).SetMargin(new Margin(5))
        ).SetWrap(true);

        // Act: Each item is 30 + 10 (margin) = 40px wide
        // Two fit in 100px, third wraps
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert
        Assert.AreEqual(80, stack.ElementSize.Width); // 2 * 40
        Assert.AreEqual(40, stack.ElementSize.Height); // 2 rows * 20 (10 + margins)

        // First row - margins are applied by arrange
        Assert.AreEqual(5, stack.Children[0].Position.X);
        Assert.AreEqual(5, stack.Children[0].Position.Y);
        Assert.AreEqual(45, stack.Children[1].Position.X);
        Assert.AreEqual(5, stack.Children[1].Position.Y);

        // Second row
        Assert.AreEqual(5, stack.Children[2].Position.X);
        Assert.AreEqual(25, stack.Children[2].Position.Y);
    }

    [TestMethod]
    public void HStack_Wrap_Disabled_NoWrapping()
    {
        // Arrange: Without wrap, items are placed in single row
        // Use explicit Left alignment to prevent stretch behavior
        var stack = new HStack(
            new Solid(40, 10).SetHorizontalAlignment(HorizontalAlignment.Left),
            new Solid(40, 10).SetHorizontalAlignment(HorizontalAlignment.Left),
            new Solid(40, 10).SetHorizontalAlignment(HorizontalAlignment.Left)
        ).SetWrap(false);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: All in one row (ElementSize is constrained to available space)
        Assert.AreEqual(100, stack.ElementSize.Width); // Constrained to available
        Assert.AreEqual(10, stack.ElementSize.Height);
        // All children on same Y (not wrapped)
        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(0, stack.Children[1].Position.Y);
        Assert.AreEqual(0, stack.Children[2].Position.Y);
        // Children positioned sequentially
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(40, stack.Children[1].Position.X);
        Assert.AreEqual(80, stack.Children[2].Position.X);
    }

    [TestMethod]
    public void HStack_Wrap_EmptyStack()
    {
        // Arrange
        var stack = new HStack().SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert
        Assert.AreEqual(0, stack.ElementSize.Width);
        Assert.AreEqual(0, stack.ElementSize.Height);
    }

    [TestMethod]
    public void HStack_Wrap_SingleItem()
    {
        // Arrange
        var stack = new HStack(new Solid(50, 20)).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert
        Assert.AreEqual(50, stack.ElementSize.Width);
        Assert.AreEqual(20, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
    }

    [TestMethod]
    public void HStack_Wrap_ItemLargerThanContainer()
    {
        // Arrange: Single item larger than container
        var stack = new HStack(
            new Solid(150, 10),
            new Solid(30, 10)
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: Large item on first row alone, second item wraps
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(0, stack.Children[1].Position.X);
        Assert.AreEqual(10, stack.Children[1].Position.Y);
    }
}
