using System;

namespace PlusUi.core;

/// <summary>
/// Default <see cref="IGlobalInputService"/> implementation: a pure broadcast hub. It holds no
/// state and performs no hit-testing; <see cref="InputService"/> pushes raw events into it via the
/// internal <c>Raise*</c> methods, and subscribers receive them.
/// </summary>
public class GlobalInputService : IGlobalInputService
{
    public event Action<PointerInputEvent>? PointerMoved;
    public event Action<PointerInputEvent>? PointerDown;
    public event Action<PointerInputEvent>? PointerUp;
    public event Action<ScrollInputEvent>? Scrolled;
    public event Action<KeyInputEvent>? KeyDown;
    public event Action<KeyInputEvent>? KeyUp;

    internal void RaisePointerMoved(PointerInputEvent e) => PointerMoved?.Invoke(e);
    internal void RaisePointerDown(PointerInputEvent e) => PointerDown?.Invoke(e);
    internal void RaisePointerUp(PointerInputEvent e) => PointerUp?.Invoke(e);
    internal void RaiseScrolled(ScrollInputEvent e) => Scrolled?.Invoke(e);
    internal void RaiseKeyDown(KeyInputEvent e) => KeyDown?.Invoke(e);
    internal void RaiseKeyUp(KeyInputEvent e) => KeyUp?.Invoke(e);
}
