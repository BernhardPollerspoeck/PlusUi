using PlusUi.core;

namespace UiPlus.core.Tests;

[TestClass]
public class ScrollViewTests
{
    [TestMethod]
    public void TestScrollView_Properties_DefaultValues()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content);
        
        // Assert
        Assert.IsTrue(scrollView.CanScrollHorizontally, "CanScrollHorizontally should be true by default");
        Assert.IsTrue(scrollView.CanScrollVertically, "CanScrollVertically should be true by default");
        Assert.AreEqual(0, scrollView.HorizontalOffset, "HorizontalOffset should be 0 by default");
        Assert.AreEqual(0, scrollView.VerticalOffset, "VerticalOffset should be 0 by default");
        Assert.IsFalse(scrollView.IsScrolling, "IsScrolling should be false by default");
    }
    
    [TestMethod]
    public void TestScrollView_CanScrollHorizontally_SetterAndBinder()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content);
        
        // Act - Test setter
        var result1 = scrollView.SetCanScrollHorizontally(false);
        
        // Assert
        Assert.IsFalse(scrollView.CanScrollHorizontally, "CanScrollHorizontally should be set to false");
        Assert.AreSame(scrollView, result1, "Method should return the ScrollView for chaining");
        
        // Act - Test binding
        bool propertyValue = true;
        var result2 = scrollView.BindCanScrollHorizontally("TestProperty", () => propertyValue);
        
        // Verify initial binding
        Assert.IsTrue(scrollView.CanScrollHorizontally, "CanScrollHorizontally should be bound to the property value");
        Assert.AreSame(scrollView, result2, "Method should return the ScrollView for chaining");
        
        // Change the bound property and update bindings
        propertyValue = false;
        scrollView.UpdateBindings("TestProperty");
        
        // Verify binding update took effect
        Assert.IsFalse(scrollView.CanScrollHorizontally, "CanScrollHorizontally should reflect the updated bound property");
    }
    
    [TestMethod]
    public void TestScrollView_CanScrollVertically_SetterAndBinder()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content);
        
        // Act - Test setter
        var result1 = scrollView.SetCanScrollVertically(false);
        
        // Assert
        Assert.IsFalse(scrollView.CanScrollVertically, "CanScrollVertically should be set to false");
        Assert.AreSame(scrollView, result1, "Method should return the ScrollView for chaining");
        
        // Act - Test binding
        bool propertyValue = true;
        var result2 = scrollView.BindCanScrollVertically("TestProperty", () => propertyValue);
        
        // Verify initial binding
        Assert.IsTrue(scrollView.CanScrollVertically, "CanScrollVertically should be bound to the property value");
        Assert.AreSame(scrollView, result2, "Method should return the ScrollView for chaining");
        
        // Change the bound property and update bindings
        propertyValue = false;
        scrollView.UpdateBindings("TestProperty");
        
        // Verify binding update took effect
        Assert.IsFalse(scrollView.CanScrollVertically, "CanScrollVertically should reflect the updated bound property");
    }
    
    [TestMethod]
    public void TestScrollView_HorizontalOffset_SetterAndBinder()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content);
        
        // Act - Test setter
        var result1 = scrollView.SetHorizontalOffset(50);
        scrollView.Measure(new Size(100, 100)); // Needed to get element sizes
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        
        // Assert
        Assert.AreEqual(50, scrollView.HorizontalOffset, "HorizontalOffset should be set to 50");
        Assert.AreSame(scrollView, result1, "Method should return the ScrollView for chaining");
        
        // Act - Test binding
        float propertyValue = 75;
        var result2 = scrollView.BindHorizontalOffset("TestProperty", () => propertyValue);
        scrollView.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.AreEqual(75, scrollView.HorizontalOffset, "HorizontalOffset should be bound to the property value");
        Assert.AreSame(scrollView, result2, "Method should return the ScrollView for chaining");
        
        // Change the bound property and update bindings
        propertyValue = 25;
        scrollView.UpdateBindings("TestProperty");
        
        // Verify binding update took effect
        Assert.AreEqual(25, scrollView.HorizontalOffset, "HorizontalOffset should reflect the updated bound property");
    }
    
    [TestMethod]
    public void TestScrollView_VerticalOffset_SetterAndBinder()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content);
        
        // Act - Test setter
        var result1 = scrollView.SetVerticalOffset(50);
        scrollView.Measure(new Size(100, 100)); // Needed to get element sizes
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        
        // Assert
        Assert.AreEqual(50, scrollView.VerticalOffset, "VerticalOffset should be set to 50");
        Assert.AreSame(scrollView, result1, "Method should return the ScrollView for chaining");
        
        // Act - Test binding
        float propertyValue = 75;
        var result2 = scrollView.BindVerticalOffset("TestProperty", () => propertyValue);
        scrollView.UpdateBindings("TestProperty");
        
        // Verify binding
        Assert.AreEqual(75, scrollView.VerticalOffset, "VerticalOffset should be bound to the property value");
        Assert.AreSame(scrollView, result2, "Method should return the ScrollView for chaining");
        
        // Change the bound property and update bindings
        propertyValue = 25;
        scrollView.UpdateBindings("TestProperty");
        
        // Verify binding update took effect
        Assert.AreEqual(25, scrollView.VerticalOffset, "VerticalOffset should reflect the updated bound property");
    }
    
    [TestMethod]
    public void TestScrollView_Measure_HorizontalScrollEnabled_ContentWidthUnlimited()
    {
        // Arrange
        var content = new Solid(200, 100);
        var scrollView = new ScrollView(content)
            .SetCanScrollHorizontally(true)
            .SetCanScrollVertically(false);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        
        // Assert
        Assert.AreEqual(200, content.ElementSize.Width, "Content width should be its desired width when horizontal scrolling is enabled");
        Assert.AreEqual(100, content.ElementSize.Height, "Content height should be constrained by the available height when vertical scrolling is disabled");
    }
    
    [TestMethod]
    public void TestScrollView_Measure_VerticalScrollEnabled_ContentHeightUnlimited()
    {
        // Arrange
        var content = new Solid(100, 200);
        var scrollView = new ScrollView(content)
            .SetCanScrollHorizontally(false)
            .SetCanScrollVertically(true);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        
        // Assert
        Assert.AreEqual(100, content.ElementSize.Width, "Content width should be constrained by the available width when horizontal scrolling is disabled");
        Assert.AreEqual(200, content.ElementSize.Height, "Content height should be its desired height when vertical scrolling is enabled");
    }
    
    [TestMethod]
    public void TestScrollView_Measure_BothScrollEnabled_ContentUnlimited()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content)
            .SetCanScrollHorizontally(true)
            .SetCanScrollVertically(true);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        
        // Assert
        Assert.AreEqual(200, content.ElementSize.Width, "Content width should be its desired width when horizontal scrolling is enabled");
        Assert.AreEqual(200, content.ElementSize.Height, "Content height should be its desired height when vertical scrolling is enabled");
    }
    
    [TestMethod]
    public void TestScrollView_Arrange_ContentPositionedWithOffsets()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        scrollView.SetHorizontalOffset(30);
        scrollView.SetVerticalOffset(40);
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        
        // Assert
        Assert.AreEqual(-30, content.Position.X, "Content X position should be adjusted by horizontal offset");
        Assert.AreEqual(-40, content.Position.Y, "Content Y position should be adjusted by vertical offset");
    }
    
    [TestMethod]
    public void TestScrollView_Arrange_DifferentAlignments()
    {
        // Arrange - Center alignment
        var content1 = new Solid(50, 50);
        var scrollView1 = new ScrollView(content1)
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView1.Measure(availableSize);
        scrollView1.Arrange(new Rect(0, 0, 100, 100));
        
        // Assert
        Assert.AreEqual(25, scrollView1.Position.X, "ScrollView X position should be centered");
        Assert.AreEqual(25, scrollView1.Position.Y, "ScrollView Y position should be centered");
        
        // Arrange - Right/Bottom alignment
        var content2 = new Solid(50, 50);
        var scrollView2 = new ScrollView(content2)
            .SetHorizontalAlignment(HorizontalAlignment.Right)
            .SetVerticalAlignment(VerticalAlignment.Bottom);
        
        // Act
        scrollView2.Measure(availableSize);
        scrollView2.Arrange(new Rect(0, 0, 100, 100));
        
        // Assert
        Assert.AreEqual(50, scrollView2.Position.X, "ScrollView X position should be right-aligned");
        Assert.AreEqual(50, scrollView2.Position.Y, "ScrollView Y position should be bottom-aligned");
    }
    
    [TestMethod]
    public void TestScrollView_HitTest_ReturnsContentWhenHit()
    {
        // Arrange
        var content = new Solid(100, 100);
        var scrollView = new ScrollView(content);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        var hit = scrollView.HitTest(new Point(50, 50));
        
        // Assert
        Assert.IsNotNull(hit, "Hit test should return a result when inside bounds");
        Assert.AreSame(content, hit, "Hit test should return the content when it's hit");
    }
    
    [TestMethod]
    public void TestScrollView_HitTest_ReturnsNullWhenOutsideBounds()
    {
        // Arrange
        var content = new Solid(100, 100);
        var scrollView = new ScrollView(content);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        var hit = scrollView.HitTest(new Point(150, 150));
        
        // Assert
        Assert.IsNull(hit, "Hit test should return null when outside bounds");
    }
    
    [TestMethod]
    public void TestScrollView_HitTest_AdjustsPointWithScrollOffsets()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        scrollView.SetHorizontalOffset(50);
        scrollView.SetVerticalOffset(50);
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        var hit = scrollView.HitTest(new Point(50, 50));
        
        // Assert
        Assert.IsNotNull(hit, "Hit test should return a result when inside bounds");
        Assert.AreSame(content, hit, "Hit test should return the content after adjusting for scroll offset");
    }
    
    [TestMethod]
    public void TestScrollView_HandleScroll_UpdatesOffsets()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        scrollView.HandleScroll(10, 20);
        
        // Assert
        Assert.AreEqual(10, scrollView.HorizontalOffset, "Horizontal offset should be updated after HandleScroll");
        Assert.AreEqual(20, scrollView.VerticalOffset, "Vertical offset should be updated after HandleScroll");
    }
    
    [TestMethod]
    public void TestScrollView_HandleScroll_WithDisabledScrolling()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content)
            .SetCanScrollHorizontally(false)
            .SetCanScrollVertically(false);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        scrollView.HandleScroll(10, 20);
        
        // Assert
        Assert.AreEqual(0, scrollView.HorizontalOffset, "Horizontal offset should not change when horizontal scrolling is disabled");
        Assert.AreEqual(0, scrollView.VerticalOffset, "Vertical offset should not change when vertical scrolling is disabled");
    }
    
    [TestMethod]
    public void TestScrollView_HandleScroll_ClampsToValidRange()
    {
        // Arrange
        var content = new Solid(200, 200);
        var scrollView = new ScrollView(content);
        var availableSize = new Size(100, 100);
        
        // Act - Try to scroll beyond content start
        scrollView.Measure(availableSize);
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        scrollView.HandleScroll(-10, -20);
        
        // Assert
        Assert.AreEqual(0, scrollView.HorizontalOffset, "Horizontal offset should be clamped to minimum 0");
        Assert.AreEqual(0, scrollView.VerticalOffset, "Vertical offset should be clamped to minimum 0");
        
        // Act - Try to scroll beyond content end
        scrollView.HandleScroll(200, 200);
        
        // Assert
        Assert.AreEqual(100, scrollView.HorizontalOffset, "Horizontal offset should be clamped to maximum (content width - viewport width)");
        Assert.AreEqual(100, scrollView.VerticalOffset, "Vertical offset should be clamped to maximum (content height - viewport height)");
    }
    
    [TestMethod]
    public void TestScrollView_ContentSmallerThanViewport_NoScrollingPossible()
    {
        // Arrange
        var content = new Solid(50, 50);  // Smaller than the viewport
        var scrollView = new ScrollView(content);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        scrollView.Arrange(new Rect(0, 0, 100, 100));
        scrollView.HandleScroll(10, 10);
        
        // Assert
        Assert.AreEqual(0, scrollView.HorizontalOffset, "Horizontal offset should be 0 when content is smaller than viewport");
        Assert.AreEqual(0, scrollView.VerticalOffset, "Vertical offset should be 0 when content is smaller than viewport");
    }
    [TestMethod]
    public void TestScrollView_Measure_WithGrid_UsesNaturalSize()
    {
        // Arrange
        var grid = new Grid()
            .AddColumn(200)
            .AddColumn(200)
            .AddRow(50);
            
        var scrollView = new ScrollView(grid)
            .SetCanScrollHorizontally(true)
            .SetCanScrollVertically(true);
        var availableSize = new Size(100, 100);
        
        // Act
        scrollView.Measure(availableSize);
        
        // Assert - Verify that the grid's measured size reflects the actual columns/rows
        Assert.AreEqual(400, grid.ElementSize.Width, "Grid width should be the sum of column widths (200+200)");
        Assert.AreEqual(50, grid.ElementSize.Height, "Grid height should be the row height (50)");
    }
}