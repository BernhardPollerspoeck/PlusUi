using PlusUi.core;

namespace UiPlus.core.Tests;
/// <summary>
/// this class proves simple element alignment and margin calculation
/// </summary>
[TestClass]
public sealed class LabelTests
{
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_LeftTopAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label" };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(0, label.Position.X);
        Assert.AreEqual(0, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_CenterTopAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Center };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(0, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_RightTopAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Right };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width, label.Position.X);
        Assert.AreEqual(0, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_LeftCenterAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", VerticalAlignment = VerticalAlignment.Center };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(0, label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_CenterCenterAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_RightCenterAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width, label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_LeftBottomAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", VerticalAlignment = VerticalAlignment.Bottom };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(0, label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_CenterBottomAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_RightBottomAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width, label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height, label.Position.Y);
    }

    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_LeftTopAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", Margin = new Margin(10) };
        var availableSize = new Size(100, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.AreEqual(10, label.Position.X);
        Assert.AreEqual(10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_CenterTopAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Center, Margin = new Margin(10) };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_RightTopAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Right, Margin = new Margin(10) };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width - 10, label.Position.X);
        Assert.AreEqual(10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_LeftCenterAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", VerticalAlignment = VerticalAlignment.Center, Margin = new Margin(10) };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(10, label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_CenterCenterAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Margin(10) };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_RightCenterAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Margin = new Margin(10) };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width - 10, label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_LeftBottomAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", VerticalAlignment = VerticalAlignment.Bottom, Margin = new Margin(10) };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(10, label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height - 10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_CenterBottomAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Margin(10) };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height - 10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_RightBottomAligned()
    {
        // Arrange
        var label = new Label { Text = "Test Label", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Margin(10) };
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width - 10, label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height - 10, label.Position.Y);
    }

}
