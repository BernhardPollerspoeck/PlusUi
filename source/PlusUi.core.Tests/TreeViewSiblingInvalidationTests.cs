using PlusUi.core;

namespace PlusUi.core.Tests;

/// <summary>
/// Repro for the FLINK "blank TreeView" bug:
/// A hover restyle on a SIBLING Button (Button.IsHovered setter calls InvalidateMeasure,
/// dirtying only the button's ancestor chain) makes the shared Grid re-run MeasureInternal
/// next frame. Grid's first pass probes EVERY child with dontStretch:true, which bypasses
/// the NeedsMeasure guard and re-runs TreeView.MeasureInternal. TreeView.MeasureInternal
/// destroys and recreates all item elements (Children.Clear + fresh template instances),
/// but the TreeView's own NeedsArrange stays false (the invalidation walked the BUTTON's
/// chain, not the TreeView's), so UiElement.Arrange skips ArrangeInternal and the fresh
/// labels are never positioned: they stay at Position (0,0), outside the TreeView's clip
/// rect => expanders/selection (drawn from the snapshot at the TreeView's own position)
/// remain visible while all item labels vanish.
/// </summary>
[TestClass]
public class TreeViewSiblingInvalidationTests
{
    private const float CanvasWidth = 800f;
    private const float CanvasHeight = 600f;
    private const float ToolbarHeight = 50f;

    private static (Grid grid, Button button, TreeView treeView, List<object> items) BuildLayout(int itemCount)
    {
        var items = new List<object>();
        for (var i = 0; i < itemCount; i++)
        {
            items.Add($"Item {i}");
        }

        var treeView = new TreeView()
            .SetItemsSource(items)
            .SetItemTemplate((item, depth) => new Label().SetText((string)item));
        treeView.SetHorizontalAlignment(HorizontalAlignment.Stretch);
        treeView.SetVerticalAlignment(VerticalAlignment.Stretch);

        var button = new Button().SetText("Start");

        var grid = new Grid()
            .AddRow(Row.Absolute, (int)ToolbarHeight)
            .AddRow(Row.Star, 1)
            .AddColumn(Column.Star, 1);
        grid.AddChild(button, row: 0, column: 0);
        grid.AddChild(treeView, row: 1, column: 0);

        return (grid, button, treeView, items);
    }

    private static void RunFrame(Grid grid)
    {
        grid.Measure(new Size(CanvasWidth, CanvasHeight));
        grid.Arrange(new Rect(0, 0, CanvasWidth, CanvasHeight));
    }

    [TestMethod]
    public void TreeView_SiblingButtonHover_ItemElementsStayArranged()
    {
        // Arrange: initial frame lays out toolbar button + tree
        var (grid, button, treeView, _) = BuildLayout(5);
        RunFrame(grid);

        Assert.AreEqual(5, treeView.Children.Count, "Sanity: 5 item elements after initial frame");
        var expectedPositions = treeView.Children.Select(c => c.Position).ToList();
        Assert.IsTrue(expectedPositions[0].Y >= ToolbarHeight,
            $"Sanity: first item must be arranged inside the tree viewport (y >= {ToolbarHeight}), was {expectedPositions[0].Y}");
        Assert.IsTrue(expectedPositions[1].Y > expectedPositions[0].Y,
            "Sanity: rows must be stacked");

        // Act: hover the sibling toolbar button (real hover path: InputService sets
        // IsHovered=true, Button.IsHovered setter calls InvalidateMeasure) and run a frame
        button.IsHovered = true;
        RunFrame(grid);

        // Assert: the tree's item elements must still be arranged at their previous positions
        Assert.AreEqual(5, treeView.Children.Count, "5 item elements after hover frame");
        for (var i = 0; i < 5; i++)
        {
            var actual = treeView.Children[i].Position;
            var expected = expectedPositions[i];
            Assert.AreEqual(expected.X, actual.X, 0.5f,
                $"Item {i} X position must survive a sibling hover invalidation (expected {expected.X}, was {actual.X})");
            Assert.AreEqual(expected.Y, actual.Y, 0.5f,
                $"Item {i} Y position must survive a sibling hover invalidation (expected {expected.Y}, was {actual.Y})");
        }
    }

    [TestMethod]
    public void TreeView_AfterHoverBlank_MouseWheelDoesNotRestore_ClickDoes()
    {
        // Arrange: content fits the viewport (5 * 32 = 160 < 550) so maxOffset == 0
        var (grid, button, treeView, items) = BuildLayout(5);
        RunFrame(grid);
        var expectedPositions = treeView.Children.Select(c => c.Position).ToList();

        button.IsHovered = true;
        RunFrame(grid);

        // Act 1: mouse wheel over the tree. Since 7d8a261 the ScrollOffset setter
        // early-returns when the clamped value is unchanged (maxOffset == 0), so no
        // InvalidateMeasure happens and nothing is re-arranged.
        treeView.HandleScroll(0, 10);
        RunFrame(grid);

        var restoredByWheel = true;
        for (var i = 0; i < treeView.Children.Count && i < expectedPositions.Count; i++)
        {
            if (Math.Abs(treeView.Children[i].Position.Y - expectedPositions[i].Y) > 0.5f)
            {
                restoredByWheel = false;
            }
        }

        // Act 2: clicking a row (SetSelectedItem invalidates the TREE's own chain)
        treeView.SetSelectedItem(items[1]);
        RunFrame(grid);

        var restoredByClick = true;
        for (var i = 0; i < treeView.Children.Count && i < expectedPositions.Count; i++)
        {
            if (Math.Abs(treeView.Children[i].Position.Y - expectedPositions[i].Y) > 0.5f)
            {
                restoredByClick = false;
            }
        }

        Assert.IsTrue(restoredByClick, "Clicking a row must restore the arrangement");
        Assert.IsTrue(restoredByWheel,
            "Mouse wheel over a fitting tree must also leave/restore the arrangement (fails since 7d8a261: setter early-returns without InvalidateMeasure)");
    }

    [TestMethod]
    public void TreeView_ScrollOffset_SurvivesSiblingHoverInvalidation()
    {
        // Arrange: 30 items * 32px = 960px content, viewport = 550px => maxOffset = 410.
        // Grid's probe pass measures the TreeView with the FULL grid size (800x600), and
        // TreeView.MeasureInternal clamps _scrollOffset against that probe height:
        // max(0, 960 - 600) = 360 < 400, silently losing part of a legitimate offset.
        var (grid, button, treeView, _) = BuildLayout(30);
        RunFrame(grid);

        treeView.ScrollOffset = 400f;
        Assert.AreEqual(400f, treeView.ScrollOffset, 0.01f, "Sanity: offset 400 is within maxOffset 410");
        RunFrame(grid);
        Assert.AreEqual(400f, treeView.ScrollOffset, 0.01f,
            "Offset must survive the frame its own invalidation triggered");

        // Act: hover the sibling button, next frame re-measures through the Grid probe
        button.IsHovered = true;
        RunFrame(grid);

        // Assert
        Assert.AreEqual(400f, treeView.ScrollOffset, 0.01f,
            "A hover on an unrelated sibling must not move the tree's scroll position");
    }
}
