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
        var button = new Button() { Text = "Test Button", Padding = new(10) };
        var availableSize = new Size(100, 100);
        //Act
        button.Measure(availableSize);
        button.Arrange(new Rect(0, 0, 100, 100));
        //Assert
        Assert.AreEqual(0, button.Position.X);
        Assert.AreEqual(0, button.Position.Y);
        Assert.AreEqual(_textWidth + 20, button.ElementSize.Width);
    }
}
