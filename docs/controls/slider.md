---
title: Slider
layout: default
parent: Controls
nav_order: 80
---

# Slider

A slider control for selecting a value within a range. Supports horizontal orientation with customizable min/max values and keyboard navigation.

---

## Basic Usage

```csharp
// Simple slider (0-100 range)
new Slider()
    .SetValue(50)

// Custom range slider
new Slider()
    .SetMinimum(0)
    .SetMaximum(10)
    .SetValue(5)

// Data-bound slider
new Slider()
    .SetMinimum(0)
    .SetMaximum(100)
    .BindValue(nameof(vm.Volume), () => vm.Volume, value => vm.Volume = value)
```

---

## Slider-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetValue(float)` | Sets current value |
| `BindValue(name, getter, setter)` | Two-way binds value |
| `SetMinimum(float)` | Sets minimum value (default: 0) |
| `BindMinimum(name, getter)` | Binds minimum |
| `SetMaximum(float)` | Sets maximum value (default: 100) |
| `BindMaximum(name, getter)` | Binds maximum |
| `SetMinimumTrackColor(Color)` | Sets filled portion color (default: iOS blue) |
| `BindMinimumTrackColor(name, getter)` | Binds minimum track color |
| `SetMaximumTrackColor(Color)` | Sets unfilled portion color (default: light gray) |
| `BindMaximumTrackColor(name, getter)` | Binds maximum track color |
| `SetThumbColor(Color)` | Sets thumb/knob color (default: White) |
| `BindThumbColor(name, getter)` | Binds thumb color |

{: .note }
> Slider has a default height of 30 pixels and stretches horizontally. The value is automatically clamped to the min/max range.

---

## Keyboard Navigation

When focused, the slider responds to keyboard input:

| Key | Action |
|:----|:-------|
| `Left Arrow` / `Down Arrow` | Decrease value by 5% |
| `Right Arrow` / `Up Arrow` | Increase value by 5% |

---

## Layout Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetMargin(Margin)` | Sets outer margin |
| `BindMargin(name, getter)` | Binds margin |
| `SetHorizontalAlignment(HorizontalAlignment)` | Sets horizontal alignment (default: Stretch) |
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
| `HorizontalAlignment.Stretch` | Stretch to fill width (default) |

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
> Slider is focusable by default (`IsFocusable = true`).

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
> Slider has `AccessibilityRole.Slider` by default. The accessibility value automatically reports the current value and range (e.g., "50 (0 to 100)").

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

### Volume Slider with Label

```csharp
new VStack()
    .AddChildren(
        new HStack()
            .AddChildren(
                new Label().SetText("Volume"),
                new Label()
                    .BindText(nameof(vm.Volume), () => $"{vm.Volume:F0}%")
            ),
        new Slider()
            .SetMargin(new Margin(0, 8, 0, 0))
            .SetMinimum(0)
            .SetMaximum(100)
            .BindValue(nameof(vm.Volume), () => vm.Volume, v => vm.Volume = v)
    )
```

### Custom Styled Slider

```csharp
new Slider()
    .SetMinimum(0)
    .SetMaximum(100)
    .SetValue(50)
    .SetMinimumTrackColor(new Color(52, 199, 89))  // Green filled portion
    .SetMaximumTrackColor(new Color(60, 60, 60))   // Dark gray unfilled
    .SetThumbColor(Colors.White)
    .BindValue(nameof(vm.Progress), () => vm.Progress, v => vm.Progress = v)
```

### Temperature Slider with Custom Range

```csharp
new VStack()
    .AddChildren(
        new Label()
            .BindText(nameof(vm.Temperature), () => $"Temperature: {vm.Temperature:F1}°C"),
        new Slider()
            .SetMargin(new Margin(0, 4, 0, 0))
            .SetMinimum(-20)
            .SetMaximum(50)
            .SetMinimumTrackColor(Colors.Orange)
            .BindValue(nameof(vm.Temperature), () => vm.Temperature, v => vm.Temperature = v)
    )
```

### Accessible Slider

```csharp
new Slider()
    .SetMinimum(0)
    .SetMaximum(100)
    .SetAccessibilityLabel("Brightness")
    .SetAccessibilityHint("Drag to adjust screen brightness")
    .BindValue(nameof(vm.Brightness), () => vm.Brightness, v => vm.Brightness = v)
```

### Fixed Width Slider

```csharp
new Slider()
    .SetHorizontalAlignment(HorizontalAlignment.Left)
    .SetDesiredWidth(200)
    .SetMinimum(0)
    .SetMaximum(100)
    .BindValue(nameof(vm.Value), () => vm.Value, v => vm.Value = v)
```
