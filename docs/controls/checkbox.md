---
title: Checkbox
layout: default
parent: Controls
nav_order: 50
---

# Checkbox

A checkbox control for binary on/off selection with a checkmark indicator. Supports two-way data binding for MVVM patterns.

---

## Basic Usage

```csharp
// Simple checkbox
new Checkbox()
    .SetIsChecked(true)

// Styled checkbox
new Checkbox()
    .SetColor(Colors.Blue)
    .SetDesiredSize(new Size(24, 24))

// Data-bound checkbox
new Checkbox()
    .BindIsChecked(nameof(vm.AcceptTerms), () => vm.AcceptTerms, value => vm.AcceptTerms = value)
```

---

## Checkbox-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetIsChecked(bool)` | Sets checked state |
| `BindIsChecked(name, getter, setter)` | Two-way binds checked state |
| `SetColor(Color)` | Sets checkbox stroke/checkmark color (default: White) |
| `BindColor(name, getter)` | Binds color |

{: .note }
> Checkbox has a default size of 22x22 pixels. Use `SetDesiredSize` to change.

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
> Checkbox is focusable by default (`IsFocusable = true`).

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
> Checkbox has `AccessibilityRole.Checkbox` by default. The accessibility value automatically reflects "Checked" or "Unchecked" state.

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

### Checkbox with Label

```csharp
new HStack()
    .SetVerticalAlignment(VerticalAlignment.Center)
    .AddChildren(
        new Checkbox()
            .BindIsChecked(nameof(vm.RememberMe), () => vm.RememberMe, v => vm.RememberMe = v),
        new Label()
            .SetText("Remember me")
            .SetMargin(new Margin(8, 0, 0, 0))
    )
```

### Styled Checkbox

```csharp
new Checkbox()
    .SetColor(new Color(0, 122, 255))
    .SetDesiredSize(new Size(28, 28))
    .SetBackground(new Color(40, 40, 40))
    .SetCornerRadius(6)
    .BindIsChecked(nameof(vm.IsSelected), () => vm.IsSelected, v => vm.IsSelected = v)
```

### Accessible Checkbox

```csharp
new Checkbox()
    .SetAccessibilityLabel("Accept terms and conditions")
    .SetAccessibilityHint("Check to accept the terms")
    .BindIsChecked(nameof(vm.AcceptTerms), () => vm.AcceptTerms, v => vm.AcceptTerms = v)
```

### Checkbox with Tooltip

```csharp
new Checkbox()
    .SetTooltip("Enable notifications")
    .SetTooltipPlacement(TooltipPlacement.Right)
    .BindIsChecked(nameof(vm.EnableNotifications), () => vm.EnableNotifications, v => vm.EnableNotifications = v)
```
