using CursorType = PlusUi.core.CursorType;

namespace PlusUi.desktop.Cursors;

/// <summary>
/// The universal backstop: renders any <see cref="CursorType"/> as a self-drawn custom cursor.
/// Last in the provider chain, so it only runs for cursors the OS/GLFW couldn't supply (and for
/// the PlusUi-branded cursor, which has no standard equivalent).
/// </summary>
internal sealed class SelfDrawnCursorProvider(CursorBitmapFactory factory) : SilkCursorProvider
{
    public override bool CanProvide(CursorType cursor) => Cursor is not null;

    public override void Apply(CursorType cursor)
    {
        if (Cursor is not { } c)
        {
            return;
        }

        var bitmap = factory.Get(cursor);
        c.Type = Silk.NET.Input.CursorType.Custom;
        c.HotspotX = bitmap.HotspotX;
        c.HotspotY = bitmap.HotspotY;
        c.Image = bitmap.Image;
    }
}
