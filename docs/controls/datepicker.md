---
title: DatePicker
layout: default
parent: Controls
nav_order: 200
---

# DatePicker

A date picker control with calendar popup for date selection. Supports date range restrictions, custom display formats, keyboard input, and two-way binding.

---

## Basic Usage

```csharp
// Simple date picker
new DatePicker()
    .SetPlaceholder("Select date...")

// With constraints and formatting
new DatePicker()
    .SetPlaceholder("Birth date")
    .SetDisplayFormat("dd.MM.yyyy")
    .SetMinDate(new DateOnly(1900, 1, 1))
    .SetMaxDate(DateOnly.FromDateTime(DateTime.Today))
    .BindSelectedDate(nameof(vm.BirthDate), () => vm.BirthDate, v => vm.BirthDate = v)

// Pre-selected date
new DatePicker()
    .SetSelectedDate(DateOnly.FromDateTime(DateTime.Today))
```

---

## DatePicker-Specific Methods

### Date Methods

| Method | Description |
|:-------|:------------|
| `SetSelectedDate(DateOnly?)` | Sets selected date |
| `BindSelectedDate(name, getter)` | One-way binds selected date |
| `BindSelectedDate(name, getter, setter)` | Two-way binds selected date |
| `SetMinDate(DateOnly?)` | Sets minimum selectable date |
| `BindMinDate(name, getter)` | Binds minimum date |
| `SetMaxDate(DateOnly?)` | Sets maximum selectable date |
| `BindMaxDate(name, getter)` | Binds maximum date |

### Display Methods

| Method | Description |
|:-------|:------------|
| `SetDisplayFormat(string)` | Sets date format string (default: "dd.MM.yyyy") |
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

### Calendar Appearance Methods

| Method | Description |
|:-------|:------------|
| `SetCalendarBackground(SKColor)` | Sets calendar popup background |
| `BindCalendarBackground(name, getter)` | Binds calendar background |
| `SetHoverBackground(SKColor)` | Sets day hover color |
| `BindHoverBackground(name, getter)` | Binds hover color |
| `SetSelectedBackground(SKColor)` | Sets selected day background |
| `BindSelectedBackground(name, getter)` | Binds selected background |
| `SetTodayBorderColor(SKColor)` | Sets today's border color |
| `BindTodayBorderColor(name, getter)` | Binds today border color |

### Behavior Methods

| Method | Description |
|:-------|:------------|
| `SetWeekStart(DayOfWeekStart)` | Sets first day of week (default: Monday) |
| `BindWeekStart(name, getter)` | Binds week start |
| `SetShowTodayButton(bool)` | Shows/hides "Today" button (default: true) |
| `BindShowTodayButton(name, getter)` | Binds show today button |
| `SetIsOpen(bool)` | Opens or closes calendar |
| `BindIsOpen(name, getter)` | Binds open state |

### DayOfWeekStart Values

| Value | Description |
|:------|:------------|
| `DayOfWeekStart.Monday` | Week starts on Monday (default) |
| `DayOfWeekStart.Sunday` | Week starts on Sunday |

{: .note }
> DatePicker has a default size of 200x40 pixels. Keyboard navigation: Arrow keys to navigate days, Enter/Space to select, Escape to close. Direct keyboard input is supported (e.g., typing "25.12.2024").

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
> DatePicker has `AccessibilityRole.DatePicker` by default. It automatically reports the selected date and Expanded/HasPopup traits.

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

### Birth Date Picker

```csharp
new DatePicker()
    .SetPlaceholder("Date of birth")
    .SetDisplayFormat("dd MMMM yyyy")
    .SetMinDate(new DateOnly(1900, 1, 1))
    .SetMaxDate(DateOnly.FromDateTime(DateTime.Today))
    .BindSelectedDate(nameof(vm.BirthDate), () => vm.BirthDate, d => vm.BirthDate = d)
    .SetAccessibilityLabel("Birth date")
```

### Appointment Date Range

```csharp
new HStack()
    .AddChildren(
        new DatePicker()
            .SetPlaceholder("Start date")
            .BindSelectedDate(nameof(vm.StartDate), () => vm.StartDate, d => vm.StartDate = d)
            .BindMaxDate(nameof(vm.EndDate), () => vm.EndDate),
        new Label().SetText("to").SetMargin(new Margin(8, 0)),
        new DatePicker()
            .SetPlaceholder("End date")
            .BindSelectedDate(nameof(vm.EndDate), () => vm.EndDate, d => vm.EndDate = d)
            .BindMinDate(nameof(vm.StartDate), () => vm.StartDate)
    )
```

### Future Dates Only

```csharp
new DatePicker()
    .SetPlaceholder("Delivery date")
    .SetMinDate(DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
    .SetDisplayFormat("ddd, dd MMM yyyy")
    .BindSelectedDate(nameof(vm.DeliveryDate), () => vm.DeliveryDate, d => vm.DeliveryDate = d)
```

### Styled Calendar

```csharp
new DatePicker()
    .SetPlaceholder("Select date...")
    .SetBackground(new Color(50, 50, 50))
    .SetCornerRadius(8)
    .SetTextColor(Colors.White)
    .SetCalendarBackground(new Color(35, 35, 35))
    .SetSelectedBackground(Colors.Blue)
    .SetTodayBorderColor(Colors.Green)
```

### US Format with Sunday Start

```csharp
new DatePicker()
    .SetDisplayFormat("MM/dd/yyyy")
    .SetWeekStart(DayOfWeekStart.Sunday)
    .BindSelectedDate(nameof(vm.Date), () => vm.Date, d => vm.Date = d)
```

### Form with Date Picker

```csharp
new Grid()
    .AddColumn(Column.Auto)
    .AddColumn(Column.Star, 1)
    .AddRow(Row.Auto)
    .AddChild(new Label().SetText("Event Date:"), row: 0, column: 0)
    .AddChild(
        new DatePicker()
            .SetPlaceholder("Choose a date")
            .BindSelectedDate(nameof(vm.EventDate), () => vm.EventDate, d => vm.EventDate = d)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch),
        row: 0, column: 1)
```
