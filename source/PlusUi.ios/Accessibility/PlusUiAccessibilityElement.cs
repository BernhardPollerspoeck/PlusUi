using Foundation;
using PlusUi.core;
using UIKit;

namespace PlusUi.ios.Accessibility;

/// <summary>
/// UIAccessibilityElement wrapper for a PlusUi element.
/// </summary>
public sealed class PlusUiAccessibilityElement : UIAccessibilityElement
{
    private readonly UiElement _element;

    public PlusUiAccessibilityElement(NSObject container, UiElement element)
        : base(container)
    {
        _element = element;
        UpdateProperties();
    }

    private void UpdateProperties()
    {
        // Set accessibility label
        AccessibilityLabel = _element.GetComputedAccessibilityLabel();

        // Set accessibility hint
        AccessibilityHint = _element.AccessibilityHint;

        // Set accessibility value
        AccessibilityValue = _element.GetComputedAccessibilityValue();

        // Set accessibility traits
        AccessibilityTraits = (ulong)GetUIAccessibilityTraits();

        // Set frame in container coordinates
        var frame = new CoreGraphics.CGRect(
            _element.Position.X,
            _element.Position.Y,
            _element.ElementSize.Width,
            _element.ElementSize.Height);
        AccessibilityFrame = UIAccessibility.ConvertFrameToScreenCoordinates(frame, (UIView)AccessibilityContainer!);

        // Set whether it's an accessibility element
        IsAccessibilityElement = _element.IsAccessibilityElement && _element.IsVisible;
    }

    private UIAccessibilityTrait GetUIAccessibilityTraits()
    {
        var traits = UIAccessibilityTrait.None;

        // Map role to traits
        traits |= _element.AccessibilityRole switch
        {
            AccessibilityRole.Button => UIAccessibilityTrait.Button,
            AccessibilityRole.Link => UIAccessibilityTrait.Link,
            AccessibilityRole.Image => UIAccessibilityTrait.Image,
            AccessibilityRole.Heading => UIAccessibilityTrait.Header,
            AccessibilityRole.Label => UIAccessibilityTrait.StaticText,
            AccessibilityRole.TextInput => UIAccessibilityTrait.None,
            AccessibilityRole.Slider => UIAccessibilityTrait.Adjustable,
            _ => UIAccessibilityTrait.None
        };

        // Map additional traits from AccessibilityTraits
        var elementTraits = _element.GetComputedAccessibilityTraits();

        if (elementTraits.HasFlag(AccessibilityTrait.Disabled))
        {
            traits |= UIAccessibilityTrait.NotEnabled;
        }

        if (elementTraits.HasFlag(AccessibilityTrait.Selected))
        {
            traits |= UIAccessibilityTrait.Selected;
        }

        if (elementTraits.HasFlag(AccessibilityTrait.Busy))
        {
            traits |= UIAccessibilityTrait.UpdatesFrequently;
        }

        return traits;
    }

    /// <summary>
    /// Called when VoiceOver user performs an activation action.
    /// </summary>
    public void HandleAccessibilityActivate()
    {
        // Trigger activation on the element (like a tap)
    }

    /// <summary>
    /// Called when VoiceOver user performs an increment action (for adjustable elements like sliders).
    /// </summary>
    public void HandleAccessibilityIncrement()
    {
        // Handle increment for slider elements
    }

    /// <summary>
    /// Called when VoiceOver user performs a decrement action (for adjustable elements like sliders).
    /// </summary>
    public void HandleAccessibilityDecrement()
    {
        // Handle decrement for slider elements
    }
}
