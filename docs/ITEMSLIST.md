# ItemsList Control

The `ItemsList<T>` control is a powerful, virtualized list component for displaying collections of data efficiently.

## Features

- **Data Binding**: Bind to any `IEnumerable<T>` collection via the `ItemsSource` property
- **Template Support**: Use `ItemTemplate` to define how each item is rendered
- **Scrolling**: Supports both horizontal and vertical scrolling orientations
- **Virtualization**: Automatically virtualizes items for efficient rendering of large datasets
- **Observable Collections**: Supports `ObservableCollection` for automatic UI updates

## Basic Usage

### Vertical List

```csharp
new ItemsList<MyItemModel>()
    .SetItemsSource(myItems)
    .SetItemTemplate(item => 
        new Label().SetText(item.Name)
    )
    .SetOrientation(Orientation.Vertical)
    .SetCanScrollVertically(true)
```

### Horizontal List

```csharp
new ItemsList<MyItemModel>()
    .SetItemsSource(myItems)
    .SetItemTemplate(item => 
        new Label().SetText(item.Name)
    )
    .SetOrientation(Orientation.Horizontal)
    .SetCanScrollHorizontally(true)
```

### With Data Binding

```csharp
new ItemsList<MyItemModel>()
    .BindItemsSource(nameof(viewModel.Items), () => viewModel.Items)
    .SetItemTemplate(item => 
        new VStack(
            new Label().SetText(item.Title),
            new Label().SetText(item.Description)
        )
    )
```

### Complex Item Templates

```csharp
new ItemsList<MyItemModel>()
    .SetItemsSource(myItems)
    .SetItemTemplate(item => 
        new HStack(
            new Image()
                .SetImageSource(item.ImageUrl)
                .SetDesiredWidth(50)
                .SetDesiredHeight(50),
            new VStack(
                new Label()
                    .SetText(item.Title)
                    .SetTextSize(16),
                new Label()
                    .SetText(item.Description)
                    .SetTextSize(12)
            )
        )
        .SetBackgroundColor(new SKColor(40, 40, 40))
        .SetMargin(new Margin(5))
        .SetCornerRadius(5)
    )
    .SetOrientation(Orientation.Vertical)
    .SetCanScrollVertically(true)
```

## Properties

### ItemsSource
- **Type**: `IEnumerable<T>`
- **Description**: The collection of items to display
- **Methods**: `SetItemsSource()`, `BindItemsSource()`

### ItemTemplate
- **Type**: `Func<T, UiElement>`
- **Description**: A function that generates a UI element for each item
- **Methods**: `SetItemTemplate()`, `BindItemTemplate()`

### Orientation
- **Type**: `Orientation` (Vertical or Horizontal)
- **Description**: The direction items are laid out
- **Default**: `Orientation.Vertical`
- **Methods**: `SetOrientation()`, `BindOrientation()`

### CanScrollVertically
- **Type**: `bool`
- **Description**: Enables/disables vertical scrolling
- **Default**: `true`
- **Methods**: `SetCanScrollVertically()`, `BindCanScrollVertically()`

### CanScrollHorizontally
- **Type**: `bool`
- **Description**: Enables/disables horizontal scrolling
- **Default**: `false`
- **Methods**: `SetCanScrollHorizontally()`, `BindCanScrollHorizontally()`

### VerticalOffset
- **Type**: `float`
- **Description**: Current vertical scroll position
- **Methods**: `SetVerticalOffset()`, `BindVerticalOffset()`

### HorizontalOffset
- **Type**: `float`
- **Description**: Current horizontal scroll position
- **Methods**: `SetHorizontalOffset()`, `BindHorizontalOffset()`

## Virtualization

The `ItemsList` control implements virtualization automatically. This means:

1. Only visible items are rendered
2. Items are recycled as you scroll
3. Large datasets (thousands of items) can be handled efficiently
4. Memory usage is optimized

The virtualization is transparent - you don't need to configure anything. Just provide your data and template.

## Observable Collections

For dynamic collections that change over time, use `ObservableCollection<T>`:

```csharp
public ObservableCollection<MyItem> Items { get; set; } = new();

// In your page
new ItemsList<MyItem>()
    .BindItemsSource(nameof(viewModel.Items), () => viewModel.Items)
    .SetItemTemplate(item => new Label().SetText(item.Name))
```

When items are added or removed from the collection, the UI updates automatically.

## Styling

You can style individual items in the template or the entire list:

```csharp
new ItemsList<MyItem>()
    .SetItemsSource(items)
    .SetItemTemplate(item => 
        new Label()
            .SetText(item.Name)
            .SetTextColor(SKColors.White)
            .SetBackgroundColor(new SKColor(50, 50, 50))
            .SetMargin(new Margin(2))
    )
    .SetBackgroundColor(new SKColor(20, 20, 20))
    .SetCornerRadius(10)
    .SetMargin(new Margin(10))
```

## Performance Tips

1. Keep item templates simple for best scrolling performance
2. Avoid complex calculations in the template function
3. Use fixed-size items when possible for more accurate virtualization
4. The control estimates item sizes from the first item, so ensure it's representative

## Example

See `/samples/Sandbox/Pages/ItemsListDemo/` for a complete working example demonstrating:
- Vertical scrolling list with 100+ items
- Horizontal scrolling list
- Dynamic add/remove operations
- Custom item templates
- Observable collection integration
