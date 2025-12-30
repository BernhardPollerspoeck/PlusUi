---
title: Best Practices
layout: default
parent: Guides
nav_order: 3
---

# Best Practices

Guidelines for writing maintainable, performant PlusUi applications.

---

## MVVM Pattern

### Do: Separate Concerns

```csharp
// ViewModel - Logic and data
public partial class ProductViewModel(IProductService productService) : ObservableObject
{
    [ObservableProperty]
    private string _productName = "";

    [ObservableProperty]
    private decimal _price;

    [RelayCommand]
    private async Task LoadProduct(int id)
    {
        var product = await productService.GetProductAsync(id);
        ProductName = product.Name;
        Price = product.Price;
    }
}

// Page - Only UI
public class ProductPage(ProductViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label().BindText(nameof(vm.ProductName), () => vm.ProductName),
            new Label().BindText(nameof(vm.Price), () => $"${vm.Price:F2}")
        );
    }
}
```

### Don't: Put Logic in Pages

```csharp
// BAD - Logic in page
public class ProductPage(IProductService service) : UiPageElement(null)
{
    private Product? _product;

    protected override UiElement Build()
    {
        // Don't load data here!
        _product = service.GetProduct(1).Result; // Blocking call!

        return new Label().SetText(_product?.Name);
    }
}
```

---

## Data Binding

### Do: Use Proper Bindings

```csharp
// One-way binding (ViewModel -> View)
new Label()
    .BindText(nameof(vm.Title), () => vm.Title)

// Two-way binding (ViewModel <-> View)
new Entry()
    .BindText(nameof(vm.SearchQuery), () => vm.SearchQuery, value => vm.SearchQuery = value)

// Command binding
new Button()
    .SetCommand(vm.SaveCommand)
```

### Don't: Forget Property Names

```csharp
// BAD - Binding won't update when property changes
new Label()
    .SetText(vm.Title)  // Static text, never updates!

// GOOD
new Label()
    .BindText(nameof(vm.Title), () => vm.Title)
```

---

## Layout Performance

### Do: Use Appropriate Containers

```csharp
// For simple vertical lists
new VStack(item1, item2, item3)

// For simple horizontal lists
new HStack(item1, item2, item3)

// For grid layouts
new Grid()
    .AddColumn(Column.Star)
    .AddColumn(Column.Auto)
    .AddRow(Row.Auto)

// For large scrollable lists
new ItemsList<T>()
    .SetItemsSource(items)
    .SetItemTemplate(item => new Label().SetText(item.Name))
```

### Don't: Over-Nest Layouts

```csharp
// BAD - Too many nested containers
new VStack(
    new HStack(
        new VStack(
            new HStack(
                new Label().SetText("Deep!")
            )
        )
    )
)

// GOOD - Use Grid for complex layouts
new Grid()
    .AddColumn(Column.Star)
    .AddColumn(Column.Auto)
    .AddRow(Row.Auto)
    .AddChild(row: 0, column: 0, child: new Label().SetText("Simple!"))
```

---

## Control Creation

### Do: Create Controls in Build()

```csharp
public class MyPage(MyViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        // Controls are created fresh each time Build() is called
        return new VStack(
            new Label().BindText(nameof(vm.Title), () => vm.Title),
            new Button().SetText("Click").SetCommand(vm.ClickCommand)
        );
    }
}
```

### Don't: Store Control References

```csharp
// BAD - Don't cache controls
public class MyPage(MyViewModel vm) : UiPageElement(vm)
{
    private Label? _titleLabel; // Don't do this!

    protected override UiElement Build()
    {
        _titleLabel = new Label().SetText("Title");
        return _titleLabel;
    }

    public void UpdateTitle(string title)
    {
        _titleLabel?.SetText(title); // Don't do this!
    }
}
```

---

## Custom Controls

### Do: Use Partial Classes with GenerateShadowMethods

```csharp
using PlusUi.core.Attributes;

[GenerateShadowMethods]
public partial class MyCustomButton : UiElement
{
    protected internal override bool IsFocusable => true;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Button;

    // Properties with internal setter, public Set method
    internal string Text { get; set; } = "";
    public MyCustomButton SetText(string text)
    {
        Text = text;
        InvalidateMeasure();
        return this;
    }

    // Don't forget Bind method for every Set method!
    public MyCustomButton BindText(string propertyName, Func<string> getter)
    {
        RegisterBinding(propertyName, () => Text = getter());
        return this;
    }

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        // Custom rendering
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // Return desired size
        return new Size(100, 40);
    }
}
```

### Don't: Duplicate Base Properties

```csharp
// BAD - Background already exists in UiElement!
public partial class MyControl : UiElement
{
    internal Color BackgroundColor { get; set; } // Don't redefine!

    public MyControl()
    {
        // GOOD - Use inherited SetBackground
        SetBackground(new SolidColorBackground(Colors.Blue));
    }
}
```

---

## Async Operations

### Do: Use Async Commands

```csharp
public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _service.GetDataAsync();
            Items = data;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

### Don't: Block the UI Thread

```csharp
// BAD - Blocks UI
[RelayCommand]
private void LoadData()
{
    var data = _service.GetDataAsync().Result; // Blocking!
    Items = data;
}
```

---

## Memory Management

### Do: Dispose Resources

```csharp
public class MyPage : UiPageElement, IDisposable
{
    private readonly Timer _timer;

    public MyPage(MyViewModel vm) : base(vm)
    {
        _timer = new Timer(OnTick, null, 0, 1000);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
```

### Do: Use Page Lifecycle

```csharp
public class MyPage(MyViewModel vm) : UiPageElement(vm)
{
    public override void Appearing()
    {
        base.Appearing();
        // Start animations, load data
    }

    public override void Disappearing()
    {
        base.Disappearing();
        // Stop animations, save state
    }
}
```

---

## Margins and Padding

### Do: Be Consistent

```csharp
// Define constants
public static class Spacing
{
    public static readonly Margin Small = new(4);
    public static readonly Margin Medium = new(8);
    public static readonly Margin Large = new(16);
    public static readonly Margin Section = new(24);
}

// Use them consistently
new VStack(
    new Label().SetMargin(Spacing.Medium),
    new Button().SetMargin(Spacing.Medium)
)
```

### Margin vs Padding

```csharp
// Margin - Space OUTSIDE the element
new Button()
    .SetMargin(new Margin(10))  // Space around the button

// Padding - Space INSIDE the element
new Button()
    .SetPadding(new Margin(10)) // Space inside the button (around text)
```

---

## Summary Checklist

- [ ] Use MVVM: Logic in ViewModel, UI in Page
- [ ] Always bind with property names for updates
- [ ] Keep layout hierarchy shallow
- [ ] Create controls in Build(), don't cache references
- [ ] Use async/await, never .Result or .Wait()
- [ ] Clean up resources in Dispose or Disappearing
- [ ] Define spacing constants for consistency
- [ ] Partial class + `[GenerateShadowMethods]` for custom controls
- [ ] Set + Bind method pairs for all properties
