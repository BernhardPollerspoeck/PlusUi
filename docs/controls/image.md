---
title: Image
layout: default
parent: Controls
nav_order: 90
---

# Image

Displays static and animated images from various sources including embedded resources, files, and URLs. Supports automatic GIF animation playback, SVG rendering, and aspect ratio control.

---

## Basic Usage

```csharp
// Embedded resource
new Image()
    .SetImageSource("logo.png")

// Local file
new Image()
    .SetImageSource("file:/images/photo.jpg")

// Web image
new Image()
    .SetImageSource("https://example.com/image.png")

// With aspect ratio
new Image()
    .SetImageSource("photo.jpg")
    .SetAspect(Aspect.AspectFill)
    .SetDesiredSize(new Size(200, 200))
```

---

## Image Sources

Images can be loaded from different sources using prefixes:

| Prefix | Description | Example |
|:-------|:------------|:--------|
| (none) | Embedded resource in any loaded assembly | `"logo.png"` |
| `file:` | Local file path | `"file:/path/to/image.png"` |
| `http://` or `https://` | Web image (loaded asynchronously) | `"https://example.com/image.png"` |

---

## Image-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetImageSource(string)` | Sets the image source path/URL |
| `BindImageSource(name, getter)` | Binds image source |
| `SetAspect(Aspect)` | Sets scaling mode (default: Fill) |
| `BindAspect(name, getter)` | Binds aspect |
| `SetTintColor(Color)` | Sets tint color for SVG images |
| `BindTintColor(name, getter)` | Binds tint color |

### Aspect Values

| Value | Description |
|:------|:------------|
| `Aspect.Fill` | Stretches to fill space, may distort |
| `Aspect.AspectFit` | Scales to fit within space, preserves ratio, may leave empty space |
| `Aspect.AspectFill` | Scales to fill space, preserves ratio, may crop |

---

## Supported Formats

- **Static Images**: PNG, JPEG, WebP, BMP, ICO
- **Animated Images**: GIF (automatic playback)
- **Vector Graphics**: SVG (rendered at display size for crisp scaling)

{: .note }
> GIF animations play automatically when loaded. SVG images are rendered at the element's size for optimal quality at any scale.

---

## Layout Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetMargin(Margin)` | Sets outer margin |
| `BindMargin(name, getter)` | Binds margin |
| `SetHorizontalAlignment(HorizontalAlignment)` | Sets horizontal alignment |
| `BindHorizontalAlignment(name, getter)` | Binds horizontal alignment |
| `SetVerticalAlignment(VerticalAlignment)` | Sets vertical alignment |
| `BindVerticalAlignment(name, getter)` | Binds vertical alignment |
| `SetDesiredSize(Size)` | Sets explicit size |
| `BindDesiredSize(name, getter)` | Binds size |
| `SetDesiredWidth(float)` | Sets explicit width |
| `BindDesiredWidth(name, getter)` | Binds width |
| `SetDesiredHeight(float)` | Sets explicit height |
| `BindDesiredHeight(name, getter)` | Binds height |

### HorizontalAlignment Values

| Value | Description |
|:------|:------------|
| `HorizontalAlignment.Undefined` | Default behavior |
| `HorizontalAlignment.Left` | Align to left |
| `HorizontalAlignment.Center` | Center horizontally |
| `HorizontalAlignment.Right` | Align to right |
| `HorizontalAlignment.Stretch` | Stretch to fill width |

### VerticalAlignment Values

| Value | Description |
|:------|:------------|
| `VerticalAlignment.Undefined` | Default behavior |
| `VerticalAlignment.Top` | Align to top |
| `VerticalAlignment.Center` | Center vertically |
| `VerticalAlignment.Bottom` | Align to bottom |
| `VerticalAlignment.Stretch` | Stretch to fill height |

---

## Appearance Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetIsVisible(bool)` | Shows/hides element |
| `BindIsVisible(name, getter)` | Binds visibility |
| `SetOpacity(float)` | Sets opacity 0.0-1.0 (default: 1.0) |
| `BindOpacity(name, getter)` | Binds opacity |
| `SetBackground(IBackground)` | Sets background (gradient, solid, etc.) |
| `SetBackground(Color)` | Sets solid color background |
| `BindBackground(name, getter)` | Binds background |
| `SetCornerRadius(float)` | Sets corner radius |
| `BindCornerRadius(name, getter)` | Binds corner radius |
| `SetVisualOffset(Point)` | Offsets visual position |
| `BindVisualOffset(name, getter)` | Binds visual offset |

---

## Shadow Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetShadowColor(Color)` | Sets shadow color |
| `BindShadowColor(name, getter)` | Binds shadow color |
| `SetShadowOffset(Point)` | Sets shadow offset |
| `BindShadowOffset(name, getter)` | Binds shadow offset |
| `SetShadowBlur(float)` | Sets shadow blur radius |
| `BindShadowBlur(name, getter)` | Binds shadow blur |
| `SetShadowSpread(float)` | Sets shadow spread |
| `BindShadowSpread(name, getter)` | Binds shadow spread |

---

## Focus Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetTabIndex(int?)` | Sets tab order (null = auto) |
| `BindTabIndex(name, getter)` | Binds tab index |
| `SetTabStop(bool)` | Include in tab navigation (default: true) |
| `BindTabStop(name, getter)` | Binds tab stop |
| `SetFocusRingColor(Color)` | Sets focus ring color |
| `SetFocusRingWidth(float)` | Sets focus ring stroke width |
| `SetFocusRingOffset(float)` | Sets focus ring offset from bounds |
| `SetFocusedBackground(IBackground)` | Sets background when focused |
| `SetFocusedBackground(Color)` | Sets solid background when focused |
| `SetFocusedBorderColor(Color?)` | Sets border color when focused |

{: .note }
> Image is not focusable by default (`IsFocusable = false`).

---

## Accessibility Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetAccessibilityLabel(string?)` | Sets screen reader label |
| `BindAccessibilityLabel(name, getter)` | Binds accessibility label |
| `SetAccessibilityHint(string?)` | Sets additional context hint |
| `BindAccessibilityHint(name, getter)` | Binds accessibility hint |
| `SetAccessibilityValue(string?)` | Sets current value description |
| `BindAccessibilityValue(name, getter)` | Binds accessibility value |
| `SetAccessibilityTraits(AccessibilityTrait)` | Sets accessibility traits |
| `BindAccessibilityTraits(name, getter)` | Binds accessibility traits |
| `AddAccessibilityTraits(AccessibilityTrait)` | Adds traits to existing |
| `RemoveAccessibilityTraits(AccessibilityTrait)` | Removes specific traits |
| `SetIsAccessibilityElement(bool)` | Include in accessibility tree |
| `SetHighContrastBackground(IBackground)` | Background for high contrast mode |
| `SetHighContrastBackground(Color)` | Solid background for high contrast |
| `SetHighContrastForeground(Color)` | Text color for high contrast mode |
| `SetEnforceMinimumTouchTarget(bool)` | Enforce 44x44 minimum size |

{: .note }
> Image has `AccessibilityRole.Image` by default. Always set an accessibility label for decorative images or images conveying information.

---

## Tooltip Methods

Extension methods available on all `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetTooltip(string)` | Sets tooltip text |
| `SetTooltip(UiElement)` | Sets tooltip with custom content |
| `SetTooltip(Action<TooltipAttachment>)` | Configures tooltip via builder |
| `SetTooltipPlacement(TooltipPlacement)` | Sets tooltip position |
| `SetTooltipShowDelay(int)` | Sets show delay in ms |
| `SetTooltipHideDelay(int)` | Sets hide delay in ms |
| `BindTooltipContent(name, getter)` | Binds tooltip content |
| `BindTooltipPlacement(name, getter)` | Binds tooltip placement |
| `BindTooltipShowDelay(name, getter)` | Binds show delay |
| `BindTooltipHideDelay(name, getter)` | Binds hide delay |

---

## Context Menu Methods

Extension methods available on all `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetContextMenu(ContextMenu)` | Sets context menu |
| `SetContextMenu(Action<ContextMenu>)` | Configures context menu via builder |
| `SetContextMenuBackground(IBackground)` | Sets menu background |
| `SetContextMenuBackground(Color)` | Sets menu background color |
| `SetContextMenuHoverBackgroundColor(Color)` | Sets hover color |
| `SetContextMenuTextColor(Color)` | Sets menu text color |
| `BindContextMenuBackground(name, getter)` | Binds menu background |
| `BindContextMenuHoverBackgroundColor(name, getter)` | Binds hover color |
| `BindContextMenuTextColor(name, getter)` | Binds text color |

---

## Other Methods

| Method | Description |
|:-------|:------------|
| `IgnoreStyling()` | Prevents global styles from being applied |
| `SetDebug(bool)` | Shows debug bounds (red border) |

---

## Examples

### Fixed Size Image with AspectFit

```csharp
new Image()
    .SetImageSource("photo.jpg")
    .SetAspect(Aspect.AspectFit)
    .SetDesiredSize(new Size(300, 200))
    .SetBackground(new Color(30, 30, 30))
```

### Circular Avatar Image

```csharp
new Image()
    .SetImageSource("https://example.com/avatar.jpg")
    .SetAspect(Aspect.AspectFill)
    .SetDesiredSize(new Size(64, 64))
    .SetCornerRadius(32)
```

### SVG Icon with Tint

```csharp
new Image()
    .SetImageSource("Assets/Icons/settings.svg")
    .SetTintColor(Colors.Blue)
    .SetDesiredSize(new Size(24, 24))
```

### Data-bound Image

```csharp
new Image()
    .BindImageSource(nameof(vm.ProfileImageUrl), () => vm.ProfileImageUrl)
    .SetAspect(Aspect.AspectFill)
    .SetDesiredSize(new Size(100, 100))
    .SetCornerRadius(50)
```

### Animated GIF

```csharp
new Image()
    .SetImageSource("loading.gif")
    .SetDesiredSize(new Size(48, 48))
```

### Image with Shadow

```csharp
new Image()
    .SetImageSource("product.png")
    .SetAspect(Aspect.AspectFit)
    .SetDesiredSize(new Size(200, 200))
    .SetShadowColor(new Color(0, 0, 0, 80))
    .SetShadowOffset(new Point(0, 4))
    .SetShadowBlur(8)
```

### Accessible Image

```csharp
new Image()
    .SetImageSource("chart.png")
    .SetAccessibilityLabel("Sales chart showing 20% growth in Q4")
    .SetDesiredSize(new Size(400, 300))
```
