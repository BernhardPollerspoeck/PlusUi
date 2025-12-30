---
title: ItemsList
layout: default
parent: Controls
nav_order: 220
---

# ItemsList

A high-performance virtualized list control for displaying large collections of data. Only renders visible items to optimize memory and performance. Supports vertical and horizontal orientations and observable collections.

---

## Basic Usage

```csharp
// Simple vertical list
new ItemsList<Person>()
    .SetItemsSource(people)
    .SetItemTemplate((person, index) =>
        new Label().SetText(person.Name)
    )

// Horizontal list (card carousel)
new ItemsList<Product>()
    .SetItemsSource(products)
    .SetOrientation(Orientation.Horizontal)
    .SetItemTemplate((product, index) =>
        new VStack(
            new Image().SetImageSource(product.ImageUrl),
            new Label().SetText(product.Name)
        )
    )

// Bound to observable collection
new ItemsList<TodoItem>()
    .BindItemsSource(nameof(vm.TodoItems), () => vm.TodoItems)
    .SetItemTemplate((item, index) =>
        new HStack(
            new Checkbox().BindIsChecked(nameof(item.IsComplete), () => item.IsComplete, v => item.IsComplete = v),
            new Label().BindText(nameof(item.Title), () => item.Title)
        )
    )
```

---

## ItemsList-Specific Methods

### Data Methods

| Method | Description |
|:-------|:------------|
| `SetItemsSource(IEnumerable<T>?)` | Sets the data source |
| `BindItemsSource(name, getter)` | Binds data source (supports INotifyCollectionChanged) |
| `SetItemTemplate(Func<T, int, UiElement>)` | Sets the template function for creating item UI |
| `BindItemTemplate(name, getter)` | Binds item template |

### Scroll Methods

| Method | Description |
|:-------|:------------|
| `SetScrollOffset(float)` | Sets scroll position in pixels |
| `BindScrollOffset(name, getter)` | Binds scroll offset |
| `SetScrollFactor(float)` | Sets scroll speed multiplier (default: 1.0) |
| `BindScrollFactor(name, getter)` | Binds scroll factor |

### Layout Methods

| Method | Description |
|:-------|:------------|
| `SetOrientation(Orientation)` | Sets vertical or horizontal orientation (default: Vertical) |
| `BindOrientation(name, getter)` | Binds orientation |

### Orientation Values

| Value | Description |
|:------|:------------|
| `Orientation.Vertical` | Items flow top to bottom (default) |
| `Orientation.Horizontal` | Items flow left to right |

{: .note }
> ItemsList uses UI virtualization - only visible items are rendered, making it efficient for thousands of items. It automatically responds to collection changes when using ObservableCollection.

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
| `SetCornerRadius(float)` | Sets corner radius (content is clipped) |
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
> ItemsList has `AccessibilityRole.List` by default.

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

### Contact List

```csharp
new ItemsList<Contact>()
    .BindItemsSource(nameof(vm.Contacts), () => vm.Contacts)
    .SetItemTemplate((contact, index) =>
        new HStack()
            .SetMargin(new Margin(8))
            .AddChildren(
                new Image()
                    .SetImageSource(contact.AvatarUrl)
                    .SetDesiredSize(new Size(48, 48))
                    .SetCornerRadius(24),
                new VStack()
                    .SetMargin(new Margin(12, 0, 0, 0))
                    .AddChildren(
                        new Label()
                            .SetText(contact.Name)
                            .SetFontWeight(FontWeight.Bold),
                        new Label()
                            .SetText(contact.Email)
                            .SetTextColor(Colors.Gray)
                    )
            )
    )
    .SetVerticalAlignment(VerticalAlignment.Stretch)
```

### Horizontal Card Carousel

```csharp
new ItemsList<Movie>()
    .SetOrientation(Orientation.Horizontal)
    .SetDesiredHeight(280)
    .BindItemsSource(nameof(vm.Movies), () => vm.Movies)
    .SetItemTemplate((movie, index) =>
        new Border()
            .SetDesiredWidth(180)
            .SetMargin(new Margin(8))
            .SetCornerRadius(12)
            .SetBackground(new Color(40, 40, 40))
            .AddChild(
                new VStack()
                    .AddChildren(
                        new Image()
                            .SetImageSource(movie.PosterUrl)
                            .SetDesiredHeight(200)
                            .SetAspect(Aspect.AspectFill),
                        new Label()
                            .SetText(movie.Title)
                            .SetMargin(new Margin(8))
                    )
            )
    )
```

### Todo List with Checkboxes

```csharp
new ItemsList<TodoItem>()
    .BindItemsSource(nameof(vm.Todos), () => vm.Todos)
    .SetItemTemplate((todo, index) =>
        new HStack()
            .SetMargin(new Margin(4, 8))
            .AddChildren(
                new Checkbox()
                    .BindIsChecked(nameof(todo.IsDone), () => todo.IsDone, v => todo.IsDone = v),
                new Label()
                    .BindText(nameof(todo.Title), () => todo.Title)
                    .SetMargin(new Margin(8, 0, 0, 0))
            )
    )
```

### Alternating Row Colors

```csharp
new ItemsList<LogEntry>()
    .SetItemsSource(logs)
    .SetItemTemplate((log, index) =>
        new HStack()
            .SetBackground(index % 2 == 0
                ? new Color(40, 40, 40)
                : new Color(50, 50, 50))
            .SetPadding(new Margin(12, 8))
            .AddChildren(
                new Label().SetText(log.Timestamp.ToString("HH:mm:ss")),
                new Label().SetText(log.Message).SetMargin(new Margin(16, 0, 0, 0))
            )
    )
```

### Observable Collection Updates

```csharp
// In ViewModel
public ObservableCollection<Message> Messages { get; } = new();

// Adding items automatically updates the list
Messages.Add(new Message { Text = "Hello!" });
Messages.RemoveAt(0);
Messages.Clear();

// In View
new ItemsList<Message>()
    .BindItemsSource(nameof(vm.Messages), () => vm.Messages)
    .SetItemTemplate((msg, index) =>
        new Label().SetText(msg.Text)
    )
```

### Large Dataset

```csharp
// Virtualization handles 10,000+ items efficiently
new ItemsList<DataRow>()
    .SetItemsSource(Enumerable.Range(0, 10000)
        .Select(i => new DataRow { Id = i, Name = $"Item {i}" }))
    .SetItemTemplate((row, index) =>
        new Label().SetText($"{row.Id}: {row.Name}")
    )
    .SetVerticalAlignment(VerticalAlignment.Stretch)
```
