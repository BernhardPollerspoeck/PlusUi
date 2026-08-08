namespace PlusUi.core.Tests;

/// <summary>
/// Regression tests for a layout that ignores a changed available size.
/// <para>
/// Invalidation only ever travels up to the root, so nothing marks a descendant dirty when
/// the window is resized. Measuring the tree again therefore used to return every cached
/// child size unchanged: the window grew, the content did not follow, and it stayed that way
/// until some unrelated event — hovering a button, moving the mouse over a control — dirtied
/// a path through the tree and dragged the layout along with it. The bug reads like a
/// rendering problem and is a measuring one.
/// </para>
/// <para>
/// Each pass measures and arranges, because that is what a frame does. Asserting after a
/// bare Measure would test a half-finished layout: stretch is resolved in Arrange.
/// </para>
/// </summary>
[TestClass]
public class ResizeRemeasureTests
{
    /// <summary>A stretching child, built the way the existing layout tests build one.</summary>
    private static Grid StretchChild() =>
        new Grid()
            .AddColumn(Column.Star, 1)
            .AddRow(Row.Auto)
            .AddChild(new Solid(null, 40, new Color(255, 0, 0)), row: 0, column: 0);

    private static Grid Host(UiElement child) =>
        new Grid()
            .AddColumn(Column.Star, 1)
            .AddRow(Row.Star, 1)
            .AddChild(child, row: 0, column: 0);

    private static void Frame(UiElement root, float width, float height)
    {
        root.Measure(new Size(width, height));
        root.Arrange(new Rect(0, 0, width, height));
    }

    [TestMethod]
    public void Resize_Larger_ChildFollowsWithoutInvalidation()
    {
        var child = StretchChild();
        var root = Host(child);

        Frame(root, 400, 300);
        Assert.AreEqual(400, child.ElementSize.Width, 0.5);

        // The window grew. Nothing calls InvalidateMeasure on the child - that is precisely
        // the situation being tested, not an omission.
        Frame(root, 900, 300);

        Assert.AreEqual(900, child.ElementSize.Width, 0.5,
            $"Child should follow the new available width, but stayed at {child.ElementSize.Width}");
    }

    [TestMethod]
    public void Resize_Smaller_ChildFollowsWithoutInvalidation()
    {
        var child = StretchChild();
        var root = Host(child);

        Frame(root, 900, 300);
        Assert.AreEqual(900, child.ElementSize.Width, 0.5);

        // Shrinking is the direction that pushes content off the edge of the window, so it
        // gets its own test rather than being assumed symmetric with growing.
        Frame(root, 400, 300);

        Assert.AreEqual(400, child.ElementSize.Width, 0.5,
            $"Child should follow the new available width, but stayed at {child.ElementSize.Width}");
    }

    [TestMethod]
    public void Resize_PropagatesThroughSeveralLevels()
    {
        // The depth is the point: the re-measure has to reach the bottom, and it does because
        // each level hands a different available size to the next.
        var leaf = StretchChild();
        var middle = new VStack(leaf);
        var root = Host(middle);

        Frame(root, 500, 400);
        Assert.AreEqual(500, leaf.ElementSize.Width, 0.5);

        Frame(root, 800, 400);

        Assert.AreEqual(800, leaf.ElementSize.Width, 0.5,
            $"Leaf below a VStack should follow the resize, but stayed at {leaf.ElementSize.Width}");
    }

    [TestMethod]
    public void SameSizeTwice_LeavesTheLayoutAlone()
    {
        // The other half of the contract. A changed available size is a reason to measure
        // again; an unchanged one must not become an excuse to redo the work every frame.
        var child = StretchChild();
        var root = Host(child);

        Frame(root, 400, 300);
        var first = child.ElementSize;

        Frame(root, 400, 300);

        Assert.AreEqual(first.Width, child.ElementSize.Width, 0.001);
        Assert.AreEqual(first.Height, child.ElementSize.Height, 0.001);
    }
}
