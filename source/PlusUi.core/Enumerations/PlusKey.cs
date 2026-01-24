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

    /// <summary>
    /// A key (for Ctrl+A select all).
    /// </summary>
    A = 65,

    /// <summary>
    /// C key (for Ctrl+C copy).
    /// </summary>
    C = 67,

    /// <summary>
    /// V key (for Ctrl+V paste).
    /// </summary>
    V = 86,

    /// <summary>
    /// X key (for Ctrl+X cut).
    /// </summary>
    X = 88,
}