namespace PlusUi.core.Structures;

public readonly struct Rect(float x, float y, float width, float height)
{
    public static readonly Rect Empty = new(0, 0, 0, 0);
    public float X { get; } = x;
    public float Y { get; } = y;
    public float Width { get; } = width;
    public float Height { get; } = height;
    public float Left => X;
    public float Top => Y;
    public float Right => X + Width;
    public float Bottom => Y + Height;
    public float CenterX => X + (Width / 2);
    public float CenterY => Y + (Height / 2);
    public bool Contains(Point point)
    {
        return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
    }

    public Rect(Point location, Size size)
        : this(location.X, location.Y, size.Width, size.Height)
    {
    }
}