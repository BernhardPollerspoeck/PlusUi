namespace PlusUi.core.Tests;

/// <summary>
/// Covers <see cref="UiTextElement.GetVerticalTextOffset"/>, the one piece of the feature that
/// can be checked without a canvas. The offset is what both branches of Label add to the
/// baseline, so getting it right is what makes the alignment right.
/// </summary>
[TestClass]
public class VerticalTextAlignmentTests
{
    private const float TextSize = 20f;

    /// <summary>
    /// The height one line occupies, derived through the public contract rather than read off
    /// the font: with Bottom alignment the offset <b>is</b> the leftover space, so subtracting
    /// it from a known container height leaves the text height. Deriving it this way keeps the
    /// tests measuring what callers can observe.
    /// </summary>
    private static float LineHeight()
    {
        const float probe = 400f;
        var label = Sized(VerticalTextAlignment.Bottom);

        return probe - label.GetVerticalTextOffset(probe, 1);
    }

    private static Label Sized(VerticalTextAlignment alignment) =>
        (Label)new Label()
            .SetText("Text")
            .SetTextSize(TextSize)
            .SetVerticalTextAlignment(alignment);

    [TestMethod]
    public void Default_IsTop()
    {
        // Anything else would move text in every existing application on upgrade.
        var label = new Label();

        Assert.AreEqual(VerticalTextAlignment.Top, label.VerticalTextAlignment);
    }

    [TestMethod]
    public void Top_IsAlwaysZero()
    {
        var label = Sized(VerticalTextAlignment.Top);

        Assert.AreEqual(0f, label.GetVerticalTextOffset(500f, 1));
        Assert.AreEqual(0f, label.GetVerticalTextOffset(500f, 4));
        Assert.AreEqual(0f, label.GetVerticalTextOffset(0f, 1));
    }

    [TestMethod]
    public void Center_HalvesTheLeftoverSpace()
    {
        var label = Sized(VerticalTextAlignment.Center);
        var line = LineHeight();

        var offset = label.GetVerticalTextOffset(line + 40f, 1);

        Assert.AreEqual(20f, offset, 0.01f);
    }

    [TestMethod]
    public void Bottom_TakesAllTheLeftoverSpace()
    {
        var label = Sized(VerticalTextAlignment.Bottom);
        var line = LineHeight();

        var offset = label.GetVerticalTextOffset(line + 40f, 1);

        Assert.AreEqual(40f, offset, 0.01f);
    }

    [TestMethod]
    public void Center_AccountsForEveryLine()
    {
        // Three lines occupy three line heights; centring one line's worth would push a
        // wrapped paragraph off the bottom.
        var label = Sized(VerticalTextAlignment.Center);
        var line = LineHeight();

        var offset = label.GetVerticalTextOffset((line * 3) + 60f, 3);

        Assert.AreEqual(30f, offset, 0.01f);
    }

    [TestMethod]
    public void LineCountBelowOne_CountsAsOne()
    {
        // An empty label still occupies a line, and a zero would make it "fit" any height and
        // report an offset large enough to push the caret out of view.
        var label = Sized(VerticalTextAlignment.Center);

        Assert.AreEqual(
            label.GetVerticalTextOffset(200f, 1),
            label.GetVerticalTextOffset(200f, 0),
            0.01f);
    }

    [TestMethod]
    public void TextTallerThanElement_StaysAtTheTop()
    {
        // Shifting here would move the FIRST line out of the clip rectangle, so the reader
        // would lose the beginning of the text instead of the end.
        var center = Sized(VerticalTextAlignment.Center);
        var bottom = Sized(VerticalTextAlignment.Bottom);

        Assert.AreEqual(0f, center.GetVerticalTextOffset(4f, 1));
        Assert.AreEqual(0f, bottom.GetVerticalTextOffset(4f, 3));
    }

    [TestMethod]
    public void ExactFit_ProducesNoOffset()
    {
        var label = Sized(VerticalTextAlignment.Center);
        var line = LineHeight();

        Assert.AreEqual(0f, label.GetVerticalTextOffset(line, 1), 0.01f);
    }

    [TestMethod]
    public void OffsetUsesFontMetrics_NotTextSize()
    {
        // The distinction the whole fix rests on: a line is taller than its em size, so
        // centring against TextSize leaves the text sitting too low by a predictable amount.
        var label = Sized(VerticalTextAlignment.Center);
        var line = LineHeight();

        Assert.IsTrue(line > TextSize, "a line should be taller than the em size");

        var container = line + 40f;
        var naive = (container - TextSize) / 2f;

        Assert.AreNotEqual(naive, label.GetVerticalTextOffset(container, 1), 0.01f);
    }

    [TestMethod]
    public void SetVerticalTextAlignment_IsFluentAndSticks()
    {
        var label = new Label();

        var returned = label.SetVerticalTextAlignment(VerticalTextAlignment.Bottom);

        Assert.AreSame(label, returned);
        Assert.AreEqual(VerticalTextAlignment.Bottom, label.VerticalTextAlignment);
    }

    [TestMethod]
    public void BindVerticalTextAlignment_AppliesTheBoundValue()
    {
        var source = new AlignmentSource { Alignment = VerticalTextAlignment.Center };
        var label = new Label();

        label.BindVerticalTextAlignment(() => source.Alignment);

        Assert.AreEqual(VerticalTextAlignment.Center, label.VerticalTextAlignment);
    }

    private sealed class AlignmentSource
    {
        public VerticalTextAlignment Alignment { get; set; }
    }
}
