---
title: TimePicker
layout: default
parent: Controls
nav_order: 210
---

# TimePicker

A time picker control with hour/minute selector popup. Supports 12/24-hour formats, minute increments, time range restrictions, keyboard input, and two-way binding.

---

## Basic Usage

```csharp
// Simple time picker
new TimePicker()
    .SetPlaceholder("Select time...")

// With constraints
new TimePicker()
    .SetSelectedTime(new TimeOnly(9, 0))
    .SetMinuteIncrement(15)
    .Set24HourFormat(true)
    .BindSelectedTime(nameof(vm.StartTime), () => vm.StartTime, v => vm.StartTime = v)

// 12-hour format with AM/PM
new TimePicker()
    .Set24HourFormat(false)
    .SetDisplayFormat("hh:mm tt")
```

---

## TimePicker-Specific Methods

### Time Methods

| Method | Description |
|:-------|:------------|
| `SetSelectedTime(TimeOnly?)` | Sets selected time |
| `BindSelectedTime(name, getter)` | One-way binds selected time |
| `BindSelectedTime(name, getter, setter)` | Two-way binds selected time |
| `SetMinTime(TimeOnly?)` | Sets minimum selectable time |
| `BindMinTime(name, getter)` | Binds minimum time |
| `SetMaxTime(TimeOnly?)` | Sets maximum selectable time |
| `BindMaxTime(name, getter)` | Binds maximum time |
| `SetMinuteIncrement(int)` | Sets minute increment (1, 5, 10, 15, or 30) |
| `BindMinuteIncrement(name, getter)` | Binds minute increment |
| `Set24HourFormat(bool)` | Enables 24-hour format (default: true) |
| `Bind24HourFormat(name, getter)` | Binds 24-hour format |

### Display Methods

| Method | Description |
|:-------|:------------|
| `SetDisplayFormat(string)` | Sets time format string (default: "HH:mm") |
| `BindDisplayFormat(name, getter)` | Binds display format |
| `SetPlaceholder(string)` | Sets placeholder text |
| `BindPlaceholder(name, getter)` | Binds placeholder |
| `SetPlaceholderColor(SKColor)` | Sets placeholder color (default: gray) |
| `BindPlaceholderColor(name, getter)` | Binds placeholder color |

### Text Appearance Methods

| Method | Description |
|:-------|:------------|
| `SetTextColor(SKColor)` | Sets text color (default: white) |
| `BindTextColor(name, getter)` | Binds text color |
| `SetTextSize(float)` | Sets text size (default: 14) |
| `BindTextSize(name, getter)` | Binds text size |
| `SetFontFamily(string)` | Sets font family |
| `BindFontFamily(name, getter)` | Binds font family |
| `SetPadding(Margin)` | Sets internal padding (default: 12, 8) |
| `BindPadding(name, getter)` | Binds padding |

### Selector Appearance Methods

| Method | Description |
|:-------|:------------|
| `SetSelectorBackground(SKColor)` | Sets selector popup background |
| `BindSelectorBackground(name, getter)` | Binds selector background |
| `SetHoverBackground(SKColor)` | Sets hover highlight color |
| `BindHoverBackground(name, getter)` | Binds hover color |
| `SetSelectedBackground(SKColor)` | Sets selected time background |
| `BindSelectedBackground(name, getter)` | Binds selected background |

### State Methods

| Method | Description |
|:-------|:------------|
| `SetIsOpen(bool)` | Opens or closes selector |
| `BindIsOpen(name, getter)` | Binds open state |

{: .note }
> TimePicker has a default size of 150x40 pixels. Keyboard navigation: Arrow Up/Down for hours, Arrow Left/Right for minutes, Enter/Space to confirm, Escape to close. Direct keyboard input is supported (e.g., typing "09:30").

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

## Focus Methods

Methods inherited from `UiElement`:

| Method | Description |
|:-------|:------------|
| `SetTabIndex(int)` | Sets tab order |
| `BindTabIndex(name, getter)` | Binds tab index |
| `SetTabStop(bool)` | Enables/disables tab stop (default: true) |
| `BindTabStop(name, getter)` | Binds tab stop |

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
> TimePicker has `AccessibilityRole.TimePicker` by default. It automatically reports the selected time and Expanded/HasPopup traits.

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

### Meeting Time Picker

```csharp
new TimePicker()
    .SetPlaceholder("Meeting time")
    .SetMinuteIncrement(15)
    .SetMinTime(new TimeOnly(9, 0))
    .SetMaxTime(new TimeOnly(17, 0))
    .BindSelectedTime(nameof(vm.MeetingTime), () => vm.MeetingTime, t => vm.MeetingTime = t)
    .SetAccessibilityLabel("Meeting time")
```

### Time Range Selection

```csharp
new HStack()
    .AddChildren(
        new TimePicker()
            .SetPlaceholder("Start")
            .BindSelectedTime(nameof(vm.StartTime), () => vm.StartTime, t => vm.StartTime = t)
            .BindMaxTime(nameof(vm.EndTime), () => vm.EndTime),
        new Label().SetText("to").SetMargin(new Margin(8, 0)),
        new TimePicker()
            .SetPlaceholder("End")
            .BindSelectedTime(nameof(vm.EndTime), () => vm.EndTime, t => vm.EndTime = t)
            .BindMinTime(nameof(vm.StartTime), () => vm.StartTime)
    )
```

### 12-Hour Format (US)

```csharp
new TimePicker()
    .Set24HourFormat(false)
    .SetDisplayFormat("h:mm tt")
    .SetMinuteIncrement(30)
    .BindSelectedTime(nameof(vm.Time), () => vm.Time, t => vm.Time = t)
```

### Alarm Time Picker

```csharp
new TimePicker()
    .SetPlaceholder("Wake up time")
    .SetSelectedTime(new TimeOnly(7, 0))
    .SetMinuteIncrement(5)
    .Set24HourFormat(true)
    .BindSelectedTime(nameof(vm.AlarmTime), () => vm.AlarmTime, t => vm.AlarmTime = t)
```

### Styled Time Picker

```csharp
new TimePicker()
    .SetPlaceholder("Select time...")
    .SetBackground(new Color(50, 50, 50))
    .SetCornerRadius(8)
    .SetTextColor(Colors.White)
    .SetSelectorBackground(new Color(35, 35, 35))
    .SetSelectedBackground(Colors.Blue)
    .SetHoverBackground(new Color(60, 60, 60))
```

### Form with Time Picker

```csharp
new Grid()
    .AddColumn(Column.Auto)
    .AddColumn(Column.Star, 1)
    .AddRow(Row.Auto)
    .AddChild(new Label().SetText("Reminder:"), row: 0, column: 0)
    .AddChild(
        new TimePicker()
            .SetPlaceholder("Set time")
            .SetMinuteIncrement(15)
            .BindSelectedTime(nameof(vm.ReminderTime), () => vm.ReminderTime, t => vm.ReminderTime = t)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch),
        row: 0, column: 1)
```

### Business Hours Only

```csharp
new TimePicker()
    .SetPlaceholder("Appointment time")
    .SetMinTime(new TimeOnly(8, 0))
    .SetMaxTime(new TimeOnly(18, 0))
    .SetMinuteIncrement(30)
    .Set24HourFormat(true)
    .BindSelectedTime(nameof(vm.AppointmentTime), () => vm.AppointmentTime, t => vm.AppointmentTime = t)
```
