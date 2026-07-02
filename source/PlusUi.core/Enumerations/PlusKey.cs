namespace PlusUi.core;

/// <summary>
/// Represents special keyboard keys for input handling across platforms.
/// </summary>
public enum PlusKey
{
    /// <summary>
    /// Unknown or unmapped key.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Backspace key for deleting the previous character.
    /// </summary>
    Backspace = 1,

    /// <summary>
    /// Enter/Return key.
    /// </summary>
    Enter = 2,

    /// <summary>
    /// Tab key for navigation between controls.
    /// </summary>
    Tab = 3,

    /// <summary>
    /// Space bar key.
    /// </summary>
    Space = 5,

    /// <summary>
    /// Shift+Tab key combination for reverse focus navigation.
    /// </summary>
    ShiftTab = 6,

    /// <summary>
    /// Escape key for canceling or closing.
    /// </summary>
    Escape = 7,

    /// <summary>
    /// Arrow Up key for directional navigation.
    /// </summary>
    ArrowUp = 8,

    /// <summary>
    /// Arrow Down key for directional navigation.
    /// </summary>
    ArrowDown = 9,

    /// <summary>
    /// Arrow Left key for directional navigation.
    /// </summary>
    ArrowLeft = 10,

    /// <summary>
    /// Arrow Right key for directional navigation.
    /// </summary>
    ArrowRight = 11,

    /// <summary>
    /// Delete key for deleting the next character.
    /// </summary>
    Delete = 12,

    /// <summary>
    /// Home key for jumping to the beginning.
    /// </summary>
    Home = 13,

    /// <summary>
    /// End key for jumping to the end.
    /// </summary>
    End = 14,

    // ---- Letters (ASCII-aligned, A-Z). A/C/V/X predate the full set but keep their values. ----
    A = 65,
    B = 66,
    C = 67,
    D = 68,
    E = 69,
    F = 70,
    G = 71,
    H = 72,
    I = 73,
    J = 74,
    K = 75,
    L = 76,
    M = 77,
    N = 78,
    O = 79,
    P = 80,
    Q = 81,
    R = 82,
    S = 83,
    T = 84,
    U = 85,
    V = 86,
    W = 87,
    X = 88,
    Y = 89,
    Z = 90,

    // ---- Digits (top-row, ASCII-aligned '0'-'9'). ----
    D0 = 48,
    D1 = 49,
    D2 = 50,
    D3 = 51,
    D4 = 52,
    D5 = 53,
    D6 = 54,
    D7 = 55,
    D8 = 56,
    D9 = 57,

    // ---- Function keys (VK-aligned, F1 = 0x70). ----
    F1 = 112,
    F2 = 113,
    F3 = 114,
    F4 = 115,
    F5 = 116,
    F6 = 117,
    F7 = 118,
    F8 = 119,
    F9 = 120,
    F10 = 121,
    F11 = 122,
    F12 = 123,

    // ---- Modifier keys (VK-aligned left/right variants). ----
    LeftShift = 160,
    RightShift = 161,
    LeftCtrl = 162,
    RightCtrl = 163,
    LeftAlt = 164,
    RightAlt = 165,
}