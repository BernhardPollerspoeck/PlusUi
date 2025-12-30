---
title: Theming
layout: default
parent: Guides
nav_order: 2
---

# Theming

PlusUi provides a flexible styling system to customize the appearance of your application.

---

## Using the Default Style

Apply the built-in default style:

```csharp
public void ConfigureApp(HostApplicationBuilder builder)
{
    builder.StylePlusUi<DefaultStyle>();
}
```

---

## Creating a Custom Style

Create your own style class implementing `IApplicationStyle`:

```csharp
using PlusUi.core;

public class MyCustomStyle : IApplicationStyle
{
    public void ConfigureStyle(Style style)
    {
        // Style all Labels
        style.AddStyle<Label>(label => label
            .SetTextColor(new Color(240, 240, 240))
            .SetTextSize(16));

        // Style all Buttons
        style.AddStyle<Button>(button => button
            .SetTextColor(Colors.White)
            .SetBackground(new SolidColorBackground(new Color(0, 122, 255)))
            .SetCornerRadius(8)
            .SetPadding(new Margin(16, 8)));

        // Style all Entry fields
        style.AddStyle<Entry>(entry => entry
            .SetTextColor(Colors.White)
            .SetBackground(new SolidColorBackground(new Color(45, 45, 45)))
            .SetCornerRadius(4)
            .SetPadding(new Margin(12, 8)));

        // Style page backgrounds
        style.AddStyle<UiPageElement>(page => page
            .SetBackground(new SolidColorBackground(new Color(30, 30, 30))));
    }
}
```

Then register it:

```csharp
builder.StylePlusUi<MyCustomStyle>();
```

---

## Colors

### Using Built-in Colors

```csharp
.SetTextColor(Colors.White)
.SetBackground(new SolidColorBackground(Colors.Blue))
```

### Custom Colors

```csharp
// RGB
var myColor = new Color(255, 128, 0);

// RGBA (with alpha)
var transparentColor = new Color(255, 255, 255, 128);

// From hex (extension method)
var hexColor = new Color(0x3498db);
```

---

## Backgrounds

### Solid Color

```csharp
.SetBackground(new SolidColorBackground(new Color(45, 45, 45)))
```

### Linear Gradient

```csharp
// Two colors with angle
.SetBackground(new LinearGradient(Colors.Blue, Colors.Purple, 45))

// Multi-stop gradient
.SetBackground(new MultiStopGradient(
    90,  // angle in degrees
    new GradientStop(Colors.Red, 0f),
    new GradientStop(Colors.Yellow, 0.5f),
    new GradientStop(Colors.Green, 1f)))
```

### Radial Gradient

```csharp
.SetBackground(new RadialGradient(Colors.White, Colors.Black))
```

---

## Fonts

### Setting Font Size

```csharp
.SetTextSize(24)
```

### Font Weight

```csharp
.SetFontWeight(FontWeight.Bold)
.SetFontWeight(FontWeight.Light)
.SetFontWeight(FontWeight.SemiBold)
```

### Custom Fonts

Load a custom font file:

```csharp
// In your style or page
.SetFontFamily("path/to/MyFont.ttf")
```

{: .note }
> Embed fonts as EmbeddedResource in your project for cross-platform compatibility.

---

## Borders and Corners

### Corner Radius

```csharp
.SetCornerRadius(8)        // All corners
.SetCornerRadius(8, 8, 0, 0)  // Top-left, Top-right, Bottom-right, Bottom-left
```

### Border Control

```csharp
new Border()
    .SetStrokeColor(Colors.Red)
    .SetStrokeThickness(2)
    .SetStrokeType(StrokeType.Solid)    // or Dashed, Dotted
    .SetCornerRadius(10)
    .AddChild(content)
```

---

## Shadows

Add shadows to controls:

```csharp
.SetShadow(new Shadow(
    offsetX: 0,
    offsetY: 4,
    blurRadius: 8,
    color: new Color(0, 0, 0, 100)))
```

---

## Page-Specific Styles

Override styles for a specific page:

```csharp
public class SettingsPage(SettingsPageViewModel vm) : UiPageElement(vm)
{
    protected override void ConfigurePageStyles(Style pageStyle)
    {
        // These styles only apply to this page
        pageStyle.AddStyle<Button>(button => button
            .SetBackground(new SolidColorBackground(Colors.Green)));
    }

    protected override UiElement Build()
    {
        // ...
    }
}
```

---

## Ignoring Global Styles

Prevent a control from receiving global styles:

```csharp
new Label()
    .SetText("Unstyled Label")
    .IgnoreStyling()
```

---

## Hover States

Some controls support hover states:

```csharp
new Button()
    .SetText("Hover me")
    .SetBackground(new SolidColorBackground(Colors.Blue))
    .SetHoverBackground(new SolidColorBackground(Colors.DarkBlue))
```

---

## Dark/Light Mode

Create styles that adapt to the system theme:

```csharp
public class AdaptiveStyle : IApplicationStyle
{
    private readonly bool _isDarkMode;

    public AdaptiveStyle(bool isDarkMode)
    {
        _isDarkMode = isDarkMode;
    }

    public void ConfigureStyle(Style style)
    {
        var bgColor = _isDarkMode
            ? new Color(30, 30, 30)
            : new Color(250, 250, 250);

        var textColor = _isDarkMode
            ? Colors.White
            : Colors.Black;

        style.AddStyle<UiPageElement>(page => page
            .SetBackground(new SolidColorBackground(bgColor)));

        style.AddStyle<Label>(label => label
            .SetTextColor(textColor));
    }
}
```

---

## Best Practices

{: .tip }
> **Consistency** - Define a style class for your app and use it everywhere.

{: .tip }
> **Color Palette** - Create a static class with your color constants for reuse.

{: .warning }
> **Don't Over-Style** - Avoid setting too many properties inline. Use styles instead.

---

## Example: Complete Theme

```csharp
public class MyAppTheme : IApplicationStyle
{
    // Color palette
    private static readonly Color Primary = new(0, 122, 255);
    private static readonly Color Secondary = new(88, 86, 214);
    private static readonly Color Background = new(18, 18, 18);
    private static readonly Color Surface = new(30, 30, 30);
    private static readonly Color TextPrimary = new(255, 255, 255);
    private static readonly Color TextSecondary = new(180, 180, 180);

    public void ConfigureStyle(Style style)
    {
        style.AddStyle<UiPageElement>(page => page
            .SetBackground(new SolidColorBackground(Background)));

        style.AddStyle<Label>(label => label
            .SetTextColor(TextPrimary)
            .SetTextSize(16));

        style.AddStyle<Button>(button => button
            .SetTextColor(TextPrimary)
            .SetBackground(new SolidColorBackground(Primary))
            .SetHoverBackground(new SolidColorBackground(Secondary))
            .SetCornerRadius(8)
            .SetPadding(new Margin(16, 10)));

        style.AddStyle<Entry>(entry => entry
            .SetTextColor(TextPrimary)
            .SetPlaceholderColor(TextSecondary)
            .SetBackground(new SolidColorBackground(Surface))
            .SetCornerRadius(6)
            .SetPadding(new Margin(12, 8)));

        style.AddStyle<Border>(border => border
            .SetStrokeColor(new Color(60, 60, 60))
            .SetCornerRadius(8));
    }
}
```
