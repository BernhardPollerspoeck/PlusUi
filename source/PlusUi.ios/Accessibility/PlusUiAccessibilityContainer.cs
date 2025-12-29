using PlusUi.core;
using UIKit;

namespace PlusUi.ios.Accessibility;

/// <summary>
/// Container that provides accessibility elements for PlusUi elements.
/// </summary>
public sealed class PlusUiAccessibilityContainer(UIView hostView, Func<UiElement?> rootProvider)
{
    private readonly Dictionary<int, PlusUiAccessibilityElement> _elementCache = new();
    private List<UIAccessibilityElement>? _cachedElements;

    public void InvalidateAccessibilityElements()
    {
        _cachedElements = null;
        _elementCache.Clear();
    }

    public UIAccessibilityElement? GetAccessibilityElement(UiElement element)
    {
        var hashCode = element.GetHashCode();
        if (!_elementCache.TryGetValue(hashCode, out var accessibilityElement))
        {
            accessibilityElement = new PlusUiAccessibilityElement(hostView, element);
            _elementCache[hashCode] = accessibilityElement;
        }
        return accessibilityElement;
    }

    public UIAccessibilityElement[] GetAccessibilityElements()
    {
        if (_cachedElements != null)
        {
            return _cachedElements.ToArray();
        }

        var elements = new List<UIAccessibilityElement>();
        var root = rootProvider();
        if (root != null)
        {
            CollectAccessibilityElements(root, elements);
        }

        _cachedElements = elements;
        return elements.ToArray();
    }

    private void CollectAccessibilityElements(UiElement element, List<UIAccessibilityElement> elements)
    {
        if (!element.IsVisible)
        {
            return;
        }

        if (element.IsAccessibilityElement)
        {
            elements.Add(GetAccessibilityElement(element)!);
        }

        if (element is UiLayoutElement layoutElement)
        {
            foreach (var child in layoutElement.Children)
            {
                CollectAccessibilityElements(child, elements);
            }
        }
    }
}
