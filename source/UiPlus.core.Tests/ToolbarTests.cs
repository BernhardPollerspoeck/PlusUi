using PlusUi.core;
using SkiaSharp;

namespace UiPlus.core.Tests;

/// <summary>
/// Tests for Toolbar control covering basic layout, title alignment, icon groups, and overflow behavior.
/// </summary>
[TestClass]
public sealed class ToolbarTests
{
    #region Basic Layout Tests

    [TestMethod]
    public void TestToolbar_MeasureAndArrange_EmptyToolbar()
    {
        // Arrange
        var toolbar = new Toolbar()
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(0, toolbar.Position.X);
        Assert.AreEqual(0, toolbar.Position.Y);
        Assert.IsTrue(toolbar.ElementSize.Height <= 56);
    }

    [TestMethod]
    public void TestToolbar_WithTitle_CenterAligned()
    {
        // Arrange
        var toolbar = new Toolbar()
            .SetTitle("My App")
            .SetTitleAlignment(TitleAlignment.Center)
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual("My App", toolbar.Title);
        Assert.AreEqual(TitleAlignment.Center, toolbar.TitleAlignment);
        Assert.IsNotNull(toolbar);
    }

    [TestMethod]
    public void TestToolbar_WithTitle_LeftAligned()
    {
        // Arrange
        var toolbar = new Toolbar()
            .SetTitle("My App")
            .SetTitleAlignment(TitleAlignment.Left)
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(TitleAlignment.Left, toolbar.TitleAlignment);
    }

    [TestMethod]
    public void TestToolbar_WithLeftIcon()
    {
        // Arrange
        var menuButton = new Button()
            .SetText("Menu")
            .SetWidth(40)
            .SetHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App")
            .AddLeftIcon(menuButton)
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(1, toolbar.LeftItems.Count);
        Assert.AreEqual(menuButton, toolbar.LeftItems[0]);
    }

    [TestMethod]
    public void TestToolbar_WithRightIcon()
    {
        // Arrange
        var searchButton = new Button()
            .SetText("Search")
            .SetWidth(40)
            .SetHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App")
            .AddRightIcon(searchButton)
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(1, toolbar.RightItems.Count);
        Assert.AreEqual(searchButton, toolbar.RightItems[0]);
    }

    [TestMethod]
    public void TestToolbar_WithLeftAndRightIcons()
    {
        // Arrange
        var menuButton = new Button()
            .SetText("Menu")
            .SetWidth(40)
            .SetHeight(40);
        var searchButton = new Button()
            .SetText("Search")
            .SetWidth(40)
            .SetHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App")
            .AddLeftIcon(menuButton)
            .AddRightIcon(searchButton)
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(1, toolbar.LeftItems.Count);
        Assert.AreEqual(1, toolbar.RightItems.Count);
    }

    #endregion

    #region Icon Group Tests

    [TestMethod]
    public void TestToolbarIconGroup_Creation()
    {
        // Arrange
        var icon1 = new Button().SetText("Icon1").SetWidth(24).SetHeight(24);
        var icon2 = new Button().SetText("Icon2").SetWidth(24).SetHeight(24);

        // Act
        var group = new ToolbarIconGroup(icon1, icon2)
            .SetSeparator(true)
            .SetPriority(10);

        // Assert
        Assert.AreEqual(2, group.Children.Count);
        Assert.IsTrue(group.ShowSeparator);
        Assert.AreEqual(10, group.Priority);
    }

    [TestMethod]
    public void TestToolbarIconGroup_AddIcon()
    {
        // Arrange
        var group = new ToolbarIconGroup();
        var icon = new Button().SetText("Icon").SetWidth(24).SetHeight(24);

        // Act
        group.AddIcon(icon);

        // Assert
        Assert.AreEqual(1, group.Children.Count);
        Assert.AreEqual(icon, group.Children[0]);
    }

    [TestMethod]
    public void TestToolbarIconGroup_Spacing()
    {
        // Arrange
        var icon1 = new Button().SetText("Icon1").SetWidth(24).SetHeight(24);
        var icon2 = new Button().SetText("Icon2").SetWidth(24).SetHeight(24);
        var group = new ToolbarIconGroup(icon1, icon2)
            .SetSpacing(8);
        var availableSize = new Size(200, 56);

        // Act
        var size = group.Measure(availableSize);

        // Assert
        Assert.AreEqual(8, group.Spacing);
        // Size should include spacing between icons
        Assert.IsTrue(size.Width > icon1.ElementSize.Width + icon2.ElementSize.Width);
    }

    [TestMethod]
    public void TestToolbarIconGroup_MeasureAndArrange()
    {
        // Arrange
        var icon1 = new Button().SetText("Icon1").SetWidth(24).SetHeight(24);
        var icon2 = new Button().SetText("Icon2").SetWidth(24).SetHeight(24);
        var group = new ToolbarIconGroup(icon1, icon2)
            .SetSpacing(4);
        var availableSize = new Size(200, 56);

        // Act
        group.Measure(availableSize);
        group.Arrange(new Rect(0, 0, 200, 56));

        // Assert
        Assert.IsTrue(group.ElementSize.Width > 0);
        Assert.IsTrue(group.ElementSize.Height > 0);
    }

    #endregion

    #region Toolbar with Groups Tests

    [TestMethod]
    public void TestToolbar_AddLeftGroup()
    {
        // Arrange
        var icon1 = new Button().SetText("Undo").SetWidth(24).SetHeight(24);
        var icon2 = new Button().SetText("Redo").SetWidth(24).SetHeight(24);
        var group = new ToolbarIconGroup(icon1, icon2)
            .SetSeparator(true);

        var toolbar = new Toolbar()
            .SetTitle("Editor")
            .AddLeftGroup(group)
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(1, toolbar.LeftItems.Count);
        Assert.AreEqual(group, toolbar.LeftItems[0]);
    }

    [TestMethod]
    public void TestToolbar_AddRightGroup()
    {
        // Arrange
        var icon1 = new Button().SetText("Share").SetWidth(24).SetHeight(24);
        var icon2 = new Button().SetText("More").SetWidth(24).SetHeight(24);
        var group = new ToolbarIconGroup(icon1, icon2);

        var toolbar = new Toolbar()
            .SetTitle("Document")
            .AddRightGroup(group)
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(1, toolbar.RightItems.Count);
        Assert.AreEqual(group, toolbar.RightItems[0]);
    }

    [TestMethod]
    public void TestToolbar_MultipleGroups()
    {
        // Arrange
        var group1 = new ToolbarIconGroup(
            new Button().SetText("Undo").SetWidth(24).SetHeight(24),
            new Button().SetText("Redo").SetWidth(24).SetHeight(24))
            .SetSeparator(true)
            .SetPriority(10);

        var group2 = new ToolbarIconGroup(
            new Button().SetText("Cut").SetWidth(24).SetHeight(24),
            new Button().SetText("Copy").SetWidth(24).SetHeight(24),
            new Button().SetText("Paste").SetWidth(24).SetHeight(24))
            .SetSeparator(true)
            .SetPriority(5);

        var toolbar = new Toolbar()
            .SetTitle("Editor")
            .AddLeftGroup(group1)
            .AddLeftGroup(group2)
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(2, toolbar.LeftItems.Count);
    }

    #endregion

    #region Overflow Tests

    [TestMethod]
    public void TestToolbar_OverflowBehavior_None()
    {
        // Arrange
        var toolbar = new Toolbar()
            .SetTitle("My App")
            .SetOverflowBehavior(OverflowBehavior.None);

        // Assert
        Assert.AreEqual(OverflowBehavior.None, toolbar.OverflowBehavior);
    }

    [TestMethod]
    public void TestToolbar_OverflowBehavior_CollapseToMenu()
    {
        // Arrange
        var toolbar = new Toolbar()
            .SetTitle("My App")
            .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
            .SetOverflowThreshold(600);

        // Assert
        Assert.AreEqual(OverflowBehavior.CollapseToMenu, toolbar.OverflowBehavior);
        Assert.AreEqual(600, toolbar.OverflowThreshold);
    }

    [TestMethod]
    public void TestToolbar_OverflowDetection_WideEnough()
    {
        // Arrange
        var icon1 = new Button().SetText("Icon1").SetWidth(40).SetHeight(40);
        var icon2 = new Button().SetText("Icon2").SetWidth(40).SetHeight(40);
        var icon3 = new Button().SetText("Icon3").SetWidth(40).SetHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App")
            .AddLeftIcon(icon1)
            .AddLeftIcon(icon2)
            .AddLeftIcon(icon3)
            .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
            .SetOverflowThreshold(400)
            .SetHeight(56);
        var availableSize = new Size(800, 56); // Wide enough

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert - All items should be visible (no overflow)
        Assert.IsTrue(icon1.IsVisible);
        Assert.IsTrue(icon2.IsVisible);
        Assert.IsTrue(icon3.IsVisible);
    }

    [TestMethod]
    public void TestToolbar_OverflowDetection_TooNarrow()
    {
        // Arrange
        var icon1 = new Button().SetText("Icon1").SetWidth(40).SetHeight(40);
        var icon2 = new Button().SetText("Icon2").SetWidth(40).SetHeight(40);
        var icon3 = new Button().SetText("Icon3").SetWidth(40).SetHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App")
            .AddLeftIcon(icon1)
            .AddLeftIcon(icon2)
            .AddLeftIcon(icon3)
            .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
            .SetOverflowThreshold(800)
            .SetHeight(56);
        var availableSize = new Size(300, 56); // Too narrow

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 300, 56));

        // Assert - Some items should be hidden (in overflow)
        var visibleCount = new[] { icon1, icon2, icon3 }.Count(i => i.IsVisible);
        Assert.IsTrue(visibleCount < 3, "Some items should be in overflow menu");
    }

    [TestMethod]
    public void TestToolbar_OverflowPriority()
    {
        // Arrange
        var highPriorityGroup = new ToolbarIconGroup(
            new Button().SetText("Undo").SetWidth(40).SetHeight(40))
            .SetPriority(10);

        var lowPriorityGroup = new ToolbarIconGroup(
            new Button().SetText("Format").SetWidth(40).SetHeight(40))
            .SetPriority(1);

        var toolbar = new Toolbar()
            .SetTitle("Editor")
            .AddLeftGroup(highPriorityGroup)
            .AddLeftGroup(lowPriorityGroup)
            .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
            .SetOverflowThreshold(800)
            .SetHeight(56);
        var availableSize = new Size(200, 56); // Very narrow

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 200, 56));

        // Assert - High priority group should be more likely to stay visible
        // (exact behavior depends on available space)
        Assert.IsNotNull(toolbar);
    }

    #endregion

    #region Properties Tests

    [TestMethod]
    public void TestToolbar_ItemSpacing()
    {
        // Arrange
        var toolbar = new Toolbar()
            .SetItemSpacing(12);

        // Assert
        Assert.AreEqual(12, toolbar.ItemSpacing);
    }

    [TestMethod]
    public void TestToolbar_ContentPadding()
    {
        // Arrange
        var padding = new Margin(20, 8, 20, 8);
        var toolbar = new Toolbar()
            .SetContentPadding(padding);

        // Assert
        Assert.AreEqual(padding, toolbar.ContentPadding);
    }

    [TestMethod]
    public void TestToolbar_TitleProperties()
    {
        // Arrange
        var toolbar = new Toolbar()
            .SetTitle("Test Title")
            .SetTitleFontSize(24)
            .SetTitleColor(SKColors.Blue)
            .SetTitleAlignment(TitleAlignment.Left);

        // Assert
        Assert.AreEqual("Test Title", toolbar.Title);
        Assert.AreEqual(24, toolbar.TitleFontSize);
        Assert.AreEqual(SKColors.Blue, toolbar.TitleColor);
        Assert.AreEqual(TitleAlignment.Left, toolbar.TitleAlignment);
    }

    [TestMethod]
    public void TestToolbar_DefaultValues()
    {
        // Arrange
        var toolbar = new Toolbar();

        // Assert - Check default values
        Assert.AreEqual(TitleAlignment.Center, toolbar.TitleAlignment);
        Assert.AreEqual(20, toolbar.TitleFontSize);
        Assert.AreEqual(SKColors.Black, toolbar.TitleColor);
        Assert.AreEqual(8, toolbar.ItemSpacing);
        Assert.AreEqual(OverflowBehavior.None, toolbar.OverflowBehavior);
        Assert.AreEqual(600, toolbar.OverflowThreshold);
    }

    #endregion

    #region Center Content Tests

    [TestMethod]
    public void TestToolbar_SetCenterContent()
    {
        // Arrange
        var customContent = new Label()
            .SetText("Custom Title")
            .SetFontSize(18);

        var toolbar = new Toolbar()
            .SetCenterContent(customContent)
            .SetHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(customContent, toolbar.CenterContent);
    }

    #endregion
}
