namespace PlusUi.core;

/// <summary>
/// Central default values for all PlusUi controls.
/// Used as property defaults throughout the framework for a coherent out-of-the-box appearance.
/// </summary>
public static class PlusUiDefaults
{
    #region Typography

    /// <summary>Default font size for most controls.</summary>
    public const float FontSize = 14f;

    /// <summary>Large font size for headers and titles.</summary>
    public const float FontSizeLarge = 18f;

    /// <summary>Small font size for captions and hints.</summary>
    public const float FontSizeSmall = 12f;

    #endregion

    #region Colors - Dark Theme

    /// <summary>Page/Window background - darkest layer (30, 30, 30).</summary>
    public static readonly Color BackgroundPage = new(30, 30, 30);

    /// <summary>Primary background for panels and containers (45, 45, 45).</summary>
    public static readonly Color BackgroundPrimary = new(45, 45, 45);

    /// <summary>Secondary background for elevated surfaces (55, 55, 55).</summary>
    public static readonly Color BackgroundSecondary = new(55, 55, 55);

    /// <summary>Control background for buttons, inputs etc. (60, 60, 60).</summary>
    public static readonly Color BackgroundControl = new(60, 60, 60);

    /// <summary>Hover state background (75, 75, 75).</summary>
    public static readonly Color BackgroundHover = new(75, 75, 75);

    /// <summary>Pressed/active state background (85, 85, 85).</summary>
    public static readonly Color BackgroundPressed = new(85, 85, 85);

    /// <summary>Selected/active state background - Windows Blue (0, 120, 215).</summary>
    public static readonly Color BackgroundSelected = new(0, 120, 215);

    /// <summary>Input field background - slightly lighter for visibility (50, 50, 50).</summary>
    public static readonly Color BackgroundInput = new(50, 50, 50);

    /// <summary>Primary text color - White.</summary>
    public static readonly Color TextPrimary = Colors.White;

    /// <summary>Secondary text color for less emphasis (180, 180, 180).</summary>
    public static readonly Color TextSecondary = new(180, 180, 180);

    /// <summary>Placeholder text color (128, 128, 128).</summary>
    public static readonly Color TextPlaceholder = new(128, 128, 128);

    /// <summary>Disabled text color (100, 100, 100).</summary>
    public static readonly Color TextDisabled = new(100, 100, 100);

    /// <summary>Primary accent color - Windows Blue (0, 120, 215).</summary>
    public static readonly Color AccentPrimary = new(0, 120, 215);

    /// <summary>Success accent color - Green (52, 199, 89).</summary>
    public static readonly Color AccentSuccess = new(52, 199, 89);

    /// <summary>Error accent color - Red (255, 59, 48).</summary>
    public static readonly Color AccentError = new(255, 59, 48);

    /// <summary>Warning accent color - Orange (255, 149, 0).</summary>
    public static readonly Color AccentWarning = new(255, 149, 0);

    /// <summary>Border and separator color (80, 80, 80).</summary>
    public static readonly Color BorderColor = new(80, 80, 80);

    /// <summary>Border color for focused elements (100, 100, 100).</summary>
    public static readonly Color BorderFocused = new(100, 100, 100);

    /// <summary>Shadow color with alpha (0, 0, 0, 80).</summary>
    public static readonly Color ShadowColor = new(0, 0, 0, 80);

    /// <summary>Track color for sliders and progress bars (70, 70, 70).</summary>
    public static readonly Color TrackColor = new(70, 70, 70);

    #endregion

    #region Colors - High Contrast

    /// <summary>High contrast background - White.</summary>
    public static readonly Color HcBackground = Colors.White;

    /// <summary>High contrast foreground/text - Black.</summary>
    public static readonly Color HcForeground = Colors.Black;

    /// <summary>High contrast button background - Yellow for visibility.</summary>
    public static readonly Color HcButtonBackground = Colors.Yellow;

    /// <summary>High contrast input background - White.</summary>
    public static readonly Color HcInputBackground = Colors.White;

    /// <summary>High contrast link color - Blue.</summary>
    public static readonly Color HcLinkColor = Colors.Blue;

    #endregion

    #region Spacing

    /// <summary>Horizontal padding for controls like buttons, text fields.</summary>
    public const float PaddingHorizontal = 12f;

    /// <summary>Vertical padding for controls.</summary>
    public const float PaddingVertical = 8f;

    /// <summary>Standard spacing between elements.</summary>
    public const float Spacing = 8f;

    /// <summary>Small spacing for tight layouts.</summary>
    public const float SpacingSmall = 4f;

    /// <summary>Large spacing for section separation.</summary>
    public const float SpacingLarge = 16f;

    #endregion

    #region Dimensions

    /// <summary>Standard item height for lists, dropdowns, etc.</summary>
    public const float ItemHeight = 32f;

    /// <summary>Standard corner radius for rounded elements.</summary>
    public const float CornerRadius = 4f;

    /// <summary>Standard scrollbar width.</summary>
    public const float ScrollbarWidth = 12f;

    /// <summary>Standard icon size.</summary>
    public const float IconSize = 16f;

    /// <summary>Large icon size.</summary>
    public const float IconSizeLarge = 24f;

    /// <summary>Minimum thumb size for scrollbars.</summary>
    public const float ScrollbarMinThumbSize = 20f;

    /// <summary>Standard thumb radius for sliders.</summary>
    public const float SliderThumbRadius = 14f;

    /// <summary>Standard progress bar height.</summary>
    public const float ProgressBarHeight = 8f;

    /// <summary>Circle size for radio buttons and checkboxes.</summary>
    public const float CheckboxSize = 20f;

    /// <summary>TreeView indentation per level.</summary>
    public const float TreeViewIndent = 20f;

    #endregion

    #region Focus

    /// <summary>Width of the focus ring stroke.</summary>
    public const float FocusRingWidth = 2f;

    /// <summary>Offset of the focus ring from element bounds.</summary>
    public const float FocusRingOffset = 2f;

    /// <summary>Default tab stop behavior - elements are included in tab navigation.</summary>
    public const bool TabStop = true;

    #endregion

    #region Element Defaults

    /// <summary>Default visibility state.</summary>
    public const bool IsVisible = true;

    /// <summary>Default opacity (fully opaque).</summary>
    public const float Opacity = 1f;

    /// <summary>Default corner radius for base elements (no rounding).</summary>
    public const float CornerRadiusNone = 0f;

    /// <summary>Default horizontal alignment.</summary>
    public const HorizontalAlignment HorizontalAlignment = core.HorizontalAlignment.Undefined;

    /// <summary>Default vertical alignment.</summary>
    public const VerticalAlignment VerticalAlignment = core.VerticalAlignment.Undefined;

    #endregion

    #region Shadow Defaults

    /// <summary>Default shadow color (transparent = no shadow).</summary>
    public static readonly Color ShadowColorNone = Colors.Transparent;

    /// <summary>Default shadow blur radius.</summary>
    public const float ShadowBlur = 0f;

    /// <summary>Default shadow spread.</summary>
    public const float ShadowSpread = 0f;

    #endregion

    #region Accessibility Defaults

    /// <summary>Default accessibility traits.</summary>
    public const AccessibilityTrait AccessibilityTraits = AccessibilityTrait.None;

    /// <summary>Default accessibility element visibility.</summary>
    public const bool IsAccessibilityElement = true;

    #endregion

    #region Border

    /// <summary>Default border stroke color.</summary>
    public static readonly Color StrokeColor = Colors.Black;

    /// <summary>Default border stroke thickness.</summary>
    public const float StrokeThickness = 1f;

    /// <summary>Default border stroke type.</summary>
    public const StrokeType StrokeType = core.StrokeType.Solid;

    #endregion

    #region Button

    /// <summary>Default icon position for buttons.</summary>
    public const IconPosition IconPosition = core.IconPosition.Leading;

    #endregion

    #region Stack (HStack/VStack)

    /// <summary>Default spacing between stack children.</summary>
    public const float StackSpacing = 0f;

    /// <summary>Default wrap behavior for stacks.</summary>
    public const bool StackWrap = false;

    #endregion

    #region Image

    /// <summary>Default image aspect mode.</summary>
    public const Aspect ImageAspect = Aspect.AspectFit;

    #endregion

    #region ItemsList

    /// <summary>Default list orientation.</summary>
    public const Orientation ListOrientation = Orientation.Vertical;

    /// <summary>Default scroll factor.</summary>
    public const float ScrollFactor = 1f;

    #endregion

    #region Text

    /// <summary>Default font weight.</summary>
    public const FontWeight FontWeight = core.FontWeight.Regular;

    /// <summary>Default font style.</summary>
    public const FontStyle FontStyle = core.FontStyle.Normal;

    /// <summary>Default horizontal text alignment.</summary>
    public const HorizontalTextAlignment HorizontalTextAlignment = core.HorizontalTextAlignment.Left;

    /// <summary>Default text wrapping mode.</summary>
    public const TextWrapping TextWrapping = core.TextWrapping.NoWrap;

    /// <summary>Default text truncation mode.</summary>
    public const TextTruncation TextTruncation = core.TextTruncation.None;

    #endregion
}
