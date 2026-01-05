using System.ComponentModel;
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
            Properties = ExtractProperties(element, id),
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
    private List<PropertyDto> ExtractProperties(UiElement element, string elementId)
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

                var propertyDto = new PropertyDto
                {
                    Name = prop.Name,
                    Type = prop.PropertyType.Name,
                    Value = valueString,
                    CanWrite = prop.CanWrite && prop.SetMethod != null,
                    IsInternal = !prop.GetMethod!.IsPublic,
                    Path = prop.Name,
                    ElementId = elementId
                };

                ExpandComplexProperty(propertyDto, value, prop.Name, elementId);
                properties.Add(propertyDto);
            }
            catch
            {
                // Skip properties that throw on access
            }
        }

        return properties.OrderBy(p => p.Name).ToList();
    }

    private void ExpandComplexProperty(PropertyDto property, object? value, string parentPath, string elementId, int depth = 0)
    {
        if (value == null || depth > 2) return;

        var type = value.GetType();

        if (IsSimpleType(type))
            return;

        if (type.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
            return;

        if (type == typeof(Type) || type.IsSubclassOf(typeof(Type)))
            return;

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            try
            {
                if (prop.GetIndexParameters().Length > 0)
                    continue;

                if (!prop.CanRead || prop.GetMethod == null)
                    continue;

                if (prop.DeclaringType == typeof(UiElement) || prop.DeclaringType?.IsSubclassOf(typeof(UiElement)) == true)
                    continue;

                if (prop.GetCustomAttribute<ObsoleteAttribute>() != null)
                    continue;

                if (prop.PropertyType == typeof(Type) || prop.PropertyType.IsSubclassOf(typeof(Type)))
                    continue;

                if (prop.PropertyType.IsGenericParameter)
                    continue;

                if (prop.PropertyType.IsPointer || prop.PropertyType.IsByRef)
                    continue;

                object? childValue = null;
                try
                {
                    childValue = prop.GetValue(value);
                }
                catch (TargetInvocationException)
                {
                    continue;
                }
                catch (NotSupportedException)
                {
                    continue;
                }

                var childValueString = FormatValue(childValue);
                var childPath = $"{parentPath}.{prop.Name}";

                var childDto = new PropertyDto
                {
                    Name = prop.Name,
                    Type = prop.PropertyType.Name,
                    Value = childValueString,
                    CanWrite = prop.CanWrite && prop.SetMethod?.IsPublic == true,
                    IsInternal = false,
                    Path = childPath,
                    ElementId = elementId
                };

                ExpandComplexProperty(childDto, childValue, childPath, elementId, depth + 1);
                property.Children.Add(childDto);
            }
            catch (Exception)
            {
            }
        }
    }

    private bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid);
    }

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

    /// <summary>
    /// Updates a property value on an element.
    /// </summary>
    public bool UpdateProperty(UiElement root, string elementId, string propertyPath, string value)
    {
        try
        {
            // Ensure IDs are populated
            SerializeTree(root);

            // Find the element by ID
            var element = _elementIds.FirstOrDefault(kvp => kvp.Value == elementId).Key;
            if (element == null)
                return false;

            // Parse property path (e.g., "Margin.Left" -> ["Margin", "Left"])
            var pathParts = propertyPath.Split('.');
            if (pathParts.Length == 0)
                return false;

            // For simple properties (no nesting), set directly
            if (pathParts.Length == 1)
            {
                var property = element.GetType().GetProperty(pathParts[0],
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (property == null || !property.CanWrite)
                    return false;

                var convertedValue = ConvertValue(value, property.PropertyType);
                if (convertedValue == null && property.PropertyType.IsValueType &&
                    Nullable.GetUnderlyingType(property.PropertyType) == null)
                {
                    return false;
                }

                property.SetValue(element, convertedValue);
                return true;
            }

            // For nested properties, we need to handle structs specially
            // Build a stack of objects and properties to navigate back up
            var objectStack = new Stack<(object obj, PropertyInfo prop, bool isStruct)>();
            object? currentObject = element;

            // Navigate down the path, tracking each step
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                var propertyName = pathParts[i];
                var type = currentObject.GetType();
                var property = type.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (property == null)
                    return false;

                var isStruct = property.PropertyType.IsValueType && !property.PropertyType.IsPrimitive && !property.PropertyType.IsEnum;
                objectStack.Push((currentObject, property, isStruct));

                currentObject = property.GetValue(currentObject);
                if (currentObject == null)
                    return false;
            }

            // Set the final property
            var finalPropertyName = pathParts[^1];
            var finalType = currentObject.GetType();
            var finalProperty = finalType.GetProperty(finalPropertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (finalProperty == null)
                return false;

            var finalConvertedValue = ConvertValue(value, finalProperty.PropertyType);
            if (finalConvertedValue == null && finalProperty.PropertyType.IsValueType &&
                Nullable.GetUnderlyingType(finalProperty.PropertyType) == null)
            {
                return false;
            }

            // Handle readonly struct properties (like Point.X, Point.Y)
            if (!finalProperty.CanWrite)
            {
                // For readonly properties in structs, we need to recreate the entire struct
                if (currentObject.GetType().IsValueType)
                {
                    currentObject = CreateStructWithModifiedProperty(currentObject, finalPropertyName, finalConvertedValue);
                }
                else
                {
                    return false; // Can't set readonly property on reference types
                }
            }
            else
            {
                finalProperty.SetValue(currentObject, finalConvertedValue);
            }

            // Now propagate the changes back up the stack for structs
            while (objectStack.Count > 0)
            {
                var (parentObject, parentProperty, isStruct) = objectStack.Pop();

                // For structs, we need to set the modified copy back to the parent
                if (isStruct)
                {
                    parentProperty.SetValue(parentObject, currentObject);
                }

                currentObject = parentObject;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a new struct instance with one property modified.
    /// Used for readonly structs with primary constructors like Point, Size, etc.
    /// </summary>
    private object CreateStructWithModifiedProperty(object structInstance, string propertyName, object? newValue)
    {
        var structType = structInstance.GetType();

        // Get all public properties
        var properties = structType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(p => p.MetadataToken) // Preserve declaration order
            .ToArray();

        // Try to find a constructor that matches the properties
        var constructors = structType.GetConstructors();

        // Look for primary constructor (matches property count and types)
        foreach (var ctor in constructors)
        {
            var parameters = ctor.GetParameters();
            if (parameters.Length != properties.Length)
                continue;

            // Check if parameter types match property types
            bool matches = true;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != properties[i].PropertyType)
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                // Build arguments for constructor
                var args = new object?[parameters.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    if (string.Equals(properties[i].Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        args[i] = newValue;
                    }
                    else
                    {
                        args[i] = properties[i].GetValue(structInstance);
                    }
                }

                return ctor.Invoke(args);
            }
        }

        throw new InvalidOperationException($"Could not find suitable constructor for struct {structType.Name}");
    }

    /// <summary>
    /// Converts a string value to the target type.
    /// </summary>
    private object? ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        try
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Primitives and common types
            if (underlyingType == typeof(string))
                return value;

            if (underlyingType == typeof(int))
                return int.Parse(value);

            if (underlyingType == typeof(float))
                return float.Parse(value);

            if (underlyingType == typeof(double))
                return double.Parse(value);

            if (underlyingType == typeof(bool))
                return bool.Parse(value);

            if (underlyingType == typeof(byte))
                return byte.Parse(value);

            // Enums
            if (underlyingType.IsEnum)
                return Enum.Parse(underlyingType, value, ignoreCase: true);

            // Color (hex string like #RRGGBBAA)
            if (underlyingType == typeof(Color))
            {
                if (value.StartsWith("#") && value.Length >= 7)
                {
                    var r = Convert.ToByte(value.Substring(1, 2), 16);
                    var g = Convert.ToByte(value.Substring(3, 2), 16);
                    var b = Convert.ToByte(value.Substring(5, 2), 16);
                    var a = value.Length >= 9 ? Convert.ToByte(value.Substring(7, 2), 16) : (byte)255;
                    return new Color(r, g, b, a);
                }
            }

            // SKColor (hex string like #RRGGBBAA)
            if (underlyingType == typeof(SKColor))
            {
                if (value.StartsWith("#") && value.Length >= 7)
                {
                    var r = Convert.ToByte(value.Substring(1, 2), 16);
                    var g = Convert.ToByte(value.Substring(3, 2), 16);
                    var b = Convert.ToByte(value.Substring(5, 2), 16);
                    var a = value.Length >= 9 ? Convert.ToByte(value.Substring(7, 2), 16) : (byte)255;
                    return new SKColor(r, g, b, a);
                }
            }

            // Fallback: use type converter
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(underlyingType);
            if (converter.CanConvertFrom(typeof(string)))
                return converter.ConvertFromString(value);

            return null;
        }
        catch
        {
            return null;
        }
    }
}
