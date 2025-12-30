---
title: Button
layout: default
parent: Controls
nav_order: 40
---

# Button

A clickable button control that can display text and/or icons, and execute commands when clicked.

---

## Basic Usage

```csharp
// Simple button with text
new Button()
    .SetText("Click Me")
    .SetCommand(myCommand)

// Button with click handler
new Button()
    .SetText("Submit")
    .SetOnClick(() => HandleSubmit())

// Button with icon
new Button()
    .SetIcon("Assets/save.svg")
    .SetText("Save")
    .SetIconPosition(IconPosition.Leading)
```

---

## Button-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetPadding(Margin)` | Sets internal padding |
| `BindPadding(name, getter)` | Binds padding |
| `SetCommand(ICommand)` | Sets command to execute on click |
| `BindCommand(name, getter)` | Binds command |
| `SetCommandParameter(object)` | Sets parameter passed to command |
| `BindCommandParameter(name, getter)` | Binds command parameter |
| `SetOnClick(Action)` | Sets click handler action |
| `BindOnClick(name, getter)` | Binds click handler |
| `SetHoverBackground(IBackground)` | Sets background shown on hover |
| `BindHoverBackground(name, getter)` | Binds hover background |
| `SetIcon(string)` | Sets icon path (image or SVG) |
| `BindIcon(name, getter)` | Binds icon |
| `SetIconPosition(IconPosition)` | Sets icon position (default: Leading) |
| `BindIconPosition(name, getter)` | Binds icon position |
| `SetIconTintColor(Color)` | Sets SVG icon tint color |
| `BindIconTintColor(name, getter)` | Binds icon tint color |

### IconPosition Values

| Value | Description |
|:------|:------------|
| `IconPosition.None` | No icon displayed |
| `IconPosition.Leading` | Icon before text (default) |
| `IconPosition.Trailing` | Icon after text |

{: .note }
> IconPosition is a flags enum. You can combine `Leading` and `Trailing` to show icons on both sides.

---

## Text Methods

Methods inherited from `UiTextElement`:

| Method | Description |
|:-------|:------------|
| `SetText(string)` | Sets the button text |
| `BindText(name, getter)` | Binds text |
| `SetTextSize(float)` | Sets font size (default: 12) |
| `BindTextSize(name, getter)` | Binds font size |
| `SetTextColor(Color)` | Sets text color (default: White) |
| `BindTextColor(name, getter)` | Binds text color |
| `SetFontFamily(string)` | Sets custom font family |
| `BindFontFamily(name, getter)` | Binds font family |
| `SetFontWeight(FontWeight)` | Sets font weight (default: Regular) |
| `BindFontWeight(name, getter)` | Binds font weight |
| `SetFontStyle(FontStyle)` | Sets font style (Normal, Italic, Oblique) |
| `BindFontStyle(name, getter)` | Binds font style |
| `SetHorizontalTextAlignment(HorizontalTextAlignment)` | Sets text alignment (default: Center) |
| `BindHorizontalTextAlignment(name, getter)` | Binds text alignment |
| `SetTextWrapping(TextWrapping)` | Sets wrapping mode (default: NoWrap) |
| `BindTextWrapping(name, getter)` | Binds wrapping mode |
| `SetMaxLines(int)` | Limits number of displayed lines |
| `BindMaxLines(name, getter)` | Binds max lines |
| `SetTextTruncation(TextTruncation)` | Sets truncation mode (default: None) |
| `BindTextTruncation(name, getter)` | Binds truncation mode |
| `SetSupportsSystemFontScaling(bool)` | Enables/disables system font scaling |

{: .note }
> Button has `HorizontalTextAlignment.Center` by default, unlike most other text controls.

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
> Button is focusable by default (`IsFocusable = true`).

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
> Button has `AccessibilityRole.Button` by default. The accessibility label defaults to the button text.

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

### Styled Button

```csharp
new Button()
    .SetText("Submit")
    .SetTextSize(16)
    .SetTextColor(Colors.White)
    .SetFontWeight(FontWeight.SemiBold)
    .SetBackground(new Color(0, 122, 255))
    .SetHoverBackground(new Color(0, 100, 220))
    .SetCornerRadius(8)
    .SetPadding(new Margin(16, 12))
    .SetCommand(vm.SubmitCommand)
```

### Button with Icon

```csharp
new Button()
    .SetIcon("Assets/Icons/save.svg")
    .SetIconTintColor(Colors.White)
    .SetText("Save")
    .SetIconPosition(IconPosition.Leading)
    .SetTextColor(Colors.White)
    .SetBackground(new Color(52, 199, 89))
    .SetPadding(new Margin(12, 8))
    .SetOnClick(() => SaveDocument())
```

### Icon-Only Button

```csharp
new Button()
    .SetIcon("Assets/Icons/settings.svg")
    .SetIconTintColor(Colors.Gray)
    .SetBackground(Colors.Transparent)
    .SetHoverBackground(new Color(128, 128, 128, 50))
    .SetCornerRadius(4)
    .SetPadding(new Margin(8))
    .SetTooltip("Settings")
    .SetOnClick(() => OpenSettings())
```

### Data-bound Button

```csharp
new Button()
    .BindText(nameof(vm.ButtonLabel), () => vm.ButtonLabel)
    .BindCommand(nameof(vm.ExecuteCommand), () => vm.ExecuteCommand)
    .BindCommandParameter(nameof(vm.SelectedItem), () => vm.SelectedItem)
    .BindIsVisible(nameof(vm.ShowButton), () => vm.ShowButton)
```

### Button with Both Icons

```csharp
new Button()
    .SetIcon("Assets/Icons/arrow.svg")
    .SetIconPosition(IconPosition.Leading | IconPosition.Trailing)
    .SetText("Navigate")
    .SetPadding(new Margin(16, 10))
```

### Accessible Button

```csharp
new Button()
    .SetText("Delete")
    .SetAccessibilityLabel("Delete selected items")
    .SetAccessibilityHint("Double tap to delete all selected items permanently")
    .SetCommand(vm.DeleteCommand)
```
