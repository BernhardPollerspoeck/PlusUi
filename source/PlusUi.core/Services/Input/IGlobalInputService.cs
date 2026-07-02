using System;

namespace PlusUi.core;

/// <summary>
/// A framework-wide, subscribable stream of raw pointer and keyboard input, decoupled from
/// hit-testing and focus. Inject this anywhere (e.g. a ViewModel) to listen to input globally —
/// independent of which control is under the pointer or focused. This is the counterpart to the
/// per-frame <see cref="GameCanvas"/>: rendering and input are separated, so a game can draw in a
/// <see cref="GameCanvas"/> while reading input from here.
/// </summary>
/// <remarks>
/// Events are raised by <see cref="InputService"/> in addition to (not instead of) the normal
/// UI input pipeline, so subscribing here never suppresses button clicks, focus, or gestures.
/// Positions are in page coordinates (the space used by hit-testing). Handlers run synchronously
/// on the render/input thread — keep them fast and do not throw.
/// </remarks>
public interface IGlobalInputService
{
    /// <summary>Raised when the pointer moves.</summary>
    event Action<PointerInputEvent>? PointerMoved;

    /// <summary>Raised when a pointer button goes down.</summary>
    event Action<PointerInputEvent>? PointerDown;

    /// <summary>Raised when a pointer button is released.</summary>
    event Action<PointerInputEvent>? PointerUp;

    /// <summary>Raised on a scroll/wheel event.</summary>
    event Action<ScrollInputEvent>? Scrolled;

    /// <summary>Raised when a key goes down (full key set, includes auto-repeat).</summary>
    event Action<KeyInputEvent>? KeyDown;

    /// <summary>Raised when a key is released.</summary>
    event Action<KeyInputEvent>? KeyUp;
}
