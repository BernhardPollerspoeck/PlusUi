using PlusUi.core;
using Silk.NET.Input;
using CursorType = PlusUi.core.CursorType;

namespace PlusUi.desktop.Cursors;

/// <summary>
/// Base for desktop cursor providers backed by the Silk.NET mouse. The mouse is wired in by
/// <see cref="DesktopCursorService"/> once the input context exists.
/// </summary>
internal abstract class SilkCursorProvider : ICursorProvider
{
    public IMouse? Mouse { get; set; }

    protected ICursor? Cursor => Mouse?.Cursor;

    public abstract bool CanProvide(CursorType cursor);

    public abstract void Apply(CursorType cursor);
}
