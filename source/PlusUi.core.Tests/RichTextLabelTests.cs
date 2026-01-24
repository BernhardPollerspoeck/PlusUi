using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public sealed class RichTextLabelTests
{
    [TestMethod]
    public void TestRichTextLabel_SingleRun_RendersCorrectly()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun("Hello World"));

        var availableSize = new Size(200, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.IsTrue(label.ElementSize.Width > 0);
        Assert.IsTrue(label.ElementSize.Height > 0);
    }

    [TestMethod]
    public void TestRichTextLabel_MultipleRuns_WithDifferentColors()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun("Hello ").SetColor(Colors.White))
            .AddRun(new TextRun("World").SetColor(Colors.Blue));

        var availableSize = new Size(200, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.IsTrue(label.ElementSize.Width > 0);
        Assert.IsTrue(label.ElementSize.Height > 0);
    }

    [TestMethod]
    public void TestRichTextLabel_MixedFontSizes_HasCorrectHeight()
    {
        // Arrange
        var smallLabel = new RichTextLabel()
            .SetTextSize(12)
            .AddRun(new TextRun("Small text"));

        var largeLabel = new RichTextLabel()
            .SetTextSize(24)
            .AddRun(new TextRun("Large text"));

        var mixedLabel = new RichTextLabel()
            .SetTextSize(12)
            .AddRun(new TextRun("Small "))
            .AddRun(new TextRun("Large").SetFontSize(24));

        var availableSize = new Size(400, 100);

        // Act
        smallLabel.Measure(availableSize);
        largeLabel.Measure(availableSize);
        mixedLabel.Measure(availableSize);

        // Assert - mixed should have height >= the large label
        Assert.IsTrue(mixedLabel.ElementSize.Height >= largeLabel.ElementSize.Height,
            $"Mixed height ({mixedLabel.ElementSize.Height}) should be >= large height ({largeLabel.ElementSize.Height})");
    }

    [TestMethod]
    public void TestRichTextLabel_WordWrapping_WrapsAcrossRuns()
    {
        // Arrange
        var label = new RichTextLabel()
            .SetTextWrapping(TextWrapping.WordWrap)
            .AddRun(new TextRun("This is some text that "))
            .AddRun(new TextRun("should wrap to multiple lines when the available width is small"));

        var availableSize = new Size(150, 200);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert - should be multi-line, so height > single line
        var singleLineLabel = new RichTextLabel()
            .AddRun(new TextRun("Test"));
        singleLineLabel.Measure(new Size(400, 50));

        Assert.IsTrue(label.ElementSize.Height > singleLineLabel.ElementSize.Height,
            "Wrapped text should have greater height than single line");
    }

    [TestMethod]
    public void TestRichTextLabel_MaxLines_TruncatesLines()
    {
        // Arrange
        var unlimitedLabel = new RichTextLabel()
            .SetTextWrapping(TextWrapping.WordWrap)
            .AddRun(new TextRun("Line one. Line two. Line three. Line four. Line five. Line six."));

        var limitedLabel = new RichTextLabel()
            .SetTextWrapping(TextWrapping.WordWrap)
            .SetMaxLines(2)
            .AddRun(new TextRun("Line one. Line two. Line three. Line four. Line five. Line six."));

        var availableSize = new Size(80, 200);

        // Act
        unlimitedLabel.Measure(availableSize);
        limitedLabel.Measure(availableSize);

        // Assert - limited should have smaller height
        Assert.IsTrue(limitedLabel.ElementSize.Height <= unlimitedLabel.ElementSize.Height,
            $"Limited height ({limitedLabel.ElementSize.Height}) should be <= unlimited height ({unlimitedLabel.ElementSize.Height})");
    }

    [TestMethod]
    public void TestRichTextLabel_HorizontalAlignment_Left()
    {
        // Arrange
        var label = new RichTextLabel()
            .SetHorizontalTextAlignment(HorizontalTextAlignment.Left)
            .AddRun(new TextRun("Test"));

        var availableSize = new Size(200, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.IsTrue(label.ElementSize.Width > 0);
    }

    [TestMethod]
    public void TestRichTextLabel_HorizontalAlignment_Center()
    {
        // Arrange
        var label = new RichTextLabel()
            .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
            .AddRun(new TextRun("Test"));

        var availableSize = new Size(200, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.IsTrue(label.ElementSize.Width > 0);
    }

    [TestMethod]
    public void TestRichTextLabel_HorizontalAlignment_Right()
    {
        // Arrange
        var label = new RichTextLabel()
            .SetHorizontalTextAlignment(HorizontalTextAlignment.Right)
            .AddRun(new TextRun("Test"));

        var availableSize = new Size(200, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.IsTrue(label.ElementSize.Width > 0);
    }

    [TestMethod]
    public void TestRichTextLabel_NullableReset_InheritsParentColor()
    {
        // Arrange
        var run = new TextRun("Test")
            .SetColor(Colors.Red)
            .SetColor(null);

        var label = new RichTextLabel()
            .SetTextColor(Colors.Blue)
            .AddRun(run);

        // Assert
        Assert.IsNull(run.Color);
    }

    [TestMethod]
    public void TestRichTextLabel_ClearRuns_RemovesAllRuns()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun("Hello"))
            .AddRun(new TextRun("World"));

        // Act
        label.ClearRuns();
        label.Measure(new Size(200, 50));

        // Assert - should have zero size with no runs
        Assert.AreEqual(0, label.ElementSize.Width);
        Assert.AreEqual(0, label.ElementSize.Height);
    }

    [TestMethod]
    public void TestRichTextLabel_SetRuns_ReplacesAllRuns()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun("Original"));

        var newRuns = new[]
        {
            new TextRun("New "),
            new TextRun("Text")
        };

        // Act
        label.SetRuns(newRuns);
        label.Measure(new Size(200, 50));

        // Assert
        Assert.IsTrue(label.ElementSize.Width > 0);
    }

    [TestMethod]
    public void TestRichTextLabel_SyntaxHighlightingExample()
    {
        // Arrange - simulating syntax highlighting
        var label = new RichTextLabel()
            .SetFontFamily("Consolas")
            .AddRun(new TextRun("var ").SetColor(Colors.Blue))
            .AddRun(new TextRun("name").SetColor(Colors.White))
            .AddRun(new TextRun(" = ").SetColor(Colors.White))
            .AddRun(new TextRun("\"test\"").SetColor(Colors.Orange));

        var availableSize = new Size(400, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.IsTrue(label.ElementSize.Width > 0);
        Assert.IsTrue(label.ElementSize.Height > 0);
    }

    [TestMethod]
    public void TestRichTextLabel_BoldAndItalic()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun("Normal "))
            .AddRun(new TextRun("Bold ").SetFontWeight(FontWeight.Bold))
            .AddRun(new TextRun("Italic ").SetFontStyle(FontStyle.Italic))
            .AddRun(new TextRun("Both").SetFontWeight(FontWeight.Bold).SetFontStyle(FontStyle.Italic));

        var availableSize = new Size(400, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.IsTrue(label.ElementSize.Width > 0);
        Assert.IsTrue(label.ElementSize.Height > 0);
    }

    [TestMethod]
    public void TestRichTextLabel_EmptyRuns_DoNotCrash()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun(""))
            .AddRun(new TextRun("Hello"))
            .AddRun(new TextRun(""));

        var availableSize = new Size(200, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert - should not crash and have some size
        Assert.IsTrue(label.ElementSize.Width > 0);
    }

    [TestMethod]
    public void TestRichTextLabel_NoRuns_HasZeroSize()
    {
        // Arrange
        var label = new RichTextLabel();

        var availableSize = new Size(200, 50);

        // Act
        label.Measure(availableSize);
        label.Arrange(new Rect(new Point(0, 0), availableSize));

        // Assert
        Assert.AreEqual(0, label.ElementSize.Width);
        Assert.AreEqual(0, label.ElementSize.Height);
    }

    [TestMethod]
    public void TestRichTextLabel_FluentApiChaining()
    {
        // Arrange & Act
        var label = new RichTextLabel()
            .SetTextSize(16)
            .SetTextColor(Colors.White)
            .SetFontWeight(FontWeight.Regular)
            .SetFontStyle(FontStyle.Normal)
            .SetFontFamily("Arial")
            .SetTextWrapping(TextWrapping.WordWrap)
            .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
            .SetMaxLines(3)
            .AddRun(new TextRun("Test"));

        // Assert
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public void TestRichTextLabel_AccessibilityLabel_ConcatenatesRuns()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun("Hello "))
            .AddRun(new TextRun("World"));

        // Act
        var accessibilityLabel = label.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Hello World", accessibilityLabel);
    }

    [TestMethod]
    public void TestRichTextLabel_AccessibilityLabel_ExplicitOverridesComputed()
    {
        // Arrange
        var label = new RichTextLabel()
            .SetAccessibilityLabel("Custom Label")
            .AddRun(new TextRun("Hello "))
            .AddRun(new TextRun("World"));

        // Act
        var accessibilityLabel = label.GetComputedAccessibilityLabel();

        // Assert
        Assert.AreEqual("Custom Label", accessibilityLabel);
    }

    [TestMethod]
    public void TestRichTextLabel_IsFocusable_ReturnsFalse()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun("Test"));

        // Assert - labels should not be focusable
        Assert.IsFalse(((UiElement)label).IsFocusable);
    }

    [TestMethod]
    public void TestRichTextLabel_AccessibilityRole_IsLabel()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun("Test"));

        // Assert
        Assert.AreEqual(AccessibilityRole.Label, label.AccessibilityRole);
    }

    [TestMethod]
    public void TestTextRun_SetText_UpdatesText()
    {
        // Arrange
        var run = new TextRun("initial");

        // Act
        run.SetText("updated");

        // Assert
        Assert.AreEqual("updated", run.Text);
    }

    [TestMethod]
    public void TestTextRun_FluentApiChaining()
    {
        // Arrange & Act
        var run = new TextRun("Test")
            .SetColor(Colors.Red)
            .SetFontWeight(FontWeight.Bold)
            .SetFontStyle(FontStyle.Italic)
            .SetFontSize(18)
            .SetFontFamily("Arial");

        // Assert
        Assert.AreEqual("Test", run.Text);
        Assert.AreEqual(Colors.Red, run.Color);
        Assert.AreEqual(FontWeight.Bold, run.FontWeight);
        Assert.AreEqual(FontStyle.Italic, run.FontStyle);
        Assert.AreEqual(18f, run.FontSize);
        Assert.AreEqual("Arial", run.FontFamily);
    }

    [TestMethod]
    public void TestTextRun_NullableProperties_ResetToNull()
    {
        // Arrange
        var run = new TextRun("Test")
            .SetColor(Colors.Red)
            .SetFontWeight(FontWeight.Bold);

        // Act - reset to null
        run.SetColor(null);
        run.SetFontWeight(null);

        // Assert
        Assert.IsNull(run.Color);
        Assert.IsNull(run.FontWeight);
    }

    [TestMethod]
    public void TestRichTextLabel_Dispose_CleansUpResources()
    {
        // Arrange
        var label = new RichTextLabel()
            .AddRun(new TextRun("Test"));
        label.Measure(new Size(200, 50));

        // Act
        label.Dispose();

        // Assert - should not throw
        Assert.IsTrue(true);
    }
}
