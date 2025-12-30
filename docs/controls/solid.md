---
title: Solid
layout: default
parent: Controls
nav_order: 180
---

# Solid

A simple rectangular element with a solid color background. Useful for creating spacers, dividers, or colored blocks in layouts.

---

## Basic Usage

```csharp
// Fixed size colored rectangle
new Solid(width: 100, height: 50, color: Colors.Blue)

// Spacer that stretches
new Solid()
    .SetBackground(Colors.LightGray)

// Vertical divider
new Solid(width: 1, height: null, color: Colors.Gray)

// Horizontal divider
new Solid(width: null, height: 1, color: Colors.Gray)
```

---

## Constructor Parameters

| Parameter | Type | Description |
|:----------|:-----|:------------|
| `width` | `float?` | Optional fixed width in pixels |
| `height` | `float?` | Optional fixed height in pixels |
| `color` | `Color?` | Optional background color |

{: .note }
> Solid defaults to `HorizontalAlignment.Stretch` and `VerticalAlignment.Stretch`. If width or height is specified, the element uses a fixed size for that dimension.

---

## Layout Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetMargin(Margin)` | Sets outer margin |
| `BindMargin(name, getter)` | Binds margin |
| `SetHorizontalAlignment(HorizontalAlignment)` | Sets horizontal alignment (default: Stretch) |
| `BindHorizontalAlignment(name, getter)` | Binds horizontal alignment |
| `SetVerticalAlignment(VerticalAlignment)` | Sets vertical alignment (default: Stretch) |
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
> Solid has `AccessibilityRole.None` by default as it's typically decorative.

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

### Spacer Between Elements

```csharp
new VStack()
    .AddChildren(
        new Label().SetText("Top"),
        new Solid()
            .SetVerticalAlignment(VerticalAlignment.Stretch),  // Flexible spacer
        new Label().SetText("Bottom")
    )
```

### Fixed Spacer

```csharp
new VStack()
    .AddChildren(
        new Label().SetText("Section 1"),
        new Solid(height: 20),  // 20px fixed spacer
        new Label().SetText("Section 2")
    )
```

### Colored Rectangle

```csharp
new Solid(width: 200, height: 100, color: Colors.Blue)
    .SetCornerRadius(8)
```

### Gradient Background

```csharp
new Solid(width: 300, height: 150)
    .SetBackground(new LinearGradient(Colors.Blue, Colors.Purple, 45))
    .SetCornerRadius(12)
```

### Sidebar Placeholder

```csharp
new HStack()
    .AddChildren(
        new Solid(width: 250, color: new Color(40, 40, 40)),  // Sidebar
        new VStack()
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
            .AddChildren(/* main content */)
    )
```

### Status Indicator

```csharp
new Solid(width: 12, height: 12)
    .SetCornerRadius(6)  // Circle
    .BindBackground(nameof(vm.Status), () =>
        vm.Status == "online" ? Colors.Green :
        vm.Status == "away" ? Colors.Yellow :
        Colors.Gray)
```

### Header Background

```csharp
new Grid()
    .AddRow(Row.Absolute, 60)
    .AddRow(Row.Star, 1)
    .AddColumn(Column.Star, 1)
    .AddChild(
        new Solid(color: new Color(30, 30, 30)),
        row: 0, column: 0)
    .AddChild(
        new Label().SetText("Header").SetTextColor(Colors.White),
        row: 0, column: 0)
    .AddChild(
        content,
        row: 1, column: 0)
```

### Card with Shadow

```csharp
new Solid(width: 300, height: 200)
    .SetBackground(Colors.White)
    .SetCornerRadius(12)
    .SetShadowColor(new Color(0, 0, 0, 50))
    .SetShadowOffset(new Point(0, 4))
    .SetShadowBlur(8)
```
