---
title: Migration Guide
layout: default
nav_order: 7
---

# Migration Guide

Guides for migrating from other UI frameworks to PlusUi.

---

## From WPF

### XAML to C# Fluent API

**WPF XAML:**
```xml
<StackPanel Orientation="Vertical">
    <TextBlock Text="Hello" FontSize="24" Foreground="White"/>
    <Button Content="Click Me" Command="{Binding MyCommand}"/>
</StackPanel>
```

**PlusUi:**
```csharp
new VStack(
    new Label()
        .SetText("Hello")
        .SetTextSize(24)
        .SetTextColor(Colors.White),
    new Button()
        .SetText("Click Me")
        .SetCommand(vm.MyCommand)
)
```

### Layout Mapping

| WPF | PlusUi |
|:----|:-------|
| `StackPanel Orientation="Vertical"` | `VStack` |
| `StackPanel Orientation="Horizontal"` | `HStack` |
| `Grid` | `Grid` |
| `UniformGrid` | `UniformGrid` |
| `Border` | `Border` |
| `ScrollViewer` | `ScrollView` |

### Control Mapping

| WPF | PlusUi |
|:----|:-------|
| `TextBlock` | `Label` |
| `TextBox` | `Entry` |
| `Button` | `Button` |
| `CheckBox` | `Checkbox` |
| `RadioButton` | `RadioButton` |
| `ComboBox` | `ComboBox<T>` |
| `ListBox` | `ItemsList<T>` |
| `DataGrid` | `DataGrid<T>` |
| `Image` | `Image` |

### Data Binding

**WPF:**
```xml
<TextBlock Text="{Binding Name}"/>
<TextBox Text="{Binding Name, Mode=TwoWay}"/>
```

**PlusUi:**
```csharp
new Label()
    .BindText(nameof(vm.Name), () => vm.Name)

new Entry()
    .BindText(nameof(vm.Name), () => vm.Name, v => vm.Name = v)
```

### Styles

**WPF:**
```xml
<Button.Style>
    <Style TargetType="Button">
        <Setter Property="Background" Value="Blue"/>
        <Setter Property="Foreground" Value="White"/>
    </Style>
</Button.Style>
```

**PlusUi:**
```csharp
new Button()
    .SetBackground(new SolidColorBackground(Colors.Blue))
    .SetTextColor(Colors.White)

// Or use a reusable style method
button.ApplyPrimaryButtonStyle()

public static T ApplyPrimaryButtonStyle<T>(this T button) where T : Button
{
    return button
        .SetBackground(new SolidColorBackground(Colors.Blue))
        .SetTextColor(Colors.White)
        .SetCornerRadius(8);
}
```

---

## From MAUI / Xamarin

### XAML to C# Fluent API

**MAUI XAML:**
```xml
<VerticalStackLayout>
    <Label Text="Welcome" FontSize="32"/>
    <Entry Placeholder="Enter name" Text="{Binding Name}"/>
    <Button Text="Submit" Command="{Binding SubmitCommand}"/>
</VerticalStackLayout>
```

**PlusUi:**
```csharp
new VStack(
    new Label()
        .SetText("Welcome")
        .SetTextSize(32),
    new Entry()
        .SetPlaceholder("Enter name")
        .BindText(nameof(vm.Name), () => vm.Name, v => vm.Name = v),
    new Button()
        .SetText("Submit")
        .SetCommand(vm.SubmitCommand)
)
```

### Layout Mapping

| MAUI | PlusUi |
|:-----|:-------|
| `VerticalStackLayout` | `VStack` |
| `HorizontalStackLayout` | `HStack` |
| `Grid` | `Grid` |
| `FlexLayout` | `HStack` with `.SetWrap(true)` |
| `ScrollView` | `ScrollView` |
| `Border` | `Border` |
| `Frame` | `Border` |

### Control Mapping

| MAUI | PlusUi |
|:-----|:-------|
| `Label` | `Label` |
| `Entry` | `Entry` |
| `Button` | `Button` |
| `CheckBox` | `Checkbox` |
| `RadioButton` | `RadioButton` |
| `Picker` | `ComboBox<T>` |
| `DatePicker` | `DatePicker` |
| `TimePicker` | `TimePicker` |
| `Slider` | `Slider` |
| `CollectionView` | `ItemsList<T>` |
| `Image` | `Image` |

### Dependency Injection

**MAUI:**
```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainViewModel>();
        return builder.Build();
    }
}
```

**PlusUi:**
```csharp
public class App : IAppConfiguration
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<MainPage>();
        services.AddTransient<MainViewModel>();
    }
}
```

---

## From Avalonia

### XAML to C# Fluent API

**Avalonia XAML:**
```xml
<StackPanel>
    <TextBlock Text="Title" FontSize="28" Foreground="White"/>
    <TextBox Watermark="Search..." Text="{Binding Query}"/>
</StackPanel>
```

**PlusUi:**
```csharp
new VStack(
    new Label()
        .SetText("Title")
        .SetTextSize(28)
        .SetTextColor(Colors.White),
    new Entry()
        .SetPlaceholder("Search...")
        .BindText(nameof(vm.Query), () => vm.Query, v => vm.Query = v)
)
```

### Layout Mapping

| Avalonia | PlusUi |
|:---------|:-------|
| `StackPanel` (Vertical) | `VStack` |
| `StackPanel` (Horizontal) | `HStack` |
| `Grid` | `Grid` |
| `UniformGrid` | `UniformGrid` |
| `Border` | `Border` |
| `ScrollViewer` | `ScrollView` |
| `WrapPanel` | `HStack` / `VStack` with `.SetWrap(true)` |

### ReactiveUI to MVVM Toolkit

**Avalonia with ReactiveUI:**
```csharp
public class MainViewModel : ReactiveObject
{
    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public ReactiveCommand<Unit, Unit> SubmitCommand { get; }
}
```

**PlusUi with CommunityToolkit.Mvvm:**
```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [RelayCommand]
    private void Submit()
    {
        // Command logic
    }
}
```

---

## Common Migration Steps

### 1. Project Setup

1. Create a new .NET 10 project
2. Add PlusUi NuGet packages:
   ```bash
   dotnet add package PlusUi.core
   dotnet add package PlusUi.desktop  # or other platform-specific package
   ```
3. Add CommunityToolkit.Mvvm for MVVM support:
   ```bash
   dotnet add package CommunityToolkit.Mvvm
   ```

### 2. Convert ViewModels

Most ViewModels can be migrated with minimal changes. Replace your base class and property notification:

```csharp
// Before (INotifyPropertyChanged manual implementation)
public class MyViewModel : INotifyPropertyChanged
{
    private string _name;
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }
}

// After (CommunityToolkit.Mvvm)
public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name;
}
```

### 3. Convert Views/Pages

Convert XAML views to C# fluent API. For each XAML element:

1. Identify the equivalent PlusUi control
2. Convert properties to fluent method calls
3. Convert bindings to `Bind*` methods
4. Wrap in layout containers as needed

### 4. Update Navigation

```csharp
// Inject INavigationService
public MainViewModel(INavigationService navigation)
{
    _navigation = navigation;
}

// Navigate to pages
_navigation.NavigateTo<DetailsPage>();

// Navigate back
_navigation.GoBack();
```

### 5. Handle Platform Differences

For platform-specific code:

```csharp
if (PlatformInfo.IsWindows)
{
    // Windows-specific behavior
}
else if (PlatformInfo.IsAndroid)
{
    // Android-specific behavior
}
```

---

## Tips for Successful Migration

1. **Start with ViewModels** - They often need the least changes
2. **Convert one page at a time** - Don't try to migrate everything at once
3. **Use the fluent API** - It's more concise than XAML in most cases
4. **Leverage type safety** - Generic controls like `ItemsList<T>` catch errors at compile time
5. **Test frequently** - Run the app after each page conversion
