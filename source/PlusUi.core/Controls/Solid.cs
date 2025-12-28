using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A simple rectangular element with a solid color background.
/// Useful for creating spacers, dividers, or colored blocks in layouts.
/// </summary>
/// <example>
/// <code>
/// // Fixed size colored rectangle
/// new Solid(width: 100, height: 50, color: Colors.Blue);
///
/// // Spacer that stretches
/// new Solid()
///     .SetBackground(new SolidColorBackground(Colors.LightGray));
///
/// // Vertical divider
/// new Solid(width: 1, height: null, color: Colors.Gray);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class Solid : UiElement
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.None;

    public Solid(float? width = null, float? height = null, Color? color = null)
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
        if (width is not null || height is not null)
        {
            SetDesiredSize(new Size(width ?? 0, height ?? 0));
        }
        if (color is not null)
        {
            SetBackground(new SolidColorBackground(color.Value));
        }
    }
}