---
title: Controls
layout: default
nav_order: 4
has_children: true
---

# Controls Reference

PlusUi provides a comprehensive set of UI controls for building cross-platform applications.

---

## All Controls

### Text Controls
- [Label](label.html) - Display static or dynamic text
- [Entry](entry.html) - Single-line text input
- [Link](link.html) - Clickable hyperlink

### Input Controls
- [Button](button.html) - Clickable button with text/icon
- [Checkbox](checkbox.html) - Binary on/off selection
- [RadioButton](radiobutton.html) - Mutually exclusive selection
- [Toggle](toggle.html) - On/off switch
- [Slider](slider.html) - Value range selection
- [ComboBox](combobox.html) - Dropdown selection
- [DatePicker](datepicker.html) - Date selection with calendar
- [TimePicker](timepicker.html) - Time selection

### Layout Controls
- [VStack](vstack.html) - Vertical stack layout
- [HStack](hstack.html) - Horizontal stack layout
- [Grid](grid.html) - Row/column grid layout
- [UniformGrid](uniformgrid.html) - Equal-sized cell grid
- [ScrollView](scrollview.html) - Scrollable container
- [Scrollbar](scrollbar.html) - Scrollbar for custom scroll implementations
- [Border](border.html) - Container with border stroke
- [TabControl](tabcontrol.html) - Tabbed content container
- [Toolbar](toolbar.html) - Application toolbar

### Display Controls
- [Image](image.html) - Display images (static, animated, SVG)
- [ProgressBar](progressbar.html) - Determinate progress indicator
- [ActivityIndicator](activityindicator.html) - Loading spinner
- [Separator](separator.html) - Visual divider line
- [Solid](solid.html) - Colored rectangle/spacer

### Data Controls
- [ItemsList](itemslist.html) - Virtualized list for large datasets
- [DataGrid](datagrid.html) - Tabular data with columns
- [TreeView](treeview.html) - Hierarchical tree structure

### Menu Controls
- [Menu](menu.html) - Horizontal menu bar with dropdowns
- [ContextMenu](contextmenu.html) - Right-click context menus

### Other Controls
- [UserControl](usercontrol.html) - Creating custom reusable components
- [Gestures](gestures.html) - Tap, DoubleTap, LongPress, Swipe, Pinch
- [Popups](popups.html) - Modal dialogs and overlays

---

## Common Properties

All controls inherit from `UiElement` and share these common properties:

### Sizing

```csharp
.SetDesiredSize(new Size(200, 100))  // Fixed size
.SetDesiredWidth(200)                 // Fixed width only
.SetDesiredHeight(100)                // Fixed height only
```

### Alignment

```csharp
.SetHorizontalAlignment(HorizontalAlignment.Center)
.SetVerticalAlignment(VerticalAlignment.Center)

// Values: Left/Top, Center, Right/Bottom, Stretch (default)
```

### Margins

```csharp
.SetMargin(new Margin(10))           // All sides
.SetMargin(new Margin(10, 20))       // Horizontal, Vertical
.SetMargin(new Margin(10, 20, 30, 40)) // Left, Top, Right, Bottom
```

### Background

```csharp
.SetBackground(new SolidColorBackground(Colors.Blue))
.SetBackground(new LinearGradient(Colors.Blue, Colors.Purple, 45))
```

### Corner Radius

```csharp
.SetCornerRadius(8)
```

### Visibility

```csharp
.SetIsVisible(false)
.BindIsVisible(nameof(vm.ShowItem), () => vm.ShowItem)
```

### Tooltips

```csharp
.SetTooltip("Helpful text")
.SetTooltipPlacement(TooltipPlacement.Top)
.SetTooltipShowDelay(500)
```

---

## Fluent API Pattern

All PlusUi controls use a fluent API pattern:

```csharp
new Button()
    .SetText("Click Me")
    .SetTextSize(16)
    .SetTextColor(Colors.White)
    .SetBackground(new SolidColorBackground(Colors.Blue))
    .SetPadding(new Margin(20, 10))
    .SetCornerRadius(8)
    .SetCommand(vm.ClickCommand)
```

---

## Data Binding Pattern

Every `Set*` method has a corresponding `Bind*` method:

```csharp
// Static value
.SetText("Hello")

// Bound value (updates when property changes)
.BindText(nameof(vm.Greeting), () => vm.Greeting)

// Two-way binding (for input controls)
.BindText(nameof(vm.Name), () => vm.Name, value => vm.Name = value)
```

---

## Accessibility

Controls support accessibility features:

```csharp
.SetAccessibilityLabel("Submit button")
.SetAccessibilityHint("Double-tap to submit the form")
.SetTabIndex(1)
.SetTabStop(true)
```
