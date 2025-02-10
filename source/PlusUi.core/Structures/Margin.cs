namespace PlusUi.core.Structures;

public struct Margin(float left, float top, float right, float bottom)
{
    public float Left { get; set; } = left;
    public float Top { get; set; } = top;
    public float Right { get; set; } = right;
    public float Bottom { get; set; } = bottom;

    public Margin(float size) : this(size, size, size, size)
    {
    }
}
