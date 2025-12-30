---
title: UniformGrid
layout: default
parent: Controls
nav_order: 135
---

# UniformGrid

A grid layout where all cells have equal size. Children are automatically placed in cells from left to right, top to bottom. Simplifies grid layouts when all cells should be the same size.

---

## Basic Usage

```csharp
// Calculator-style layout with 4 columns
new UniformGrid()
    .SetColumns(4)
    .AddChildren(
        new Button().SetText("7"), new Button().SetText("8"), new Button().SetText("9"), new Button().SetText("/"),
        new Button().SetText("4"), new Button().SetText("5"), new Button().SetText("6"), new Button().SetText("*"),
        new Button().SetText("1"), new Button().SetText("2"), new Button().SetText("3"), new Button().SetText("-"),
        new Button().SetText("0"), new Button().SetText("."), new Button().SetText("="), new Button().SetText("+")
    )

// Fixed 3x3 grid
new UniformGrid()
    .SetRows(3)
    .SetColumns(3)
    .AddChildren(children)

// Auto-calculated square-ish grid
new UniformGrid()
    .AddChildren(item1, item2, item3, item4, item5, item6)
```

---

## UniformGrid-Specific Methods

| Method | Description |
|:-------|:------------|
| `SetRows(int)` | Sets number of rows (calculated if not set) |
| `BindRows(name, getter)` | Binds row count |
| `SetColumns(int)` | Sets number of columns (calculated if not set) |
| `BindColumns(name, getter)` | Binds column count |

{: .note }
> If only Rows is set, Columns is calculated from child count. If only Columns is set, Rows is calculated. If neither is set, a square-ish grid is created.

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
> UniformGrid has `AccessibilityRole.Grid` by default.

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

### Calculator Keypad

```csharp
new UniformGrid()
    .SetColumns(4)
    .SetDesiredSize(new Size(300, 400))
    .AddChildren(
        new Button().SetText("7"), new Button().SetText("8"), new Button().SetText("9"), new Button().SetText("/"),
        new Button().SetText("4"), new Button().SetText("5"), new Button().SetText("6"), new Button().SetText("*"),
        new Button().SetText("1"), new Button().SetText("2"), new Button().SetText("3"), new Button().SetText("-"),
        new Button().SetText("0"), new Button().SetText("."), new Button().SetText("="), new Button().SetText("+")
    )
```

### Image Gallery

```csharp
new UniformGrid()
    .SetColumns(3)
    .AddChildren(
        images.Select(img =>
            new Image()
                .SetImageSource(img.Path)
                .SetAspect(Aspect.AspectFill)
        ).ToArray()
    )
```

### Icon Grid

```csharp
new UniformGrid()
    .SetColumns(4)
    .SetRows(2)
    .SetDesiredSize(new Size(200, 100))
    .AddChildren(
        CreateIcon("home"),
        CreateIcon("search"),
        CreateIcon("settings"),
        CreateIcon("profile"),
        CreateIcon("notifications"),
        CreateIcon("messages"),
        CreateIcon("favorites"),
        CreateIcon("help")
    )
```

### Tic-Tac-Toe Board

```csharp
new UniformGrid()
    .SetRows(3)
    .SetColumns(3)
    .SetDesiredSize(new Size(300, 300))
    .SetBackground(new Color(40, 40, 40))
    .AddChildren(
        cells.Select(cell =>
            new Button()
                .BindText(nameof(cell.Value), () => cell.Value)
                .SetCommand(cell.ClickCommand)
        ).ToArray()
    )
```

### Auto-Layout Grid

```csharp
// Automatically creates 3x2 grid for 6 items
new UniformGrid()
    .AddChildren(
        new Label().SetText("Item 1"),
        new Label().SetText("Item 2"),
        new Label().SetText("Item 3"),
        new Label().SetText("Item 4"),
        new Label().SetText("Item 5"),
        new Label().SetText("Item 6")
    )
```

### Dynamic Columns

```csharp
new UniformGrid()
    .BindColumns(nameof(vm.ColumnCount), () => vm.ColumnCount)
    .AddChildren(tiles)
```
