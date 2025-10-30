using PlusUi.core;
using SkiaSharp;

namespace UiPlus.core.Tests;

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
        var font = new SKFont(SKTypeface.Default) { Size = 12 };
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
}
