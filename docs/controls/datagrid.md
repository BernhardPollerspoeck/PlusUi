---
title: DataGrid
layout: default
parent: Controls
nav_order: 250
---

# DataGrid

A high-performance virtualized data grid control for displaying tabular data. Supports multiple column types, selection, alternating row styles, sorting, and custom row styling. Uses virtualization for efficient rendering of large datasets.

---

## Basic Usage

```csharp
// Simple data grid
new DataGrid<Person>()
    .AddColumn(new DataGridTextColumn<Person>("Name", p => p.Name))
    .AddColumn(new DataGridTextColumn<Person>("Email", p => p.Email))
    .AddColumn(new DataGridTextColumn<Person>("Age", p => p.Age.ToString()))
    .SetItemsSource(people)

// With selection and styling
new DataGrid<Order>()
    .AddColumn(new DataGridTextColumn<Order>("Order #", o => o.Id))
    .AddColumn(new DataGridTextColumn<Order>("Customer", o => o.CustomerName))
    .AddColumn(new DataGridTextColumn<Order>("Total", o => o.Total.ToString("C")))
    .SetItemsSource(orders)
    .SetSelectionMode(SelectionMode.Single)
    .SetAlternatingRowStyles(true)
    .BindSelectedItem(nameof(vm.SelectedOrder), () => vm.SelectedOrder, o => vm.SelectedOrder = o)
```

---

## DataGrid-Specific Methods

### Column Methods

| Method | Description |
|:-------|:------------|
| `AddColumn(DataGridColumn<T>)` | Adds a column to the grid |
| `RemoveColumn(DataGridColumn<T>)` | Removes a column |

### Data Methods

| Method | Description |
|:-------|:------------|
| `SetItemsSource(IEnumerable<T>?)` | Sets the data source |
| `BindItemsSource(name, getter)` | Binds data source (supports INotifyCollectionChanged) |

### Selection Methods

| Method | Description |
|:-------|:------------|
| `SetSelectionMode(SelectionMode)` | Sets selection mode (default: Single) |
| `SetSelectedItem(T?)` | Sets selected item |
| `BindSelectedItem(name, getter, setter)` | Two-way binds selected item |
| `SelectItem(T)` | Programmatically selects an item |
| `DeselectItem(T)` | Programmatically deselects an item |
| `ClearSelection()` | Clears all selections |

### SelectionMode Values

| Value | Description |
|:------|:------------|
| `SelectionMode.None` | No selection allowed |
| `SelectionMode.Single` | Single item selection (default) |
| `SelectionMode.Multiple` | Multiple item selection |

### Layout Methods

| Method | Description |
|:-------|:------------|
| `SetRowHeight(float)` | Sets data row height (default: 32) |
| `SetHeaderHeight(float)` | Sets header row height (default: 36) |
| `SetCellPadding(Margin)` | Sets cell padding (default: 8, 4) |
| `BindCellPadding(name, getter)` | Binds cell padding |

### Grid Lines

| Method | Description |
|:-------|:------------|
| `SetShowRowSeparators(bool)` | Shows/hides row lines (default: true) |
| `BindShowRowSeparators(name, getter)` | Binds row separators |
| `SetShowColumnSeparators(bool)` | Shows/hides column lines (default: true) |
| `BindShowColumnSeparators(name, getter)` | Binds column separators |
| `SetSeparatorColor(SKColor)` | Sets separator line color |
| `BindSeparatorColor(name, getter)` | Binds separator color |
| `SetSeparatorThickness(float)` | Sets separator line thickness (default: 1) |
| `BindSeparatorThickness(name, getter)` | Binds thickness |
| `SetHeaderSeparatorColor(SKColor)` | Sets header separator color |
| `BindHeaderSeparatorColor(name, getter)` | Binds header separator color |

### Row Styling

| Method | Description |
|:-------|:------------|
| `SetAlternatingRowStyles(bool)` | Enables alternating row styles (default: true) |
| `SetEvenRowStyle(IBackground?, Color?)` | Sets style for even rows |
| `SetOddRowStyle(IBackground?, Color?)` | Sets style for odd rows |
| `SetRowStyleCallback(Func<T, int, DataGridRowStyle>)` | Sets custom row styling function |

### Scrolling

| Method | Description |
|:-------|:------------|
| `SetScrollOffset(float)` | Sets vertical scroll position |
| `SetHorizontalScrollOffset(float)` | Sets horizontal scroll position |
| `SetShowVerticalScrollbar(bool)` | Shows/hides vertical scrollbar (default: true) |
| `SetShowHorizontalScrollbar(bool)` | Shows/hides horizontal scrollbar (default: true) |

{: .note }
> DataGrid has `AccessibilityRole.Grid` by default. It uses UI virtualization for efficient rendering of large datasets.

---

## Column Types

### DataGridTextColumn

Displays text content.

```csharp
new DataGridTextColumn<Person>("Name", p => p.Name)
    .SetWidth(DataGridColumnWidth.Star(2))
```

### DataGridCheckboxColumn

Displays a checkbox with optional two-way binding.

```csharp
new DataGridCheckboxColumn<Task>("Done", t => t.IsComplete, (t, v) => t.IsComplete = v)
```

### DataGridButtonColumn

Displays a clickable button.

```csharp
new DataGridButtonColumn<Order>("Actions", "Delete", o => vm.DeleteCommand.Execute(o))
```

### DataGridLinkColumn

Displays a clickable link.

```csharp
new DataGridLinkColumn<Document>("Link", d => d.Url, d => d.Title)
```

### DataGridImageColumn

Displays an image.

```csharp
new DataGridImageColumn<Product>("Image", p => p.ThumbnailUrl)
```

### DataGridComboBoxColumn

Displays a dropdown selector.

```csharp
new DataGridComboBoxColumn<Order, Status>("Status", o => o.Status, Enum.GetValues<Status>(), (o, v) => o.Status = v)
```

### DataGridProgressColumn

Displays a progress bar.

```csharp
new DataGridProgressColumn<Download>("Progress", d => d.Progress)
```

### DataGridSliderColumn

Displays a slider.

```csharp
new DataGridSliderColumn<Setting>("Value", s => s.Value, (s, v) => s.Value = v, 0, 100)
```

### DataGridTemplateColumn

Displays custom content via template.

```csharp
new DataGridTemplateColumn<Person>("Actions", p =>
    new HStack(
        new Button().SetText("Edit").SetCommand(vm.EditCommand).SetCommandParameter(p),
        new Button().SetText("Delete").SetCommand(vm.DeleteCommand).SetCommandParameter(p)
    )
)
```

---

## Column Width

| Type | Description |
|:-----|:------------|
| `DataGridColumnWidth.Absolute(float)` | Fixed pixel width |
| `DataGridColumnWidth.Star(float)` | Proportional width (divides remaining space) |
| `DataGridColumnWidth.Auto` | Size to fit content |

```csharp
new DataGridTextColumn<Person>("Name", p => p.Name)
    .SetWidth(DataGridColumnWidth.Star(2))  // Takes 2 parts of remaining space
```

---

## Examples

### Employee Directory

```csharp
new DataGrid<Employee>()
    .AddColumn(new DataGridImageColumn<Employee>("", e => e.PhotoUrl)
        .SetWidth(DataGridColumnWidth.Absolute(50)))
    .AddColumn(new DataGridTextColumn<Employee>("Name", e => e.Name)
        .SetWidth(DataGridColumnWidth.Star(2)))
    .AddColumn(new DataGridTextColumn<Employee>("Department", e => e.Department)
        .SetWidth(DataGridColumnWidth.Star(1)))
    .AddColumn(new DataGridTextColumn<Employee>("Email", e => e.Email)
        .SetWidth(DataGridColumnWidth.Star(2)))
    .SetItemsSource(employees)
    .SetSelectionMode(SelectionMode.Single)
    .BindSelectedItem(nameof(vm.SelectedEmployee), () => vm.SelectedEmployee, e => vm.SelectedEmployee = e)
```

### Task List with Checkboxes

```csharp
new DataGrid<TodoItem>()
    .AddColumn(new DataGridCheckboxColumn<TodoItem>("Done", t => t.IsComplete, (t, v) => t.IsComplete = v)
        .SetWidth(DataGridColumnWidth.Absolute(50)))
    .AddColumn(new DataGridTextColumn<TodoItem>("Task", t => t.Title)
        .SetWidth(DataGridColumnWidth.Star(1)))
    .AddColumn(new DataGridTextColumn<TodoItem>("Due", t => t.DueDate?.ToString("d") ?? "-")
        .SetWidth(DataGridColumnWidth.Absolute(100)))
    .SetItemsSource(todos)
```

### Custom Row Styling

```csharp
new DataGrid<Order>()
    .AddColumn(new DataGridTextColumn<Order>("Order #", o => o.Id))
    .AddColumn(new DataGridTextColumn<Order>("Status", o => o.Status))
    .AddColumn(new DataGridTextColumn<Order>("Total", o => o.Total.ToString("C")))
    .SetItemsSource(orders)
    .SetRowStyleCallback((order, index) => order.Status switch
    {
        "Pending" => new DataGridRowStyle(new SolidColorBackground(new Color(255, 200, 100, 50)), null),
        "Complete" => new DataGridRowStyle(new SolidColorBackground(new Color(100, 255, 100, 50)), null),
        "Cancelled" => new DataGridRowStyle(new SolidColorBackground(new Color(255, 100, 100, 50)), null),
        _ => new DataGridRowStyle()
    })
```

### With Actions Column

```csharp
new DataGrid<User>()
    .AddColumn(new DataGridTextColumn<User>("Username", u => u.Username))
    .AddColumn(new DataGridTextColumn<User>("Email", u => u.Email))
    .AddColumn(new DataGridTemplateColumn<User>("Actions", u =>
        new HStack()
            .AddChildren(
                new Button().SetText("Edit").SetCommand(vm.EditUserCommand).SetCommandParameter(u),
                new Button().SetText("Delete").SetCommand(vm.DeleteUserCommand).SetCommandParameter(u)
            )
    ).SetWidth(DataGridColumnWidth.Absolute(150)))
    .SetItemsSource(users)
```

### Styled Grid

```csharp
new DataGrid<Product>()
    .AddColumn(new DataGridTextColumn<Product>("Name", p => p.Name))
    .AddColumn(new DataGridTextColumn<Product>("Price", p => p.Price.ToString("C")))
    .SetItemsSource(products)
    .SetRowHeight(40)
    .SetHeaderHeight(48)
    .SetCellPadding(new Margin(12, 8))
    .SetSeparatorColor(new SKColor(80, 80, 80))
    .SetAlternatingRowStyles(true)
    .SetEvenRowStyle(new SolidColorBackground(new Color(40, 40, 40)), null)
    .SetOddRowStyle(new SolidColorBackground(new Color(50, 50, 50)), null)
```
