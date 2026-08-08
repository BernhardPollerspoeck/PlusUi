using PlusUi.core;
using PlusUi.core.Services;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.Windowing;
// PlusUi.core carries its own WindowBorder/WindowState enums for the public configuration
// surface. Inside this class the Silk ones are meant, so alias them the same way
// WindowManager does rather than fully qualifying every use.
using WindowBorder = Silk.NET.Windowing.WindowBorder;
using WindowState = Silk.NET.Windowing.WindowState;

namespace PlusUi.desktop;

/// <summary>
/// Desktop window control, backed by the Silk.NET window and GLFW's monitor API.
/// <para>
/// Silk and GLFW types stay inside this class on purpose — <c>PlusUi.core</c> has no
/// windowing dependency, and the public surface (<see cref="IWindowService"/>,
/// <see cref="DisplayInfo"/>) is built entirely from PlusUi's own types so that consumers
/// never take one on either.
/// </para>
/// </summary>
public sealed class DesktopWindowService : IWindowService
{
    /// <summary>
    /// Everything that has to be put back by <see cref="RestoreNormal"/>. Captured as one
    /// value so there is no way to save half of it.
    /// </summary>
    private readonly record struct PreviousState(
        Vector2D<int> Position,
        Vector2D<int> Size,
        WindowBorder Border,
        WindowState State,
        bool TopMost);

    /// <summary>The limits last handed to <see cref="SetSizeLimits"/>, null meaning unbounded.</summary>
    internal readonly record struct SizeLimits(
        float? MinWidth,
        float? MinHeight,
        float? MaxWidth,
        float? MaxHeight)
    {
        /// <summary>
        /// The nearest size to <paramref name="width"/> × <paramref name="height"/> that the
        /// limits allow. A minimum wins over a maximum where the two contradict, because the
        /// minimum is the one that keeps a layout usable.
        /// </summary>
        public (float Width, float Height) Clamp(float width, float height)
        {
            if (MaxWidth is > 0 && width > MaxWidth) width = MaxWidth.Value;
            if (MaxHeight is > 0 && height > MaxHeight) height = MaxHeight.Value;
            if (MinWidth is > 0 && width < MinWidth) width = MinWidth.Value;
            if (MinHeight is > 0 && height < MinHeight) height = MinHeight.Value;

            return (width, height);
        }
    }

    private IWindow? _window;
    private PreviousState? _previous;
    private SizeLimits _limits;

    /// <summary>The limits currently in force. For tests — the window itself is not needed to decide them.</summary>
    internal SizeLimits CurrentLimits => _limits;

    /// <summary>
    /// Sets the window reference (called by WindowManager after the window is created).
    /// </summary>
    internal void SetWindow(IWindow window) => _window = window;

    public bool IsSupported => _window is not null;

    public bool IsBorderless => _previous is not null;

    public void EnterBorderless(Rect bounds, bool topMost)
    {
        if (_window is null)
            return;

        // Saved only on the way in. A second call re-targets the window and deliberately
        // keeps the original state, so RestoreNormal still returns to where the user had it
        // - saving again would record the overlay as the place to go back to.
        //
        // Re-targeting matters: moving the window out of the way, doing something to the
        // screen underneath it, then covering the desktop is one continuous operation. Going
        // through RestoreNormal in between would flash the window back at its old position.
        _previous ??= new PreviousState(
            _window.Position,
            _window.Size,
            _window.WindowBorder,
            _window.WindowState,
            _window.TopMost);

        // Order matters here, and each step has a reason:
        // 1. A maximized window ignores position and size changes - leave that state first.
        // 2. Drop the decoration before positioning, otherwise the frame is still part of
        //    the geometry and the window ends up offset by the border width.
        // 3. Position before size is irrelevant, but top-most last avoids the window
        //    briefly floating above everything while still at its old geometry.
        _window.WindowState = WindowState.Normal;
        _window.WindowBorder = WindowBorder.Hidden;
        _window.Position = new Vector2D<int>((int)MathF.Round(bounds.X), (int)MathF.Round(bounds.Y));
        _window.Size = new Vector2D<int>((int)MathF.Round(bounds.Width), (int)MathF.Round(bounds.Height));
        _window.TopMost = topMost;
    }

    public void RestoreNormal()
    {
        if (_window is null || _previous is not { } previous)
            return;

        // Reverse order of EnterBorderless: geometry is restored while the window is still
        // undecorated and in Normal state, because restoring the border or a Maximized
        // state first would make the window manager recompute the geometry we are about to
        // set - and the saved position would be silently discarded.
        _window.TopMost = previous.TopMost;
        _window.Position = previous.Position;
        _window.Size = previous.Size;
        _window.WindowBorder = previous.Border;
        _window.WindowState = previous.State;

        _previous = null;
    }

    public Rect Bounds => _window is null
        ? Rect.Empty
        : new Rect(_window.Position.X, _window.Position.Y, _window.Size.X, _window.Size.Y);

    public void MoveTo(float x, float y)
    {
        if (_window is null)
            return;

        // Only the position. Notably _previous is left untouched, so dragging a borderless
        // window around does not become the state RestoreNormal returns to.
        _window.Position = new Vector2D<int>((int)MathF.Round(x), (int)MathF.Round(y));
    }

    public void Resize(float width, float height)
    {
        if (_window is null)
            return;

        // Clamped here rather than left to GLFW, because GLFW cannot always do it. Its size
        // limits are enforced through the window manager's interactive resize, which a
        // borderless window does not have — GLFW_RESIZABLE is off for one, and nothing ever
        // asks the window how small it may get. Measured: with a decorated window the limits
        // hold, with a borderless one the same call shrinks straight past them.
        //
        // So the limits live here as well. An application that sets them is entitled to have
        // them apply to its own resizes on every kind of window, not just the ones where the
        // window manager happens to help.
        (width, height) = _limits.Clamp(width, height);

        var w = (int)MathF.Round(width);
        var h = (int)MathF.Round(height);

        // Dropped rather than clamped. A drag handle pulled past the opposite edge produces
        // these constantly, and a window collapsed to nothing has no edge left to grab.
        if (w < 1 || h < 1)
            return;

        _window.Size = new Vector2D<int>(w, h);
    }

    public void Close() => _window?.Close();

    public unsafe void SetSizeLimits(float? minWidth, float? minHeight, float? maxWidth, float? maxHeight)
    {
        // GLFW_DONT_CARE. Passing it for a bound removes that bound rather than pinning it
        // to zero or to some arbitrary large number.
        const int dontCare = -1;

        // Remembered before GLFW is asked, and remembered even when there is no window yet:
        // these are the application's limits, and Resize below has to honour them on every
        // path GLFW does not cover.
        _limits = new SizeLimits(minWidth, minHeight, maxWidth, maxHeight);

        var handle = _window?.Native?.Glfw;
        if (handle is null)
            return;

        static int Limit(float? value) => value is > 0 ? (int)MathF.Round(value.Value) : dontCare;

        Glfw.GetApi().SetWindowSizeLimits(
            (WindowHandle*)handle.Value,
            Limit(minWidth), Limit(minHeight),
            Limit(maxWidth), Limit(maxHeight));

        // A limit takes effect now, not at the next resize. GLFW does this itself for the
        // windows it can, and staying silent for the others would make the same call mean two
        // different things depending on whether the window has a border.
        var (fittedWidth, fittedHeight) = _limits.Clamp(_window!.Size.X, _window.Size.Y);
        if ((int)fittedWidth != _window.Size.X || (int)fittedHeight != _window.Size.Y)
            Resize(fittedWidth, fittedHeight);
    }

    public unsafe IReadOnlyList<DisplayInfo> GetDisplays()
    {
        // GLFW is only initialized once the window exists. Before that there is nothing
        // sensible to report, and asking would fail rather than return an empty list.
        if (_window is null)
            return [];

        var glfw = Glfw.GetApi();
        var monitors = glfw.GetMonitors(out var count);
        if (monitors is null || count <= 0)
            return [];

        var primary = glfw.GetPrimaryMonitor();
        var displays = new List<DisplayInfo>(count);

        for (var i = 0; i < count; i++)
        {
            var monitor = monitors[i];

            // The video mode carries the resolution. Without it the entry would be a
            // display of unknown size, which is worse than not listing it at all.
            var mode = glfw.GetVideoMode(monitor);
            if (mode is null)
                continue;

            glfw.GetMonitorPos(monitor, out var x, out var y);

            // Per monitor, not global: on mixed-DPI setups the factors differ, and a single
            // global value would be wrong for every display but one.
            glfw.GetMonitorContentScale(monitor, out var scaleX, out _);

            var name = glfw.GetMonitorName(monitor);

            displays.Add(new DisplayInfo(
                Index: i,
                Name: string.IsNullOrEmpty(name) ? $"Display {i}" : name,
                X: x,
                Y: y,
                Width: mode->Width,
                Height: mode->Height,
                Scale: scaleX,
                IsPrimary: monitor == primary));
        }

        return displays;
    }

    public Rect VirtualDesktopBounds
    {
        get
        {
            var displays = GetDisplays();

            // Before the window exists there are no monitors to union. Falling back to the
            // window geometry keeps callers on a single code path; it is the honest answer
            // for "the area available to me" at that point.
            if (displays.Count == 0)
            {
                return _window is null
                    ? Rect.Empty
                    : new Rect(_window.Position.X, _window.Position.Y, _window.Size.X, _window.Size.Y);
            }

            var left = displays.Min(d => d.X);
            var top = displays.Min(d => d.Y);
            var right = displays.Max(d => d.X + d.Width);
            var bottom = displays.Max(d => d.Y + d.Height);

            return new Rect(left, top, right - left, bottom - top);
        }
    }
}
