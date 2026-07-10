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
    public void GetText_WhenClipboardHasNoText_ReturnsNull()
    {
        // Arrange
        var handler = new DesktopKeyboardHandler();
        var keyboard = new FakeKeyboard
        {
            ClipboardGetException = new Silk.NET.GLFW.GlfwException("FormatUnavailable: Win32: Failed to convert clipboard to string")
        };
        handler.SetKeyboard(keyboard);
        var service = new DesktopClipboardService(handler);

        // Act
        var text = service.GetText();

        // Assert
        Assert.IsNull(text);
    }

    [TestMethod]
    public void SetText_WhenClipboardUnavailable_DoesNotThrow()
    {
        // Arrange
        var handler = new DesktopKeyboardHandler();
        var keyboard = new FakeKeyboard
        {
            ClipboardSetException = new Silk.NET.GLFW.GlfwException("PlatformError: Win32: Failed to open clipboard")
        };
        handler.SetKeyboard(keyboard);
        var service = new DesktopClipboardService(handler);

        // Act & Assert
        service.SetText("ignored");
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
