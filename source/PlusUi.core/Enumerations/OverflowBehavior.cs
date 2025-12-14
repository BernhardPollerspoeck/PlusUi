namespace PlusUi.core;

/// <summary>
/// Specifies how a toolbar handles overflow when there is insufficient space for all items.
/// </summary>
public enum OverflowBehavior
{
    /// <summary>
    /// No overflow handling - items are clipped when there is insufficient space.
    /// </summary>
    None = 0,

    /// <summary>
    /// Items that don't fit are moved to an overflow menu accessible via a "..." button.
    /// </summary>
    CollapseToMenu = 1,

    /// <summary>
    /// Toolbar becomes horizontally scrollable when items don't fit.
    /// </summary>
    Scroll = 2
}
