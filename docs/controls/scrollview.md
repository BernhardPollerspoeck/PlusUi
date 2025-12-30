---
title: ScrollView
layout: default
parent: Controls
nav_order: 140
---

# ScrollView

A scrollable container that allows scrolling through content larger than the visible area. Supports both horizontal and vertical scrolling with mouse wheel and drag gestures.

---

## Basic Usage

```csharp
// Vertical scrolling (default)
new ScrollView(
    new VStack(
        new Label().SetText("Item 1"),
        new Label().SetText("Item 2"),
        new Label().SetText("Item 3")
        // ... many more items
    )
)

// Vertical only
new ScrollView(content)
    .SetCanScrollHorizontally(false)
    .SetCanScrollVertically(true)

// Both directions
new ScrollView(largeContent)
    .SetCanScrollHorizontally(true)
    .SetCanScrollVertically(true)
```

---

## ScrollView-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetCanScrollHorizontally(bool)` | Enables/disables horizontal scrolling (default: true) |
| `BindCanScrollHorizontally(name, getter)` | Binds horizontal scroll setting |
| `SetCanScrollVertically(bool)` | Enables/disables vertical scrolling (default: true) |
| `BindCanScrollVertically(name, getter)` | Binds vertical scroll setting |
| `SetScrollFactor(float)` | Sets scroll speed multiplier (default: 1.0) |
| `BindScrollFactor(name, getter)` | Binds scroll factor |
| `SetHorizontalOffset(float)` | Sets horizontal scroll position |
| `BindHorizontalOffset(name, getter)` | Binds horizontal offset |
| `SetVerticalOffset(float)` | Sets vertical scroll position |
| `BindVerticalOffset(name, getter)` | Binds vertical offset |

{: .note }
> ScrollView takes a single content element via constructor. Scroll offsets are automatically clamped to valid ranges.

---

## Container Methods

Methods inherited from `UiLayoutElement`:

| Method | Description |
|:-------|:------------|
| `SetFocusScope(FocusScopeMode)` | Sets focus trap behavior |
| `BindFocusScope(name, getter)` | Binds focus scope |
| `SetAccessibilityLandmark(AccessibilityLandmark)` | Sets accessibility landmark |
| `BindAccessibilityLandmark(name, getter)` | Binds landmark |

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
| `SetCornerRadius(float)` | Sets corner radius (content is clipped) |
| `BindCornerRadius(name, getter)` | Binds corner radius |
| `SetVisualOffset(Point)` | Offsets visual position |
| `BindVisualOffset(name, getter)` | Binds visual offset |

{: .note }
> Setting `CornerRadius` on a ScrollView clips the content to rounded corners.

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
> ScrollView has `AccessibilityRole.ScrollView` by default.

---

## Tooltip Methods

Extension methods available on all `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetTooltip(string)` | Sets tooltip text |
| `SetTooltip(UiElement)` | Sets tooltip with custom content |
| `SetTooltipPlacement(TooltipPlacement)` | Sets tooltip position |

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

### Vertical List

```csharp
new ScrollView(
    new VStack()
        .AddChildren(
            Enumerable.Range(1, 100)
                .Select(i => new Label().SetText($"Item {i}"))
                .ToArray()
        )
)
.SetCanScrollHorizontally(false)
.SetVerticalAlignment(VerticalAlignment.Stretch)
```

### Fixed Height Scrollable Area

```csharp
new ScrollView(
    new VStack()
        .AddChildren(content)
)
.SetDesiredHeight(300)
.SetCanScrollHorizontally(false)
.SetBackground(new Color(30, 30, 30))
.SetCornerRadius(8)
```

### Horizontal Scroll Only

```csharp
new ScrollView(
    new HStack()
        .AddChildren(
            cards.Select(card =>
                new Border()
                    .SetDesiredWidth(200)
                    .SetMargin(new Margin(8))
                    .AddChild(card)
            ).ToArray()
        )
)
.SetDesiredHeight(250)
.SetCanScrollHorizontally(true)
.SetCanScrollVertically(false)
```

### Two-Dimensional Scrolling

```csharp
new ScrollView(
    new Image()
        .SetImageSource("large-map.png")
        .SetDesiredSize(new Size(2000, 2000))
)
.SetDesiredSize(new Size(400, 300))
.SetCanScrollHorizontally(true)
.SetCanScrollVertically(true)
```

### Programmatic Scroll Position

```csharp
new ScrollView(content)
    .SetVerticalOffset(0)  // Start at top
    // Later: scroll to position
    .SetVerticalOffset(500)
```

### Faster Scroll Speed

```csharp
new ScrollView(content)
    .SetScrollFactor(2.0f)  // 2x scroll speed
    .SetCanScrollVertically(true)
```

### Scrollable Content in Stretching Layout

```csharp
new VStack()
    .SetVerticalAlignment(VerticalAlignment.Stretch)
    .AddChildren(
        new Label().SetText("Header"),
        new ScrollView(
            new VStack().AddChildren(/* many items */)
        )
        .SetVerticalAlignment(VerticalAlignment.Stretch)  // Takes remaining space
        .SetCanScrollHorizontally(false),
        new Label().SetText("Footer")
    )
```

### Data-bound Scroll Position

```csharp
new ScrollView(content)
    .BindVerticalOffset(nameof(vm.ScrollPosition), () => vm.ScrollPosition)
    .SetCanScrollVertically(true)
```
