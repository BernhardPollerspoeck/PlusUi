namespace PlusUi.core;

/// <summary>
/// Platform-neutral mouse cursor shapes. Mapped to the concrete platform cursor by the
/// active <see cref="IPlatformCursorService"/>. Has no effect on touch-only platforms.
/// </summary>
public enum CursorType
{
    /// <summary>The platform's default cursor (usually an arrow).</summary>
    Default,
    /// <summary>An explicit arrow cursor.</summary>
    Arrow,
    /// <summary>The PlusUi-branded arrow cursor (self-drawn; tip at the top as the hotspot).</summary>
    PlusUi,
    /// <summary>A pointing hand, typically used for clickable elements.</summary>
    Hand,
    /// <summary>An I-beam, typically used for editable text.</summary>
    Text,
    /// <summary>A crosshair.</summary>
    Crosshair,
    /// <summary>A busy/wait indicator (hourglass / spinner).</summary>
    Wait,
    /// <summary>Working in the background — arrow with a small busy indicator.</summary>
    Progress,
    /// <summary>A "not allowed" indicator.</summary>
    NotAllowed,
    /// <summary>Horizontal resize (←→).</summary>
    ResizeHorizontal,
    /// <summary>Vertical resize (↑↓).</summary>
    ResizeVertical,
    /// <summary>Move / resize in all directions.</summary>
    ResizeAll,
    /// <summary>Diagonal resize (↖↘).</summary>
    ResizeNwse,
    /// <summary>Diagonal resize (↙↗).</summary>
    ResizeNesw,
}
