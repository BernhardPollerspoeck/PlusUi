---
title: Entry
layout: default
parent: Controls
nav_order: 20
---

# Entry

A single-line text input control for user text entry. Supports password masking, placeholders, keyboard types, and two-way data binding.

---

## Basic Usage

```csharp
// Simple text entry
new Entry()
    .SetPlaceholder("Enter your name...")
    .BindText(nameof(vm.Name), () => vm.Name, value => vm.Name = value)

// Password entry
new Entry()
    .SetIsPassword(true)
    .SetPlaceholder("Password")

// Email entry with mobile keyboard
new Entry()
    .SetKeyboard(KeyboardType.Email)
    .SetReturnKey(ReturnKeyType.Done)
```

---

## Entry-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetPadding(Margin)` | Sets internal padding |
| `BindPadding(name, getter)` | Binds padding |
| `SetIsPassword(bool)` | Enables password masking |
| `BindIsPassword(name, getter)` | Binds password mode |
| `SetPasswordChar(char)` | Sets mask character (default: •) |
| `BindPasswordChar(name, getter)` | Binds mask character |
| `SetPlaceholder(string)` | Sets placeholder text |
| `BindPlaceholder(name, getter)` | Binds placeholder |
| `SetPlaceholderColor(Color)` | Sets placeholder color (default: gray) |
| `BindPlaceholderColor(name, getter)` | Binds placeholder color |
| `SetMaxLength(int)` | Limits input length |
| `BindMaxLength(name, getter)` | Binds max length |
| `SetKeyboard(KeyboardType)` | Sets mobile keyboard type |
| `BindKeyboard(name, getter)` | Binds keyboard type |
| `SetReturnKey(ReturnKeyType)` | Sets return key label |
| `BindReturnKey(name, getter)` | Binds return key type |
| `BindText(name, getter, setter)` | Two-way binds text content |

### KeyboardType Values

| Value | Description |
|:------|:------------|
| `KeyboardType.Default` | Standard alphanumeric keyboard (default) |
| `KeyboardType.Text` | Optimized for general text entry |
| `KeyboardType.Numeric` | Numeric keyboard for numbers |
| `KeyboardType.Email` | Email keyboard with @ and . accessible |
| `KeyboardType.Telephone` | Phone number entry |
| `KeyboardType.Url` | URL entry with / and .com accessible |

### ReturnKeyType Values

| Value | Description |
|:------|:------------|
| `ReturnKeyType.Default` | Default "Return" or "Enter" label |
| `ReturnKeyType.Go` | "Go" label for navigation |
| `ReturnKeyType.Send` | "Send" label for messages |
| `ReturnKeyType.Search` | "Search" label for search fields |
| `ReturnKeyType.Next` | "Next" label to move to next field |
| `ReturnKeyType.Done` | "Done" label to dismiss keyboard |

---

## Text Methods

Methods inherited from `UiTextElement`:

| Method | Description |
|:-------|:------------|
| `SetText(string)` | Sets the text content |
| `BindText(name, getter)` | Binds text (one-way) |
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
> Entry is focusable by default (`IsFocusable = true`) and shows a blinking cursor when focused.

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
> Entry has `AccessibilityRole.TextInput` by default. For password fields, the accessibility value is masked.

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

### Styled Entry Field

```csharp
new Entry()
    .SetPlaceholder("Enter username")
    .SetPlaceholderColor(new Color(120, 120, 120))
    .SetTextColor(Colors.White)
    .SetBackground(new Color(45, 45, 45))
    .SetCornerRadius(8)
    .SetPadding(new Margin(12, 10))
    .BindText(nameof(vm.Username), () => vm.Username, value => vm.Username = value)
```

### Password Entry with Max Length

```csharp
new Entry()
    .SetPlaceholder("Password (8-20 characters)")
    .SetIsPassword(true)
    .SetPasswordChar('*')
    .SetMaxLength(20)
    .SetBackground(new Color(50, 50, 50))
    .SetCornerRadius(6)
    .BindText(nameof(vm.Password), () => vm.Password, value => vm.Password = value)
```

### Mobile-Optimized Email Entry

```csharp
new Entry()
    .SetPlaceholder("email@example.com")
    .SetKeyboard(KeyboardType.Email)
    .SetReturnKey(ReturnKeyType.Next)
    .SetTextSize(16)
    .BindText(nameof(vm.Email), () => vm.Email, value => vm.Email = value)
```

### Search Field

```csharp
new Entry()
    .SetPlaceholder("Search...")
    .SetKeyboard(KeyboardType.Text)
    .SetReturnKey(ReturnKeyType.Search)
    .SetAccessibilityLabel("Search field")
    .SetAccessibilityHint("Enter text to search")
    .BindText(nameof(vm.SearchQuery), () => vm.SearchQuery, value => vm.SearchQuery = value)
```

### Entry with Focus Styling

```csharp
new Entry()
    .SetPlaceholder("Click to focus")
    .SetBackground(new Color(40, 40, 40))
    .SetFocusedBackground(new Color(50, 50, 60))
    .SetFocusedBorderColor(Colors.Blue)
    .SetFocusRingColor(Colors.Blue)
    .SetFocusRingWidth(2)
    .BindText(nameof(vm.Value), () => vm.Value, value => vm.Value = value)
```
