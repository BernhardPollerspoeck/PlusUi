---
title: Border
layout: default
parent: Controls
nav_order: 100
---

# Border

A container control that draws a stroke border around its single child element. Supports solid, dashed, and dotted stroke patterns with customizable colors and thickness.

---

## Basic Usage

```csharp
// Simple border
new Border()
    .SetStrokeColor(Colors.Red)
    .SetStrokeThickness(2)
    .AddChild(
        new Label().SetText("Content")
    )

// Styled border
new Border()
    .SetStrokeColor(Colors.Blue)
    .SetStrokeThickness(2)
    .SetStrokeType(StrokeType.Dashed)
    .SetBackground(new Color(240, 240, 240))
    .SetCornerRadius(8)
    .AddChild(content)
```

---

## Border-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetStrokeColor(Color)` | Sets border stroke color (default: Black) |
| `BindStrokeColor(name, getter)` | Binds stroke color |
| `SetStrokeThickness(float)` | Sets stroke width in pixels (default: 1) |
| `BindStrokeThickness(name, getter)` | Binds stroke thickness |
| `SetStrokeType(StrokeType)` | Sets stroke pattern (default: Solid) |
| `BindStrokeType(name, getter)` | Binds stroke type |
| `AddChild(UiElement)` | Adds the single child element |

### StrokeType Values

| Value | Description |
|:------|:------------|
| `StrokeType.Solid` | Continuous solid line (default) |
| `StrokeType.Dashed` | Dashed pattern |
| `StrokeType.Dotted` | Dotted pattern |

{: .note }
> Border can only contain a single child. The child is positioned inside the stroke area with proper spacing.

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
| `HorizontalAlignment.Undefined` | Inherits from child |
| `HorizontalAlignment.Left` | Align to left |
| `HorizontalAlignment.Center` | Center horizontally |
| `HorizontalAlignment.Right` | Align to right |
| `HorizontalAlignment.Stretch` | Stretch to fill width |

### VerticalAlignment Values

| Value | Description |
|:------|:------------|
| `VerticalAlignment.Undefined` | Inherits from child |
| `VerticalAlignment.Top` | Align to top |
| `VerticalAlignment.Center` | Center vertically |
| `VerticalAlignment.Bottom` | Align to bottom |
| `VerticalAlignment.Stretch` | Stretch to fill height |

{: .note }
> When alignment is `Undefined`, Border inherits its alignment from its child element.

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

### Card with Border

```csharp
new Border()
    .SetStrokeColor(new Color(200, 200, 200))
    .SetStrokeThickness(1)
    .SetBackground(Colors.White)
    .SetCornerRadius(12)
    .SetShadowColor(new Color(0, 0, 0, 30))
    .SetShadowOffset(new Point(0, 2))
    .SetShadowBlur(4)
    .AddChild(
        new VStack()
            .SetMargin(new Margin(16))
            .AddChildren(
                new Label().SetText("Card Title").SetFontWeight(FontWeight.Bold),
                new Label().SetText("Card content goes here")
            )
    )
```

### Dashed Border Box

```csharp
new Border()
    .SetStrokeColor(Colors.Gray)
    .SetStrokeThickness(2)
    .SetStrokeType(StrokeType.Dashed)
    .SetCornerRadius(8)
    .AddChild(
        new Label()
            .SetText("Drop files here")
            .SetMargin(new Margin(24))
            .SetHorizontalAlignment(HorizontalAlignment.Center)
    )
```

### Dotted Border

```csharp
new Border()
    .SetStrokeColor(Colors.Blue)
    .SetStrokeThickness(3)
    .SetStrokeType(StrokeType.Dotted)
    .AddChild(
        new Label().SetText("Dotted border").SetMargin(new Margin(16))
    )
```

### Outlined Button-Style Border

```csharp
new Border()
    .SetStrokeColor(new Color(0, 122, 255))
    .SetStrokeThickness(2)
    .SetCornerRadius(8)
    .SetBackground(Colors.Transparent)
    .AddChild(
        new Label()
            .SetText("Outlined")
            .SetTextColor(new Color(0, 122, 255))
            .SetMargin(new Margin(12, 8))
            .SetHorizontalAlignment(HorizontalAlignment.Center)
    )
```

### Data-bound Border

```csharp
new Border()
    .BindStrokeColor(nameof(vm.BorderColor), () => vm.BorderColor)
    .BindStrokeThickness(nameof(vm.BorderWidth), () => vm.BorderWidth)
    .BindIsVisible(nameof(vm.ShowBorder), () => vm.ShowBorder)
    .AddChild(content)
```
