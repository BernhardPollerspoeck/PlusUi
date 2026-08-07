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

    private IWindow? _window;
    private PreviousState? _previous;

    /// <summary>
    /// Sets the window reference (called by WindowManager after the window is created).
    /// </summary>
    internal void SetWindow(IWindow window) => _window = window;

    public bool IsSupported => _window is not null;

    public bool IsBorderless => _previous is not null;

    public void EnterBorderless(Rect bounds, bool topMost)
    {
        // Ignoring the second call is what keeps RestoreNormal honest: a re-entry would
        // otherwise save the overlay state as the state to return to.
        if (_window is null || _previous is not null)
            return;

        _previous = new PreviousState(
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
