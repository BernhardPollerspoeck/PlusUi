namespace PlusUi.core;

public readonly struct SizeI(int width, int height)
{
    public static readonly SizeI Empty = new(0, 0);
    public int Width { get; } = width;
    public int Height { get; } = height;
}