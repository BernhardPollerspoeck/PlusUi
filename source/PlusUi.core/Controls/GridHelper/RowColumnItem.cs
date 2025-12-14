namespace PlusUi.core.Controls.GridHelper;

public class RowColumnItem<TType>(string? propertyName, TType type, float? fixedSize = null, Func<float>? boundSize = null)
    where TType : struct
{
    public TType Type { get; } = type;

    public float? FixedSize { get; } = fixedSize;
    public string? PropertyName { get; } = propertyName;
    public Func<float>? BoundSize { get; } = boundSize;

    public float MeasuredSize { get; set; }

    public event Action? SizeChanged;

    public float GetSize()
    {
        var newSize = FixedSize ?? BoundSize?.Invoke() ?? 0;
        if (newSize != MeasuredSize)
        {
            MeasuredSize = newSize;
            SizeChanged?.Invoke();
        }
        return MeasuredSize;
    }
}
