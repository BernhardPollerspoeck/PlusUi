using System.Reflection;
using PlusUi.core.Services.DebugBridge.Models;
using SkiaSharp;

namespace PlusUi.core.Services.DebugBridge;

/// <summary>
/// Inspects and serializes the UI tree for debugging.
/// Uses IDebugInspectable for traversal and Reflection for property extraction.
/// </summary>
internal class DebugTreeInspector
{
    private readonly Dictionary<UiElement, string> _elementIds = new();
    private int _nextId = 1;

    /// <summary>
    /// Serializes the entire UI tree starting from root.
    /// </summary>
    public TreeNodeDto SerializeTree(UiElement root)
    {
        _elementIds.Clear();
        _nextId = 1;
        return SerializeElement(root);
    }

    /// <summary>
    /// Serializes a single element and its children.
    /// </summary>
    private TreeNodeDto SerializeElement(UiElement element)
    {
        var id = GetOrCreateElementId(element);
        var node = new TreeNodeDto
        {
            Id = id,
            Type = element.GetType().Name,
            IsVisible = element.IsVisible,
            Properties = ExtractProperties(element),
            Children = []
        };

        // Traverse children via IDebugInspectable
        if (element is IDebugInspectable inspectable)
        {
            foreach (var child in inspectable.GetDebugChildren())
            {
                if (child != null)
                {
                    node.Children.Add(SerializeElement(child));
                }
            }
        }

        return node;
    }

    /// <summary>
    /// Extracts properties from an element via Reflection.
    /// </summary>
    private List<PropertyDto> ExtractProperties(UiElement element)
    {
        var properties = new List<PropertyDto>();
        var type = element.GetType();

        // Get all public and internal instance properties
        var propInfos = type.GetProperties(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var prop in propInfos)
        {
            try
            {
                // Skip indexed properties
                if (prop.GetIndexParameters().Length > 0)
                    continue;

                // Skip obsolete properties
                if (prop.GetCustomAttribute<ObsoleteAttribute>() != null)
                    continue;

                var value = prop.GetValue(element);
                var valueString = FormatValue(value);

                properties.Add(new PropertyDto
                {
                    Name = prop.Name,
                    Type = prop.PropertyType.Name,
                    Value = valueString,
                    CanWrite = prop.CanWrite && prop.SetMethod != null,
                    IsInternal = !prop.GetMethod!.IsPublic
                });
            }
            catch
            {
                // Skip properties that throw on access
            }
        }

        return properties.OrderBy(p => p.Name).ToList();
    }

    /// <summary>
    /// Formats a property value for display.
    /// </summary>
    private string FormatValue(object? value)
    {
        if (value == null)
            return "null";

        // SKColor special handling
        if (value is SKColor color)
            return $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}{color.Alpha:X2}";

        // SKPaint
        if (value is SKPaint paint)
            return $"SKPaint(Color: #{paint.Color.Red:X2}{paint.Color.Green:X2}{paint.Color.Blue:X2}{paint.Color.Alpha:X2}, AA: {paint.IsAntialias})";

        // SKFont
        if (value is SKFont font)
            return $"SKFont(Size: {font.Size}, Hinting: {font.Hinting}, Subpixel: {font.Subpixel})";

        // SKTypeface
        if (value is SKTypeface typeface)
            return $"SKTypeface({typeface.FamilyName}, Weight: {typeface.FontWeight}, Style: {typeface.FontSlant})";

        // Point
        if (value is Point point)
            return $"Point({point.X}, {point.Y})";

        // Size
        if (value is Size size)
            return $"Size({size.Width}x{size.Height})";

        // Rect
        if (value is Rect rect)
            return $"Rect(X:{rect.X}, Y:{rect.Y}, W:{rect.Width}, H:{rect.Height})";

        // Color (PlusUi Color)
        if (value is Color clr)
            return $"#{clr.R:X2}{clr.G:X2}{clr.B:X2}{clr.A:X2}";

        // Enums
        if (value.GetType().IsEnum)
            return value.ToString() ?? "null";

        // Collections
        if (value is System.Collections.ICollection collection)
            return $"Count: {collection.Count}";

        // Strings
        if (value is string str)
            return str.Length > 100 ? str.Substring(0, 100) + "..." : str;

        // Numbers, booleans, etc.
        if (value.GetType().IsPrimitive || value is decimal || value is DateTime || value is DateTimeOffset)
            return value.ToString() ?? "null";

        // Objects (show type name)
        return $"[{value.GetType().Name}]";
    }

    /// <summary>
    /// Gets or creates a unique ID for an element.
    /// </summary>
    private string GetOrCreateElementId(UiElement element)
    {
        if (_elementIds.TryGetValue(element, out var id))
            return id;

        id = $"elem_{_nextId++}";
        _elementIds[element] = id;
        return id;
    }

    /// <summary>
    /// Gets details for a specific element by ID.
    /// </summary>
    public TreeNodeDto? GetElementDetails(UiElement root, string elementId)
    {
        // First, ensure IDs are populated
        SerializeTree(root);

        // Find the element by ID
        var element = _elementIds.FirstOrDefault(kvp => kvp.Value == elementId).Key;
        if (element == null)
            return null;

        return SerializeElement(element);
    }
}
