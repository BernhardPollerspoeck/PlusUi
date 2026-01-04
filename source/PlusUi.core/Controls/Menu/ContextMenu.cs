using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services.DebugBridge;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A popup context menu that can be attached to any UI element.
/// Opens when the user right-clicks (or long-presses on touch devices).
/// </summary>
/// <example>
/// <code>
/// // Attach to an element
/// myButton.SetContextMenu(new ContextMenu()
///     .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X").SetCommand(vm.CutCommand))
///     .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C").SetCommand(vm.CopyCommand))
///     .AddItem(new MenuItem().SetText("Paste").SetShortcut("Ctrl+V").SetCommand(vm.PasteCommand))
///     .AddSeparator()
///     .AddItem(new MenuItem().SetText("Delete").SetCommand(vm.DeleteCommand)));
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class ContextMenu : UiElement, IDebugInspectable
{
    private static readonly IBackground DefaultBackground = new SolidColorBackground(new Color(45, 45, 45));

    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Menu;

    /// <summary>
    /// Returns the menu overlay for debug inspection.
    /// </summary>
    IEnumerable<UiElement> IDebugInspectable.GetDebugChildren() =>
        _overlay != null ? [_overlay] : [];

    public ContextMenu()
    {
        Background = DefaultBackground;
    }

    #region Items
    internal List<object> Items { get; } = new();

    /// <summary>
    /// Adds a menu item to the context menu.
    /// </summary>
    public ContextMenu AddItem(MenuItem item)
    {
        Items.Add(item);
        return this;
    }

    /// <summary>
    /// Adds a separator to the context menu.
    /// </summary>
    public ContextMenu AddItem(MenuSeparator separator)
    {
        Items.Add(separator);
        return this;
    }

    /// <summary>
    /// Adds a separator to the context menu.
    /// </summary>
    public ContextMenu AddSeparator()
    {
        Items.Add(new MenuSeparator());
        return this;
    }
    #endregion

    #region Colors
    internal Color HoverBackgroundColor
    {
        get => field;
        set { field = value; }
    } = new Color(65, 65, 65);

    public ContextMenu SetHoverBackgroundColor(Color color)
    {
        HoverBackgroundColor = color;
        return this;
    }

    public ContextMenu BindHoverBackgroundColor(string propertyName, Func<Color> propertyGetter)
    {
        RegisterBinding(propertyName, () => HoverBackgroundColor = propertyGetter());
        return this;
    }

    internal Color TextColor
    {
        get => field;
        set { field = value; }
    } = Colors.White;

    public ContextMenu SetTextColor(Color color)
    {
        TextColor = color;
        return this;
    }

    public ContextMenu BindTextColor(string propertyName, Func<Color> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextColor = propertyGetter());
        return this;
    }
    #endregion

    #region State
    private MenuOverlay? _overlay;
    private IOverlayService? _overlayService;
    #endregion

    /// <summary>
    /// Opens the context menu at the specified position.
    /// </summary>
    /// <param name="position">The position to open the menu (typically mouse position).</param>
    internal void Open(Point position)
    {
        if (Items.Count == 0) return;

        Close(); // Close any existing

        _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
        if (_overlayService == null) return;

        _overlay = new MenuOverlay(
            Items,
            position,
            openToRight: true,
            parentOverlay: null,
            onDismiss: () => _overlay = null)
        {
            HoverBackgroundColor = HoverBackgroundColor,
            TextColor = TextColor
        };

        // Pass background from this ContextMenu to the overlay
        if (Background != null)
        {
            _overlay.SetBackground(Background);
        }

        _overlayService.RegisterOverlay(_overlay);
    }

    /// <summary>
    /// Closes the context menu if it's open.
    /// </summary>
    internal void Close()
    {
        if (_overlay != null)
        {
            _overlayService?.UnregisterOverlay(_overlay);
            _overlay = null;
        }
    }

    /// <summary>
    /// Gets whether the context menu is currently open.
    /// </summary>
    public bool IsOpen => _overlay != null;
}
