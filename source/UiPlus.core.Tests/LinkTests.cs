using PlusUi.core;

namespace UiPlus.core.Tests;

/// <summary>
/// Tests for Link control functionality including layout, alignment, and URL handling
/// </summary>
[TestClass]
public sealed class LinkTests
{
    [TestMethod]
    public void TestLinkMeasureAndArrange_NoMargin_LeftTopAligned()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com");
        var availableSize = new Size(100, 50);

        // Act
        link.Measure(availableSize);
        link.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.AreEqual(0, link.Position.X);
        Assert.AreEqual(0, link.Position.Y);
    }

    [TestMethod]
    public void TestLinkMeasureAndArrange_NoMargin_CenterTopAligned()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com")
            .SetHorizontalAlignment(HorizontalAlignment.Center);
        var availableSize = new Size(100, 50);

        // Act
        link.Measure(availableSize);
        link.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.AreEqual(50 - (link.ElementSize.Width / 2), link.Position.X);
        Assert.AreEqual(0, link.Position.Y);
    }

    [TestMethod]
    public void TestLinkMeasureAndArrange_NoMargin_RightTopAligned()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com")
            .SetHorizontalAlignment(HorizontalAlignment.Right);
        var availableSize = new Size(100, 50);

        // Act
        link.Measure(availableSize);
        link.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.AreEqual(100 - link.ElementSize.Width, link.Position.X);
        Assert.AreEqual(0, link.Position.Y);
    }

    [TestMethod]
    public void TestLinkMeasureAndArrange_WithMargin_CenterCenterAligned()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com")
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);

        // Act
        link.Measure(availableSize);
        link.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.AreEqual(50 - (link.ElementSize.Width / 2), link.Position.X);
        Assert.AreEqual(25 - (link.ElementSize.Height / 2), link.Position.Y);
    }

    [TestMethod]
    public void TestLink_SetUrl()
    {
        // Arrange & Act
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com");

        // Assert
        Assert.IsNotNull(link);
        Assert.AreEqual("https://example.com", link.Url);
    }

    [TestMethod]
    public void TestLink_SetUnderlineThickness()
    {
        // Arrange & Act
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com")
            .SetUnderlineThickness(2f);

        // Assert
        Assert.IsNotNull(link);
        Assert.AreEqual(2f, link.UnderlineThickness);
    }

    [TestMethod]
    public void TestLink_SetTextWrapping_NoWrap()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com")
            .SetTextWrapping(TextWrapping.NoWrap);

        // Assert
        Assert.IsNotNull(link);
    }

    [TestMethod]
    public void TestLink_SetTextWrapping_Wrap()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com")
            .SetTextWrapping(TextWrapping.Wrap);

        // Assert
        Assert.IsNotNull(link);
    }

    [TestMethod]
    public void TestLink_SetTextWrapping_WordWrap()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com")
            .SetTextWrapping(TextWrapping.WordWrap);

        // Assert
        Assert.IsNotNull(link);
    }

    [TestMethod]
    public void TestLink_SetMaxLines()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link with multiple words that should wrap")
            .SetUrl("https://example.com")
            .SetMaxLines(2);

        // Assert
        Assert.IsNotNull(link);
    }

    [TestMethod]
    public void TestLink_SetTextTruncation_End()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com")
            .SetTextTruncation(TextTruncation.End);

        // Assert
        Assert.IsNotNull(link);
    }

    [TestMethod]
    public void TestLink_ChainedSetters()
    {
        // Arrange & Act
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com")
            .SetTextWrapping(TextWrapping.WordWrap)
            .SetMaxLines(3)
            .SetTextTruncation(TextTruncation.End)
            .SetUnderlineThickness(1.5f)
            .SetTextSize(14);

        // Assert
        Assert.IsNotNull(link);
        Assert.AreEqual("https://example.com", link.Url);
        Assert.AreEqual(1.5f, link.UnderlineThickness);
    }

    [TestMethod]
    public void TestLink_InvokeCommand_WithValidUrl()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link")
            .SetUrl("https://example.com");

        // Act - InvokeCommand should not throw
        link.InvokeCommand();

        // Assert - Command was invoked without error
        Assert.IsNotNull(link);
    }

    [TestMethod]
    public void TestLink_InvokeCommand_WithoutUrl()
    {
        // Arrange
        var link = new Link()
            .SetText("Test Link");

        // Act - InvokeCommand should not throw even without URL
        link.InvokeCommand();

        // Assert - Command was invoked without error
        Assert.IsNotNull(link);
    }
}
