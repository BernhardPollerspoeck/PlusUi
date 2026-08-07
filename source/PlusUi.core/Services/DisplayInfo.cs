namespace PlusUi.core.Services;

/// <summary>
/// A connected display, as reported by <see cref="IWindowService.GetDisplays"/>.
/// <para>
/// All measurements are in <b>screen coordinates</b> — the same unit used for window
/// positions. Deliberately NOT the same unit as the layout units of the UI: PlusUi divides
/// pointer positions by the display density before they reach layout. Code that maps these
/// values onto the pixels of a screen capture has to account for <see cref="Scale"/>.
/// </para>
/// </summary>
/// <param name="Index">Position in the system's monitor enumeration.</param>
/// <param name="Name">Display name as reported by the system.</param>
/// <param name="X">Left edge within the virtual desktop. Negative when the display sits
/// to the left of the primary one.</param>
/// <param name="Y">Top edge within the virtual desktop. May be negative.</param>
/// <param name="Width">Width in screen coordinates.</param>
/// <param name="Height">Height in screen coordinates.</param>
/// <param name="Scale">Scaling factor of this display (1.0 = 100%, 1.5 = 150%). Differs
/// per display on mixed-DPI setups.</param>
/// <param name="IsPrimary">Whether this is the system's primary display.</param>
public sealed record DisplayInfo(
    int Index,
    string Name,
    int X,
    int Y,
    int Width,
    int Height,
    float Scale,
    bool IsPrimary)
{
    /// <summary>
    /// The bounds as a <see cref="Rect"/>, for callers already working with one. The
    /// individual values stay integral because display bounds are integral.
    /// </summary>
    public Rect Bounds => new(X, Y, Width, Height);
}
