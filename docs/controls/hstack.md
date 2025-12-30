---
title: HStack
layout: default
parent: Controls
nav_order: 120
---

# HStack

A horizontal stack layout that arranges child elements from left to right. Supports automatic size calculation, alignment, and wrapping to multiple rows.

---

## Basic Usage

```csharp
// Simple horizontal layout via constructor
new HStack(
    new Label().SetText("Name:"),
    new Entry().SetPlaceholder("Enter name")
)

// Using AddChildren fluently
new HStack()
    .AddChildren(
        new Button().SetText("Cancel"),
        new Button().SetText("OK")
    )

// Wrapping layout (like a WrapPanel)
new HStack(tag1, tag2, tag3, tag4, tag5)
    .SetWrap(true)
```

---

## HStack-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetWrap(bool)` | Enables row wrapping when content exceeds width |
| `BindWrap(name, getter)` | Binds wrap mode |

{: .note }
> When `Wrap` is enabled, elements that exceed the available width wrap to the next row (flowing top to bottom).

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
> Children with `HorizontalAlignment.Stretch` divide remaining horizontal space equally.

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
> HStack has `AccessibilityRole.Container` by default.

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

### Button Row

```csharp
new HStack()
    .SetHorizontalAlignment(HorizontalAlignment.Right)
    .AddChildren(
        new Button().SetText("Cancel").SetMargin(new Margin(0, 0, 8, 0)),
        new Button().SetText("Save")
    )
```

### Form Row with Label

```csharp
new HStack()
    .SetVerticalAlignment(VerticalAlignment.Center)
    .AddChildren(
        new Label().SetText("Email:").SetDesiredWidth(80),
        new Entry()
            .SetPlaceholder("Enter email")
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
    )
```

### Centered Row

```csharp
new HStack()
    .SetHorizontalAlignment(HorizontalAlignment.Center)
    .AddChildren(
        new Image().SetImageSource("avatar.png").SetDesiredSize(new Size(40, 40)),
        new VStack()
            .SetMargin(new Margin(12, 0, 0, 0))
            .AddChildren(
                new Label().SetText("John Doe").SetFontWeight(FontWeight.Bold),
                new Label().SetText("Online").SetTextColor(Colors.Green)
            )
    )
```

### Stretching Children

```csharp
new HStack()
    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
    .AddChildren(
        new Button()
            .SetText("Option 1")
            .SetHorizontalAlignment(HorizontalAlignment.Stretch),  // Takes 50%
        new Button()
            .SetText("Option 2")
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)   // Takes 50%
    )
```

### Tag Cloud with Wrapping

```csharp
new HStack()
    .SetWrap(true)
    .AddChildren(
        CreateTag("JavaScript"),
        CreateTag("TypeScript"),
        CreateTag("C#"),
        CreateTag("Python"),
        CreateTag("Rust"),
        CreateTag("Go")
    )

// Helper method
UiElement CreateTag(string text) => new Border()
    .SetBackground(new Color(0, 122, 255, 50))
    .SetCornerRadius(12)
    .SetMargin(new Margin(4))
    .AddChild(
        new Label()
            .SetText(text)
            .SetTextColor(new Color(0, 122, 255))
            .SetMargin(new Margin(8, 4))
    );
```

### Toolbar Navigation

```csharp
new HStack()
    .SetAccessibilityLandmark(AccessibilityLandmark.Navigation)
    .SetBackground(new Color(40, 40, 40))
    .AddChildren(
        new Button().SetText("Home"),
        new Button().SetText("Products"),
        new Button().SetText("About"),
        new Button().SetText("Contact")
    )
```

### Space Between Items

```csharp
new HStack()
    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
    .AddChildren(
        new Label().SetText("Left"),
        new Label()
            .SetHorizontalAlignment(HorizontalAlignment.Stretch),  // Spacer
        new Label().SetText("Right")
    )
```
