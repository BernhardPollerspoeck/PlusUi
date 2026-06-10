using System.Linq;
using PlusUi.core;

namespace PlusUi.core.Tests;

/// <summary>
/// Failing tests that PROVE the layout/behaviour bugs reported in uifix.md.
/// These are RED on purpose - they document the bugs for review before any core fix.
/// Each test maps to an entry in uifix.md.
/// </summary>
[TestClass]
public sealed class UiFixTests
{
    // uifix.md > Separator: vertical separator makes the row extremely tall.
    // HStack measures non-stretch children with dontStretch=true (natural size), but
    // Separator.MeasureInternal returns the FULL available height for a vertical separator,
    // so the row inflates to the available height (huge inside a ScrollView).
    [TestMethod]
    public void Separator_Vertical_InHStack_DoesNotInflateRowToAvailableHeight()
    {
        //Arrange
        var hstack = new HStack(
            new Solid(20, 20),
            new Separator().SetOrientation(Orientation.Vertical),
            new Solid(20, 20));

        //Act
        hstack.Measure(new Size(200, 200));
        hstack.Arrange(new Rect(0, 0, 200, 200));

        //Assert: row should be as tall as its content (20), not the full available height (200)
        Assert.AreEqual(20f, hstack.ElementSize.Height, 0.5f,
            "Vertical Separator must not grow the HStack to the full available height.");
    }

    // uifix.md > Separator (root cause, isolated): a vertical Separator asked for its natural
    // size (dontStretch = true) must not claim the full available height.
    [TestMethod]
    public void Separator_Vertical_NaturalMeasure_DoesNotClaimFullAvailableHeight()
    {
        //Arrange
        var separator = (Separator)new Separator().SetOrientation(Orientation.Vertical);

        //Act
        separator.Measure(new Size(200, 200), dontStretch: true);

        //Assert
        Assert.AreNotEqual(200f, separator.ElementSize.Height,
            "A vertical Separator measured for its natural size must not claim the full available height.");
    }

    // uifix.md > All pages: scroll does not reach the end.
    // ScrollView computes maxOffset = content.ElementSize.Height - viewport, ignoring the
    // content's own Margin. With a margin-bearing content the last margin pixels are unreachable.
    [TestMethod]
    public void ScrollView_MaxScrollOffset_IncludesContentMargin()
    {
        //Arrange: content 1000 tall with 100 top + 100 bottom margin -> laid-out extent 1200
        var content = new Solid(200, 1000).SetMargin(new Margin(0, 100, 0, 100));
        var scrollView = new ScrollView(content).SetCanScrollHorizontally(false);

        //Act: viewport 400 tall; scroll past the end so VerticalOffset clamps to the max
        scrollView.Measure(new Size(200, 400));
        scrollView.Arrange(new Rect(0, 0, 200, 400));
        scrollView.SetVerticalOffset(99999);

        //Assert: to reveal the whole content incl. margin, max scroll must be 1200 - 400 = 800.
        // Current (buggy) value is 600 (= 1000 - 400), ignoring the 200px content margin.
        Assert.AreEqual(800f, scrollView.VerticalOffset, 0.5f,
            "ScrollView max scroll offset must include the content's margin.");
    }

    // uifix.md > TreeView: children have too little indentation.
    // ArrangeInternal adds the expander gutter only for nodes that HAVE children, so a leaf
    // child is placed at depth*indentation only - barely past its parent's text.
    [TestMethod]
    public void TreeView_ExpandedLeafChild_IsIndentedAFullLevelPastParent()
    {
        //Arrange
        var leaf = new Category { Name = "Leaf" };
        var root = new Category { Name = "Root", SubCategories = [leaf] };

        var treeView = new TreeView()
            .SetItemHeight(32f)
            .SetIndentation(20f)
            .SetExpanderSize(16f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemTemplate((item, _) => new Label().SetText(((Category)item).Name))
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        //Act
        treeView.ExpandNode(root);
        treeView.Measure(new Size(300, 300), true);
        treeView.Arrange(new Rect(0, 0, 300, 300));

        //Assert: realized rows are [0]=root, [1]=leaf child; child must be indented a full level
        Assert.HasCount(2, treeView.Children, "root + 1 expanded child should be realized");
        var rootEl = treeView.Children[0];
        var childEl = treeView.Children[1];
        Assert.IsGreaterThanOrEqualTo(rootEl.Position.X + treeView.Indentation, childEl.Position.X,
            $"Child X ({childEl.Position.X}) should be >= parent X ({rootEl.Position.X}) + one indentation level ({treeView.Indentation}).");
    }

    // uifix.md > ComboBox: selecting an item does nothing visible.
    // The dropdown click path calls SetSelectedIndex + InvokeSetters but never fires the
    // OnSelectionChanged callback (only the keyboard path does).
    [TestMethod]
    public void ComboBox_SelectViaDropdownClickPath_FiresOnSelectionChanged()
    {
        //Arrange
        var combo = new ComboBox<string>();
        combo.SetItemsSource(new[] { "Item 1", "Item 2", "Item 3" });
        var callbackFired = false;
        string? callbackItem = null;
        combo.SetOnSelectionChanged(item => { callbackFired = true; callbackItem = item; });

        //Act: replicate exactly what ComboBoxDropdownOverlay does on an item click
        combo.SetSelectedIndex(1);
        combo.InvokeSetters();

        //Assert: selection itself works...
        Assert.AreEqual("Item 2", combo.SelectedItem);
        //...but the callback must also fire (it currently does not on mouse selection)
        Assert.IsTrue(callbackFired, "OnSelectionChanged must fire when an item is clicked in the dropdown.");
        Assert.AreEqual("Item 2", callbackItem);
    }

    // uifix.md > Entry: text and placeholder are not vertically centered.
    [TestMethod]
    public void Entry_SingleLineText_IsVerticallyCentered()
    {
        var entry = new Entry().SetText("Ay");
        entry.Measure(new Size(200, 40));
        entry.Arrange(new Rect(0, 0, 200, 40));

        var top = entry.GetSingleLineTextTop();
        var textHeight = entry.GetTextBlockHeight();
        var gapAbove = top - entry.Position.Y;
        var gapBelow = (entry.Position.Y + entry.ElementSize.Height) - (top + textHeight);

        Assert.AreEqual(gapBelow, gapAbove, 1.0f,
            "Single-line Entry text should be vertically centered (equal gap above and below).");
    }

    // uifix.md > Button: "Stretched is not really stretched".
    // A HorizontalAlignment.Stretch child of a (stretched) VStack should fill the stack width.
    [TestMethod]
    public void VStack_HorizontalStretchChild_FillsStackWidth()
    {
        var stack = (VStack)new VStack(
                new Button().SetText("X").SetHorizontalAlignment(HorizontalAlignment.Stretch))
            .SetHorizontalAlignment(HorizontalAlignment.Stretch);
        stack.Measure(new Size(300, 100));
        stack.Arrange(new Rect(0, 0, 300, 100));

        Assert.AreEqual(300f, stack.Children[0].ElementSize.Width, 0.5f,
            "A HorizontalAlignment.Stretch child should fill the VStack width.");
    }

    // uifix.md > TreeView: expanding only shows children after an unrelated event (hover).
    // Root cause is a framework invalidation gap: a node measured with dontStretch=true is left
    // dirty WITHOUT propagating up, so InvalidateMeasure can stop below a clean ancestor whose
    // Measure then short-circuits the cascade. Here the TreeView sits behind such a clean ancestor.
    [TestMethod]
    public void Invalidation_DeepChangeBehindCleanAncestor_IsRemeasuredFromRoot()
    {
        var root = new Category { Name = "Root", SubCategories = [new Category { Name = "Child" }] };
        var tree = new TreeView()
            .SetItemHeight(30)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemTemplate((item, _) => new Label().SetText(((Category)item).Name))
            .SetItemsSource(new List<object> { root });
        tree.BuildNodes();

        // Two nesting levels so the outer (root) ends up CLEAN while the inner stays dirty.
        var outer = new VStack(new VStack(tree));
        outer.Measure(new Size(300, 300));
        outer.Arrange(new Rect(0, 0, 300, 300));
        var collapsedCount = tree.Children.Count; // root only

        tree.ExpandNode(root);

        // Simulate the next frame: only re-measure from the root (as RenderService does).
        outer.Measure(new Size(300, 300));
        outer.Arrange(new Rect(0, 0, 300, 300));

        Assert.IsGreaterThan(collapsedCount, tree.Children.Count,
            "After expanding, a re-measure from the root must reach and re-measure the TreeView.");
    }

    // uifix.md > TabControl: switching a tab makes everything below disappear.
    // Root cause: TabControl measured to the FULL available height; inside a vertical ScrollView
    // that available height is float.MaxValue, so the control explodes and breaks the layout.
    [TestMethod]
    public void TabControl_InUnboundedHeight_SizesToContentNotAvailable()
    {
        var tabs = new TabControl()
            .AddTab(new TabItem().SetHeader("A").SetContent(new Solid(100, 50)))
            .AddTab(new TabItem().SetHeader("B").SetContent(new Solid(100, 80)));

        tabs.Measure(new Size(400, float.MaxValue));

        Assert.IsLessThan(10000f, tabs.ElementSize.Height,
            "TabControl must size to header+content, not fill an unbounded available height.");
    }

    private sealed class Category
    {
        public string Name { get; set; } = "";
        public List<Category> SubCategories { get; set; } = [];
    }
}
