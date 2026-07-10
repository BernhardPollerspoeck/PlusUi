using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusUi.core;
using PlusUi.core.Services;

namespace PlusUi.core.Tests;

[TestClass]
[DoNotParallelize]
public class EntryClipboardTests
{
    private static FakeClipboardService Clipboard =>
        (FakeClipboardService)ServiceProviderService.ServiceProvider!.GetRequiredService<IClipboardService>();

    [TestInitialize]
    public void ResetClipboard()
    {
        Clipboard.Text = null;
    }

    [TestMethod]
    public void Entry_Paste_InsertsClipboardTextAtCursor()
    {
        // Arrange
        var entry = new Entry();
        entry.SetText("Hello");
        Clipboard.Text = "!!";

        // Act
        entry.HandleInput(PlusKey.End, shift: false, ctrl: false);
        entry.HandleInput(PlusKey.V, shift: false, ctrl: true);

        // Assert
        Assert.AreEqual("Hello!!", entry.Text);
    }

    [TestMethod]
    public void Entry_Paste_ReplacesSelection()
    {
        // Arrange
        var entry = new Entry();
        entry.SetText("Hello");
        Clipboard.Text = "Bye";

        // Act
        entry.HandleInput(PlusKey.A, shift: false, ctrl: true);
        entry.HandleInput(PlusKey.V, shift: false, ctrl: true);

        // Assert
        Assert.AreEqual("Bye", entry.Text);
    }

    [TestMethod]
    public void Entry_Paste_RespectsMaxLength()
    {
        // Arrange
        var entry = new Entry();
        entry.SetText("abc").SetMaxLength(5);
        Clipboard.Text = "defgh";

        // Act
        entry.HandleInput(PlusKey.End, shift: false, ctrl: false);
        entry.HandleInput(PlusKey.V, shift: false, ctrl: true);

        // Assert
        Assert.AreEqual("abcde", entry.Text);
    }

    [TestMethod]
    public void Entry_Paste_WithEmptyClipboard_DoesNothing()
    {
        // Arrange
        var entry = new Entry();
        entry.SetText("Hello");

        // Act
        entry.HandleInput(PlusKey.End, shift: false, ctrl: false);
        entry.HandleInput(PlusKey.V, shift: false, ctrl: true);

        // Assert
        Assert.AreEqual("Hello", entry.Text);
    }

    [TestMethod]
    public void Entry_Copy_WritesSelectionToClipboard()
    {
        // Arrange
        var entry = new Entry();
        entry.SetText("Hello");

        // Act
        entry.HandleInput(PlusKey.A, shift: false, ctrl: true);
        entry.HandleInput(PlusKey.C, shift: false, ctrl: true);

        // Assert
        Assert.AreEqual("Hello", Clipboard.Text);
        Assert.AreEqual("Hello", entry.Text);
    }

    [TestMethod]
    public void Entry_Cut_WritesSelectionToClipboardAndDeletesIt()
    {
        // Arrange
        var entry = new Entry();
        entry.SetText("Hello");

        // Act
        entry.HandleInput(PlusKey.A, shift: false, ctrl: true);
        entry.HandleInput(PlusKey.X, shift: false, ctrl: true);

        // Assert
        Assert.AreEqual("Hello", Clipboard.Text);
        Assert.AreEqual(string.Empty, entry.Text);
    }
}
