---
title: Toggle
layout: default
parent: Controls
nav_order: 70
---

# Toggle

A toggle/switch control for on/off states. Provides a modern iOS/Android style switch UI with a sliding thumb indicator.

---

## Basic Usage

```csharp
// Simple toggle
new Toggle()
    .SetIsOn(true)

// Styled toggle
new Toggle()
    .SetOnColor(Colors.Green)
    .SetOffColor(Colors.Gray)
    .SetThumbColor(Colors.White)

// Data-bound toggle
new Toggle()
    .BindIsOn(nameof(vm.IsDarkMode), () => vm.IsDarkMode, value => vm.IsDarkMode = value)
```

---

## Toggle-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetIsOn(bool)` | Sets on/off state |
| `BindIsOn(name, getter, setter)` | Two-way binds on/off state |
| `SetOnColor(Color)` | Sets track color when on (default: iOS green) |
| `BindOnColor(name, getter)` | Binds on color |
| `SetOffColor(Color)` | Sets track color when off (default: gray) |
| `BindOffColor(name, getter)` | Binds off color |
| `SetThumbColor(Color)` | Sets thumb/circle color (default: White) |
| `BindThumbColor(name, getter)` | Binds thumb color |

{: .note }
> Toggle has a default size of 50x28 pixels. Use `SetDesiredSize` to customize.

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
> Toggle is focusable by default (`IsFocusable = true`).

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
> Toggle has `AccessibilityRole.Toggle` by default. The accessibility value automatically reflects "On" or "Off" state.

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

### Toggle with Label

```csharp
new HStack()
    .SetVerticalAlignment(VerticalAlignment.Center)
    .AddChildren(
        new Label().SetText("Dark Mode"),
        new Toggle()
            .SetMargin(new Margin(12, 0, 0, 0))
            .BindIsOn(nameof(vm.IsDarkMode), () => vm.IsDarkMode, v => vm.IsDarkMode = v)
    )
```

### Custom Colored Toggle

```csharp
new Toggle()
    .SetOnColor(new Color(0, 122, 255))  // Blue when on
    .SetOffColor(new Color(60, 60, 60))  // Dark gray when off
    .SetThumbColor(Colors.White)
    .BindIsOn(nameof(vm.IsEnabled), () => vm.IsEnabled, v => vm.IsEnabled = v)
```

### Large Toggle

```csharp
new Toggle()
    .SetDesiredSize(new Size(70, 38))
    .SetOnColor(Colors.Green)
    .BindIsOn(nameof(vm.PowerOn), () => vm.PowerOn, v => vm.PowerOn = v)
```

### Accessible Toggle

```csharp
new Toggle()
    .SetAccessibilityLabel("Notifications")
    .SetAccessibilityHint("Toggle to enable or disable notifications")
    .BindIsOn(nameof(vm.NotificationsEnabled), () => vm.NotificationsEnabled, v => vm.NotificationsEnabled = v)
```

### Toggle with Tooltip

```csharp
new Toggle()
    .SetTooltip("Enable auto-save")
    .SetTooltipPlacement(TooltipPlacement.Top)
    .BindIsOn(nameof(vm.AutoSave), () => vm.AutoSave, v => vm.AutoSave = v)
```

### Settings Row Pattern

```csharp
new HStack()
    .SetPadding(new Margin(16, 12))
    .SetBackground(new Color(40, 40, 40))
    .SetCornerRadius(8)
    .AddChildren(
        new VStack()
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
            .AddChildren(
                new Label()
                    .SetText("Push Notifications")
                    .SetTextSize(16),
                new Label()
                    .SetText("Receive alerts about important updates")
                    .SetTextSize(12)
                    .SetTextColor(new Color(150, 150, 150))
            ),
        new Toggle()
            .SetVerticalAlignment(VerticalAlignment.Center)
            .BindIsOn(nameof(vm.PushEnabled), () => vm.PushEnabled, v => vm.PushEnabled = v)
    )
```
