using PlusUi.desktop;

namespace PlusUi.desktop.Tests;

[TestClass]
public class DesktopClipboardServiceTests
{
    [TestMethod]
    public void GetText_ReturnsKeyboardClipboardText()
    {
        // Arrange
        var handler = new DesktopKeyboardHandler();
        var keyboard = new FakeKeyboard { ClipboardText = "Hello" };
        handler.SetKeyboard(keyboard);
        var service = new DesktopClipboardService(handler);

        // Act
        var text = service.GetText();

        // Assert
        Assert.AreEqual("Hello", text);
    }

    [TestMethod]
    public void SetText_WritesKeyboardClipboardText()
    {
        // Arrange
        var handler = new DesktopKeyboardHandler();
        var keyboard = new FakeKeyboard();
        handler.SetKeyboard(keyboard);
        var service = new DesktopClipboardService(handler);

        // Act
        service.SetText("World");

        // Assert
        Assert.AreEqual("World", keyboard.ClipboardText);
    }

    [TestMethod]
    public void GetText_WithoutKeyboard_ReturnsNull()
    {
        // Arrange
        var service = new DesktopClipboardService(new DesktopKeyboardHandler());

        // Act
        var text = service.GetText();

        // Assert
        Assert.IsNull(text);
    }

    [TestMethod]
    public void SetText_WithoutKeyboard_DoesNotThrow()
    {
        // Arrange
        var service = new DesktopClipboardService(new DesktopKeyboardHandler());

        // Act & Assert
        service.SetText("ignored");
    }
}
