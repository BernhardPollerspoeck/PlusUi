---
title: ComboBox
layout: default
parent: Controls
nav_order: 190
---

# ComboBox

A combo box (dropdown) control that allows users to select an item from a list. Supports data binding, custom display formatting, and keyboard navigation.

---

## Basic Usage

```csharp
// Simple combo box with string items
new ComboBox<string>()
    .SetItemsSource(new[] { "Option 1", "Option 2", "Option 3" })
    .SetPlaceholder("Select an option...")

// Combo box with custom objects and display function
new ComboBox<Person>()
    .SetItemsSource(people)
    .SetDisplayFunc(person => person.Name)
    .BindSelectedItem(nameof(vm.SelectedPerson), () => vm.SelectedPerson, p => vm.SelectedPerson = p)

// Bound to observable collection
new ComboBox<Country>()
    .BindItemsSource(nameof(vm.Countries), () => vm.Countries)
    .SetDisplayFunc(c => $"{c.Flag} {c.Name}")
    .BindSelectedItem(nameof(vm.SelectedCountry), () => vm.SelectedCountry, c => vm.SelectedCountry = c)
```

---

## ComboBox-Specific Methods

### Data Methods

| Method | Description |
|:-------|:------------|
| `SetItemsSource(IEnumerable<T>?)` | Sets the items collection |
| `BindItemsSource(name, getter)` | Binds items collection (supports INotifyCollectionChanged) |
| `SetSelectedItem(T?)` | Sets selected item |
| `BindSelectedItem(name, getter, setter)` | Two-way binds selected item |
| `SetSelectedIndex(int)` | Sets selected item by index |
| `BindSelectedIndex(name, getter, setter)` | Two-way binds selected index |
| `SetDisplayFunc(Func<T, string>)` | Sets display text formatter |
| `BindDisplayFunc(name, getter)` | Binds display formatter |

### Appearance Methods

| Method | Description |
|:-------|:------------|
| `SetPlaceholder(string)` | Sets placeholder text when nothing selected |
| `BindPlaceholder(name, getter)` | Binds placeholder |
| `SetPlaceholderColor(SKColor)` | Sets placeholder text color (default: gray) |
| `BindPlaceholderColor(name, getter)` | Binds placeholder color |
| `SetTextColor(SKColor)` | Sets text color (default: white) |
| `BindTextColor(name, getter)` | Binds text color |
| `SetTextSize(float)` | Sets text size (default: 14) |
| `BindTextSize(name, getter)` | Binds text size |
| `SetFontFamily(string)` | Sets font family |
| `BindFontFamily(name, getter)` | Binds font family |
| `SetPadding(Margin)` | Sets internal padding (default: 12, 8) |
| `BindPadding(name, getter)` | Binds padding |

### Dropdown Appearance

| Method | Description |
|:-------|:------------|
| `SetDropdownBackground(SKColor)` | Sets dropdown background color |
| `BindDropdownBackground(name, getter)` | Binds dropdown background |
| `SetHoverBackground(SKColor)` | Sets hover highlight color |
| `BindHoverBackground(name, getter)` | Binds hover background |

### State Methods

| Method | Description |
|:-------|:------------|
| `SetIsOpen(bool)` | Opens or closes dropdown |
| `BindIsOpen(name, getter)` | Binds open state |

{: .note }
> ComboBox has a default size of 200x40 pixels. The dropdown opens downward if space permits, otherwise upward. Keyboard navigation: Arrow keys to navigate, Enter/Space to select, Escape to close.

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
> ComboBox has `AccessibilityRole.ComboBox` by default. It automatically reports the selected value and Expanded/HasPopup traits.

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

### Country Selector

```csharp
new ComboBox<Country>()
    .SetItemsSource(countries)
    .SetDisplayFunc(c => $"{c.Flag} {c.Name}")
    .SetPlaceholder("Select country...")
    .BindSelectedItem(nameof(vm.Country), () => vm.Country, c => vm.Country = c)
    .SetDesiredWidth(250)
```

### Enum Selection

```csharp
new ComboBox<Priority>()
    .SetItemsSource(Enum.GetValues<Priority>())
    .SetDisplayFunc(p => p.ToString())
    .SetSelectedItem(Priority.Normal)
    .BindSelectedItem(nameof(vm.Priority), () => vm.Priority, p => vm.Priority = p)
```

### Styled ComboBox

```csharp
new ComboBox<string>()
    .SetItemsSource(new[] { "Small", "Medium", "Large", "Extra Large" })
    .SetPlaceholder("Select size...")
    .SetBackground(new Color(50, 50, 50))
    .SetCornerRadius(8)
    .SetTextColor(Colors.White)
    .SetDropdownBackground(new Color(40, 40, 40))
    .SetHoverBackground(new Color(70, 70, 70))
```

### Form with ComboBox

```csharp
new Grid()
    .AddColumn(Column.Auto)
    .AddColumn(Column.Star, 1)
    .AddRow(Row.Auto)
    .AddChild(new Label().SetText("Category:"), row: 0, column: 0)
    .AddChild(
        new ComboBox<Category>()
            .BindItemsSource(nameof(vm.Categories), () => vm.Categories)
            .SetDisplayFunc(c => c.Name)
            .BindSelectedItem(nameof(vm.SelectedCategory), () => vm.SelectedCategory, c => vm.SelectedCategory = c)
            .SetHorizontalAlignment(HorizontalAlignment.Stretch),
        row: 0, column: 1)
```

### Index-based Selection

```csharp
new ComboBox<string>()
    .SetItemsSource(options)
    .SetSelectedIndex(0)  // Select first item
    .BindSelectedIndex(nameof(vm.SelectedIndex), () => vm.SelectedIndex, i => vm.SelectedIndex = i)
```

### Observable Collection

```csharp
// Items added/removed will automatically update the dropdown
new ComboBox<TodoItem>()
    .BindItemsSource(nameof(vm.TodoItems), () => vm.TodoItems)  // ObservableCollection<TodoItem>
    .SetDisplayFunc(item => item.Title)
    .BindSelectedItem(nameof(vm.SelectedTodo), () => vm.SelectedTodo, t => vm.SelectedTodo = t)
```
