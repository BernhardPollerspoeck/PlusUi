using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public class TabControlTests
{
    [TestMethod]
    public void TestTabControl_BindTabPosition_UpdatesTabPosition()
    {
        // Arrange
        var tabControl = new TestableTabControl();
        tabControl.AddTab(new TabItem().SetHeader("Tab1").SetContent(new Label().SetText("Content1")));

        var position = TabPosition.Top;
        tabControl.BindTabPosition(() => position);

        // Initial measure
        tabControl.Measure(new Size(500, 300));
        tabControl.Arrange(new Rect(0, 0, 500, 300));

        // Act - change position and call UpdateBindings
        position = TabPosition.Right;
        tabControl.UpdateBindings(nameof(position));

        // Assert - TabPosition should be updated
        Assert.AreEqual(TabPosition.Right, tabControl.TabPosition, "TabPosition should be updated via binding");
    }

    [TestMethod]
    public void TestTabControl_BindTabPosition_MeasureInternalCalledAfterUpdate()
    {
        // Arrange
        var tabControl = new TestableTabControl();
        tabControl.AddTab(new TabItem().SetHeader("Tab1").SetContent(new Label().SetText("Content1")));

        var position = TabPosition.Top;
        tabControl.BindTabPosition(() => position);

        // Initial measure
        tabControl.Measure(new Size(500, 300));
        tabControl.Arrange(new Rect(0, 0, 500, 300));
        tabControl.ResetMeasureCount();

        // Act - change position and call UpdateBindings, then Measure again
        position = TabPosition.Right;
        tabControl.UpdateBindings(nameof(position));
        tabControl.Measure(new Size(500, 300));

        // Assert - MeasureInternal should have been called
        Assert.IsTrue(tabControl.MeasureInternalCallCount > 0,
            "MeasureInternal should be called after TabPosition change and Measure");
    }

    [TestMethod]
    public void TestTabControl_InScrollView_BindTabPosition_PropagatesInvalidation()
    {
        // Arrange - This simulates the actual scenario from the demo page
        var tabControl = new TestableTabControl();
        tabControl.AddTab(new TabItem().SetHeader("Tab1").SetContent(new Label().SetText("Content1")));

        var vstack = new VStack().AddChild(tabControl);
        var scrollView = new ScrollView(vstack).SetCanScrollHorizontally(false);

        var position = TabPosition.Top;
        tabControl.BindTabPosition(() => position);

        // Initial measure with dontStretch (like ScrollView does)
        scrollView.Measure(new Size(500, 300));
        scrollView.Arrange(new Rect(0, 0, 500, 300));
        tabControl.ResetMeasureCount();

        // Act - change position and call UpdateBindings
        position = TabPosition.Right;
        tabControl.UpdateBindings(nameof(position));

        // Measure again (simulating next frame)
        scrollView.Measure(new Size(500, 300));
        scrollView.Arrange(new Rect(0, 0, 500, 300));

        // Assert - TabControl's MeasureInternal should have been called
        Assert.IsTrue(tabControl.MeasureInternalCallCount > 0,
            $"MeasureInternal should be called. TabPosition is {tabControl.TabPosition}");
    }

    [TestMethod]
    public void TestTabControl_InScrollView_MeasuredTabPositionUpdated()
    {
        // Arrange
        var tabControl = new TestableTabControl();
        tabControl.AddTab(new TabItem().SetHeader("Tab1").SetContent(new Label().SetText("Content1")));

        var vstack = new VStack().AddChild(tabControl);
        var scrollView = new ScrollView(vstack).SetCanScrollHorizontally(false);

        var position = TabPosition.Top;
        tabControl.BindTabPosition(() => position);

        // Initial measure
        scrollView.Measure(new Size(500, 300));
        scrollView.Arrange(new Rect(0, 0, 500, 300));

        Assert.AreEqual(TabPosition.Top, tabControl.GetMeasuredTabPosition(),
            "Initial MeasuredTabPosition should be Top");

        // Act - change position and call UpdateBindings
        position = TabPosition.Right;
        tabControl.UpdateBindings(nameof(position));

        // Measure again (simulating next frame)
        scrollView.Measure(new Size(500, 300));
        scrollView.Arrange(new Rect(0, 0, 500, 300));

        // Assert - MeasuredTabPosition should reflect the new position
        Assert.AreEqual(TabPosition.Right, tabControl.GetMeasuredTabPosition(),
            "MeasuredTabPosition should be updated to Right after binding update and re-measure");
    }

    // Helper class to track MeasureInternal calls
    private class TestableTabControl : TabControl
    {
        public int MeasureInternalCallCount { get; private set; }

        public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
        {
            MeasureInternalCallCount++;
            return base.MeasureInternal(availableSize, dontStretch);
        }

        public void ResetMeasureCount()
        {
            MeasureInternalCallCount = 0;
        }

        public TabPosition GetMeasuredTabPosition()
        {
            // Access the private field via reflection for testing
            var field = typeof(TabControl).GetField("_measuredTabPosition",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (TabPosition)(field?.GetValue(this) ?? TabPosition.Top);
        }
    }
}
