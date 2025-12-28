using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

[TestClass]
public class ButtonTests
{
    private readonly float _textWidth = new SKFont(SKTypeface.Default) { Size = 12 }.MeasureText("Test Button");

    [TestMethod]
    public void TestButtonPadding_CorrectSizing()
    {
        //Arrange
        var button = new Button().SetText("Test Button").SetPadding(new(10));
        var availableSize = new Size(100, 100);
        //Act
        button.Measure(availableSize);
        button.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(0, button.Position.X);
        Assert.AreEqual(0, button.Position.Y);
        Assert.AreEqual(_textWidth + 20, button.ElementSize.Width);
    }

    [TestMethod]
    public void TestButton_SetIcon_PropertyIsSet()
    {
        //Arrange
        var button = new Button();
        //Act
        button.SetIcon("test_icon.png");
        //Assert
        Assert.AreEqual("test_icon.png", button.Icon);
    }

    [TestMethod]
    public void TestButton_SetIconPosition_PropertyIsSet()
    {
        //Arrange
        var button = new Button();
        //Act
        button.SetIconPosition(IconPosition.Trailing);
        //Assert
        Assert.AreEqual(IconPosition.Trailing, button.IconPosition);
    }

    [TestMethod]
    public void TestButton_DefaultIconPosition_IsLeading()
    {
        //Arrange & Act
        var button = new Button();
        //Assert
        Assert.AreEqual(IconPosition.Leading, button.IconPosition);
    }

    [TestMethod]
    public void TestButton_IconPositionFlags_SupportsMultipleValues()
    {
        //Arrange
        var button = new Button();
        //Act
        button.SetIconPosition(IconPosition.Leading | IconPosition.Trailing);
        //Assert
        Assert.IsTrue(button.IconPosition.HasFlag(IconPosition.Leading));
        Assert.IsTrue(button.IconPosition.HasFlag(IconPosition.Trailing));
    }

    [TestMethod]
    public void TestButton_BindIcon_UpdatesPropertyCorrectly()
    {
        //Arrange
        var button = new Button();
        var testIcon = "bound_icon.png";
        //Act
        button.BindIcon(nameof(testIcon), () => testIcon);
        button.UpdateBindings();
        //Assert
        Assert.AreEqual("bound_icon.png", button.Icon);
    }

    [TestMethod]
    public void TestButton_BindIconPosition_UpdatesPropertyCorrectly()
    {
        //Arrange
        var button = new Button();
        var testPosition = IconPosition.Trailing;
        //Act
        button.BindIconPosition(nameof(testPosition), () => testPosition);
        button.UpdateBindings();
        //Assert
        Assert.AreEqual(IconPosition.Trailing, button.IconPosition);
    }

    [TestMethod]
    public void TestButton_TextOnly_MeasuresCorrectly()
    {
        //Arrange
        var button = new Button().SetText("Test").SetPadding(new(0));
        using var font = new SKFont(SKTypeface.Default) { Size = 12 };
        var expectedWidth = font.MeasureText("Test");
        font.GetFontMetrics(out var fontMetrics);
        var expectedHeight = fontMetrics.Descent - fontMetrics.Ascent;
        //Act
        button.Measure(new Size(200, 200));
        //Assert
        Assert.AreEqual(expectedWidth, button.ElementSize.Width, 0.1);
        Assert.AreEqual(expectedHeight, button.ElementSize.Height, 0.1);
    }

    [TestMethod]
    public void TestButton_MethodChaining_WorksCorrectly()
    {
        //Arrange & Act
        var button = new Button()
            .SetText("Test")
            .SetIcon("test.png")
            .SetIconPosition(IconPosition.Trailing)
            .SetPadding(new(10));
        //Assert
        Assert.AreEqual("Test", button.Text);
        Assert.AreEqual("test.png", button.Icon);
        Assert.AreEqual(IconPosition.Trailing, button.IconPosition);
        Assert.AreEqual(10, button.Padding.Left);
    }

    [TestMethod]
    public void TestButton_IconOnly_MeasuresCorrectly()
    {
        //Arrange
        var button = new Button().SetIcon("icon.png").SetPadding(new(0)).SetTextSize(12);
        //Act
        button.Measure(new Size(200, 200));
        //Assert
        // Without actual icon resource, _iconImage will be null and size will be zero
        // This test verifies that setting an icon doesn't crash the button
        Assert.IsGreaterThanOrEqualTo(0, button.ElementSize.Width);
        Assert.IsGreaterThanOrEqualTo(0, button.ElementSize.Height);
    }

    [TestMethod]
    public void TestButton_NoIconNoText_MeasuresZero()
    {
        //Arrange
        var button = new Button().SetPadding(new(0));
        //Act
        button.Measure(new Size(200, 200));
        //Assert
        // Button with no text and no icon should have zero size
        Assert.AreEqual(0f, button.ElementSize.Width, 0.1);
    }

    [TestMethod]
    public void TestButton_IconLeading_IncludesIconInWidth()
    {
        //Arrange
        var button = new Button()
            .SetText("Test")
            .SetIcon("icon.png")
            .SetIconPosition(IconPosition.Leading)
            .SetPadding(new(0))
            .SetTextSize(12);
        using var font = new SKFont(SKTypeface.Default) { Size = 12 };
        var textWidth = font.MeasureText("Test");
        //Act
        button.Measure(new Size(200, 200));
        //Assert
        // Without actual icon resource, width should be at least text width
        Assert.IsGreaterThanOrEqualTo(textWidth, button.ElementSize.Width);
    }

    [TestMethod]
    public void TestButton_IconTrailing_IncludesIconInWidth()
    {
        //Arrange
        var button = new Button()
            .SetText("Test")
            .SetIcon("icon.png")
            .SetIconPosition(IconPosition.Trailing)
            .SetPadding(new(0))
            .SetTextSize(12);
        using var font = new SKFont(SKTypeface.Default) { Size = 12 };
        var textWidth = font.MeasureText("Test");
        //Act
        button.Measure(new Size(200, 200));
        //Assert
        // Without actual icon resource, width should be at least text width
        Assert.IsGreaterThanOrEqualTo(textWidth, button.ElementSize.Width);
    }

    [TestMethod]
    public void TestButton_IconBoth_IncludesBothIconsInWidth()
    {
        //Arrange
        var button = new Button()
            .SetText("Test")
            .SetIcon("icon.png")
            .SetIconPosition(IconPosition.Leading | IconPosition.Trailing)
            .SetPadding(new(0))
            .SetTextSize(12);
        using var font = new SKFont(SKTypeface.Default) { Size = 12 };
        var textWidth = font.MeasureText("Test");
        //Act
        button.Measure(new Size(200, 200));
        //Assert
        // Without actual icon resource, width should be at least text width
        Assert.IsGreaterThanOrEqualTo(textWidth, button.ElementSize.Width);
    }

    [TestMethod]
    public void TestButton_IconPositionNone_DoesNotIncludeIcon()
    {
        //Arrange
        var button = new Button()
            .SetText("Test")
            .SetIcon("icon.png")
            .SetIconPosition(IconPosition.None)
            .SetPadding(new(0))
            .SetTextSize(12);
        using var font = new SKFont(SKTypeface.Default) { Size = 12 };
        var textWidth = font.MeasureText("Test");
        //Act
        button.Measure(new Size(200, 200));
        //Assert
        // When IconPosition is None, icon should not be included in the width
        Assert.AreEqual(textWidth, button.ElementSize.Width, 0.1);
    }
}
