using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public sealed class VStackWrapTests
{
    [TestMethod]
    public void VStack_Wrap_SingleColumn_NoWrapping()
    {
        // Arrange: 3 items of 20px each = 60px total, fits in 100px
        var stack = new VStack(
            new Solid(10, 20),
            new Solid(10, 20),
            new Solid(10, 20)
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: All in one column
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(60, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(20, stack.Children[1].Position.Y);
        Assert.AreEqual(40, stack.Children[2].Position.Y);
        // All on same X
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[1].Position.X);
        Assert.AreEqual(0, stack.Children[2].Position.X);
    }

    [TestMethod]
    public void VStack_Wrap_TwoColumns()
    {
        // Arrange: 3 items of 40px each = 120px total, only 100px available
        // Should wrap: [40, 40] [40]
        var stack = new VStack(
            new Solid(10, 40),
            new Solid(10, 40),
            new Solid(10, 40)
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: Two columns
        Assert.AreEqual(20, stack.ElementSize.Width); // 2 columns * 10px
        Assert.AreEqual(80, stack.ElementSize.Height); // max column height

        // First column
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(0, stack.Children[1].Position.X);
        Assert.AreEqual(40, stack.Children[1].Position.Y);

        // Second column
        Assert.AreEqual(10, stack.Children[2].Position.X);
        Assert.AreEqual(0, stack.Children[2].Position.Y);
    }

    [TestMethod]
    public void VStack_Wrap_ThreeColumns()
    {
        // Arrange: 5 items of 40px each, only 100px available
        // Should wrap: [40, 40] [40, 40] [40]
        var stack = new VStack(
            new Solid(10, 40),
            new Solid(10, 40),
            new Solid(10, 40),
            new Solid(10, 40),
            new Solid(10, 40)
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: Three columns
        Assert.AreEqual(30, stack.ElementSize.Width); // 3 columns * 10px
        Assert.AreEqual(80, stack.ElementSize.Height);

        // Column 1
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[1].Position.X);
        // Column 2
        Assert.AreEqual(10, stack.Children[2].Position.X);
        Assert.AreEqual(10, stack.Children[3].Position.X);
        // Column 3
        Assert.AreEqual(20, stack.Children[4].Position.X);
    }

    [TestMethod]
    public void VStack_Wrap_DifferentWidths_ColumnWidthIsMax()
    {
        // Arrange: Items with different widths
        var stack = new VStack(
            new Solid(10, 40),
            new Solid(20, 40), // Wider
            new Solid(15, 40)  // Wraps to second column
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert
        // First column width = max(10, 20) = 20
        // Second column starts at X=20
        Assert.AreEqual(35, stack.ElementSize.Width); // 20 + 15
        Assert.AreEqual(80, stack.ElementSize.Height);

        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[1].Position.X);
        Assert.AreEqual(20, stack.Children[2].Position.X); // Second column
    }

    [TestMethod]
    public void VStack_Wrap_WithMargins()
    {
        // Arrange: Items with margins
        var stack = new VStack(
            new Solid(10, 30).SetMargin(new Margin(5)),
            new Solid(10, 30).SetMargin(new Margin(5)),
            new Solid(10, 30).SetMargin(new Margin(5))
        ).SetWrap(true);

        // Act: Each item is 30 + 10 (margin) = 40px tall
        // Two fit in 100px, third wraps
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert
        Assert.AreEqual(40, stack.ElementSize.Width); // 2 columns * 20 (10 + margins)
        Assert.AreEqual(80, stack.ElementSize.Height); // 2 * 40

        // First column
        Assert.AreEqual(5, stack.Children[0].Position.X);
        Assert.AreEqual(5, stack.Children[0].Position.Y);
        Assert.AreEqual(5, stack.Children[1].Position.X);
        Assert.AreEqual(45, stack.Children[1].Position.Y);

        // Second column
        Assert.AreEqual(25, stack.Children[2].Position.X);
        Assert.AreEqual(5, stack.Children[2].Position.Y);
    }

    [TestMethod]
    public void VStack_Wrap_Disabled_NoWrapping()
    {
        // Arrange: Without wrap, items are placed in single column
        // Use explicit Top alignment to prevent stretch behavior
        var stack = new VStack(
            new Solid(10, 40).SetVerticalAlignment(VerticalAlignment.Top),
            new Solid(10, 40).SetVerticalAlignment(VerticalAlignment.Top),
            new Solid(10, 40).SetVerticalAlignment(VerticalAlignment.Top)
        ).SetWrap(false);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: All in one column (ElementSize is constrained to available space)
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height); // Constrained to available
        // All children on same X (not wrapped)
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[1].Position.X);
        Assert.AreEqual(0, stack.Children[2].Position.X);
        // Children positioned sequentially
        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(40, stack.Children[1].Position.Y);
        Assert.AreEqual(80, stack.Children[2].Position.Y);
    }

    [TestMethod]
    public void VStack_Wrap_EmptyStack()
    {
        // Arrange
        var stack = new VStack().SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert
        Assert.AreEqual(0, stack.ElementSize.Width);
        Assert.AreEqual(0, stack.ElementSize.Height);
    }

    [TestMethod]
    public void VStack_Wrap_SingleItem()
    {
        // Arrange
        var stack = new VStack(new Solid(20, 50)).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert
        Assert.AreEqual(20, stack.ElementSize.Width);
        Assert.AreEqual(50, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
    }

    [TestMethod]
    public void VStack_Wrap_ItemLargerThanContainer()
    {
        // Arrange: Single item larger than container
        var stack = new VStack(
            new Solid(10, 150),
            new Solid(10, 30)
        ).SetWrap(true);

        // Act
        stack.Measure(new Size(100, 100));
        stack.Arrange(new Rect(0, 0, 100, 100));

        // Assert: Large item on first column alone, second item wraps
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
        Assert.AreEqual(10, stack.Children[1].Position.X);
        Assert.AreEqual(0, stack.Children[1].Position.Y);
    }
}
