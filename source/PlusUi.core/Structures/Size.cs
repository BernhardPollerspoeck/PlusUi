namespace PlusUi.core.Structures;

public readonly struct Size(float width, float height)
{
    public static readonly Size Empty = new(0, 0);
    public float Width { get; } = width;
    public float Height { get; } = height;

    public static Size operator +(Size size, Margin margin)
    {
        return new Size(size.Width + margin.Left + margin.Right, size.Height + margin.Top + margin.Bottom);
    }
}
