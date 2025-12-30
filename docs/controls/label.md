---
title: Label
layout: default
parent: Controls
nav_order: 10
---

# Label

A non-editable text control for displaying static or dynamic text.

---

## Basic Usage

```csharp
// Simple label
new Label().SetText("Hello World")

// Styled label
new Label()
    .SetText("Title")
    .SetTextSize(24)
    .SetFontWeight(FontWeight.Bold)
    .SetTextColor(Colors.Blue)

// Data-bound label
new Label()
    .BindText(nameof(vm.UserName), () => vm.UserName)
```

---

## Text Methods

Methods inherited from `UiTextElement`:

| Method | Description |
|:-------|:------------|
| `SetText(string)` | Sets the text content |
| `BindText(name, getter)` | Binds text to a property |
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
| `SetHorizontalTextAlignment(HorizontalTextAlignment)` | Sets text alignment (default: Left) |
| `BindHorizontalTextAlignment(name, getter)` | Binds text alignment |
| `SetTextWrapping(TextWrapping)` | Sets wrapping mode (default: NoWrap) |
| `BindTextWrapping(name, getter)` | Binds wrapping mode |
| `SetMaxLines(int)` | Limits number of displayed lines |
| `BindMaxLines(name, getter)` | Binds max lines |
| `SetTextTruncation(TextTruncation)` | Sets truncation mode (default: None) |
| `BindTextTruncation(name, getter)` | Binds truncation mode |
| `SetSupportsSystemFontScaling(bool)` | Enables/disables system font scaling |

### FontWeight Values

| Value | Description |
|:------|:------------|
| `FontWeight.Thin` | Weight 100 |
| `FontWeight.ExtraLight` | Weight 200 |
| `FontWeight.Light` | Weight 300 |
| `FontWeight.Regular` | Weight 400 (default) |
| `FontWeight.Medium` | Weight 500 |
| `FontWeight.SemiBold` | Weight 600 |
| `FontWeight.Bold` | Weight 700 |
| `FontWeight.ExtraBold` | Weight 800 |
| `FontWeight.Black` | Weight 900 |

### TextWrapping Values

| Value | Description |
|:------|:------------|
| `TextWrapping.NoWrap` | Single line, no wrapping (default) |
| `TextWrapping.Wrap` | Wrap at character boundaries |
| `TextWrapping.WordWrap` | Wrap at word boundaries |

### TextTruncation Values

| Value | Description |
|:------|:------------|
| `TextTruncation.None` | No truncation (default) |
| `TextTruncation.Start` | Ellipsis at start: `...text` |
| `TextTruncation.Middle` | Ellipsis in middle: `te...xt` |
| `TextTruncation.End` | Ellipsis at end: `text...` |

### HorizontalTextAlignment Values

| Value | Description |
|:------|:------------|
| `HorizontalTextAlignment.Left` | Align text left (default) |
| `HorizontalTextAlignment.Center` | Center text |
| `HorizontalTextAlignment.Right` | Align text right |

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
> Label is not focusable by default (`IsFocusable = false`).

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

### TooltipPlacement Values

| Value | Description |
|:------|:------------|
| `TooltipPlacement.Auto` | Automatic positioning (default) |
| `TooltipPlacement.Top` | Above element |
| `TooltipPlacement.Bottom` | Below element |
| `TooltipPlacement.Left` | Left of element |
| `TooltipPlacement.Right` | Right of element |

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

### Multi-line Label with Wrapping

```csharp
new Label()
    .SetText("This is a long text that will wrap to multiple lines when it exceeds the available width.")
    .SetTextWrapping(TextWrapping.WordWrap)
    .SetDesiredWidth(200)
```

### Truncated Label

```csharp
new Label()
    .SetText("Very long text that gets truncated")
    .SetTextTruncation(TextTruncation.End)
    .SetDesiredWidth(100)
```

### Styled Heading

```csharp
new Label()
    .SetText("Welcome")
    .SetTextSize(32)
    .SetFontWeight(FontWeight.Bold)
    .SetTextColor(new Color(52, 199, 89))
    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
```

### Label with Shadow

```csharp
new Label()
    .SetText("Shadow Text")
    .SetTextSize(24)
    .SetShadowColor(new Color(0, 0, 0, 128))
    .SetShadowOffset(new Point(2, 2))
    .SetShadowBlur(4)
```

### Data-bound Label

```csharp
new Label()
    .BindText(nameof(vm.StatusMessage), () => vm.StatusMessage)
    .BindTextColor(nameof(vm.StatusColor), () => vm.StatusColor)
    .BindIsVisible(nameof(vm.ShowStatus), () => vm.ShowStatus)
```
