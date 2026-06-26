using PlusUi.core;
using PlusUi.desktop.Cursors;
using Silk.NET.Input;
using CursorType = PlusUi.core.CursorType;

namespace PlusUi.desktop;

/// <summary>
/// Desktop implementation of <see cref="IPlatformCursorService"/>. Resolves each cursor through
/// a provider chain — GLFW standard cursors (native OS theme) first, then a self-drawn backstop
/// that can render anything GLFW lacks (e.g. the PlusUi-branded cursor, or Wait on backends
/// without it). The mouse is wired up by <see cref="WindowManager"/> after input setup.
/// </summary>
internal sealed class DesktopCursorService : IPlatformCursorService
{
    private readonly List<SilkCursorProvider> _providers;
    private readonly CursorResolver _resolver;
    private CursorType _current = CursorType.Default;
    private bool _hasMouse;

    public DesktopCursorService()
    {
        _providers =
        [
            new GlfwCursorProvider(),
            new SelfDrawnCursorProvider(new CursorBitmapFactory()),
        ];
        _resolver = new CursorResolver(_providers);
    }

    /// <summary>Connects the Silk.NET mouse. Called by <see cref="WindowManager"/> after input setup.</summary>
    public void SetMouse(IMouse mouse)
    {
        foreach (var provider in _providers)
        {
            provider.Mouse = mouse;
        }
        _hasMouse = true;
        _resolver.Apply(_current);
    }

    public void SetCursor(CursorType cursor)
    {
        if (_current == cursor && _hasMouse)
        {
            return;
        }
        _current = cursor;
        if (_hasMouse)
        {
            _resolver.Apply(cursor);
        }
    }
}
