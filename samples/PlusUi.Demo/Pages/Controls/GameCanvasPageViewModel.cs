using System;
using System.Collections.Generic;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

/// <summary>
/// Demo view model for <see cref="GameCanvas"/>. Holds the game state (a movable circle) and reads
/// input from the framework-wide <see cref="IGlobalInputService"/> — completely decoupled from the
/// canvas that renders it. The canvas' per-frame draw loop calls <see cref="Step"/> to advance the
/// state using the frame delta.
/// </summary>
public sealed partial class GameCanvasPageViewModel : DemoPageViewModel, IDisposable
{
    private const float Speed = 240f; // pixels per second

    private readonly IGlobalInputService _input;
    private readonly HashSet<PlusKey> _pressed = [];

    public float X { get; private set; } = 120f;
    public float Y { get; private set; } = 120f;
    public float Radius { get; private set; } = 28f;
    public bool IsPointerDown { get; private set; }

    public GameCanvasPageViewModel(INavigationService navigation, IGlobalInputService input)
        : base(navigation)
    {
        _input = input;
        _input.KeyDown += OnKeyDown;
        _input.KeyUp += OnKeyUp;
        _input.PointerDown += OnPointerDown;
        _input.PointerUp += OnPointerUp;
        _input.Scrolled += OnScrolled;
    }

    private void OnKeyDown(KeyInputEvent e) => _pressed.Add(e.Key);
    private void OnKeyUp(KeyInputEvent e) => _pressed.Remove(e.Key);
    private void OnPointerDown(PointerInputEvent e) { if (e.Button == PointerButton.Left) IsPointerDown = true; }
    private void OnPointerUp(PointerInputEvent e) { if (e.Button == PointerButton.Left) IsPointerDown = false; }
    private void OnScrolled(ScrollInputEvent e) => Radius = Math.Clamp(Radius + e.DeltaY * 0.02f, 8f, 120f);

    /// <summary>Advance the game one frame. Called from the <see cref="GameCanvas"/> draw loop.</summary>
    public void Step(float dtSeconds, float width, float height)
    {
        var dx = 0f;
        var dy = 0f;
        if (_pressed.Contains(PlusKey.A) || _pressed.Contains(PlusKey.ArrowLeft)) dx -= 1f;
        if (_pressed.Contains(PlusKey.D) || _pressed.Contains(PlusKey.ArrowRight)) dx += 1f;
        if (_pressed.Contains(PlusKey.W) || _pressed.Contains(PlusKey.ArrowUp)) dy -= 1f;
        if (_pressed.Contains(PlusKey.S) || _pressed.Contains(PlusKey.ArrowDown)) dy += 1f;

        // Normalize diagonal movement so it isn't faster than axis-aligned movement.
        if (dx != 0f && dy != 0f)
        {
            const float inv = 0.70710677f;
            dx *= inv;
            dy *= inv;
        }

        X += dx * Speed * dtSeconds;
        Y += dy * Speed * dtSeconds;

        if (width > 0 && height > 0)
        {
            X = Math.Clamp(X, Radius, width - Radius);
            Y = Math.Clamp(Y, Radius, height - Radius);
        }
    }

    public void Dispose()
    {
        _input.KeyDown -= OnKeyDown;
        _input.KeyUp -= OnKeyUp;
        _input.PointerDown -= OnPointerDown;
        _input.PointerUp -= OnPointerUp;
        _input.Scrolled -= OnScrolled;
    }
}
