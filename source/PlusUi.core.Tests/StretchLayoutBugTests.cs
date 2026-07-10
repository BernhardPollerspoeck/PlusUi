using PlusUi.core;

namespace PlusUi.core.Tests;

/// <summary>
/// Regression tests for stretch children resolving against the wrong reference width.
/// Symptom 1: a Stretch child of a VStack sitting in a grid cell with X-offset is arranged
/// against the outer grid instead of the VStack and overflows to the right.
/// Symptom 2: a Stretch child collapses to its natural width when an unrelated element
/// triggers a re-layout (e.g. hover), because the corrective arrange pass is skipped.
/// </summary>
[TestClass]
public class StretchLayoutBugTests
{
    [TestMethod]
    public void VStack_InOffsetGridCell_StretchChild_StaysWithinCellWidth()
    {
        // Arrange - outer grid: Star | Absolute 740 | Star, VStack in the middle column
        var stretchChild = new Grid()
            .AddColumn(Column.Star, 1)
            .AddRow(Row.Auto)
            .AddChild(new Solid(null, 40, new Color(255, 0, 0)), row: 0, column: 0);

        var stack = new VStack(
            new Label().SetText("content-sized: ok"),
            stretchChild);

        var outer = new Grid()
            .AddColumn(Column.Star, 1)
            .AddColumn(Column.Absolute, 740)
            .AddColumn(Column.Star, 1)
            .AddRow(Row.Star, 1)
            .AddChild(stack, row: 0, column: 1);

        // Act - outer grid fills a wider container with an X-offset
        outer.Measure(new Size(870, 600));
        outer.Arrange(new Rect(20, 0, 870, 600));

        // Assert - the VStack itself sits correctly in its 740 cell
        Assert.AreEqual(740, stack.ElementSize.Width, 0.5,
            $"VStack should be exactly as wide as its cell, but was {stack.ElementSize.Width}");

        // ...and its stretch child must not be wider than the VStack
        Assert.AreEqual(740, stretchChild.ElementSize.Width, 0.5,
            $"Stretch child should resolve against the VStack width (740), but was {stretchChild.ElementSize.Width}");

        var stackRight = stack.Position.X + stack.ElementSize.Width;
        var childRight = stretchChild.Position.X + stretchChild.ElementSize.Width;
        Assert.IsTrue(childRight <= stackRight + 0.5,
            $"Stretch child (right edge {childRight}) must not overflow its parent VStack (right edge {stackRight})");
    }

    [TestMethod]
    public void VStack_StretchChild_KeepsWidth_WhenUnrelatedElementTriggersRelayout()
    {
        // Arrange - sidebar with a stretch button, main content in a second column
        var stretchButton = new Button()
            .SetText("Footer")
            .SetHorizontalAlignment(HorizontalAlignment.Stretch);

        var sidebar = new VStack(
            new Label().SetText("Sidebar"),
            stretchButton)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch);

        var mainContent = new Label().SetText("Main content");

        var root = new Grid()
            .AddColumn(Column.Absolute, 240)
            .AddColumn(Column.Star, 1)
            .AddRow(Row.Star, 1)
            .AddChild(sidebar, row: 0, column: 0)
            .AddChild(mainContent, row: 0, column: 1);

        root.Measure(new Size(1000, 600));
        root.Arrange(new Rect(0, 0, 1000, 600));

        var initialWidth = stretchButton.ElementSize.Width;
        Assert.AreEqual(240, initialWidth, 0.5,
            $"Sanity: stretch button should initially fill the 240 sidebar column, but was {initialWidth}");

        // Act - an unrelated element invalidates (this is what hover does), then a
        // normal layout pass runs with unchanged bounds
        mainContent.InvalidateMeasure();
        root.Measure(new Size(1000, 600));
        root.Arrange(new Rect(0, 0, 1000, 600));

        // Assert - the stretch button must keep its full width
        Assert.AreEqual(initialWidth, stretchButton.ElementSize.Width, 0.5,
            $"Stretch button collapsed from {initialWidth} to {stretchButton.ElementSize.Width} after an unrelated re-layout");
    }
}
