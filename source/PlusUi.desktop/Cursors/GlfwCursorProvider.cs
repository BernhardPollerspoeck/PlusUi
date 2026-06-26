using Silk.NET.Input;
using CursorType = PlusUi.core.CursorType;

namespace PlusUi.desktop.Cursors;

/// <summary>
/// Supplies the GLFW standard cursors, which GLFW loads from the native OS cursor theme. Only
/// claims a cursor when GLFW actually supports it on this platform/version (probed via
/// <see cref="ICursor.IsSupported(StandardCursor)"/>), so unsupported ones fall through to the
/// self-drawn backstop instead of showing an arrow.
/// </summary>
internal sealed class GlfwCursorProvider : SilkCursorProvider
{
    public override bool CanProvide(CursorType cursor)
    {
        if (Cursor is not { } c)
        {
            return false;
        }
        return TryMap(cursor, out var standard) && c.IsSupported(standard);
    }

    public override void Apply(CursorType cursor)
    {
        if (Cursor is not { } c || !TryMap(cursor, out var standard))
        {
            return;
        }

        try
        {
            c.Type = Silk.NET.Input.CursorType.Standard;
            c.StandardCursor = standard;
        }
        catch (InvalidOperationException)
        {
            // Backend reported support but rejected the cursor — leave it unchanged rather than crash.
        }
    }

    private static bool TryMap(CursorType cursor, out StandardCursor standard)
    {
        switch (cursor)
        {
            // GLFW has no "default" cursor — the OS default IS the arrow.
            case CursorType.Default:
            case CursorType.Arrow:
                standard = StandardCursor.Arrow;
                return true;
            case CursorType.Hand: standard = StandardCursor.Hand; return true;
            case CursorType.Text: standard = StandardCursor.IBeam; return true;
            case CursorType.Crosshair: standard = StandardCursor.Crosshair; return true;
            case CursorType.Wait: standard = StandardCursor.Wait; return true;
            case CursorType.Progress: standard = StandardCursor.WaitArrow; return true;
            case CursorType.NotAllowed: standard = StandardCursor.NotAllowed; return true;
            case CursorType.ResizeHorizontal: standard = StandardCursor.HResize; return true;
            case CursorType.ResizeVertical: standard = StandardCursor.VResize; return true;
            case CursorType.ResizeAll: standard = StandardCursor.ResizeAll; return true;
            case CursorType.ResizeNwse: standard = StandardCursor.NwseResize; return true;
            case CursorType.ResizeNesw: standard = StandardCursor.NeswResize; return true;
            // PlusUi is a self-drawn brand cursor with no GLFW equivalent.
            default:
                standard = StandardCursor.Arrow;
                return false;
        }
    }
}
