using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public sealed class FontTests
{
    [TestMethod]
    public void TestLabel_SetFontFamily()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetFontFamily("Roboto");

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetFontWeight_Regular()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetFontWeight(FontWeight.Regular);

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetFontWeight_Bold()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetFontWeight(FontWeight.Bold);

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetFontWeight_Light()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetFontWeight(FontWeight.Light);

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetFontStyle_Normal()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetFontStyle(FontStyle.Normal);

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetFontStyle_Italic()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetFontStyle(FontStyle.Italic);

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetFontStyle_Oblique()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetFontStyle(FontStyle.Oblique);

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetAllFontProperties()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetFontFamily("Roboto")
            .SetFontWeight(FontWeight.Bold)
            .SetFontStyle(FontStyle.Italic)
            .SetTextSize(24);

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestButton_SetFontFamily()
    {
        // Arrange & Act
        var button = new Button()
            .SetText("Test Button")
            .SetFontFamily("Arial");

        // Assert
        Assert.IsNotNull(button);
    }

    [TestMethod]
    public void TestButton_SetFontWeight()
    {
        // Arrange & Act
        var button = new Button()
            .SetText("Test Button")
            .SetFontWeight(FontWeight.SemiBold);

        // Assert
        Assert.IsNotNull(button);
    }

    [TestMethod]
    public void TestButton_SetAllFontProperties()
    {
        // Arrange & Act
        var button = new Button()
            .SetText("Test Button")
            .SetFontFamily("Helvetica")
            .SetFontWeight(FontWeight.Medium)
            .SetFontStyle(FontStyle.Normal);

        // Assert
        Assert.IsNotNull(button);
    }

    [TestMethod]
    public void TestEntry_SetFontFamily()
    {
        // Arrange & Act
        var entry = new Entry()
            .SetText("Test Entry")
            .SetFontFamily("Courier");

        // Assert
        Assert.IsNotNull(entry);
    }

    [TestMethod]
    public void TestEntry_SetFontWeight()
    {
        // Arrange & Act
        var entry = new Entry()
            .SetText("Test Entry")
            .SetFontWeight(FontWeight.Thin);

        // Assert
        Assert.IsNotNull(entry);
    }

    [TestMethod]
    public void TestLabel_ChainedFontSetters()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetFontFamily("Arial")
            .SetFontWeight(FontWeight.Bold)
            .SetFontStyle(FontStyle.Italic)
            .SetTextSize(18)
            .SetTextColor(SkiaSharp.SKColors.Black);

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_MeasureWithCustomFont()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetFontFamily("Roboto")
            .SetFontWeight(FontWeight.Bold)
            .SetTextSize(16);

        var availableSize = new Size(100, 50);

        // Act
        label.Measure(availableSize);

        // Assert - should not throw and should have a size
        Assert.IsGreaterThan(0, label.ElementSize.Width);
        Assert.IsGreaterThan(0, label.ElementSize.Height);
    }

    [TestMethod]
    public void TestLabel_RenderWithCustomFont()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetFontFamily("Arial")
            .SetFontWeight(FontWeight.Regular)
            .SetFontStyle(FontStyle.Normal);

        var availableSize = new Size(100, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert - should not throw
        using var surface = SkiaSharp.SKSurface.Create(new SkiaSharp.SKImageInfo(100, 50));
        label.Render(surface.Canvas);
        Assert.IsNotNull(label);
    }
}
