namespace PlusUi.core.Services;

/// <summary>
/// Controls the application window and reports the connected displays.
/// <para>
/// This interface is registered and callable on <b>every</b> platform. Where there is no
/// window to speak of — mobile, web, headless — the implementation is a deliberate no-op
/// rather than an error. That keeps app code free of platform switches and null checks; a
/// shared surface that does nothing in places is easier to write against than one that
/// only exists in places.
/// </para>
/// <para>
/// <see cref="IsSupported"/> reports whether anything actually happens. It is not an
/// invitation to branch again — it exists so UI can hide controls that would be
/// inconsequential on the current platform.
/// </para>
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// Whether this platform has window control at all. When <c>false</c>,
    /// <see cref="EnterBorderless"/> and <see cref="RestoreNormal"/> do nothing.
    /// </summary>
    bool IsSupported { get; }

    /// <summary>Whether the window is currently in borderless mode.</summary>
    bool IsBorderless { get; }

    /// <summary>
    /// Makes the window borderless and undecorated at the given rectangle.
    /// <para>
    /// The rectangle is passed <b>explicitly</b> instead of being derived from a fullscreen
    /// flag: on every common windowing system fullscreen occupies exactly one display. An
    /// overlay meant to span several displays — a region selector, for instance — is only
    /// possible as a borderless window covering the combined area. Use
    /// <see cref="VirtualDesktopBounds"/> for that.
    /// </para>
    /// <para>
    /// Bounds are in screen coordinates, not layout units. Calling this while already
    /// borderless re-targets the window and keeps the originally saved state, so
    /// <see cref="RestoreNormal"/> still returns to where the user had it. That allows
    /// moving the window out of the way and covering the screen as one continuous
    /// operation, without flashing it back at its old position in between.
    /// </para>
    /// </summary>
    /// <param name="bounds">Target rectangle in screen coordinates.</param>
    /// <param name="topMost">Whether the window should sit above all others.</param>
    void EnterBorderless(Rect bounds, bool topMost);

    /// <summary>
    /// Restores position, size, border style, window state and top-most flag to what they
    /// were before <see cref="EnterBorderless"/>. The caller does not have to remember
    /// anything. Does nothing without a preceding switch.
    /// </summary>
    void RestoreNormal();

    /// <summary>
    /// The window's current position and size in screen coordinates.
    /// </summary>
    Rect Bounds { get; }

    /// <summary>
    /// Moves the window, leaving its size, border and borderless state alone.
    /// <para>
    /// This is what a window with its own chrome needs: once the title bar is hidden, the
    /// operating system no longer offers a way to drag the window, so the application has to
    /// move it. Deliberately separate from <see cref="EnterBorderless"/> — moving is not a
    /// mode change, and routing it through one would make every drag look like the start of
    /// an overlay and destroy the state <see cref="RestoreNormal"/> returns to.
    /// </para>
    /// </summary>
    void MoveTo(float x, float y);

    /// <summary>
    /// The connected displays, in the order reported by the system.
    /// <para>
    /// Platforms without a window concept return a single entry describing the drawing
    /// surface — never an empty list. A caller looking for "the screen" therefore finds one
    /// there too, instead of having to handle a special case.
    /// </para>
    /// </summary>
    IReadOnlyList<DisplayInfo> GetDisplays();

    /// <summary>
    /// The bounding rectangle across all displays, in screen coordinates. With multiple
    /// displays the origin can be negative — a display to the left of the primary one sits
    /// at a negative X.
    /// </summary>
    Rect VirtualDesktopBounds { get; }
}
