---
title: Separator
layout: default
parent: Controls
nav_order: 170
---

# Separator

A visual separator line that can be horizontal or vertical. Used to create visual divisions between UI sections.

---

## Basic Usage

```csharp
// Horizontal separator (default)
new Separator()

// Vertical separator
new Separator()
    .SetOrientation(Orientation.Vertical)

// Styled separator
new Separator()
    .SetColor(new Color(100, 100, 100))
    .SetThickness(2)
```

---

## Separator-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetColor(Color)` | Sets line color (default: light gray) |
| `BindColor(name, getter)` | Binds line color |
| `SetThickness(float)` | Sets line thickness in pixels (default: 1) |
| `BindThickness(name, getter)` | Binds thickness |
| `SetOrientation(Orientation)` | Sets horizontal or vertical orientation (default: Horizontal) |
| `BindOrientation(name, getter)` | Binds orientation |

### Orientation Values

| Value | Description |
|:------|:------------|
| `Orientation.Horizontal` | Horizontal line (default), stretches horizontally |
| `Orientation.Vertical` | Vertical line, stretches vertically |

{: .note }
> Separator automatically sets `HorizontalAlignment.Stretch` for horizontal orientation and `VerticalAlignment.Stretch` for vertical orientation.

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
| `SetIsAccessibilityElement(bool)` | Include in accessibility tree |

{: .note }
> Separator has `AccessibilityRole.None` by default as it's purely decorative.

---

## Tooltip Methods

Extension methods available on all `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetTooltip(string)` | Sets tooltip text |
| `SetTooltip(UiElement)` | Sets tooltip with custom content |
| `SetTooltipPlacement(TooltipPlacement)` | Sets tooltip position |

---

## Context Menu Methods

Extension methods available on all `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetContextMenu(ContextMenu)` | Sets context menu |
| `SetContextMenu(Action<ContextMenu>)` | Configures context menu via builder |

---

## Other Methods

| Method | Description |
|:-------|:------------|
| `IgnoreStyling()` | Prevents global styles from being applied |
| `SetDebug(bool)` | Shows debug bounds (red border) |

---

## Examples

### Menu Divider

```csharp
new VStack()
    .AddChildren(
        new Button().SetText("Cut"),
        new Button().SetText("Copy"),
        new Button().SetText("Paste"),
        new Separator().SetMargin(new Margin(0, 8)),
        new Button().SetText("Select All")
    )
```

### Section Divider with Margins

```csharp
new VStack()
    .AddChildren(
        CreateSection("General Settings"),
        new Separator()
            .SetColor(new Color(60, 60, 60))
            .SetMargin(new Margin(0, 16)),
        CreateSection("Advanced Settings")
    )
```

### Vertical Separator in Toolbar

```csharp
new HStack()
    .SetDesiredHeight(40)
    .AddChildren(
        new Button().SetText("File"),
        new Button().SetText("Edit"),
        new Separator()
            .SetOrientation(Orientation.Vertical)
            .SetMargin(new Margin(8, 4)),
        new Button().SetText("View"),
        new Button().SetText("Help")
    )
```

### Thick Divider

```csharp
new Separator()
    .SetThickness(3)
    .SetColor(Colors.Blue)
    .SetMargin(new Margin(20, 10))
```

### Sidebar Divider

```csharp
new HStack()
    .AddChildren(
        new VStack()
            .SetDesiredWidth(250)
            .AddChildren(/* sidebar content */),
        new Separator()
            .SetOrientation(Orientation.Vertical)
            .SetColor(new Color(50, 50, 50)),
        new VStack()
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
            .AddChildren(/* main content */)
    )
```

### Data-bound Color

```csharp
new Separator()
    .BindColor(nameof(vm.ThemeColor), () => vm.ThemeColor)
```
