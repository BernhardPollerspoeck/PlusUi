# Font Demo Page

This sample page demonstrates the custom font support features implemented in PlusUi.

## How to Access

1. Run the Sandbox application (Desktop, iOS, Android, or Web)
2. From the main page, click the **"Custom Font Demo"** button
3. The Font Demo page will display various font customization examples

## Features Demonstrated

### 1. Interactive Font Controls
- **Weight Toggle Buttons**: Switch between Regular, Bold, and Light font weights
- **Italic Toggle**: Toggle between Normal and Italic font styles
- **Live Preview**: See font changes applied in real-time to a preview label

### 2. Font Weight Examples
Demonstrates all available font weights with the same sample text:
- **Thin (100)**: Ultra-light weight
- **Light (300)**: Light weight for subtle text
- **Regular (400)**: Standard reading weight
- **Medium (500)**: Slightly heavier than regular
- **SemiBold (600)**: Between medium and bold
- **Bold (700)**: Strong emphasis
- **Black (900)**: Maximum weight

### 3. Font Style Examples
Shows the three font style options:
- **Normal**: Standard upright text
- **Italic**: Slanted text for emphasis
- **Oblique**: Alternative slanted style

### 4. Typography Hierarchy
Demonstrates how to create visual hierarchy using font weights and sizes:
- Heading 1: Bold, 32pt
- Heading 2: SemiBold, 28pt
- Heading 3: Medium, 24pt
- Body Text: Regular, 16pt
- Caption: Light, 14pt

### 5. Button Font Customization
Shows how custom fonts work on interactive controls:
- Regular button with standard weight
- Bold button for primary actions
- Light italic button for secondary actions

## Code Examples

### Setting Font Weight
```csharp
new Label()
    .SetText("Bold Text")
    .SetFontWeight(FontWeight.Bold)
```

### Setting Font Style
```csharp
new Label()
    .SetText("Italic Text")
    .SetFontStyle(FontStyle.Italic)
```

### Combining Font Properties
```csharp
new Label()
    .SetText("Bold Italic Text")
    .SetFontWeight(FontWeight.Bold)
    .SetFontStyle(FontStyle.Italic)
    .SetTextSize(24)
```

### Binding Font Properties
```csharp
new Label()
    .SetText("Dynamic Font")
    .BindFontWeight(nameof(vm.SelectedFontWeight), () => vm.SelectedFontWeight)
    .BindFontStyle(nameof(vm.SelectedFontStyle), () => vm.SelectedFontStyle)
```

## Testing the Feature

1. **Static Display**: Scroll through the page to see all font weight and style examples
2. **Interactive Test**: Click the weight buttons (Regular, Bold, Light) and observe the "Dynamic Font Preview" label change
3. **Italic Toggle**: Click "Toggle Italic" to switch the preview between normal and italic styles
4. **Visual Comparison**: Compare the different weights side-by-side to see the differences

## Implementation Details

The demo page is implemented using:
- **FontDemoPage.cs**: UI layout with all visual examples
- **FontDemoPageViewModel.cs**: View model with observable properties for dynamic font changes
- Standard PlusUi fluent API for font customization

## Notes

- Font weights use system fonts by default
- Custom fonts can be registered via `builder.RegisterFont()` in the app configuration
- All controls inheriting from `UiTextElement` support these font properties (Label, Button, Entry)
- Font fallback ensures the app works even when custom fonts are unavailable
