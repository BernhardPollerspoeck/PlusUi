using PlusUi.core;

namespace PlusUi.core.Tests;
/// <summary>
/// this class proves simple element alignment and margin calculation
/// </summary>
[TestClass]
public sealed class LabelTests
{
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_LeftTopAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label");
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(0, label.Position.X);
        Assert.AreEqual(0, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_CenterTopAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Center);
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(0, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_RightTopAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Right);
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width, label.Position.X);
        Assert.AreEqual(0, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_LeftCenterAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetVerticalAlignment(VerticalAlignment.Center);
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(0, label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_CenterCenterAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_RightCenterAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Right)
            .SetVerticalAlignment(VerticalAlignment.Center);
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width, label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_LeftBottomAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetVerticalAlignment(VerticalAlignment.Bottom);
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(0, label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_CenterBottomAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Bottom);
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_NoMargin_RightBottomAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Right)
            .SetVerticalAlignment(VerticalAlignment.Bottom);
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width, label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height, label.Position.Y);
    }

    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_LeftTopAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(10, label.Position.X);
        Assert.AreEqual(10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_CenterTopAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_RightTopAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Right)
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width - 10, label.Position.X);
        Assert.AreEqual(10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_LeftCenterAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(10, label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_CenterCenterAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_RightCenterAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Right)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width - 10, label.Position.X);
        Assert.AreEqual(25 - (label.ElementSize.Height / 2), label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_LeftBottomAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetVerticalAlignment(VerticalAlignment.Bottom)
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(10, label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height - 10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_CenterBottomAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Bottom)
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(50 - (label.ElementSize.Width / 2), label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height - 10, label.Position.Y);
    }
    [TestMethod]
    public void TestLabelMeasureAndArrange_WithMargin_RightBottomAligned()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetHorizontalAlignment(HorizontalAlignment.Right)
            .SetVerticalAlignment(VerticalAlignment.Bottom)
            .SetMargin(new Margin(10));
        var availableSize = new Size(100, 50);
        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));
        // Assert
        Assert.AreEqual(100 - label.ElementSize.Width - 10, label.Position.X);
        Assert.AreEqual(50 - label.ElementSize.Height - 10, label.Position.Y);
    }

    [TestMethod]
    public void TestLabel_SetTextWrapping_NoWrap()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetTextWrapping(TextWrapping.NoWrap);
        
        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetTextWrapping_Wrap()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetTextWrapping(TextWrapping.Wrap);
        
        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetTextWrapping_WordWrap()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetTextWrapping(TextWrapping.WordWrap);
        
        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetMaxLines()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetMaxLines(2);
        
        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetTextTruncation_None()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetTextTruncation(TextTruncation.None);
        
        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetTextTruncation_Start()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetTextTruncation(TextTruncation.Start);
        
        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetTextTruncation_Middle()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetTextTruncation(TextTruncation.Middle);
        
        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_SetTextTruncation_End()
    {
        // Arrange
        var label = new Label()
            .SetText("Test Label")
            .SetTextTruncation(TextTruncation.End);
        
        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_ChainedSetters()
    {
        // Arrange & Act
        var label = new Label()
            .SetText("Test Label")
            .SetTextWrapping(TextWrapping.WordWrap)
            .SetMaxLines(3)
            .SetTextTruncation(TextTruncation.End);

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestLabel_WordWrap_ShouldNotExpandToAvailableWidth()
    {
        // Arrange - long text that will wrap
        var label = new Label()
            .SetText("This is a test with some words that will wrap to multiple lines when measured.")
            .SetTextWrapping(TextWrapping.WordWrap);

        // Act - measure with large available width
        var availableSize = new Size(800, 600);
        label.Measure(availableSize);

        // Assert - ElementSize.Width should be the width of the longest wrapped line,
        // NOT the full available width (800)
        Console.WriteLine($"Available width: {availableSize.Width}");
        Console.WriteLine($"Label ElementSize: {label.ElementSize.Width} x {label.ElementSize.Height}");

        // The text should NOT take full available width
        Assert.IsLessThan(availableSize.Width, label.ElementSize.Width,
            $"Label width ({label.ElementSize.Width}) should be less than available width ({availableSize.Width}) when text wraps");
    }

    [TestMethod]
    public void TestLabel_WordWrap_InVStack_ShouldNotExpandToAvailableWidth()
    {
        // Arrange - VStack with label like in MainPage
        var label = new Label()
            .SetText("This is a long text that will wrap. It has multiple sentences to ensure wrapping happens correctly.")
            .SetTextWrapping(TextWrapping.WordWrap);

        var vstack = new VStack(label)
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);

        // Act - measure VStack with large available width
        var availableSize = new Size(800, 600);
        vstack.Measure(availableSize);
        vstack.Arrange(new Rect(0, 0, 800, 600));

        // Assert
        Console.WriteLine($"Available width: {availableSize.Width}");
        Console.WriteLine($"VStack ElementSize: {vstack.ElementSize.Width} x {vstack.ElementSize.Height}");
        Console.WriteLine($"Label ElementSize: {label.ElementSize.Width} x {label.ElementSize.Height}");

        // VStack should be as wide as its content (label), not full available width
        Assert.IsLessThan(availableSize.Width, vstack.ElementSize.Width,
            $"VStack width ({vstack.ElementSize.Width}) should be less than available ({availableSize.Width})");
    }

    [TestMethod]
    public void TestLabel_WordWrap_InHStackWithVStack_ShouldNotExpandToAvailableWidth()
    {
        // Arrange - simulates MainPage: HStack with sidebar and content VStack
        var label = new Label()
            .SetText("This is a long text that will wrap. It has multiple sentences to ensure wrapping happens correctly.")
            .SetTextWrapping(TextWrapping.WordWrap);

        var contentVStack = new VStack(label)
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);

        var sidebar = new Solid()
            .SetDesiredSize(new Size(150, 400));

        var hstack = new HStack(sidebar, contentVStack);

        // Act - measure and arrange with window size
        var windowSize = new Size(1000, 600);
        hstack.Measure(windowSize);
        hstack.Arrange(new Rect(0, 0, 1000, 600));

        // Assert
        Console.WriteLine($"Window width: {windowSize.Width}");
        Console.WriteLine($"HStack ElementSize: {hstack.ElementSize.Width} x {hstack.ElementSize.Height}");
        Console.WriteLine($"Sidebar ElementSize: {sidebar.ElementSize.Width} x {sidebar.ElementSize.Height}");
        Console.WriteLine($"VStack ElementSize: {contentVStack.ElementSize.Width} x {contentVStack.ElementSize.Height}");
        Console.WriteLine($"Label ElementSize: {label.ElementSize.Width} x {label.ElementSize.Height}");

        // VStack should NOT be stretched to fill remaining space (1000 - 150 = 850)
        var remainingWidth = windowSize.Width - sidebar.ElementSize.Width;
        Console.WriteLine($"Remaining width after sidebar: {remainingWidth}");

        Assert.IsLessThan(remainingWidth, contentVStack.ElementSize.Width,
            $"VStack width ({contentVStack.ElementSize.Width}) should be less than remaining ({remainingWidth})");
    }

    [TestMethod]
    public void TestLabel_WordWrap_WithRealMainPageText()
    {
        // Arrange - exact text from MainPage
        var longText = """
            PlusUi is a passion project born from the desire to create something truly cross-platform.
            No more platform-specific quirks, no more "it works differently on iOS" moments.
            Just pure, consistent UI rendering powered by SkiaSharp.

            I started this project because I was frustrated with existing solutions.
            MAUI was unreliable - every update broke something else, and debugging
            platform-specific issues became a full-time job. Avalonia had its own set
            of quirks. I wanted something that just works - everywhere, the same way, every time.

            What you see here is the result of countless late nights, debugging sessions,
            and "aha!" moments. Every control, every animation, every pixel is rendered
            by our own engine. No native controls, no platform abstractions - just Skia
            drawing directly to the screen.

            Select a control from the sidebar to explore what PlusUi can do.
            Each demo shows the control in action with real, working examples.
            Feel free to play around, break things, and discover what's possible.

            This is just the beginning. There's so much more to come.
            """;

        var label = new Label()
            .SetText(longText)
            .SetTextWrapping(TextWrapping.WordWrap);

        // Act - measure with large available width
        var availableSize = new Size(800, 600);
        label.Measure(availableSize);

        // Assert
        Console.WriteLine($"Available width: {availableSize.Width}");
        Console.WriteLine($"Label ElementSize: {label.ElementSize.Width} x {label.ElementSize.Height}");

        // Should be less than 800
        Assert.IsLessThan(availableSize.Width, label.ElementSize.Width,
            $"Label width ({label.ElementSize.Width}) should be less than available ({availableSize.Width})");
    }
}
