---
title: ActivityIndicator
layout: default
parent: Controls
nav_order: 160
---

# ActivityIndicator

An activity indicator (spinner) for showing indeterminate progress/loading states. Displays an animated spinning arc.

---

## Basic Usage

```csharp
// Default spinner
new ActivityIndicator()

// Styled spinner
new ActivityIndicator()
    .SetColor(Colors.Green)
    .SetSpeed(1.5f)
    .SetStrokeThickness(4)

// Controlled spinner (start/stop)
new ActivityIndicator()
    .BindIsRunning(nameof(vm.IsLoading), () => vm.IsLoading)
```

---

## ActivityIndicator-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetIsRunning(bool)` | Starts or stops the animation (default: true) |
| `BindIsRunning(name, getter)` | Binds running state |
| `SetColor(Color)` | Sets spinner color (default: iOS blue) |
| `BindColor(name, getter)` | Binds spinner color |
| `SetSpeed(float)` | Sets rotation speed multiplier (default: 1.0, minimum: 0.1) |
| `BindSpeed(name, getter)` | Binds speed |
| `SetStrokeThickness(float)` | Sets line thickness (default: 3.0) |
| `BindStrokeThickness(name, getter)` | Binds stroke thickness |

{: .note }
> ActivityIndicator has a default size of 40x40 pixels. The spinner only animates when `IsRunning` is true. It automatically adds the `Busy` accessibility trait when running.

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
| `SetAccessibilityLabel(string?)` | Sets screen reader label (default: "Loading") |
| `BindAccessibilityLabel(name, getter)` | Binds accessibility label |
| `SetAccessibilityHint(string?)` | Sets additional context hint |
| `BindAccessibilityHint(name, getter)` | Binds accessibility hint |
| `SetAccessibilityValue(string?)` | Sets current value description |
| `BindAccessibilityValue(name, getter)` | Binds accessibility value |
| `SetAccessibilityTraits(AccessibilityTrait)` | Sets accessibility traits |
| `BindAccessibilityTraits(name, getter)` | Binds accessibility traits |
| `SetIsAccessibilityElement(bool)` | Include in accessibility tree |

{: .note }
> ActivityIndicator has `AccessibilityRole.Spinner` by default and automatically adds the `Busy` trait when running.

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

### Loading Overlay

```csharp
new Border()
    .SetBackground(new Color(0, 0, 0, 128))
    .AddChild(
        new VStack()
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center)
            .AddChildren(
                new ActivityIndicator()
                    .SetColor(Colors.White)
                    .SetDesiredSize(new Size(60, 60)),
                new Label()
                    .SetText("Loading...")
                    .SetTextColor(Colors.White)
                    .SetMargin(new Margin(0, 16, 0, 0))
            )
    )
    .BindIsVisible(nameof(vm.IsLoading), () => vm.IsLoading)
```

### Button with Loading State

```csharp
new HStack()
    .SetVerticalAlignment(VerticalAlignment.Center)
    .AddChildren(
        new ActivityIndicator()
            .SetDesiredSize(new Size(20, 20))
            .SetColor(Colors.White)
            .BindIsRunning(nameof(vm.IsSaving), () => vm.IsSaving)
            .BindIsVisible(nameof(vm.IsSaving), () => vm.IsSaving),
        new Label()
            .BindText(nameof(vm.IsSaving), () => vm.IsSaving ? "Saving..." : "Save")
            .SetTextColor(Colors.White)
    )
```

### Custom Styled Spinner

```csharp
new ActivityIndicator()
    .SetColor(new Color(255, 87, 51))  // Orange
    .SetSpeed(2.0f)                     // Faster rotation
    .SetStrokeThickness(5)              // Thicker line
    .SetDesiredSize(new Size(80, 80))   // Larger size
```

### Inline Loading Indicator

```csharp
new HStack()
    .AddChildren(
        new Label().SetText("Checking availability"),
        new ActivityIndicator()
            .SetDesiredSize(new Size(16, 16))
            .SetMargin(new Margin(8, 0, 0, 0))
            .BindIsRunning(nameof(vm.IsChecking), () => vm.IsChecking)
    )
```

### Conditional Spinner

```csharp
new ActivityIndicator()
    .SetIsRunning(false)  // Initially stopped
    .SetColor(Colors.Blue)
// Later programmatically:
// spinner.SetIsRunning(true);
```
