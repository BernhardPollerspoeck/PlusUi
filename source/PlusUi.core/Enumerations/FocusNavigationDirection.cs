namespace PlusUi.core;

/// <summary>
/// Specifies the direction for focus navigation.
/// </summary>
public enum FocusNavigationDirection
{
    /// <summary>
    /// Navigate to the next focusable element (Tab key).
    /// </summary>
    Next,

    /// <summary>
    /// Navigate to the previous focusable element (Shift+Tab).
    /// </summary>
    Previous,

    /// <summary>
    /// Navigate to the focusable element above.
    /// </summary>
    Up,

    /// <summary>
    /// Navigate to the focusable element below.
    /// </summary>
    Down,

    /// <summary>
    /// Navigate to the focusable element to the left.
    /// </summary>
    Left,

    /// <summary>
    /// Navigate to the focusable element to the right.
    /// </summary>
    Right
}
