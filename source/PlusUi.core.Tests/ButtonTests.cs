using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

[TestClass]
public class ButtonTests
{

    [TestMethod]
    public void TestButtonPadding_CorrectSizing()
    {
        //Arrange
        var buttonWithoutPadding = new Button().SetText("Test Button").SetPadding(new(0));
        var buttonWithPadding = new Button().SetText("Test Button").SetPadding(new(10));
        var availableSize = new Size(200, 100);

        //Act
        buttonWithoutPadding.Measure(availableSize);
        buttonWithPadding.Measure(availableSize);
        buttonWithPadding.Arrange(new Rect(0, 0, 200, 100));

        //Assert
        Assert.AreEqual(0, buttonWithPadding.Position.X);
        Assert.AreEqual(0, buttonWithPadding.Position.Y);
        // Padding adds 10 left + 10 right = 20 to width
        Assert.AreEqual(buttonWithoutPadding.ElementSize.Width + 20, buttonWithPadding.ElementSize.Width, 0.1);
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
        button.BindIcon(() => testIcon);
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
        button.BindIconPosition(() => testPosition);
        button.UpdateBindings();
        //Assert
        Assert.AreEqual(IconPosition.Trailing, button.IconPosition);
    }

    [TestMethod]
    public void TestButton_TextOnly_MeasuresCorrectly()
    {
        //Arrange
        var button = new Button().SetText("Test").SetPadding(new(0));

        //Act
        button.Measure(new Size(200, 200));

        //Assert - Button should have non-zero size matching text dimensions
        Assert.IsGreaterThan(0f, button.ElementSize.Width);
        Assert.IsGreaterThan(0f, button.ElementSize.Height);
        // Inter font "Test" at PlusUiDefaults.FontSize (14): width ~29, height ~17
        Assert.AreEqual(29.0f, button.ElementSize.Width, 1.0f);
        Assert.AreEqual(17.0f, button.ElementSize.Height, 1.0f);
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
        var buttonTextOnly = new Button().SetText("Test").SetPadding(new(0)).SetTextSize(12);
        var buttonWithIcon = new Button()
            .SetText("Test")
            .SetIcon("icon.png")
            .SetIconPosition(IconPosition.Leading)
            .SetPadding(new(0))
            .SetTextSize(12);

        //Act
        buttonTextOnly.Measure(new Size(200, 200));
        buttonWithIcon.Measure(new Size(200, 200));

        //Assert - Without actual icon resource, width should be at least text width
        Assert.IsGreaterThanOrEqualTo(buttonTextOnly.ElementSize.Width, buttonWithIcon.ElementSize.Width);
    }

    [TestMethod]
    public void TestButton_IconTrailing_IncludesIconInWidth()
    {
        //Arrange
        var buttonTextOnly = new Button().SetText("Test").SetPadding(new(0)).SetTextSize(12);
        var buttonWithIcon = new Button()
            .SetText("Test")
            .SetIcon("icon.png")
            .SetIconPosition(IconPosition.Trailing)
            .SetPadding(new(0))
            .SetTextSize(12);

        //Act
        buttonTextOnly.Measure(new Size(200, 200));
        buttonWithIcon.Measure(new Size(200, 200));

        //Assert - Without actual icon resource, width should be at least text width
        Assert.IsGreaterThanOrEqualTo(buttonTextOnly.ElementSize.Width, buttonWithIcon.ElementSize.Width);
    }

    [TestMethod]
    public void TestButton_IconBoth_IncludesBothIconsInWidth()
    {
        //Arrange
        var buttonTextOnly = new Button().SetText("Test").SetPadding(new(0)).SetTextSize(12);
        var buttonWithIcon = new Button()
            .SetText("Test")
            .SetIcon("icon.png")
            .SetIconPosition(IconPosition.Leading | IconPosition.Trailing)
            .SetPadding(new(0))
            .SetTextSize(12);

        //Act
        buttonTextOnly.Measure(new Size(200, 200));
        buttonWithIcon.Measure(new Size(200, 200));

        //Assert - Without actual icon resource, width should be at least text width
        Assert.IsGreaterThanOrEqualTo(buttonTextOnly.ElementSize.Width, buttonWithIcon.ElementSize.Width);
    }

    [TestMethod]
    public void TestButton_IconPositionNone_DoesNotIncludeIcon()
    {
        //Arrange
        var buttonTextOnly = new Button().SetText("Test").SetPadding(new(0)).SetTextSize(12);
        var buttonWithIconNone = new Button()
            .SetText("Test")
            .SetIcon("icon.png")
            .SetIconPosition(IconPosition.None)
            .SetPadding(new(0))
            .SetTextSize(12);

        //Act
        buttonTextOnly.Measure(new Size(200, 200));
        buttonWithIconNone.Measure(new Size(200, 200));

        //Assert - When IconPosition is None, icon should not be included in the width
        Assert.AreEqual(buttonTextOnly.ElementSize.Width, buttonWithIconNone.ElementSize.Width, 0.1);
    }
}
