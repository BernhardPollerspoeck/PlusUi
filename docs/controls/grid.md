---
title: Grid
layout: default
parent: Controls
nav_order: 130
---

# Grid

A flexible grid layout that arranges child elements in rows and columns. Supports absolute, proportional (star), and auto-sizing for rows and columns with row/column spanning.

---

## Basic Usage

```csharp
// 2x2 grid with fixed sizes
new Grid()
    .AddRow(Row.Absolute, 50)
    .AddRow(Row.Auto)
    .AddColumn(Column.Star, 1)
    .AddColumn(Column.Star, 2)
    .AddChild(new Label().SetText("Top Left"), row: 0, column: 0)
    .AddChild(new Button().SetText("Top Right"), row: 0, column: 1)

// Simple two-column form
new Grid()
    .AddColumn(Column.Auto)
    .AddColumn(Column.Star, 1)
    .AddRow(Row.Auto)
    .AddRow(Row.Auto)
    .AddChild(new Label().SetText("Name:"), row: 0, column: 0)
    .AddChild(new Entry(), row: 0, column: 1)
    .AddChild(new Label().SetText("Email:"), row: 1, column: 0)
    .AddChild(new Entry(), row: 1, column: 1)
```

---

## Grid-Specific Methods

### Row Definition Methods

| Method | Description |
|:-------|:------------|
| `AddRow(Row, int)` | Adds a row with sizing mode and size value |
| `AddRow(int)` | Adds an absolute-sized row |
| `AddBoundRow(name, Row, getter)` | Adds a bound row definition |
| `AddBoundRow(name, getter)` | Adds a bound absolute row |

### Column Definition Methods

| Method | Description |
|:-------|:------------|
| `AddColumn(Column, float)` | Adds a column with sizing mode and size value |
| `AddColumn(float)` | Adds an absolute-sized column |
| `AddBoundColumn(name, Column, getter)` | Adds a bound column definition |
| `AddBoundColumn(name, getter)` | Adds a bound absolute column |

### Child Methods

| Method | Description |
|:-------|:------------|
| `AddChild(element, row, column)` | Adds child at position |
| `AddChild(element, row, column, rowSpan, columnSpan)` | Adds child with spanning |
| `RemoveChild(element)` | Removes a child |
| `ClearChildren()` | Removes all children |

### Row Sizing Values

| Value | Description |
|:------|:------------|
| `Row.Absolute` | Fixed pixel height |
| `Row.Star` | Proportional height (divides remaining space) |
| `Row.Auto` | Sizes to fit content |

### Column Sizing Values

| Value | Description |
|:------|:------------|
| `Column.Absolute` | Fixed pixel width |
| `Column.Star` | Proportional width (divides remaining space) |
| `Column.Auto` | Sizes to fit content |

{: .note }
> Grid defaults to `HorizontalAlignment.Stretch` and `VerticalAlignment.Stretch`. If no rows/columns are defined, a single Auto row/column is added automatically.

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
| `SetHorizontalAlignment(HorizontalAlignment)` | Sets horizontal alignment (default: Stretch) |
| `BindHorizontalAlignment(name, getter)` | Binds horizontal alignment |
| `SetVerticalAlignment(VerticalAlignment)` | Sets vertical alignment (default: Stretch) |
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
> Grid has `AccessibilityRole.Container` by default.

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

### Form Layout

```csharp
new Grid()
    .AddColumn(Column.Auto)      // Labels column
    .AddColumn(Column.Star, 1)   // Inputs column (stretches)
    .AddRow(Row.Auto)
    .AddRow(Row.Auto)
    .AddRow(Row.Auto)
    .AddChild(new Label().SetText("Name:"), row: 0, column: 0)
    .AddChild(new Entry().SetPlaceholder("Full name"), row: 0, column: 1)
    .AddChild(new Label().SetText("Email:"), row: 1, column: 0)
    .AddChild(new Entry().SetPlaceholder("email@example.com"), row: 1, column: 1)
    .AddChild(new Label().SetText("Phone:"), row: 2, column: 0)
    .AddChild(new Entry().SetPlaceholder("+1 234 567 8900"), row: 2, column: 1)
```

### Dashboard Layout with Star Sizing

```csharp
new Grid()
    .AddRow(Row.Absolute, 60)   // Header (fixed 60px)
    .AddRow(Row.Star, 1)        // Content (remaining space)
    .AddRow(Row.Absolute, 40)   // Footer (fixed 40px)
    .AddColumn(Column.Star, 1)  // Single column
    .AddChild(header, row: 0, column: 0)
    .AddChild(content, row: 1, column: 0)
    .AddChild(footer, row: 2, column: 0)
```

### Sidebar Layout

```csharp
new Grid()
    .AddRow(Row.Star, 1)         // Full height
    .AddColumn(Column.Absolute, 250) // Sidebar (250px)
    .AddColumn(Column.Star, 1)       // Main content
    .AddChild(sidebar, row: 0, column: 0)
    .AddChild(mainContent, row: 0, column: 1)
```

### Row and Column Spanning

```csharp
new Grid()
    .AddRow(Row.Auto)
    .AddRow(Row.Star, 1)
    .AddRow(Row.Auto)
    .AddColumn(Column.Star, 1)
    .AddColumn(Column.Star, 1)
    // Header spans 2 columns
    .AddChild(header, row: 0, column: 0, rowSpan: 1, columnSpan: 2)
    // Sidebar spans 2 rows
    .AddChild(sidebar, row: 1, column: 0, rowSpan: 2, columnSpan: 1)
    // Content area
    .AddChild(content, row: 1, column: 1)
    // Status bar
    .AddChild(statusBar, row: 2, column: 1)
```

### Proportional Star Columns

```csharp
new Grid()
    .AddRow(Row.Auto)
    .AddColumn(Column.Star, 1)  // 1 part (25%)
    .AddColumn(Column.Star, 2)  // 2 parts (50%)
    .AddColumn(Column.Star, 1)  // 1 part (25%)
    .AddChild(new Label().SetText("25%"), row: 0, column: 0)
    .AddChild(new Label().SetText("50%"), row: 0, column: 1)
    .AddChild(new Label().SetText("25%"), row: 0, column: 2)
```

### Data-bound Column Width

```csharp
new Grid()
    .AddBoundColumn(nameof(vm.SidebarWidth), Column.Absolute, () => vm.SidebarWidth)
    .AddColumn(Column.Star, 1)
    .AddRow(Row.Star, 1)
    .AddChild(sidebar, row: 0, column: 0)
    .AddChild(content, row: 0, column: 1)
```
