---
title: ProgressBar
layout: default
parent: Controls
nav_order: 150
---

# ProgressBar

A progress bar control for showing determinate progress (0.0 to 1.0). Displays a horizontal bar with customizable colors for track and fill.

---

## Basic Usage

```csharp
// Simple progress bar
new ProgressBar()
    .SetProgress(0.5f)  // 50%

// Styled progress bar
new ProgressBar()
    .SetProgress(0.75f)
    .SetProgressColor(Colors.Green)
    .SetTrackColor(new Color(60, 60, 60))
    .SetDesiredHeight(12)

// Data-bound progress
new ProgressBar()
    .BindProgress(nameof(vm.UploadProgress), () => vm.UploadProgress)
```

---

## ProgressBar-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetProgress(float)` | Sets progress value 0.0-1.0 |
| `BindProgress(name, getter)` | Binds progress value |
| `SetProgressColor(Color)` | Sets filled portion color (default: iOS blue) |
| `BindProgressColor(name, getter)` | Binds progress color |
| `SetTrackColor(Color)` | Sets background track color (default: light gray) |
| `BindTrackColor(name, getter)` | Binds track color |

{: .note }
> ProgressBar has a default height of 8 pixels and stretches horizontally. Progress is automatically clamped between 0.0 and 1.0.

---

## Layout Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetMargin(Margin)` | Sets outer margin |
| `BindMargin(name, getter)` | Binds margin |
| `SetHorizontalAlignment(HorizontalAlignment)` | Sets horizontal alignment (default: Stretch) |
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
> ProgressBar has `AccessibilityRole.ProgressBar` by default. The accessibility value automatically reports the percentage.

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

### Upload Progress with Label

```csharp
new VStack()
    .AddChildren(
        new HStack()
            .AddChildren(
                new Label().SetText("Uploading..."),
                new Label()
                    .BindText(nameof(vm.UploadProgress), () => $"{vm.UploadProgress * 100:F0}%")
                    .SetHorizontalAlignment(HorizontalAlignment.Right)
            ),
        new ProgressBar()
            .SetMargin(new Margin(0, 4, 0, 0))
            .BindProgress(nameof(vm.UploadProgress), () => vm.UploadProgress)
    )
```

### Custom Styled Progress

```csharp
new ProgressBar()
    .SetProgress(0.65f)
    .SetProgressColor(new Color(52, 199, 89))  // Green
    .SetTrackColor(new Color(45, 45, 45))      // Dark gray
    .SetDesiredHeight(16)
```

### Thin Progress Line

```csharp
new ProgressBar()
    .SetProgress(0.3f)
    .SetDesiredHeight(2)
    .SetProgressColor(Colors.Blue)
    .SetTrackColor(Colors.Transparent)
```

### Fixed Width Progress

```csharp
new ProgressBar()
    .SetHorizontalAlignment(HorizontalAlignment.Left)
    .SetDesiredWidth(200)
    .SetProgress(0.8f)
```

### Accessible Progress Bar

```csharp
new ProgressBar()
    .SetAccessibilityLabel("Download progress")
    .SetAccessibilityHint("Shows file download completion")
    .BindProgress(nameof(vm.DownloadProgress), () => vm.DownloadProgress)
```

### Color-coded Progress

```csharp
new ProgressBar()
    .BindProgress(nameof(vm.Progress), () => vm.Progress)
    .BindProgressColor(nameof(vm.Progress), () =>
        vm.Progress < 0.3f ? Colors.Red :
        vm.Progress < 0.7f ? Colors.Orange :
        Colors.Green)
```
