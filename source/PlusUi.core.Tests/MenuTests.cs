using PlusUi.core;
using SkiaSharp;
using System.Windows.Input;

namespace PlusUi.core.Tests;

/// <summary>
/// Tests for Menu, ContextMenu, and MenuItem controls.
/// </summary>
[TestClass]
public sealed class MenuTests
{
    #region MenuItem Tests

    [TestMethod]
    public void TestMenuItem_FluentApi_SetsProperties()
    {
        // Arrange & Act
        var menuItem = new MenuItem()
            .SetText("Cut")
            .SetShortcut("Ctrl+X")
            .SetIsEnabled(true)
            .SetIsChecked(false);

        // Assert
        Assert.AreEqual("Cut", menuItem.Text);
        Assert.AreEqual("Ctrl+X", menuItem.Shortcut);
        Assert.IsTrue(menuItem.IsEnabled);
        Assert.IsFalse(menuItem.IsChecked);
    }

    [TestMethod]
    public void TestMenuItem_WithIcon_SetsIcon()
    {
        // Arrange & Act
        var menuItem = new MenuItem()
            .SetText("Open")
            .SetIcon("open.png");

        // Assert
        Assert.AreEqual("Open", menuItem.Text);
        Assert.AreEqual("open.png", menuItem.Icon);
    }

    [TestMethod]
    public void TestMenuItem_WithSubItems_HasSubItems()
    {
        // Arrange & Act
        var menuItem = new MenuItem()
            .SetText("Recent Files")
            .AddItem(new MenuItem().SetText("File1.txt"))
            .AddItem(new MenuItem().SetText("File2.txt"))
            .AddSeparator()
            .AddItem(new MenuItem().SetText("File3.txt"));

        // Assert
        Assert.IsTrue(menuItem.HasSubItems);
        Assert.HasCount(4, menuItem.Items);
        Assert.IsInstanceOfType(menuItem.Items[2], typeof(MenuSeparator));
    }

    [TestMethod]
    public void TestMenuItem_WithoutSubItems_HasNoSubItems()
    {
        // Arrange & Act
        var menuItem = new MenuItem()
            .SetText("Exit");

        // Assert
        Assert.IsFalse(menuItem.HasSubItems);
        Assert.IsEmpty(menuItem.Items);
    }

    [TestMethod]
    public void TestMenuItem_IsChecked_ToggleState()
    {
        // Arrange
        var menuItem = new MenuItem()
            .SetText("Show Toolbar")
            .SetIsChecked(true);

        // Assert
        Assert.IsTrue(menuItem.IsChecked);

        // Act
        menuItem.SetIsChecked(false);

        // Assert
        Assert.IsFalse(menuItem.IsChecked);
    }

    [TestMethod]
    public void TestMenuItem_IsEnabled_ControlsExecutability()
    {
        // Arrange
        var menuItem = new MenuItem()
            .SetText("Undo")
            .SetIsEnabled(false);

        // Assert
        Assert.IsFalse(menuItem.IsEnabled);

        // Act
        menuItem.SetIsEnabled(true);

        // Assert
        Assert.IsTrue(menuItem.IsEnabled);
    }

    [TestMethod]
    public void TestMenuItem_WithCommand_ExecutesCommand()
    {
        // Arrange
        bool commandExecuted = false;
        var command = new TestCommand(() => commandExecuted = true);
        var menuItem = new MenuItem()
            .SetText("Save")
            .SetCommand(command);

        // Act
        menuItem.Execute();

        // Assert
        Assert.IsTrue(commandExecuted);
    }

    [TestMethod]
    public void TestMenuItem_DisabledWithCommand_DoesNotExecute()
    {
        // Arrange
        bool commandExecuted = false;
        var command = new TestCommand(() => commandExecuted = true);
        var menuItem = new MenuItem()
            .SetText("Save")
            .SetIsEnabled(false)
            .SetCommand(command);

        // Act
        menuItem.Execute();

        // Assert
        Assert.IsFalse(commandExecuted);
    }

    [TestMethod]
    public void TestMenuItem_WithCommandParameter_PassesParameter()
    {
        // Arrange
        object? receivedParameter = null;
        var command = new TestCommand(param => receivedParameter = param);
        var menuItem = new MenuItem()
            .SetText("Open Recent")
            .SetCommand(command)
            .SetCommandParameter("test-file.txt");

        // Act
        menuItem.Execute();

        // Assert
        Assert.AreEqual("test-file.txt", receivedParameter);
    }

    #endregion

    #region Menu Tests

    [TestMethod]
    public void TestMenu_MeasureAndArrange_Basic()
    {
        // Arrange
        var menu = new Menu()
            .AddItem(new MenuItem().SetText("File"))
            .AddItem(new MenuItem().SetText("Edit"))
            .AddItem(new MenuItem().SetText("View"));
        var availableSize = new Size(800, 32);

        // Act
        menu.Measure(availableSize);
        menu.Arrange(new Rect(0, 0, 800, 32));

        // Assert
        Assert.AreEqual(0, menu.Position.X);
        Assert.AreEqual(0, menu.Position.Y);
        Assert.AreEqual(800, menu.ElementSize.Width);
    }

    [TestMethod]
    public void TestMenu_HasCorrectAccessibilityRole()
    {
        // Arrange
        var menu = new Menu()
            .AddItem(new MenuItem().SetText("File"));

        // Assert
        Assert.AreEqual(AccessibilityRole.Menu, menu.AccessibilityRole);
    }

    [TestMethod]
    public void TestMenu_SetColors_AppliesColors()
    {
        // Arrange & Act
        var menu = new Menu()
            .AddItem(new MenuItem().SetText("File"))
            .SetHoverBackgroundColor(new Color(100, 100, 100))
            .SetActiveBackgroundColor(new Color(150, 150, 150))
            .SetTextColor(Colors.Yellow);

        // Assert
        Assert.AreEqual(new Color(100, 100, 100), menu.HoverBackgroundColor);
        Assert.AreEqual(new Color(150, 150, 150), menu.ActiveBackgroundColor);
        Assert.AreEqual(Colors.Yellow, menu.TextColor);
    }

    [TestMethod]
    public void TestMenu_IsFocusable()
    {
        // Arrange
        var menu = new Menu()
            .AddItem(new MenuItem().SetText("File"));

        // Assert
        Assert.IsTrue(menu.IsFocusable);
    }

    #endregion

    #region ContextMenu Tests

    [TestMethod]
    public void TestContextMenu_AddItems_AddsToCollection()
    {
        // Arrange & Act
        var contextMenu = new ContextMenu()
            .AddItem(new MenuItem().SetText("Cut"))
            .AddItem(new MenuItem().SetText("Copy"))
            .AddSeparator()
            .AddItem(new MenuItem().SetText("Paste"));

        // Assert
        Assert.HasCount(4, contextMenu.Items);
        Assert.IsInstanceOfType(contextMenu.Items[2], typeof(MenuSeparator));
    }

    [TestMethod]
    public void TestContextMenu_SetColors_AppliesColors()
    {
        // Arrange & Act
        var contextMenu = new ContextMenu();
        contextMenu.SetBackground(new Color(50, 50, 50));
        contextMenu.SetHoverBackgroundColor(new Color(80, 80, 80));
        contextMenu.SetTextColor(Colors.White);

        // Assert
        var background = contextMenu.Background as SolidColorBackground;
        Assert.IsNotNull(background);
        Assert.AreEqual(new Color(50, 50, 50), background.Color);
        Assert.AreEqual(new Color(80, 80, 80), contextMenu.HoverBackgroundColor);
        Assert.AreEqual(Colors.White, contextMenu.TextColor);
    }

    [TestMethod]
    public void TestContextMenu_IsOpen_InitiallyFalse()
    {
        // Arrange
        var contextMenu = new ContextMenu()
            .AddItem(new MenuItem().SetText("Option 1"));

        // Assert
        Assert.IsFalse(contextMenu.IsOpen);
    }

    #endregion

    #region ContextMenu Extension Tests

    [TestMethod]
    public void TestSetContextMenu_AttachesToElement()
    {
        // Arrange
        var button = new Button().SetText("Test");
        var contextMenu = new ContextMenu()
            .AddItem(new MenuItem().SetText("Option 1"));

        // Act
        button.SetContextMenu(contextMenu);

        // Assert
        Assert.IsNotNull(button.ContextMenu);
        Assert.AreSame(contextMenu, button.ContextMenu);
    }

    [TestMethod]
    public void TestSetContextMenu_WithBuilder_CreatesMenu()
    {
        // Arrange
        var button = new Button().SetText("Test");

        // Act
        button.SetContextMenu(m => m
            .AddItem(new MenuItem().SetText("Option 1"))
            .AddItem(new MenuItem().SetText("Option 2")));

        // Assert
        Assert.IsNotNull(button.ContextMenu);
        Assert.HasCount(2, button.ContextMenu.Items);
    }

    [TestMethod]
    public void TestContextMenuExtensions_SetColors()
    {
        // Arrange
        var button = new Button().SetText("Test");

        // Act
        button.SetContextMenu(new ContextMenu())
            .SetContextMenuBackground(new Color(40, 40, 40))
            .SetContextMenuHoverBackgroundColor(new Color(70, 70, 70))
            .SetContextMenuTextColor(Colors.LightGray);

        // Assert
        Assert.IsNotNull(button.ContextMenu);
        var background = button.ContextMenu.Background as SolidColorBackground;
        Assert.IsNotNull(background);
        Assert.AreEqual(new Color(40, 40, 40), background.Color);
        Assert.AreEqual(new Color(70, 70, 70), button.ContextMenu.HoverBackgroundColor);
        Assert.AreEqual(Colors.LightGray, button.ContextMenu.TextColor);
    }

    #endregion

    #region MenuSeparator Tests

    [TestMethod]
    public void TestMenuSeparator_CanBeCreated()
    {
        // Arrange & Act
        var separator = new MenuSeparator();

        // Assert
        Assert.IsNotNull(separator);
    }

    [TestMethod]
    public void TestMenuItem_AddSeparator_CreatesSeparator()
    {
        // Arrange
        var menuItem = new MenuItem().SetText("Edit");

        // Act
        menuItem.AddItem(new MenuItem().SetText("Cut"));
        menuItem.AddSeparator();
        menuItem.AddItem(new MenuItem().SetText("Copy"));

        // Assert
        Assert.HasCount(3, menuItem.Items);
        Assert.IsInstanceOfType(menuItem.Items[1], typeof(MenuSeparator));
    }

    #endregion

    #region Nested Menu Tests

    [TestMethod]
    public void TestMenuItem_NestedSubmenus_CreatesHierarchy()
    {
        // Arrange & Act
        var fileMenu = new MenuItem()
            .SetText("File")
            .AddItem(new MenuItem()
                .SetText("Recent")
                .AddItem(new MenuItem().SetText("File1"))
                .AddItem(new MenuItem().SetText("File2")))
            .AddItem(new MenuItem().SetText("Exit"));

        // Assert
        Assert.HasCount(2, fileMenu.Items);
        var recentMenu = fileMenu.Items[0] as MenuItem;
        Assert.IsNotNull(recentMenu);
        Assert.IsTrue(recentMenu.HasSubItems);
        Assert.HasCount(2, recentMenu.Items);
    }

    #endregion

    #region Helper Classes

    private class TestCommand : ICommand
    {
        private readonly Action<object?>? _executeWithParam;
        private readonly Action? _execute;

        public TestCommand(Action execute)
        {
            _execute = execute;
        }

        public TestCommand(Action<object?> executeWithParam)
        {
            _executeWithParam = executeWithParam;
        }

        public event EventHandler? CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute?.Invoke();
            _executeWithParam?.Invoke(parameter);
        }
    }

    #endregion
}
