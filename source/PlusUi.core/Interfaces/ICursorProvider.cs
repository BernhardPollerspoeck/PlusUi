namespace PlusUi.core;

/// <summary>
/// A source of mouse cursors for a single <see cref="CursorType"/>. Providers are tried in
/// priority order by the <see cref="CursorResolver"/> (e.g. native OS → GLFW → self-drawn);
/// the first one that <see cref="CanProvide"/> applies the cursor.
/// </summary>
public interface ICursorProvider
{
    /// <summary>Whether this provider can supply the given cursor on the current platform.</summary>
    bool CanProvide(CursorType cursor);

    /// <summary>Applies the given cursor. Only called when <see cref="CanProvide"/> returned true.</summary>
    void Apply(CursorType cursor);
}
