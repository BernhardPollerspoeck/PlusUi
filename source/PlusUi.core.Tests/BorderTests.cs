using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

[TestClass]
public class BorderTests
{
    [TestMethod]
    public void TestBorder_DefaultValues()
    {
        // Arrange & Act
        var border = new Border();

        // Assert
        Assert.AreEqual(Colors.Black, border.StrokeColor);
        Assert.AreEqual(1f, border.StrokeThickness);
        Assert.AreEqual(StrokeType.Solid, border.StrokeType);
        Assert.IsNull(border.Background);
    }

    [TestMethod]
    public void TestBorder_SetStrokeProperties()
    {
        // Arrange
        var border = new Border();

        // Act
        border.SetStrokeColor(Colors.Red)
              .SetStrokeThickness(3f)
              .SetStrokeType(StrokeType.Dashed)
              .SetBackground(new SolidColorBackground(Colors.Blue));

        // Assert
        Assert.AreEqual(Colors.Red, border.StrokeColor);
        Assert.AreEqual(3f, border.StrokeThickness);
        Assert.AreEqual(StrokeType.Dashed, border.StrokeType);
        Assert.IsNotNull(border.Background);
        Assert.IsInstanceOfType(border.Background, typeof(SolidColorBackground));
        Assert.AreEqual(Colors.Blue, ((SolidColorBackground)border.Background).Color);
    }

    [TestMethod]
    public void TestBorder_MeasureWithChild()
    {
        // Arrange
        var border = new Border()
            .SetStrokeThickness(2f);
        var child = new Label().SetText("Test");
        border.AddChild(child);

        var availableSize = new Size(100, 100);

        // Act
        border.Measure(availableSize);

        // Assert
        // Border should add stroke thickness to child size

        Assert.IsGreaterThanOrEqualTo(4, border.ElementSize.Width); // 2 * stroke thickness
        Assert.IsGreaterThanOrEqualTo(4, border.ElementSize.Height); // 2 * stroke thickness
    }

    [TestMethod]
    public void TestBorder_ArrangeWithChild()
    {
        // Arrange
        var border = new Border()
            .SetStrokeThickness(5f);
        var child = new Label().SetText("Test");
        border.AddChild(child);

        var bounds = new Rect(0, 0, 100, 100);

        // Act
        border.Measure(new Size(100, 100));
        border.Arrange(bounds);

        // Assert
        Assert.AreEqual(0, border.Position.X);
        Assert.AreEqual(0, border.Position.Y);

        // Child should be positioned with stroke thickness offset
        Assert.AreEqual(5, child.Position.X); // stroke thickness
        Assert.AreEqual(5, child.Position.Y); // stroke thickness
    }

    [TestMethod]
    public void TestBorder_StrokeTypeValues()
    {
        // Arrange & Act & Assert
        var solidBorder = new Border().SetStrokeType(StrokeType.Solid);
        Assert.AreEqual(StrokeType.Solid, solidBorder.StrokeType);

        var dashedBorder = new Border().SetStrokeType(StrokeType.Dashed);
        Assert.AreEqual(StrokeType.Dashed, dashedBorder.StrokeType);

        var dottedBorder = new Border().SetStrokeType(StrokeType.Dotted);
        Assert.AreEqual(StrokeType.Dotted, dottedBorder.StrokeType);
    }

    [TestMethod]
    public void TestBorder_StrokeThicknessValidation()
    {
        // Arrange & Act
        var border = new Border().SetStrokeThickness(-5f);

        // Assert - negative values should be clamped to 0
        Assert.AreEqual(0f, border.StrokeThickness);
    }
}