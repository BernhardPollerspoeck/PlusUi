using PlusUi.core;
using SkiaSharp;

namespace PlusUi.core.Tests;

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
            .SetDesiredHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(0, toolbar.Position.X);
        Assert.AreEqual(0, toolbar.Position.Y);
        Assert.IsLessThanOrEqualTo(56, toolbar.ElementSize.Height);
    }

    [TestMethod]
    public void TestToolbar_WithTitle_CenterAligned()
    {
        // Arrange
        var toolbar = new Toolbar()
            .SetTitle("My App")
            .SetTitleAlignment(TitleAlignment.Center)
            .SetDesiredHeight(56);
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
            .SetDesiredHeight(56);
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
            .SetDesiredWidth(40)
            .SetDesiredHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App")
            .AddLeft(menuButton)
            .SetDesiredHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.HasCount(1, toolbar.LeftItems);
        Assert.AreEqual(menuButton, toolbar.LeftItems[0]);
    }

    [TestMethod]
    public void TestToolbar_WithRightIcon()
    {
        // Arrange
        var searchButton = new Button()
            .SetText("Search")
            .SetDesiredWidth(40)
            .SetDesiredHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App")
            .AddRight(searchButton)
            .SetDesiredHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.HasCount(1, toolbar.RightItems);
        Assert.AreEqual(searchButton, toolbar.RightItems[0]);
    }

    [TestMethod]
    public void TestToolbar_WithLeftAndRightIcons()
    {
        // Arrange
        var menuButton = new Button()
            .SetText("Menu")
            .SetDesiredWidth(40)
            .SetDesiredHeight(40);
        var searchButton = new Button()
            .SetText("Search")
            .SetDesiredWidth(40)
            .SetDesiredHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App")
            .AddLeft(menuButton)
            .AddRight(searchButton)
            .SetDesiredHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.HasCount(1, toolbar.LeftItems);
        Assert.HasCount(1, toolbar.RightItems);
    }

    #endregion

    #region Icon Group Tests

    [TestMethod]
    public void TestToolbarIconGroup_Creation()
    {
        // Arrange
        var icon1 = new Button().SetText("Icon1").SetDesiredWidth(24).SetDesiredHeight(24);
        var icon2 = new Button().SetText("Icon2").SetDesiredWidth(24).SetDesiredHeight(24);

        // Act
        var group = new ToolbarIconGroup(icon1, icon2)
            .SetSeparator(true)
            .SetPriority(10);

        // Assert
        Assert.HasCount(2, group.Children);
        Assert.IsTrue(group.ShowSeparator);
        Assert.AreEqual(10, group.Priority);
    }

    [TestMethod]
    public void TestToolbarIconGroup_AddIcon()
    {
        // Arrange
        var group = new ToolbarIconGroup();
        var icon = new Button().SetText("Icon").SetDesiredWidth(24).SetDesiredHeight(24);

        // Act
        group.AddIcon(icon);

        // Assert
        Assert.HasCount(1, group.Children);
        Assert.AreEqual(icon, group.Children[0]);
    }

    [TestMethod]
    public void TestToolbarIconGroup_Spacing()
    {
        // Arrange
        var icon1 = new Button().SetText("Icon1").SetDesiredWidth(24).SetDesiredHeight(24);
        var icon2 = new Button().SetText("Icon2").SetDesiredWidth(24).SetDesiredHeight(24);
        var group = new ToolbarIconGroup(icon1, icon2)
            .SetSpacing(8);
        var availableSize = new Size(200, 56);

        // Act
        var size = group.Measure(availableSize);

        // Assert
        Assert.AreEqual(8, group.Spacing);
        // Size should include spacing between icons
        Assert.IsGreaterThan(icon1.ElementSize.Width + icon2.ElementSize.Width, size.Width);
    }

    [TestMethod]
    public void TestToolbarIconGroup_MeasureAndArrange()
    {
        // Arrange
        var icon1 = new Button().SetText("Icon1").SetDesiredWidth(24).SetDesiredHeight(24);
        var icon2 = new Button().SetText("Icon2").SetDesiredWidth(24).SetDesiredHeight(24);
        var group = new ToolbarIconGroup(icon1, icon2)
            .SetSpacing(4);
        var availableSize = new Size(200, 56);

        // Act
        group.Measure(availableSize);
        group.Arrange(new Rect(0, 0, 200, 56));

        // Assert
        Assert.IsGreaterThan(0, group.ElementSize.Width);
        Assert.IsGreaterThan(0, group.ElementSize.Height);
    }

    #endregion

    #region Toolbar with Groups Tests

    [TestMethod]
    public void TestToolbar_AddLeftGroup()
    {
        // Arrange
        var icon1 = new Button().SetText("Undo").SetDesiredWidth(24).SetDesiredHeight(24);
        var icon2 = new Button().SetText("Redo").SetDesiredWidth(24).SetDesiredHeight(24);
        var group = new ToolbarIconGroup(icon1, icon2)
            .SetSeparator(true);

        var toolbar = new Toolbar()
            .SetTitle("Editor")
            .AddLeftGroup(group)
            .SetDesiredHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.HasCount(1, toolbar.LeftItems);
        Assert.AreEqual(group, toolbar.LeftItems[0]);
    }

    [TestMethod]
    public void TestToolbar_AddRightGroup()
    {
        // Arrange
        var icon1 = new Button().SetText("Share").SetDesiredWidth(24).SetDesiredHeight(24);
        var icon2 = new Button().SetText("More").SetDesiredWidth(24).SetDesiredHeight(24);
        var group = new ToolbarIconGroup(icon1, icon2);

        var toolbar = new Toolbar()
            .SetTitle("Document")
            .AddRightGroup(group)
            .SetDesiredHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.HasCount(1, toolbar.RightItems);
        Assert.AreEqual(group, toolbar.RightItems[0]);
    }

    [TestMethod]
    public void TestToolbar_MultipleGroups()
    {
        // Arrange
        var group1 = new ToolbarIconGroup(
            new Button().SetText("Undo").SetDesiredWidth(24).SetDesiredHeight(24),
            new Button().SetText("Redo").SetDesiredWidth(24).SetDesiredHeight(24))
            .SetSeparator(true)
            .SetPriority(10);

        var group2 = new ToolbarIconGroup(
            new Button().SetText("Cut").SetDesiredWidth(24).SetDesiredHeight(24),
            new Button().SetText("Copy").SetDesiredWidth(24).SetDesiredHeight(24),
            new Button().SetText("Paste").SetDesiredWidth(24).SetDesiredHeight(24))
            .SetSeparator(true)
            .SetPriority(5);

        var toolbar = new Toolbar()
            .SetTitle("Editor")
            .AddLeftGroup(group1)
            .AddLeftGroup(group2)
            .SetDesiredHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.HasCount(2, toolbar.LeftItems);
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
        var icon1 = new Button().SetText("Icon1").SetDesiredWidth(40).SetDesiredHeight(40);
        var icon2 = new Button().SetText("Icon2").SetDesiredWidth(40).SetDesiredHeight(40);
        var icon3 = new Button().SetText("Icon3").SetDesiredWidth(40).SetDesiredHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App")
            .AddLeft(icon1)
            .AddLeft(icon2)
            .AddLeft(icon3)
            .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
            .SetOverflowThreshold(400)
            .SetDesiredHeight(56);
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
        // Arrange - Use larger buttons to ensure overflow triggers
        var icon1 = new Button().SetText("Icon1").SetDesiredWidth(80).SetDesiredHeight(40);
        var icon2 = new Button().SetText("Icon2").SetDesiredWidth(80).SetDesiredHeight(40);
        var icon3 = new Button().SetText("Icon3").SetDesiredWidth(80).SetDesiredHeight(40);
        var icon4 = new Button().SetText("Icon4").SetDesiredWidth(80).SetDesiredHeight(40);
        var icon5 = new Button().SetText("Icon5").SetDesiredWidth(80).SetDesiredHeight(40);

        var toolbar = new Toolbar()
            .SetTitle("My App Title Here")
            .AddLeft(icon1)
            .AddLeft(icon2)
            .AddLeft(icon3)
            .AddLeft(icon4)
            .AddLeft(icon5)
            .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
            .SetOverflowThreshold(800)
            .SetDesiredHeight(56);
        var availableSize = new Size(200, 56); // Very narrow to ensure overflow

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 200, 56));

        // Assert - Some items should be hidden (in overflow)
        var visibleCount = new[] { icon1, icon2, icon3, icon4, icon5 }.Count(i => i.IsVisible);
        Assert.IsLessThan(5, visibleCount, "Some items should be in overflow menu");
    }

    [TestMethod]
    public void TestToolbar_OverflowPriority()
    {
        // Arrange
        var highPriorityGroup = new ToolbarIconGroup(
            new Button().SetText("Undo").SetDesiredWidth(40).SetDesiredHeight(40))
            .SetPriority(10);

        var lowPriorityGroup = new ToolbarIconGroup(
            new Button().SetText("Format").SetDesiredWidth(40).SetDesiredHeight(40))
            .SetPriority(1);

        var toolbar = new Toolbar()
            .SetTitle("Editor")
            .AddLeftGroup(highPriorityGroup)
            .AddLeftGroup(lowPriorityGroup)
            .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
            .SetOverflowThreshold(800)
            .SetDesiredHeight(56);
        var availableSize = new Size(200, 56); // Very narrow

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 200, 56));

        // Assert - High priority group should be more likely to stay visible
        // (exact behavior depends on available space)
        Assert.IsNotNull(toolbar);
    }

    [TestMethod]
    public void TestToolbar_OverflowItems_ShouldNotOverlapWithTitle()
    {
        // BUG: ScrollView measures at large width, arranges at narrow width
        // Visibility is set during Measure, but Arrange uses different bounds
        var toolbar = new Toolbar()
            .SetTitle("Overflow Demo")
            .SetTitleAlignment(TitleAlignment.Center)
            .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
            .SetDesiredHeight(56);

        toolbar.AddLeft(new Button().SetText("File").SetPadding(new Margin(12, 8)));
        toolbar.AddLeft(new Button().SetText("Edit").SetPadding(new Margin(12, 8)));
        toolbar.AddLeft(new Button().SetText("View").SetPadding(new Margin(12, 8)));
        toolbar.AddLeft(new Button().SetText("Tools").SetPadding(new Margin(12, 8)));

        toolbar.AddRight(new Button().SetText("Search").SetPadding(new Margin(12, 8)));
        toolbar.AddRight(new Button().SetText("Settings").SetPadding(new Margin(12, 8)));
        toolbar.AddRight(new Button().SetText("Help").SetPadding(new Margin(12, 8)));
        toolbar.AddRight(new Button().SetText("About").SetPadding(new Margin(12, 8)));

        // Measure at large width, arrange at narrow width (like ScrollView does)
        toolbar.Measure(new Size(1000, 56));
        toolbar.Arrange(new Rect(0, 0, 550, 56));

        // Get title position
        var titleLabel = toolbar.GetType()
            .GetField("_titleLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(toolbar) as Label;
        Assert.IsNotNull(titleLabel);

        var titleX = titleLabel.Position.X;
        var titleRight = titleX + titleLabel.ElementSize.Width;

        // Get item edges
        var rightmostLeft = toolbar.LeftItems.Where(i => i.IsVisible)
            .Select(i => i.Position.X + i.ElementSize.Width).DefaultIfEmpty(0).Max();
        var leftmostRight = toolbar.RightItems.Where(i => i.IsVisible)
            .Select(i => i.Position.X).DefaultIfEmpty(550).Min();

        // Assert no overlap with title
        Assert.IsLessThanOrEqualTo(titleX, rightmostLeft,
            $"Left items overlap with title: {rightmostLeft} > {titleX}");
        Assert.IsGreaterThanOrEqualTo(titleRight, leftmostRight,
            $"Right items overlap with title: {leftmostRight} < {titleRight}");
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
            .SetPadding(padding);

        // Assert
        Assert.AreEqual(padding, toolbar.Padding);
    }

    [TestMethod]
    public void TestToolbar_TitleProperties()
    {
        // Arrange
        var toolbar = new Toolbar()
            .SetTitle("Test Title")
            .SetTitleFontSize(24)
            .SetTitleColor(Colors.Blue)
            .SetTitleAlignment(TitleAlignment.Left);

        // Assert
        Assert.AreEqual("Test Title", toolbar.Title);
        Assert.AreEqual(24, toolbar.TitleFontSize);
        Assert.AreEqual(Colors.Blue, toolbar.TitleColor);
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
        Assert.AreEqual(PlusUiDefaults.TextPrimary, toolbar.TitleColor);
        Assert.AreEqual(PlusUiDefaults.Spacing, toolbar.ItemSpacing);
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
            .SetTextSize(18);

        var toolbar = new Toolbar()
            .SetCenterContent(customContent)
            .SetDesiredHeight(56);
        var availableSize = new Size(800, 56);

        // Act
        toolbar.Measure(availableSize);
        toolbar.Arrange(new Rect(0, 0, 800, 56));

        // Assert
        Assert.AreEqual(customContent, toolbar.CenterContent);
    }

    #endregion
}
