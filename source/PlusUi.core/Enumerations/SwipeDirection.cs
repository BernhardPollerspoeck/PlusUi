namespace PlusUi.core;

[Flags]
public enum SwipeDirection
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
    Horizontal = Left | Right,
    Vertical = Up | Down,
    All = Horizontal | Vertical
}
