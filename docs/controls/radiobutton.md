---
title: RadioButton
layout: default
parent: Controls
nav_order: 60
---

# RadioButton

A radio button control for mutually exclusive selection within a group. RadioButtons with the same Group value form a logical group where only one can be selected at a time.

---

## Basic Usage

```csharp
// Simple radio buttons grouped by string
new VStack()
    .AddChildren(
        new RadioButton()
            .SetText("Option A")
            .SetGroup("myGroup")
            .SetValue("A"),
        new RadioButton()
            .SetText("Option B")
            .SetGroup("myGroup")
            .SetValue("B"),
        new RadioButton()
            .SetText("Option C")
            .SetGroup("myGroup")
            .SetValue("C")
    )

// Pre-selected radio button
new RadioButton()
    .SetText("Default Choice")
    .SetGroup("choices")
    .SetIsSelected(true)
```

---

## RadioButton-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetGroup(object)` | Sets the group identifier |
| `BindGroup(name, getter)` | Binds group |
| `SetValue(object)` | Sets the value this button represents |
| `BindValue(name, getter)` | Binds value |
| `SetIsSelected(bool)` | Sets selected state |
| `BindIsSelected(name, getter, setter)` | Two-way binds selected state |
| `SetText(string)` | Sets the label text |
| `BindText(name, getter)` | Binds text |
| `SetTextSize(float)` | Sets font size (default: 14) |
| `BindTextSize(name, getter)` | Binds font size |
| `SetTextColor(Color)` | Sets text color (default: White) |
| `BindTextColor(name, getter)` | Binds text color |
| `SetCircleColor(Color)` | Sets unselected circle color (default: White) |
| `BindCircleColor(name, getter)` | Binds circle color |
| `SetSelectedColor(Color)` | Sets selected indicator color (default: iOS green) |
| `BindSelectedColor(name, getter)` | Binds selected color |

{: .note }
> RadioButtons with the same Group object form an exclusive group. When one is selected, others in the group are automatically deselected.

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
> RadioButton is focusable by default (`IsFocusable = true`).

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
> RadioButton has `AccessibilityRole.RadioButton` by default. The accessibility value automatically reflects "Selected" or "Not selected" state.

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

### Radio Button Group with String Grouping

```csharp
new VStack()
    .AddChildren(
        new Label().SetText("Select size:").SetFontWeight(FontWeight.SemiBold).SetMargin(new Margin(0, 0, 0, 12)),
        new RadioButton()
            .SetText("Small")
            .SetGroup("size")
            .SetValue("small")
            .SetIsSelected(true)
            .SetMargin(new Margin(0, 0, 0, 12)),
        new RadioButton()
            .SetText("Medium")
            .SetGroup("size")
            .SetValue("medium")
            .SetMargin(new Margin(0, 0, 0, 12)),
        new RadioButton()
            .SetText("Large")
            .SetGroup("size")
            .SetValue("large")
    )
```

### Styled Radio Buttons

```csharp
new RadioButton()
    .SetText("Premium Option")
    .SetGroup("plan")
    .SetTextColor(new Color(200, 200, 200))
    .SetTextSize(16)
    .SetCircleColor(new Color(100, 100, 100))
    .SetSelectedColor(new Color(0, 122, 255))
```

### Data-bound Radio Buttons

```csharp
new VStack()
    .AddChildren(
        new RadioButton()
            .SetText("Light Theme")
            .SetGroup("theme")
            .SetValue(Theme.Light)
            .BindIsSelected(nameof(vm.IsLightTheme),
                () => vm.Theme == Theme.Light,
                v => { if (v) vm.Theme = Theme.Light; }),
        new RadioButton()
            .SetText("Dark Theme")
            .SetGroup("theme")
            .SetValue(Theme.Dark)
            .BindIsSelected(nameof(vm.IsDarkTheme),
                () => vm.Theme == Theme.Dark,
                v => { if (v) vm.Theme = Theme.Dark; })
    )
```

### Accessible Radio Group

```csharp
new VStack()
    .AddChildren(
        new RadioButton()
            .SetText("Monthly")
            .SetGroup("billing")
            .SetAccessibilityLabel("Monthly billing")
            .SetAccessibilityHint("Select for monthly payments"),
        new RadioButton()
            .SetText("Annual (20% off)")
            .SetGroup("billing")
            .SetAccessibilityLabel("Annual billing with 20% discount")
            .SetAccessibilityHint("Select for yearly payments with savings")
    )
```
