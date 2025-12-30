---
layout: default
title: API Reference
nav_order: 4
description: "PlusUi API Reference - Controls, layouts, styling, and services"
permalink: /api
---

# API Reference
{: .fs-9 }

Complete reference for PlusUi controls, layouts, and services.
{: .fs-6 .fw-300 }

---

## Base Classes

### UiElement

The base class for all UI components.

```csharp
public abstract class UiElement
```

**Properties (via Set/Bind methods):**

| Property | Type | Description |
|:---------|:-----|:------------|
| `Background` | `IBackground` | Element background |
| `Margin` | `Margin` | Outer spacing |
| `HorizontalAlignment` | `HorizontalAlignment` | Horizontal positioning |
| `VerticalAlignment` | `VerticalAlignment` | Vertical positioning |
| `CornerRadius` | `float` | Corner rounding |
| `DesiredWidth` | `float?` | Explicit width |
| `DesiredHeight` | `float?` | Explicit height |
| `Debug` | `bool` | Show debug bounds |

**Example:**

```csharp
new Solid()
    .SetBackground(new SolidColorBackground(SKColors.Blue))
    .SetMargin(new Margin(10))
    .SetCornerRadius(8)
    .SetDesiredWidth(100)
    .SetDesiredHeight(50)
```

---

### UiTextElement

Base class for elements that display text. Inherits from `UiElement`.

```csharp
public abstract class UiTextElement : UiElement
```

**Additional Properties:**

| Property | Type | Description |
|:---------|:-----|:------------|
| `Text` | `string` | Text content |
| `TextSize` | `float` | Font size |
| `TextColor` | `SKColor` | Text color |
| `HorizontalTextAlignment` | `HorizontalTextAlignment` | Text alignment |

**Example:**

```csharp
new Label()
    .SetText("Hello World")
    .SetTextSize(24)
    .SetTextColor(SKColors.White)
    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
```

---

### UiLayoutElement

Base class for container elements. Inherits from `UiElement`.

```csharp
public abstract class UiLayoutElement : UiElement
```

**Properties:**

| Property | Type | Description |
|:---------|:-----|:------------|
| `Children` | `IEnumerable<UiElement>` | Child elements |

---

### UiPageElement

Base class for pages. Inherits from `UiLayoutElement`.

```csharp
public abstract class UiPageElement : UiLayoutElement
```

**Methods:**

| Method | Description |
|:-------|:------------|
| `Build()` | Override to define UI |
| `ConfigurePageStyles(Style)` | Override for page-scoped styles |
| `Appearing()` | Called when page becomes visible |
| `Disappearing()` | Called when page is hidden |

**Example:**

```csharp
public class MyPage(MyViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(...);
    }

    public override void Appearing()
    {
        base.Appearing();
        // Initialize data
    }
}
```

---

## Layout Controls

### VStack

Arranges children vertically.

```csharp
new VStack(
    new Label().SetText("First"),
    new Label().SetText("Second"),
    new Label().SetText("Third")
)
```

### HStack

Arranges children horizontally.

```csharp
new HStack(
    new Button().SetText("Left"),
    new Solid(), // Spacer
    new Button().SetText("Right")
)
```

### Grid

Arranges children in a grid layout.

**Properties:**

| Property | Type | Description |
|:---------|:-----|:------------|
| `Rows` | `string` | Row definitions (e.g., "Auto,*,100") |
| `Columns` | `string` | Column definitions |
| `RowSpacing` | `float` | Space between rows |
| `ColumnSpacing` | `float` | Space between columns |

**Example:**

```csharp
new Grid()
    .SetRows("Auto,*,Auto")
    .SetColumns("*,*")
    .SetRowSpacing(10)
    .SetColumnSpacing(10)
    .SetChildren(
        new Label().SetText("Header").SetGridRow(0).SetGridColumnSpan(2),
        new Solid().SetGridRow(1).SetGridColumn(0),
        new Solid().SetGridRow(1).SetGridColumn(1),
        new Button().SetText("Footer").SetGridRow(2).SetGridColumnSpan(2)
    )
```

**Grid Size Definitions:**

| Value | Meaning |
|:------|:--------|
| `Auto` | Size to content |
| `*` | Fill remaining space |
| `2*` | Fill 2x remaining space |
| `100` | Fixed 100 pixels |

### ScrollView

Provides scrollable content area.

```csharp
new ScrollView(
    new VStack(
        // Many children...
    )
)
```

### Border

Wraps content with a border.

**Properties:**

| Property | Type | Description |
|:---------|:-----|:------------|
| `BorderColor` | `SKColor` | Border color |
| `BorderWidth` | `float` | Border thickness |
| `BorderStyle` | `BorderStyle` | Solid, Dashed, Dotted |

```csharp
new Border(
    new Label().SetText("Bordered content")
)
.SetBorderColor(SKColors.Gray)
.SetBorderWidth(2)
.SetBorderStyle(BorderStyle.Solid)
.SetCornerRadius(8)
```

---

## Basic Controls

### Label

Displays text.

```csharp
// Static text
new Label()
    .SetText("Hello World")
    .SetTextSize(18)
    .SetTextColor(SKColors.White)

// Bound text
new Label()
    .BindText(nameof(vm.Message), () => vm.Message)
```

### Button

Clickable button with text.

**Properties:**

| Property | Type | Description |
|:---------|:-----|:------------|
| `Text` | `string` | Button label |
| `Padding` | `Margin` | Inner spacing |
| `Command` | `ICommand` | Click command |

```csharp
new Button()
    .SetText("Click Me")
    .SetPadding(new Margin(20, 10))
    .SetBackground(new SolidColorBackground(SKColors.DodgerBlue))
    .SetCornerRadius(8)
    .SetCommand(vm.MyCommand)
```

### Entry

Text input field.

**Properties:**

| Property | Type | Description |
|:---------|:-----|:------------|
| `Text` | `string` | Input value |
| `Padding` | `Margin` | Inner spacing |

```csharp
new Entry()
    .BindText(
        nameof(vm.Input),
        () => vm.Input,
        value => vm.Input = value
    )
    .SetPadding(new Margin(10, 5))
```

### Checkbox

Toggle checkbox.

**Properties:**

| Property | Type | Description |
|:---------|:-----|:------------|
| `IsChecked` | `bool` | Checked state |

```csharp
new Checkbox()
    .BindIsChecked(
        nameof(vm.IsEnabled),
        () => vm.IsEnabled,
        value => vm.IsEnabled = value
    )
```

### Image

Displays an image.

**Properties:**

| Property | Type | Description |
|:---------|:-----|:------------|
| `ImageSource` | `string` | Image path or URL |
| `Aspect` | `Aspect` | Sizing behavior |

**Aspect Values:**

| Value | Description |
|:------|:------------|
| `Fill` | Stretch to fill |
| `AspectFit` | Scale to fit, maintain ratio |
| `AspectFill` | Scale to fill, maintain ratio, clip |

```csharp
new Image()
    .SetImageSource("logo.png")
    .SetAspect(Aspect.AspectFit)
    .SetDesiredWidth(100)
    .SetDesiredHeight(100)
```

### Solid

A solid colored rectangle. Useful as a spacer.

```csharp
// Colored rectangle
new Solid()
    .SetBackground(new SolidColorBackground(SKColors.Red))
    .SetDesiredHeight(50)

// Transparent spacer
new Solid()
    .SetDesiredWidth(20)
```

---

## Backgrounds

### SolidColorBackground

Single solid color.

```csharp
new SolidColorBackground(SKColors.Blue)
new SolidColorBackground(new SKColor(255, 128, 0))
```

### LinearGradient

Linear color gradient.

```csharp
new LinearGradient(
    startColor: SKColors.Blue,
    endColor: SKColors.Purple,
    angle: 45  // degrees
)
```

### RadialGradient

Radial color gradient.

```csharp
new RadialGradient(
    centerColor: SKColors.White,
    edgeColor: SKColors.Gray
)
```

### MultiStopGradient

Gradient with multiple color stops.

```csharp
new MultiStopGradient(
    angle: 90,
    new GradientStop(0.0f, SKColors.Red),
    new GradientStop(0.5f, SKColors.Yellow),
    new GradientStop(1.0f, SKColors.Green)
)
```

---

## Commands

### SyncCommand

Synchronous command execution.

```csharp
public ICommand SaveCommand { get; }

public MyViewModel()
{
    SaveCommand = new SyncCommand(() => Save());

    // With parameter
    SaveCommand = new SyncCommand<string>(text => Save(text));
}
```

### AsyncCommand

Asynchronous command execution.

```csharp
public ICommand LoadCommand { get; }

public MyViewModel()
{
    LoadCommand = new AsyncCommand(async () => await LoadDataAsync());
}
```

---

## Services

### INavigationService

Page navigation.

```csharp
public class MyViewModel(INavigationService navigation) : ViewModelBase
{
    public void GoToDetails()
    {
        navigation.NavigateTo<DetailsPage>();
    }

    public void GoBack()
    {
        navigation.GoBack();
    }
}
```

### IPopupService

Modal popups.

```csharp
public class MyViewModel(IPopupService popups) : ViewModelBase
{
    public void ShowDialog()
    {
        popups.ShowPopup<MyPopup>();
    }

    public void CloseDialog()
    {
        popups.ClosePopup();
    }
}
```

### FontRegistryService

Custom font registration.

```csharp
public void ConfigureApp(HostApplicationBuilder builder)
{
    var fonts = builder.Services.GetService<FontRegistryService>();
    fonts.RegisterFont("CustomFont", "fonts/custom.ttf");
}
```

---

## ViewModels

### ViewModelBase

Base class for ViewModels with property change notifications.

```csharp
public class MyViewModel : ViewModelBase
{
    public string Name
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public int Count
    {
        get => field;
        set => SetProperty(ref field, value);
    }
}
```

---

## Alignment & Margins

### HorizontalAlignment

| Value | Description |
|:------|:------------|
| `Left` | Align to left |
| `Center` | Center horizontally |
| `Right` | Align to right |
| `Stretch` | Fill available width |

### VerticalAlignment

| Value | Description |
|:------|:------------|
| `Top` | Align to top |
| `Center` | Center vertically |
| `Bottom` | Align to bottom |
| `Stretch` | Fill available height |

### Margin

Spacing around elements.

```csharp
// Uniform
new Margin(10)

// Horizontal, Vertical
new Margin(20, 10)

// Left, Top, Right, Bottom
new Margin(10, 20, 10, 20)
```

---

## Data Binding Pattern

Every property follows the Set/Bind pattern:

```csharp
// Set - static value
element.SetPropertyName(value)

// Bind - dynamic value
element.BindPropertyName(
    propertyName,      // For change notification
    () => getter,      // Value getter
    value => setter    // Optional: value setter (for two-way)
)
```

**Example:**

```csharp
// One-way binding
new Label()
    .BindText(nameof(vm.Title), () => vm.Title)

// Two-way binding
new Entry()
    .BindText(
        nameof(vm.Input),
        () => vm.Input,
        value => vm.Input = value
    )
```
