---
title: VStack
layout: default
parent: Controls
nav_order: 110
---

# VStack

A vertical stack layout that arranges child elements from top to bottom. Supports automatic size calculation, alignment, and wrapping to multiple columns.

---

## Basic Usage

```csharp
// Simple vertical layout via constructor
new VStack(
    new Label().SetText("Title"),
    new Label().SetText("Subtitle"),
    new Button().SetText("Action")
)

// Using AddChildren fluently
new VStack()
    .AddChildren(
        new Label().SetText("First"),
        new Label().SetText("Second"),
        new Label().SetText("Third")
    )

// Wrapping layout (columns)
new VStack(item1, item2, item3, item4, item5)
    .SetWrap(true)
```

---

## VStack-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetWrap(bool)` | Enables column wrapping when content exceeds height |
| `BindWrap(name, getter)` | Binds wrap mode |

{: .note }
> When `Wrap` is enabled, elements that exceed the available height wrap to the next column (flowing left to right).

---

## Container Methods

Methods inherited from `UiLayoutElement`:

| Method | Description |
|:-------|:------------|
| `AddChild(UiElement)` | Adds a child element |
| `AddChildren(params UiElement[])` | Adds multiple children |
| `RemoveChild(UiElement)` | Removes a child element |
| `ClearChildren()` | Removes all children |
| `SetFocusScope(FocusScopeMode)` | Sets focus trap behavior |
| `BindFocusScope(name, getter)` | Binds focus scope |
| `SetAccessibilityLandmark(AccessibilityLandmark)` | Sets accessibility landmark |
| `BindAccessibilityLandmark(name, getter)` | Binds landmark |

### FocusScopeMode Values

| Value | Description |
|:------|:------------|
| `FocusScopeMode.None` | Normal tab navigation (default) |
| `FocusScopeMode.Trap` | Tab cycles within container |
| `FocusScopeMode.TrapWithEscape` | Tab cycles, Escape exits |

### AccessibilityLandmark Values

| Value | Description |
|:------|:------------|
| `AccessibilityLandmark.None` | No landmark (default) |
| `AccessibilityLandmark.Main` | Main content area |
| `AccessibilityLandmark.Navigation` | Navigation region |
| `AccessibilityLandmark.Search` | Search region |
| `AccessibilityLandmark.Banner` | Header/banner |
| `AccessibilityLandmark.ContentInfo` | Footer information |
| `AccessibilityLandmark.Form` | Form region |
| `AccessibilityLandmark.Complementary` | Complementary content |

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

{: .note }
> Children with `VerticalAlignment.Stretch` divide remaining vertical space equally.

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
> VStack has `AccessibilityRole.Container` by default.

---

## Tooltip Methods

Extension methods available on all `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetTooltip(string)` | Sets tooltip text |
| `SetTooltip(UiElement)` | Sets tooltip with custom content |
| `SetTooltipPlacement(TooltipPlacement)` | Sets tooltip position |
| `SetTooltipShowDelay(int)` | Sets show delay in ms |
| `SetTooltipHideDelay(int)` | Sets hide delay in ms |

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

### Form Layout

```csharp
new VStack()
    .SetMargin(new Margin(16))
    .AddChildren(
        new Label().SetText("Username").SetFontWeight(FontWeight.SemiBold),
        new Entry().SetPlaceholder("Enter username"),
        new Label().SetText("Password").SetFontWeight(FontWeight.SemiBold),
        new Entry().SetPlaceholder("Enter password").SetIsPassword(true),
        new Button()
            .SetText("Login")
            .SetMargin(new Margin(0, 16, 0, 0))
    )
```

### Centered Content

```csharp
new VStack()
    .SetHorizontalAlignment(HorizontalAlignment.Center)
    .SetVerticalAlignment(VerticalAlignment.Center)
    .AddChildren(
        new Image().SetImageSource("logo.png").SetDesiredSize(new Size(100, 100)),
        new Label().SetText("Welcome").SetTextSize(24),
        new Label().SetText("Please sign in to continue")
    )
```

### Stretching Child

```csharp
new VStack()
    .SetVerticalAlignment(VerticalAlignment.Stretch)
    .AddChildren(
        new Label().SetText("Header"),
        new ScrollView()
            .SetVerticalAlignment(VerticalAlignment.Stretch)  // Takes remaining space
            .AddChild(content),
        new Label().SetText("Footer")
    )
```

### Wrapping Columns

```csharp
new VStack()
    .SetWrap(true)
    .SetDesiredHeight(300)
    .AddChildren(
        new Label().SetText("Item 1"),
        new Label().SetText("Item 2"),
        new Label().SetText("Item 3"),
        new Label().SetText("Item 4"),
        new Label().SetText("Item 5")  // Wraps to second column if needed
    )
```

### Card with Background

```csharp
new VStack()
    .SetBackground(new Color(45, 45, 45))
    .SetCornerRadius(12)
    .SetMargin(new Margin(8))
    .AddChildren(
        new Label().SetText("Card Title").SetFontWeight(FontWeight.Bold).SetMargin(new Margin(16, 16, 16, 8)),
        new Label().SetText("Card content goes here").SetMargin(new Margin(16, 0, 16, 16))
    )
```

### Modal with Focus Trap

```csharp
new VStack()
    .SetFocusScope(FocusScopeMode.TrapWithEscape)
    .SetBackground(new Color(30, 30, 30))
    .SetCornerRadius(8)
    .SetMargin(new Margin(20))
    .AddChildren(
        new Label().SetText("Modal Title").SetFontWeight(FontWeight.Bold),
        new Label().SetText("Press Escape to close"),
        new Button().SetText("Close")
    )
```
