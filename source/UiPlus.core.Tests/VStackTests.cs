using PlusUi.core.Controls;
using PlusUi.core.Enumerations;
using PlusUi.core.Structures;

namespace UiPlus.core.Tests;

[TestClass]
public sealed class VStackTests
{

    [TestMethod]
    public void TestVStackMeasureAndArrange_NoMargin_NoSpacing_LeftTopAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10));
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(0, stack.Position.X);
        Assert.AreEqual(0, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_NoMargin_NoSpacing_TopCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { HorizontalAlignment = HorizontalAlignment.Center };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(45, stack.Position.X);
        Assert.AreEqual(0, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_NoMargin_NoSpacing_TopRightAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { HorizontalAlignment = HorizontalAlignment.Right };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(90, stack.Position.X);
        Assert.AreEqual(0, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(90, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_NoMargin_NoSpacing_LeftCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { VerticalAlignment = VerticalAlignment.Center };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(0, stack.Position.X);
        Assert.AreEqual(45, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_NoMargin_NoSpacing_CenterCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(45, stack.Position.X);
        Assert.AreEqual(45, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_NoMargin_NoSpacing_RightCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(90, stack.Position.X);
        Assert.AreEqual(45, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(90, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_NoMargin_NoSpacing_LeftBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { VerticalAlignment = VerticalAlignment.Bottom };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(0, stack.Position.X);
        Assert.AreEqual(90, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(90, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_NoMargin_NoSpacing_CenterBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(45, stack.Position.X);
        Assert.AreEqual(90, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(90, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_NoMargin_NoSpacing_RightBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(90, stack.Position.X);
        Assert.AreEqual(90, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(90, stack.Children[0].Position.X);
        Assert.AreEqual(90, stack.Children[0].Position.Y);
    }

    [TestMethod]
    public void TestVStackMeasureAndArrange_WithMargin_NoSpacing_LeftTopAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { Margin = new(5) };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(5, stack.Position.X);
        Assert.AreEqual(5, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(5, stack.Children[0].Position.X);
        Assert.AreEqual(5, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_WithMargin_NoSpacing_TopCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Center };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(45, stack.Position.X);
        Assert.AreEqual(5, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(5, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_WithMargin_NoSpacing_TopRightAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Right };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(85, stack.Position.X);
        Assert.AreEqual(5, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(85, stack.Children[0].Position.X);
        Assert.AreEqual(5, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_WithMargin_NoSpacing_LeftCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { Margin = new(5), VerticalAlignment = VerticalAlignment.Center };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(5, stack.Position.X);
        Assert.AreEqual(45, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(5, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_WithMargin_NoSpacing_CenterCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(45, stack.Position.X);
        Assert.AreEqual(45, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_WithMargin_NoSpacing_RightCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(85, stack.Position.X);
        Assert.AreEqual(45, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(85, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_WithMargin_NoSpacing_LeftBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { Margin = new(5), VerticalAlignment = VerticalAlignment.Bottom };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(5, stack.Position.X);
        Assert.AreEqual(85, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(5, stack.Children[0].Position.X);
        Assert.AreEqual(85, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_WithMargin_NoSpacing_CenterBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(45, stack.Position.X);
        Assert.AreEqual(85, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(85, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_WithMargin_NoSpacing_RightBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(85, stack.Position.X);
        Assert.AreEqual(85, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(10, stack.ElementSize.Height);
        Assert.AreEqual(85, stack.Children[0].Position.X);
        Assert.AreEqual(85, stack.Children[0].Position.Y);
    }

    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithNoMargin_LeftTopAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10)) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithNoMargin_TopCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { HorizontalAlignment = HorizontalAlignment.Center }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithNoMargin_TopRightAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { HorizontalAlignment = HorizontalAlignment.Right }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(90, stack.Children[0].Position.X);
        Assert.AreEqual(0, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithNoMargin_LeftCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { VerticalAlignment = VerticalAlignment.Center }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithNoMargin_CenterCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithNoMargin_RightCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(90, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithNoMargin_LeftBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { VerticalAlignment = VerticalAlignment.Bottom }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(90, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithNoMargin_CenterBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(90, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithNoMargin_RightBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(90, stack.Children[0].Position.X);
        Assert.AreEqual(90, stack.Children[0].Position.Y);
    }

    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithMargin_LeftTopAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { Margin = new(5) }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(5, stack.Children[0].Position.X);
        Assert.AreEqual(5, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithMargin_TopCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Center }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(5, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithMargin_TopRightAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Right }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(85, stack.Children[0].Position.X);
        Assert.AreEqual(5, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithMargin_LeftCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { Margin = new(5), VerticalAlignment = VerticalAlignment.Center }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(5, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithMargin_CenterCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithMargin_RightCenterAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(85, stack.Children[0].Position.X);
        Assert.AreEqual(45, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithMargin_LeftBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { Margin = new(5), VerticalAlignment = VerticalAlignment.Bottom }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(5, stack.Children[0].Position.X);
        Assert.AreEqual(85, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithMargin_CenterBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(45, stack.Children[0].Position.X);
        Assert.AreEqual(85, stack.Children[0].Position.Y);
    }
    [TestMethod]
    public void TestVStackMeasureAndArrange_ForChild_WithMargin_RightBottomAligned()
    {
        //Arrange
        var stack = new VStack(new Solid(10, 10) { Margin = new(5), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom }) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(100, stack.ElementSize.Width);
        Assert.AreEqual(100, stack.ElementSize.Height);
        Assert.AreEqual(85, stack.Children[0].Position.X);
        Assert.AreEqual(85, stack.Children[0].Position.Y);
    }

    [TestMethod]
    public void TestVStackMeasureAndArrange_ForMultibleChildren_WithUnregularMargins()
    {
        //Arrange
        var stack = new VStack(
            new Solid(10, 10) { Margin = new(0, 5, 0, 10) },
            new Solid(10, 10) { Margin = new(0, 15, 0, 20) });
        var availableSize = new Size(100, 100);
        //Act
        stack.Measure(availableSize);
        stack.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(0, stack.Position.X);
        Assert.AreEqual(0, stack.Position.Y);
        Assert.AreEqual(10, stack.ElementSize.Width);
        Assert.AreEqual(70, stack.ElementSize.Height);
        Assert.AreEqual(0, stack.Children[0].Position.X);
        Assert.AreEqual(5, stack.Children[0].Position.Y);
        Assert.AreEqual(0, stack.Children[1].Position.X);
        Assert.AreEqual(40, stack.Children[1].Position.Y);
    }

}
