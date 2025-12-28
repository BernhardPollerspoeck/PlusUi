using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

[TestClass]
public class ShadowTests
{
    [TestMethod]
    public void TestShadow_DefaultValues()
    {
        // Arrange & Act
        var element = new Border();

        // Assert
        Assert.AreEqual(Colors.Transparent, element.ShadowColor);
        Assert.AreEqual(0f, element.ShadowOffset.X);
        Assert.AreEqual(0f, element.ShadowOffset.Y);
        Assert.AreEqual(0f, element.ShadowBlur);
        Assert.AreEqual(0f, element.ShadowSpread);
    }

    [TestMethod]
    public void TestShadow_SetShadowColor()
    {
        // Arrange
        var element = new Border();
        var shadowColor = Colors.Black.WithAlpha(50);

        // Act
        element.SetShadowColor(shadowColor);

        // Assert
        Assert.AreEqual(shadowColor, element.ShadowColor);
    }

    [TestMethod]
    public void TestShadow_SetShadowOffset()
    {
        // Arrange
        var element = new Border();
        var offset = new Point(2, 4);

        // Act
        element.SetShadowOffset(offset);

        // Assert
        Assert.AreEqual(2f, element.ShadowOffset.X);
        Assert.AreEqual(4f, element.ShadowOffset.Y);
    }

    [TestMethod]
    public void TestShadow_SetShadowBlur()
    {
        // Arrange
        var element = new Border();

        // Act
        element.SetShadowBlur(8f);

        // Assert
        Assert.AreEqual(8f, element.ShadowBlur);
    }

    [TestMethod]
    public void TestShadow_SetShadowSpread()
    {
        // Arrange
        var element = new Border();

        // Act
        element.SetShadowSpread(2f);

        // Assert
        Assert.AreEqual(2f, element.ShadowSpread);
    }

    [TestMethod]
    public void TestShadow_FluentAPI()
    {
        // Arrange
        var element = new Border();
        var shadowColor = Colors.Black.WithAlpha(100);
        var offset = new Point(0, 4);

        // Act
        var result = element
            .SetShadowColor(shadowColor)
            .SetShadowOffset(offset)
            .SetShadowBlur(8f)
            .SetShadowSpread(0f);

        // Assert - fluent API should return the same instance
        Assert.AreSame(element, result);
        Assert.AreEqual(shadowColor, element.ShadowColor);
        Assert.AreEqual(0f, element.ShadowOffset.X);
        Assert.AreEqual(4f, element.ShadowOffset.Y);
        Assert.AreEqual(8f, element.ShadowBlur);
        Assert.AreEqual(0f, element.ShadowSpread);
    }

    [TestMethod]
    public void TestShadow_BindShadowColor()
    {
        // Arrange
        var element = new Border();
        var isElevated = false;

        // Act
        element.BindShadowColor("IsElevated", () => 
            isElevated ? Colors.Black.WithAlpha(100) : Colors.Transparent);

        // Initial state
        Assert.AreEqual(Colors.Transparent, element.ShadowColor);

        // Update state
        isElevated = true;
        element.UpdateBindings("IsElevated");

        // Assert
        Assert.AreEqual(Colors.Black.WithAlpha(100), element.ShadowColor);
    }

    [TestMethod]
    public void TestShadow_BindShadowBlur()
    {
        // Arrange
        var element = new Border();
        var isHovered = false;

        // Act
        element.BindShadowBlur("IsHovered", () => isHovered ? 16f : 4f);

        // Initial state
        Assert.AreEqual(4f, element.ShadowBlur);

        // Update state
        isHovered = true;
        element.UpdateBindings("IsHovered");

        // Assert
        Assert.AreEqual(16f, element.ShadowBlur);
    }

    [TestMethod]
    public void TestShadow_BindShadowOffset()
    {
        // Arrange
        var element = new Border();
        var isHovered = false;

        // Act
        element.BindShadowOffset("IsHovered", () => 
            isHovered ? new Point(0, 8) : new Point(0, 2));

        // Initial state
        Assert.AreEqual(0f, element.ShadowOffset.X);
        Assert.AreEqual(2f, element.ShadowOffset.Y);

        // Update state
        isHovered = true;
        element.UpdateBindings("IsHovered");

        // Assert
        Assert.AreEqual(0f, element.ShadowOffset.X);
        Assert.AreEqual(8f, element.ShadowOffset.Y);
    }

    [TestMethod]
    public void TestShadow_BindShadowSpread()
    {
        // Arrange
        var element = new Border();
        var elevation = 2f;

        // Act
        element.BindShadowSpread("Elevation", () => elevation);

        // Initial state
        Assert.AreEqual(2f, element.ShadowSpread);

        // Update state
        elevation = 4f;
        element.UpdateBindings("Elevation");

        // Assert
        Assert.AreEqual(4f, element.ShadowSpread);
    }

    [TestMethod]
    public void TestShadow_WorksOnLabel()
    {
        // Arrange
        var label = new Label();
        var shadowColor = Colors.Black.WithAlpha(50);

        // Act
        label.SetShadowColor(shadowColor)
             .SetShadowOffset(new Point(2, 2))
             .SetShadowBlur(4f);

        // Assert
        Assert.AreEqual(shadowColor, label.ShadowColor);
        Assert.AreEqual(2f, label.ShadowOffset.X);
        Assert.AreEqual(2f, label.ShadowOffset.Y);
        Assert.AreEqual(4f, label.ShadowBlur);
    }

    [TestMethod]
    public void TestShadow_WorksOnButton()
    {
        // Arrange
        var button = new Button();
        var shadowColor = Colors.Black.WithAlpha(75);

        // Act
        button.SetShadowColor(shadowColor)
              .SetShadowOffset(new Point(0, 3))
              .SetShadowBlur(6f)
              .SetShadowSpread(1f);

        // Assert
        Assert.AreEqual(shadowColor, button.ShadowColor);
        Assert.AreEqual(0f, button.ShadowOffset.X);
        Assert.AreEqual(3f, button.ShadowOffset.Y);
        Assert.AreEqual(6f, button.ShadowBlur);
        Assert.AreEqual(1f, button.ShadowSpread);
    }

    [TestMethod]
    public void TestShadow_WithCornerRadius()
    {
        // Arrange
        var border = new Border();

        // Act
        border.SetBackground(Colors.White)
              .SetCornerRadius(8f)
              .SetShadowColor(Colors.Black.WithAlpha(50))
              .SetShadowOffset(new Point(0, 2))
              .SetShadowBlur(8f);

        // Assert
        Assert.AreEqual(8f, border.CornerRadius);
        Assert.AreEqual(Colors.Black.WithAlpha(50), border.ShadowColor);
        Assert.AreEqual(0f, border.ShadowOffset.X);
        Assert.AreEqual(2f, border.ShadowOffset.Y);
        Assert.AreEqual(8f, border.ShadowBlur);
    }

    [TestMethod]
    public void TestShadow_ComplexBinding()
    {
        // Arrange
        var element = new Border();
        var elevation = 2;

        // Act - Bind multiple shadow properties to elevation level
        element.BindShadowColor("Elevation", () => 
                elevation > 0 ? Colors.Black.WithAlpha((byte)(elevation * 10)) : Colors.Transparent)
               .BindShadowOffset("Elevation", () => new Point(0, elevation))
               .BindShadowBlur("Elevation", () => elevation * 2f)
               .BindShadowSpread("Elevation", () => elevation * 0.5f);

        // Initial state
        Assert.AreEqual(Colors.Black.WithAlpha(20), element.ShadowColor);
        Assert.AreEqual(2f, element.ShadowOffset.Y);
        Assert.AreEqual(4f, element.ShadowBlur);
        Assert.AreEqual(1f, element.ShadowSpread);

        // Update elevation
        elevation = 6;
        element.UpdateBindings("Elevation");

        // Assert - All shadow properties should update
        Assert.AreEqual(Colors.Black.WithAlpha(60), element.ShadowColor);
        Assert.AreEqual(6f, element.ShadowOffset.Y);
        Assert.AreEqual(12f, element.ShadowBlur);
        Assert.AreEqual(3f, element.ShadowSpread);
    }
}
