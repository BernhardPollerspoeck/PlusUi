# Custom Font Support

PlusUi provides comprehensive custom font support for professional typography and branding capabilities.

## Overview

Custom fonts can be used with all text-based controls:
- `Label`
- `Button`
- `Entry`

## Font Properties

### FontFamily (string)
The name of the font family to use.

### FontWeight (enum)
Font weight/thickness:
- `Thin` = 100
- `Light` = 300
- `Regular` = 400 (default)
- `Medium` = 500
- `SemiBold` = 600
- `Bold` = 700
- `Black` = 900

### FontStyle (enum)
Font style:
- `Normal` (default)
- `Italic`
- `Oblique`

## Fluent API Usage

### Basic Font Settings

```csharp
new Label()
    .SetText("Welcome")
    .SetFontFamily("Roboto")
    .SetFontWeight(FontWeight.Bold)
    .SetFontStyle(FontStyle.Italic)
    .SetTextSize(24)
```

### Multiple Weights for Typography Hierarchy

```csharp
new VStack(
    new Label()
        .SetText("Heading")
        .SetFontFamily("Roboto")
        .SetFontWeight(FontWeight.Bold)
        .SetTextSize(32),
    new Label()
        .SetText("Subheading")
        .SetFontFamily("Roboto")
        .SetFontWeight(FontWeight.Medium)
        .SetTextSize(20),
    new Label()
        .SetText("Body text")
        .SetFontFamily("Roboto")
        .SetFontWeight(FontWeight.Regular)
        .SetTextSize(14)
)
```

### Button with Custom Font

```csharp
new Button()
    .SetText("Click Me")
    .SetFontFamily("Roboto")
    .SetFontWeight(FontWeight.SemiBold)
    .SetTextSize(16)
```

### Entry with Custom Font

```csharp
new Entry()
    .SetPlaceholder("Enter text...")
    .SetFontFamily("Courier")
    .SetFontWeight(FontWeight.Regular)
    .SetTextSize(14)
```

## Font Binding

Fonts can be dynamically bound to ViewModel properties:

```csharp
new Label()
    .SetText("Dynamic Font")
    .BindFontFamily(nameof(vm.CurrentFont), () => vm.CurrentFont)
    .BindFontWeight(nameof(vm.IsBold), () => vm.IsBold ? FontWeight.Bold : FontWeight.Regular)
    .BindFontStyle(nameof(vm.IsItalic), () => vm.IsItalic ? FontStyle.Italic : FontStyle.Normal)
```

## Font Registration

### Registering Embedded Fonts

Register custom fonts from embedded resources in your application configuration:

```csharp
public class AppConfiguration : IAppConfiguration
{
    public void ConfigureApp(HostApplicationBuilder builder)
    {
        // Register a single weight
        builder.RegisterFont(
            "Fonts/Roboto-Regular.ttf", 
            "Roboto", 
            FontWeight.Regular
        );

        // Register multiple weights
        builder.RegisterFont("Fonts/Roboto-Light.ttf", "Roboto", FontWeight.Light);
        builder.RegisterFont("Fonts/Roboto-Bold.ttf", "Roboto", FontWeight.Bold);
        builder.RegisterFont("Fonts/Roboto-Black.ttf", "Roboto", FontWeight.Black);

        // Register with styles
        builder.RegisterFont(
            "Fonts/Roboto-Italic.ttf", 
            "Roboto", 
            FontWeight.Regular, 
            FontStyle.Italic
        );
        builder.RegisterFont(
            "Fonts/Roboto-BoldItalic.ttf", 
            "Roboto", 
            FontWeight.Bold, 
            FontStyle.Italic
        );
    }
}
```

### Embedded Resource Setup

To use fonts as embedded resources, ensure they are configured in your `.csproj` file:

```xml
<ItemGroup>
    <EmbeddedResource Include="Fonts\**\*.ttf" />
    <EmbeddedResource Include="Fonts\**\*.otf" />
</ItemGroup>
```

## Font Fallback Behavior

The font system implements intelligent fallback:

1. **Exact Match**: First tries to find exact font family, weight, and style
2. **Regular Weight**: If not found, tries same family with regular weight
3. **Normal Style**: If not found, tries same family with normal style
4. **Base Font**: If not found, tries same family with regular weight and normal style
5. **System Font**: If custom font not found, falls back to system font with requested weight/style

This ensures your application continues to work even if a specific font weight or style is not registered.

## Supported Font Formats

- TTF (TrueType Font)
- OTF (OpenType Font)

## Branding Example

```csharp
public class AppConfiguration : IAppConfiguration
{
    public void ConfigureApp(HostApplicationBuilder builder)
    {
        // Register corporate brand fonts
        builder.RegisterFont("Fonts/CorporateFont-Regular.ttf", "Corporate", FontWeight.Regular);
        builder.RegisterFont("Fonts/CorporateFont-Bold.ttf", "Corporate", FontWeight.Bold);
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return new MainPage()
            .SetContent(
                new VStack(
                    new Label()
                        .SetText("Company Name")
                        .SetFontFamily("Corporate")
                        .SetFontWeight(FontWeight.Bold)
                        .SetTextSize(32),
                    new Label()
                        .SetText("Tagline text")
                        .SetFontFamily("Corporate")
                        .SetFontWeight(FontWeight.Regular)
                        .SetTextSize(16)
                )
            );
    }
}
```

## Cross-Platform Support

Custom fonts work across all supported platforms:
- Desktop (Windows, macOS, Linux)
- iOS
- Android
- Web (H.264)

## Performance Considerations

- Fonts are cached after first load for optimal performance
- Only register fonts you actually use to minimize memory footprint
- Font registration happens once at application startup
- Font rendering uses SkiaSharp's hardware-accelerated rendering

## Best Practices

1. **Register all weights you need**: Register multiple font weights during startup
2. **Use consistent font families**: Stick to 1-2 font families for visual consistency
3. **Fallback gracefully**: Always test with missing fonts to ensure graceful degradation
4. **Typography hierarchy**: Use different weights to create visual hierarchy
5. **Accessibility**: Ensure adequate font sizes and weights for readability
