namespace PlusUi.core;

/// <summary>
/// Resolves a <see cref="CursorType"/> through an ordered list of <see cref="ICursorProvider"/>s.
/// The first provider that can provide the cursor applies it. A typical desktop ordering is
/// native OS → GLFW standard → self-drawn backstop, where the backstop can provide any cursor.
/// </summary>
public sealed class CursorResolver(IReadOnlyList<ICursorProvider> providers)
{
    /// <summary>
    /// Applies the given cursor via the first provider that can provide it. Does nothing if no
    /// provider can (in practice a self-drawn backstop should always be able to).
    /// </summary>
    public void Apply(CursorType cursor)
    {
        foreach (var provider in providers)
        {
            if (provider.CanProvide(cursor))
            {
                provider.Apply(cursor);
                return;
            }
        }
    }
}
