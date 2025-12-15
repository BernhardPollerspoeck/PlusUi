namespace PlusUi.core;

/// <summary>
/// Specifies the preferred placement of a tooltip relative to its target element.
/// </summary>
public enum TooltipPlacement
{
    /// <summary>
    /// Automatically determines the best placement based on available screen space.
    /// Tries Top first, then Bottom, Right, and Left as fallbacks.
    /// </summary>
    Auto,

    /// <summary>
    /// Places the tooltip above the target element.
    /// </summary>
    Top,

    /// <summary>
    /// Places the tooltip below the target element.
    /// </summary>
    Bottom,

    /// <summary>
    /// Places the tooltip to the left of the target element.
    /// </summary>
    Left,

    /// <summary>
    /// Places the tooltip to the right of the target element.
    /// </summary>
    Right
}
