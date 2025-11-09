using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Provides a modern, ready-to-use styling system for PlusUi applications.
/// Includes both Light and Dark themes with a comprehensive color palette and sensible defaults for all controls.
///
/// Usage:
/// - Use as-is: new PlusUiStyle().ConfigureStyle(style);
/// - Copy and customize: Create your own class based on this template
/// - Override specific controls: Apply this style first, then override specific elements
/// </summary>
public class PlusUiStyle : IApplicationStyle
{
    #region Color Palette - Light Theme

    // Primary Colors
    private static readonly SKColor LightPrimary = SKColor.Parse("#2563eb");        // Blue 600
    private static readonly SKColor LightPrimaryHover = SKColor.Parse("#1d4ed8");   // Blue 700
    private static readonly SKColor LightPrimaryText = SKColor.Parse("#ffffff");    // White

    // Surface Colors
    private static readonly SKColor LightBackground = SKColor.Parse("#ffffff");     // White
    private static readonly SKColor LightSurface = SKColor.Parse("#f8fafc");        // Slate 50
    private static readonly SKColor LightSurfaceVariant = SKColor.Parse("#f1f5f9");  // Slate 100
    private static readonly SKColor LightBorder = SKColor.Parse("#e2e8f0");         // Slate 200

    // Text Colors
    private static readonly SKColor LightTextPrimary = SKColor.Parse("#0f172a");    // Slate 900
    private static readonly SKColor LightTextSecondary = SKColor.Parse("#64748b");  // Slate 500
    private static readonly SKColor LightTextDisabled = SKColor.Parse("#cbd5e1");   // Slate 300
    private static readonly SKColor LightPlaceholder = SKColor.Parse("#94a3b8");    // Slate 400

    // State Colors
    private static readonly SKColor LightSuccess = SKColor.Parse("#16a34a");        // Green 600
    private static readonly SKColor LightWarning = SKColor.Parse("#ea580c");        // Orange 600
    private static readonly SKColor LightError = SKColor.Parse("#dc2626");          // Red 600
    private static readonly SKColor LightInfo = SKColor.Parse("#0891b2");           // Cyan 600

    // Interactive Colors
    private static readonly SKColor LightInputBackground = SKColor.Parse("#ffffff"); // White
    private static readonly SKColor LightInputBorder = SKColor.Parse("#cbd5e1");    // Slate 300
    private static readonly SKColor LightInputFocus = LightPrimary;

    // Toggle & Slider Colors
    private static readonly SKColor LightToggleOn = SKColor.Parse("#22c55e");       // Green 500
    private static readonly SKColor LightToggleOff = SKColor.Parse("#cbd5e1");      // Slate 300
    private static readonly SKColor LightThumb = SKColor.Parse("#ffffff");          // White

    // Shadow
    private static readonly SKColor LightShadow = SKColor.Parse("#10182808");       // Slate 900 @ 5%

    #endregion

    #region Color Palette - Dark Theme

    // Primary Colors
    private static readonly SKColor DarkPrimary = SKColor.Parse("#3b82f6");         // Blue 500
    private static readonly SKColor DarkPrimaryHover = SKColor.Parse("#2563eb");    // Blue 600
    private static readonly SKColor DarkPrimaryText = SKColor.Parse("#ffffff");     // White

    // Surface Colors
    private static readonly SKColor DarkBackground = SKColor.Parse("#0f172a");      // Slate 900
    private static readonly SKColor DarkSurface = SKColor.Parse("#1e293b");         // Slate 800
    private static readonly SKColor DarkSurfaceVariant = SKColor.Parse("#334155");   // Slate 700
    private static readonly SKColor DarkBorder = SKColor.Parse("#475569");          // Slate 600

    // Text Colors
    private static readonly SKColor DarkTextPrimary = SKColor.Parse("#f8fafc");     // Slate 50
    private static readonly SKColor DarkTextSecondary = SKColor.Parse("#94a3b8");   // Slate 400
    private static readonly SKColor DarkTextDisabled = SKColor.Parse("#475569");    // Slate 600
    private static readonly SKColor DarkPlaceholder = SKColor.Parse("#64748b");     // Slate 500

    // State Colors
    private static readonly SKColor DarkSuccess = SKColor.Parse("#22c55e");         // Green 500
    private static readonly SKColor DarkWarning = SKColor.Parse("#f97316");         // Orange 500
    private static readonly SKColor DarkError = SKColor.Parse("#ef4444");           // Red 500
    private static readonly SKColor DarkInfo = SKColor.Parse("#06b6d4");            // Cyan 500

    // Interactive Colors
    private static readonly SKColor DarkInputBackground = SKColor.Parse("#1e293b"); // Slate 800
    private static readonly SKColor DarkInputBorder = SKColor.Parse("#475569");     // Slate 600
    private static readonly SKColor DarkInputFocus = DarkPrimary;

    // Toggle & Slider Colors
    private static readonly SKColor DarkToggleOn = SKColor.Parse("#22c55e");        // Green 500
    private static readonly SKColor DarkToggleOff = SKColor.Parse("#475569");       // Slate 600
    private static readonly SKColor DarkThumb = SKColor.Parse("#f8fafc");           // Slate 50

    // Shadow
    private static readonly SKColor DarkShadow = SKColor.Parse("#00000020");        // Black @ 12%

    #endregion

    #region Sizing & Spacing Constants

    private const float DefaultCornerRadius = 8f;
    private const float InputCornerRadius = 6f;
    private const float ButtonCornerRadius = 6f;
    private const float CardCornerRadius = 12f;

    private const float DefaultTextSize = 14f;
    private const float HeadingTextSize = 20f;
    private const float SmallTextSize = 12f;

    private const float DefaultMargin = 8f;
    private const float TightMargin = 4f;
    private const float SpacedMargin = 16f;

    private const float InputPaddingHorizontal = 12f;
    private const float InputPaddingVertical = 8f;
    private const float ButtonPaddingHorizontal = 16f;
    private const float ButtonPaddingVertical = 10f;

    // Shadow Defaults
    private const float DefaultShadowBlur = 4f;
    private const float DefaultShadowOffsetY = 2f;
    private const float CardShadowBlur = 8f;
    private const float CardShadowOffsetY = 4f;

    #endregion

    public void ConfigureStyle(Style style)
    {
        // ============================================
        // LIGHT THEME
        // ============================================

        #region Base Elements - Light

        style.AddStyle<UiElement>(Theme.Light, element => element
            .SetMargin(new(DefaultMargin))
            .SetCornerRadius(DefaultCornerRadius));

        #endregion

        #region Text Elements - Light

        style.AddStyle<Label>(Theme.Light, element => element
            .SetTextColor(LightTextPrimary)
            .SetMargin(new(TightMargin)));

        style.AddStyle<Link>(Theme.Light, element => element
            .SetTextColor(LightPrimary)
            .SetFontWeight(FontWeight.Medium)
            .SetMargin(new(TightMargin)));

        #endregion

        #region Input Controls - Light

        style.AddStyle<Entry>(Theme.Light, element => element
            .SetBackground(new SolidColorBackground(LightInputBackground))
            .SetTextColor(LightTextPrimary)
            .SetPlaceholderColor(LightPlaceholder)
            .SetPadding(new(InputPaddingHorizontal, InputPaddingVertical))
            .SetCornerRadius(InputCornerRadius)
            .SetDesiredWidth(200)
            .SetDesiredHeight(40));

        #endregion

        #region Button - Light

        style.AddStyle<Button>(Theme.Light, element => element
            .SetBackground(new SolidColorBackground(LightPrimary))
            .SetTextColor(LightPrimaryText)
            .SetFontWeight(FontWeight.SemiBold)
            .SetPadding(new(ButtonPaddingHorizontal, ButtonPaddingVertical))
            .SetCornerRadius(ButtonCornerRadius)
            .SetDesiredHeight(40));

        #endregion

        #region Selection Controls - Light

        style.AddStyle<Checkbox>(Theme.Light, element => element
            .SetColor(LightPrimary)
            .SetMargin(new(TightMargin)));

        style.AddStyle<Toggle>(Theme.Light, element => element
            .SetOnColor(LightToggleOn)
            .SetOffColor(LightToggleOff)
            .SetThumbColor(LightThumb)
            .SetMargin(new(TightMargin)));

        style.AddStyle<Slider>(Theme.Light, element => element
            .SetMinimumTrackColor(LightPrimary)
            .SetMaximumTrackColor(LightBorder)
            .SetThumbColor(LightThumb)
            .SetDesiredWidth(200));

        #endregion

        #region Progress & Activity - Light

        style.AddStyle<ProgressBar>(Theme.Light, element => element
            .SetProgressColor(LightPrimary)
            .SetTrackColor(LightBorder)
            .SetDesiredHeight(8)
            .SetCornerRadius(4)
            .SetDesiredWidth(200));

        style.AddStyle<ActivityIndicator>(Theme.Light, element => element
            .SetColor(LightPrimary)
            .SetDesiredWidth(32)
            .SetDesiredHeight(32));

        #endregion

        #region Visual Elements - Light

        style.AddStyle<Border>(Theme.Light, element => element
            .SetStrokeColor(LightBorder)
            .SetStrokeThickness(1)
            .SetCornerRadius(DefaultCornerRadius)
            .SetBackground(new SolidColorBackground(LightSurface))
            .SetPadding(new(SpacedMargin)));

        style.AddStyle<Separator>(Theme.Light, element => element
            .SetColor(LightBorder)
            .SetThickness(1)
            .SetMargin(new(DefaultMargin, TightMargin)));

        style.AddStyle<Solid>(Theme.Light, element => element
            .SetBackground(new SolidColorBackground(LightSurfaceVariant))
            .SetDesiredWidth(50)
            .SetDesiredHeight(50));

        #endregion

        // ============================================
        // DARK THEME
        // ============================================

        #region Base Elements - Dark

        style.AddStyle<UiElement>(Theme.Dark, element => element
            .SetMargin(new(DefaultMargin))
            .SetCornerRadius(DefaultCornerRadius));

        #endregion

        #region Text Elements - Dark

        style.AddStyle<Label>(Theme.Dark, element => element
            .SetTextColor(DarkTextPrimary)
            .SetMargin(new(TightMargin)));

        style.AddStyle<Link>(Theme.Dark, element => element
            .SetTextColor(DarkPrimary)
            .SetFontWeight(FontWeight.Medium)
            .SetMargin(new(TightMargin)));

        #endregion

        #region Input Controls - Dark

        style.AddStyle<Entry>(Theme.Dark, element => element
            .SetBackground(new SolidColorBackground(DarkInputBackground))
            .SetTextColor(DarkTextPrimary)
            .SetPlaceholderColor(DarkPlaceholder)
            .SetPadding(new(InputPaddingHorizontal, InputPaddingVertical))
            .SetCornerRadius(InputCornerRadius)
            .SetDesiredWidth(200)
            .SetDesiredHeight(40));

        #endregion

        #region Button - Dark

        style.AddStyle<Button>(Theme.Dark, element => element
            .SetBackground(new SolidColorBackground(DarkPrimary))
            .SetTextColor(DarkPrimaryText)
            .SetFontWeight(FontWeight.SemiBold)
            .SetPadding(new(ButtonPaddingHorizontal, ButtonPaddingVertical))
            .SetCornerRadius(ButtonCornerRadius)
            .SetDesiredHeight(40));

        #endregion

        #region Selection Controls - Dark

        style.AddStyle<Checkbox>(Theme.Dark, element => element
            .SetColor(DarkPrimary)
            .SetMargin(new(TightMargin)));

        style.AddStyle<Toggle>(Theme.Dark, element => element
            .SetOnColor(DarkToggleOn)
            .SetOffColor(DarkToggleOff)
            .SetThumbColor(DarkThumb)
            .SetMargin(new(TightMargin)));

        style.AddStyle<Slider>(Theme.Dark, element => element
            .SetMinimumTrackColor(DarkPrimary)
            .SetMaximumTrackColor(DarkBorder)
            .SetThumbColor(DarkThumb)
            .SetDesiredWidth(200));

        #endregion

        #region Progress & Activity - Dark

        style.AddStyle<ProgressBar>(Theme.Dark, element => element
            .SetProgressColor(DarkPrimary)
            .SetTrackColor(DarkBorder)
            .SetDesiredHeight(8)
            .SetCornerRadius(4)
            .SetDesiredWidth(200));

        style.AddStyle<ActivityIndicator>(Theme.Dark, element => element
            .SetColor(DarkPrimary)
            .SetDesiredWidth(32)
            .SetDesiredHeight(32));

        #endregion

        #region Visual Elements - Dark

        style.AddStyle<Border>(Theme.Dark, element => element
            .SetStrokeColor(DarkBorder)
            .SetStrokeThickness(1)
            .SetCornerRadius(DefaultCornerRadius)
            .SetBackground(new SolidColorBackground(DarkSurface))
            .SetPadding(new(SpacedMargin)));

        style.AddStyle<Separator>(Theme.Dark, element => element
            .SetColor(DarkBorder)
            .SetThickness(1)
            .SetMargin(new(DefaultMargin, TightMargin)));

        style.AddStyle<Solid>(Theme.Dark, element => element
            .SetBackground(new SolidColorBackground(DarkSurfaceVariant))
            .SetDesiredWidth(50)
            .SetDesiredHeight(50));

        #endregion
    }
}
