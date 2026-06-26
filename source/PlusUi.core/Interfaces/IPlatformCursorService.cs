namespace PlusUi.core;

/// <summary>
/// Platform abstraction for controlling the mouse cursor shape. Implemented per platform
/// (e.g. Silk.NET on desktop). On touch-only platforms there is no implementation and cursor
/// requests are simply ignored.
/// </summary>
public interface IPlatformCursorService
{
    /// <summary>
    /// Sets the current mouse cursor shape.
    /// </summary>
    void SetCursor(CursorType cursor);
}
