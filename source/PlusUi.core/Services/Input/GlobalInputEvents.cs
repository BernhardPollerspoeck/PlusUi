using System;

namespace PlusUi.core;

/// <summary>
/// Identifies a pointer/mouse button for global input events.
/// </summary>
public enum PointerButton
{
    /// <summary>No specific button (e.g. a pointer move).</summary>
    None = 0,

    /// <summary>Primary (left) button.</summary>
    Left = 1,

    /// <summary>Secondary (right) button.</summary>
    Right = 2,

    /// <summary>Middle button.</summary>
    Middle = 3,
}

/// <summary>
/// Keyboard modifier keys held while an input event was raised.
/// </summary>
[Flags]
public enum KeyModifiers
{
    /// <summary>No modifier held.</summary>
    None = 0,

    /// <summary>A Shift key is held.</summary>
    Shift = 1 << 0,

    /// <summary>A Control key is held.</summary>
    Ctrl = 1 << 1,

    /// <summary>An Alt key is held.</summary>
    Alt = 1 << 2,
}

/// <summary>
/// A raw pointer (mouse/touch) event broadcast through <see cref="IGlobalInputService"/>.
/// Position is in page coordinates (the same space used by hit-testing).
/// </summary>
public readonly struct PointerInputEvent(Point position, PointerButton button, KeyModifiers modifiers)
{
    /// <summary>Pointer position in page coordinates.</summary>
    public Point Position { get; } = position;

    /// <summary>The button involved (<see cref="PointerButton.None"/> for moves).</summary>
    public PointerButton Button { get; } = button;

    /// <summary>Modifier keys held at the time of the event.</summary>
    public KeyModifiers Modifiers { get; } = modifiers;
}

/// <summary>
/// A raw scroll/wheel event broadcast through <see cref="IGlobalInputService"/>.
/// </summary>
public readonly struct ScrollInputEvent(Point position, float deltaX, float deltaY, KeyModifiers modifiers)
{
    /// <summary>Pointer position in page coordinates.</summary>
    public Point Position { get; } = position;

    /// <summary>Horizontal scroll delta.</summary>
    public float DeltaX { get; } = deltaX;

    /// <summary>Vertical scroll delta.</summary>
    public float DeltaY { get; } = deltaY;

    /// <summary>Modifier keys held at the time of the event.</summary>
    public KeyModifiers Modifiers { get; } = modifiers;
}

/// <summary>
/// A raw keyboard event broadcast through <see cref="IGlobalInputService"/>.
/// Unlike the UI-focused <see cref="PlusKey"/> path in <see cref="InputService"/>, these events
/// carry the full, unfiltered key set (letters, digits, function keys, …) so consumers such as
/// games can react to any key and track press/release state themselves.
/// </summary>
public readonly struct KeyInputEvent(PlusKey key, KeyModifiers modifiers, bool isRepeat)
{
    /// <summary>The key that was pressed or released.</summary>
    public PlusKey Key { get; } = key;

    /// <summary>Modifier keys held at the time of the event.</summary>
    public KeyModifiers Modifiers { get; } = modifiers;

    /// <summary>True when this is an OS auto-repeat while the key is held (KeyDown only).</summary>
    public bool IsRepeat { get; } = isRepeat;
}
