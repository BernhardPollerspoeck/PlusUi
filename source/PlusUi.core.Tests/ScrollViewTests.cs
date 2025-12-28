using PlusUi.core;
using System.Windows.Input;

namespace PlusUi.core.Tests;

[TestClass]
public class ScrollViewTests
{
    private class TestCommand : ICommand
    {
        public bool WasExecuted { get; private set; }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            WasExecuted = true;
        }
    }

    [TestMethod]
    public void TestScrollView_Properties_DefaultValues()
    {
        // Arrange
        var content = new Label().SetText("Test");

        // Act
        var scrollView = new ScrollView(content);

        // Assert
        Assert.IsTrue(scrollView.CanScrollHorizontally, "CanScrollHorizontally should be true by default");
        Assert.IsTrue(scrollView.CanScrollVertically, "CanScrollVertically should be true by default");
        Assert.AreEqual(1.0f, scrollView.ScrollFactor, "ScrollFactor should be 1.0 by default");
        Assert.AreEqual(0f, scrollView.HorizontalOffset, "HorizontalOffset should be 0 by default");
        Assert.AreEqual(0f, scrollView.VerticalOffset, "VerticalOffset should be 0 by default");
        Assert.IsFalse(scrollView.IsScrolling, "IsScrolling should be false by default");
    }

    [TestMethod]
    public void TestScrollView_HitTest_ButtonWithoutScroll()
    {
        // Arrange - Create a ScrollView with a button inside
        var command = new TestCommand();
        var button = new Button()
            .SetText("Click Me")
            .SetCommand(command)
            .SetPadding(new(10));

        var scrollView = new ScrollView(button);

        // Act - Measure and arrange
        scrollView.Measure(new Size(200, 200));
        scrollView.Arrange(new Rect(0, 0, 200, 200));

        // Get button's actual position after arrange
        var buttonPos = button.Position;
        var buttonSize = button.ElementSize;

        // Hit test in the middle of the button
        var hitPoint = new Point(
            buttonPos.X + (buttonSize.Width / 2),
            buttonPos.Y + (buttonSize.Height / 2)
        );
        var hit = scrollView.HitTest(hitPoint);

        // Assert
        Assert.IsNotNull(hit, "HitTest should return a result");
        Assert.AreSame(button, hit, "HitTest should return the button");
    }

    [TestMethod]
    public void TestScrollView_HitTest_ButtonAfterVerticalScroll()
    {
        // Arrange - Create a tall content with a button
        var command = new TestCommand();
        var button = new Button()
            .SetText("Click Me")
            .SetCommand(command)
            .SetPadding(new(10));

        // Place button in a VStack with some spacing
        var content = new VStack()
            .AddChild(new Solid(100, 100)) // Spacer at top
            .AddChild(button);

        var scrollView = new ScrollView(content)
            .SetCanScrollHorizontally(false);

        // Act - Measure and arrange
        scrollView.Measure(new Size(200, 100)); // ScrollView is 100 tall
        scrollView.Arrange(new Rect(0, 0, 200, 100));

        // Scroll down by 80 pixels
        scrollView.SetVerticalOffset(80);
        scrollView.Measure(new Size(200, 100));
        scrollView.Arrange(new Rect(0, 0, 200, 100));

        // Get button's actual position after arrange (should be offset by scroll)
        var buttonPos = button.Position;
        var buttonSize = button.ElementSize;

        // Hit test in the middle of the button's visible position
        var hitPoint = new Point(
            buttonPos.X + (buttonSize.Width / 2),
            buttonPos.Y + (buttonSize.Height / 2)
        );
        var hit = scrollView.HitTest(hitPoint);

        // Assert
        Assert.IsNotNull(hit, $"HitTest should return a result. Button is at ({buttonPos.X}, {buttonPos.Y}), hit point is ({hitPoint.X}, {hitPoint.Y})");
        Assert.AreSame(button, hit, $"HitTest should return the button, but got {hit?.GetType().Name}");
    }

    [TestMethod]
    public void TestScrollView_HitTest_OutsideBounds_ReturnsNull()
    {
        // Arrange
        var button = new Button().SetText("Test");
        var scrollView = new ScrollView(button);

        // Act
        scrollView.Measure(new Size(100, 100));
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        var hit = scrollView.HitTest(new Point(150, 150));

        // Assert
        Assert.IsNull(hit, "HitTest should return null when outside bounds");
    }

    [TestMethod]
    public void TestScrollView_HitTest_OnScrollViewNotButton_ReturnsScrollView()
    {
        // Arrange - Small button in large ScrollView
        var button = new Button()
            .SetText("Small")
            .SetPadding(new(5));

        var scrollView = new ScrollView(button);

        // Act
        scrollView.Measure(new Size(200, 200));
        scrollView.Arrange(new Rect(0, 0, 200, 200));

        // Hit test in an area not covered by the button (e.g., far right)
        var hit = scrollView.HitTest(new Point(150, 10));

        // Assert - Should return ScrollView itself (for scrolling), not the button
        Assert.IsNotNull(hit, "HitTest should return a result");
        Assert.AreSame(scrollView, hit, "HitTest should return the ScrollView when not hitting the button");
    }

    [TestMethod]
    public void TestScrollView_SetCanScrollHorizontally()
    {
        // Arrange
        var content = new Label().SetText("Test");
        var scrollView = new ScrollView(content);

        // Act
        var result = scrollView.SetCanScrollHorizontally(false);

        // Assert
        Assert.IsFalse(scrollView.CanScrollHorizontally, "CanScrollHorizontally should be set to false");
        Assert.AreSame(scrollView, result, "Method should return the ScrollView for chaining");
    }

    [TestMethod]
    public void TestScrollView_SetCanScrollVertically()
    {
        // Arrange
        var content = new Label().SetText("Test");
        var scrollView = new ScrollView(content);

        // Act
        var result = scrollView.SetCanScrollVertically(false);

        // Assert
        Assert.IsFalse(scrollView.CanScrollVertically, "CanScrollVertically should be set to false");
        Assert.AreSame(scrollView, result, "Method should return the ScrollView for chaining");
    }

    [TestMethod]
    public void TestScrollView_SetScrollFactor()
    {
        // Arrange
        var content = new Label().SetText("Test");
        var scrollView = new ScrollView(content);

        // Act
        var result = scrollView.SetScrollFactor(2.0f);

        // Assert
        Assert.AreEqual(2.0f, scrollView.ScrollFactor, "ScrollFactor should be set to 2.0");
        Assert.AreSame(scrollView, result, "Method should return the ScrollView for chaining");
    }

    [TestMethod]
    public void TestScrollView_HandleScroll_UpdatesOffsets()
    {
        // Arrange
        var content = new VStack()
            .AddChild(new Solid(200, 400)); // Tall content

        var scrollView = new ScrollView(content);
        scrollView.Measure(new Size(200, 200));
        scrollView.Arrange(new Rect(0, 0, 200, 200));

        // Act - Scroll vertically
        scrollView.HandleScroll(0, 50);

        // Assert
        Assert.AreEqual(50f, scrollView.VerticalOffset, "VerticalOffset should be updated");
    }

    [TestMethod]
    public void TestScrollView_HandleScroll_RespectsCanScroll()
    {
        // Arrange
        var content = new VStack()
            .AddChild(new Solid(400, 400)); // Large content

        var scrollView = new ScrollView(content)
            .SetCanScrollHorizontally(false)
            .SetCanScrollVertically(false);

        scrollView.Measure(new Size(200, 200));
        scrollView.Arrange(new Rect(0, 0, 200, 200));

        // Act - Try to scroll
        scrollView.HandleScroll(50, 50);

        // Assert - Offsets should remain 0
        Assert.AreEqual(0f, scrollView.HorizontalOffset, "HorizontalOffset should remain 0 when scrolling disabled");
        Assert.AreEqual(0f, scrollView.VerticalOffset, "VerticalOffset should remain 0 when scrolling disabled");
    }

    [TestMethod]
    public void TestScrollView_VerticalOffset_ClampsToMaxOffset()
    {
        // Arrange
        var content = new Solid(200, 300); // Content is 300 tall
        var scrollView = new ScrollView(content);

        scrollView.Measure(new Size(200, 100)); // ScrollView is 100 tall
        scrollView.Arrange(new Rect(0, 0, 200, 100));

        // Act - Try to scroll beyond max (max should be 300-100=200)
        scrollView.SetVerticalOffset(300);

        // Assert
        Assert.AreEqual(200f, scrollView.VerticalOffset, "VerticalOffset should be clamped to max offset");
    }

    [TestMethod]
    public void TestScrollView_BindCanScrollHorizontally()
    {
        // Arrange
        var content = new Label().SetText("Test");
        var scrollView = new ScrollView(content);
        var propertyValue = false;

        // Act
        scrollView.BindCanScrollHorizontally("TestProperty", () => propertyValue);
        scrollView.UpdateBindings("TestProperty");

        // Assert
        Assert.IsFalse(scrollView.CanScrollHorizontally, "CanScrollHorizontally should be bound to property value");
    }
}
