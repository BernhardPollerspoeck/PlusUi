---
title: Scrollbar
layout: default
parent: Controls
nav_order: 145
---

# Scrollbar

A scrollbar control that displays scroll position and allows dragging to scroll content. Typically used internally by ScrollView but can also be used standalone.

---

## Basic Usage

```csharp
// Vertical scrollbar (default)
new Scrollbar()
    .SetOrientation(ScrollbarOrientation.Vertical)
    .SetOnValueChanged(offset => Console.WriteLine($"Scrolled to {offset}"))

// Horizontal scrollbar
new Scrollbar()
    .SetOrientation(ScrollbarOrientation.Horizontal)
    .SetWidth(10)
```

---

## Scrollbar-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetOrientation(ScrollbarOrientation)` | Sets vertical or horizontal orientation |
| `BindOrientation(getter)` | Binds orientation |
| `SetWidth(float)` | Sets scrollbar width/thickness (default: 12) |
| `BindWidth(getter)` | Binds width |
| `SetMinThumbSize(float)` | Sets minimum thumb size (default: 20) |
| `BindMinThumbSize(getter)` | Binds minimum thumb size |
| `SetOnValueChanged(Action<float>)` | Sets callback when scroll position changes |
| `BindOnValueChanged(getter)` | Binds value changed callback |

### ScrollbarOrientation Values

| Value | Description |
|:------|:------------|
| `ScrollbarOrientation.Vertical` | Vertical scrollbar (default) |
| `ScrollbarOrientation.Horizontal` | Horizontal scrollbar |

---

## Appearance Methods

| Method | Description |
|:-------|:------------|
| `SetThumbColor(Color)` | Sets thumb color (default: gray) |
| `BindThumbColor(getter)` | Binds thumb color |
| `SetThumbHoverColor(Color)` | Sets thumb color on hover |
| `BindThumbHoverColor(getter)` | Binds hover color |
| `SetThumbDragColor(Color)` | Sets thumb color while dragging |
| `BindThumbDragColor(getter)` | Binds drag color |
| `SetTrackColor(Color)` | Sets track background color |
| `BindTrackColor(getter)` | Binds track color |
| `SetThumbCornerRadius(float)` | Sets thumb corner radius (default: 4) |
| `BindThumbCornerRadius(getter)` | Binds thumb corner radius |
| `SetTrackCornerRadius(float)` | Sets track corner radius (default: 4) |
| `BindTrackCornerRadius(getter)` | Binds track corner radius |

---

## Auto-Hide Methods

| Method | Description |
|:-------|:------------|
| `SetAutoHide(bool)` | Enables auto-hide when not in use (default: false) |
| `BindAutoHide(getter)` | Binds auto-hide |
| `SetAutoHideDelay(int)` | Sets delay in ms before hiding (default: 1000) |
| `BindAutoHideDelay(getter)` | Binds auto-hide delay |

---

## Examples

### Custom Styled Scrollbar

```csharp
new Scrollbar()
    .SetOrientation(ScrollbarOrientation.Vertical)
    .SetWidth(8)
    .SetThumbColor(new Color(80, 80, 80))
    .SetThumbHoverColor(new Color(120, 120, 120))
    .SetTrackColor(new Color(30, 30, 30, 80))
    .SetThumbCornerRadius(4)
```

### Auto-Hiding Scrollbar

```csharp
new Scrollbar()
    .SetAutoHide(true)
    .SetAutoHideDelay(2000)  // Hide after 2 seconds
```

{: .note }
> Scrollbar has `AccessibilityRole.Scrollbar` by default. The scroll position is automatically announced as a percentage.
