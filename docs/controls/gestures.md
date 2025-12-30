---
title: Gesture Detectors
layout: default
parent: Controls
nav_order: 10
---

# Gesture Detectors

Wrap content to detect touch and mouse gestures.

---

## TapGestureDetector

Detects tap/click gestures.

### Properties

| Property | Set | Bind | Description |
|:---------|:----|:-----|:------------|
| Command | `SetCommand(ICommand)` | `BindCommand(name, getter)` | Command to execute on tap |
| CommandParameter | `SetCommandParameter(object)` | `BindCommandParameter(name, getter)` | Parameter passed to command |

### Basic Usage

```csharp
// Static command
new TapGestureDetector(
    new Label().SetText("Tap me!")
)
.SetCommand(vm.TapCommand)

// Bound command
new TapGestureDetector(
    new Label().SetText("Tap me!")
)
.BindCommand(nameof(vm.TapCommand), () => vm.TapCommand)
```

### With Parameter

```csharp
// Static parameter
new TapGestureDetector(
    new Label().SetText(item.Name)
)
.SetCommand(vm.SelectCommand)
.SetCommandParameter(item)

// Bound parameter
new TapGestureDetector(
    new Label().BindText(nameof(vm.ItemName), () => vm.ItemName)
)
.BindCommand(nameof(vm.SelectCommand), () => vm.SelectCommand)
.BindCommandParameter(nameof(vm.CurrentItem), () => vm.CurrentItem)
```

---

## DoubleTapGestureDetector

Detects double-tap gestures.

### Properties

| Property | Set | Bind | Description |
|:---------|:----|:-----|:------------|
| Command | `SetCommand(ICommand)` | `BindCommand(name, getter)` | Command to execute on double-tap |

### Basic Usage

```csharp
// Static command
new DoubleTapGestureDetector(
    new Image().SetImageSource("photo.jpg")
)
.SetCommand(vm.ZoomCommand)

// Bound command
new DoubleTapGestureDetector(
    new Image().BindImageSource(nameof(vm.ImageUrl), () => vm.ImageUrl)
)
.BindCommand(nameof(vm.LikeCommand), () => vm.LikeCommand)
```

---

## LongPressGestureDetector

Detects long-press (press and hold) gestures.

### Properties

| Property | Set | Bind | Description |
|:---------|:----|:-----|:------------|
| Command | `SetCommand(ICommand)` | `BindCommand(name, getter)` | Command to execute on long-press |

### Basic Usage

```csharp
// Static command
new LongPressGestureDetector(
    new Label().SetText("Hold me")
)
.SetCommand(vm.ShowContextCommand)

// Bound command
new LongPressGestureDetector(
    new Border()
        .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
        .AddChild(new Label().SetText("Hold for options"))
)
.BindCommand(nameof(vm.ShowOptionsCommand), () => vm.ShowOptionsCommand)
```

---

## SwipeGestureDetector

Detects swipe gestures. Command receives `SwipeDirection` as parameter.

### Properties

| Property | Set | Bind | Description |
|:---------|:----|:-----|:------------|
| Command | `SetCommand(ICommand)` | `BindCommand(name, getter)` | Command to execute on swipe |
| AllowedDirections | `SetAllowedDirections(SwipeDirection)` | `BindAllowedDirections(name, getter)` | Which directions to detect (default: All) |

### SwipeDirection Values

| Value | Description |
|:------|:------------|
| `SwipeDirection.None` | No direction |
| `SwipeDirection.Left` | Swipe left |
| `SwipeDirection.Right` | Swipe right |
| `SwipeDirection.Up` | Swipe up |
| `SwipeDirection.Down` | Swipe down |
| `SwipeDirection.All` | All directions (default) |

Combine with bitwise OR:
```csharp
SwipeDirection.Left | SwipeDirection.Right  // Horizontal only
SwipeDirection.Up | SwipeDirection.Down     // Vertical only
```

### Basic Usage

```csharp
// Detect all swipes
new SwipeGestureDetector(
    new Border()
        .SetBackground(new SolidColorBackground(Colors.Blue))
        .SetDesiredSize(new Size(200, 100))
)
.SetCommand(vm.SwipeCommand)

// Only horizontal swipes
new SwipeGestureDetector(
    new Label().SetText("Swipe left or right")
)
.SetCommand(vm.SwipeCommand)
.SetAllowedDirections(SwipeDirection.Left | SwipeDirection.Right)

// Bound directions
new SwipeGestureDetector(content)
.BindCommand(nameof(vm.SwipeCommand), () => vm.SwipeCommand)
.BindAllowedDirections(nameof(vm.AllowedSwipeDirections), () => vm.AllowedSwipeDirections)
```

### Handle Direction in ViewModel

```csharp
[RelayCommand]
private void OnSwipe(SwipeDirection direction)
{
    switch (direction)
    {
        case SwipeDirection.Left:
            NavigateNext();
            break;
        case SwipeDirection.Right:
            NavigatePrevious();
            break;
    }
}
```

---

## PinchGestureDetector

Detects pinch-to-zoom gestures. Command receives `float scale` as parameter.

### Properties

| Property | Set | Bind | Description |
|:---------|:----|:-----|:------------|
| Command | `SetCommand(ICommand)` | `BindCommand(name, getter)` | Command to execute on pinch (receives scale factor) |

### Basic Usage

```csharp
// Static command
new PinchGestureDetector(
    new Image().SetImageSource("map.jpg")
)
.SetCommand(vm.ZoomCommand)

// Bound command
new PinchGestureDetector(
    new Image().BindImageSource(nameof(vm.ImageUrl), () => vm.ImageUrl)
)
.BindCommand(nameof(vm.PinchZoomCommand), () => vm.PinchZoomCommand)
```

### Handle Scale in ViewModel

```csharp
[RelayCommand]
private void PinchZoom(float scale)
{
    // scale > 1.0 = zoom in
    // scale < 1.0 = zoom out
    CurrentZoom *= scale;
    CurrentZoom = Math.Clamp(CurrentZoom, 0.5f, 3.0f);
}
```

---

## Examples

### Tappable Card

```csharp
new TapGestureDetector(
    new Border()
        .SetBackground(new SolidColorBackground(new Color(45, 45, 45)))
        .SetCornerRadius(12)
        .SetPadding(new Margin(16))
        .AddChild(
            new VStack(
                new Label()
                    .SetText("Click anywhere")
                    .SetTextColor(Colors.White),
                new Label()
                    .SetText("This entire card is tappable")
                    .SetTextColor(Colors.Gray)
            )
        )
)
.SetCommand(vm.CardTappedCommand)
```

### Swipe-to-Delete List Item

```csharp
new SwipeGestureDetector(
    new HStack(
        new Label()
            .BindText(nameof(item.Name), () => item.Name)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch),
        new Label()
            .SetText("Swipe left to delete")
            .SetTextColor(Colors.Gray)
    ).SetPadding(new Margin(16))
)
.SetAllowedDirections(SwipeDirection.Left)
.SetCommand(vm.DeleteCommand)
.SetCommandParameter(item)
```

### Double-Tap to Like

```csharp
new DoubleTapGestureDetector(
    new Image()
        .BindImageSource(nameof(post.ImageUrl), () => post.ImageUrl)
        .SetAspect(Aspect.AspectFill)
)
.SetCommand(vm.LikeCommand)
.SetCommandParameter(post)
```

### Long-Press Context Menu

```csharp
new LongPressGestureDetector(
    new Border()
        .SetBackground(new SolidColorBackground(new Color(50, 50, 50)))
        .SetPadding(new Margin(12))
        .AddChild(
            new Label()
                .BindText(nameof(item.Name), () => item.Name)
                .SetTextColor(Colors.White)
        )
)
.SetCommand(vm.ShowOptionsCommand)
.SetCommandParameter(item)
```

### Zoomable Image

```csharp
new PinchGestureDetector(
    new Image()
        .SetImageSource("detailed-image.jpg")
        .SetAspect(Aspect.AspectFit)
)
.SetCommand(vm.HandlePinchCommand)
```

### Combined Gestures

```csharp
// Wrap multiple gesture detectors for different interactions
new DoubleTapGestureDetector(
    new LongPressGestureDetector(
        new TapGestureDetector(
            new Image()
                .BindImageSource(nameof(vm.ImageUrl), () => vm.ImageUrl)
        )
        .SetCommand(vm.SelectCommand)  // Single tap selects
    )
    .SetCommand(vm.ShowMenuCommand)    // Long press shows menu
)
.SetCommand(vm.ZoomCommand)            // Double tap zooms
```
