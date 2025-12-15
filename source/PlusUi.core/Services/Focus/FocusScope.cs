namespace PlusUi.core.Services.Focus;

/// <summary>
/// Defines a focus scope that traps tab navigation within a container.
/// Useful for dialogs, modals, and sections where focus should cycle within.
/// </summary>
public enum FocusScopeMode
{
    /// <summary>
    /// No scope - focus can move freely in and out.
    /// </summary>
    None,

    /// <summary>
    /// Traps focus within this container. Tab cycles within the scope.
    /// </summary>
    Trap,

    /// <summary>
    /// Focus enters the scope but can leave via Escape key.
    /// </summary>
    TrapWithEscape
}
