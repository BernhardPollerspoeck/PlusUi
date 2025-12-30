---
layout: default
title: Migration Guide
nav_order: 5
description: "Migrate to PlusUi from MAUI, Avalonia, WPF, or Xamarin"
permalink: /migration
---

# Migration Guide
{: .fs-9 }

Moving to PlusUi from other UI frameworks.
{: .fs-6 .fw-300 }

---

## Overview

PlusUi takes a different approach than traditional .NET UI frameworks. Instead of XAML, you write pure C# with a fluent API. This guide helps you understand the key differences and how to translate your existing knowledge.

---

## From MAUI / Xamarin.Forms

### Key Differences

| MAUI / Xamarin | PlusUi |
|:---------------|:-------|
| XAML markup | Pure C# |
| Platform-native controls | SkiaSharp rendering |
| Platform-specific styling | Identical on all platforms |
| ContentPage | UiPageElement |
| ContentView | UserControl |
| BindableProperty | Set/Bind methods |

### Layout Comparison

**MAUI XAML:**
```xml
<StackLayout Orientation="Vertical">
    <Label Text="Hello" FontSize="24" />
    <Button Text="Click" Clicked="OnClicked" />
</StackLayout>
```

**PlusUi C#:**
```csharp
new VStack(
    new Label()
        .SetText("Hello")
        .SetTextSize(24),
    new Button()
        .SetText("Click")
        .SetCommand(vm.ClickCommand)
)
```

### Data Binding Comparison

**MAUI XAML:**
```xml
<Label Text="{Binding Name}" />
<Entry Text="{Binding Input, Mode=TwoWay}" />
```

**PlusUi C#:**
```csharp
new Label()
    .BindText(nameof(vm.Name), () => vm.Name)

new Entry()
    .BindText(nameof(vm.Input), () => vm.Input, v => vm.Input = v)
```

### Page Lifecycle

| MAUI | PlusUi |
|:-----|:-------|
| `OnAppearing()` | `Appearing()` |
| `OnDisappearing()` | `Disappearing()` |

---

## From Avalonia

### Key Differences

| Avalonia | PlusUi |
|:---------|:-------|
| AXAML markup | Pure C# |
| Styled properties | Set/Bind methods |
| Platform-adaptive | Pixel-identical |
| Complex styling system | Simple fluent styling |

### Control Comparison

**Avalonia AXAML:**
```xml
<StackPanel Orientation="Vertical">
    <TextBlock Text="Hello" FontSize="24" />
    <Button Content="Click" Command="{Binding ClickCommand}" />
</StackPanel>
```

**PlusUi C#:**
```csharp
new VStack(
    new Label()
        .SetText("Hello")
        .SetTextSize(24),
    new Button()
        .SetText("Click")
        .SetCommand(vm.ClickCommand)
)
```

### Styling Comparison

**Avalonia:**
```xml
<Style Selector="Button">
    <Setter Property="Background" Value="Blue" />
</Style>
```

**PlusUi:**
```csharp
protected override void ConfigurePageStyles(Style pageStyle)
{
    pageStyle.AddStyle<Button>(b =>
        b.SetBackground(new SolidColorBackground(SKColors.Blue)));
}
```

---

## From WPF

### Key Differences

| WPF | PlusUi |
|:----|:-------|
| Windows-only | Cross-platform |
| XAML | Pure C# |
| DependencyProperty | Simple properties |
| Complex templates | Fluent configuration |

### Layout Comparison

**WPF XAML:**
```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    <TextBlock Grid.Row="0" Text="Header" />
    <Button Grid.Row="1" Content="Click" />
</Grid>
```

**PlusUi C#:**
```csharp
new Grid()
    .SetRows("Auto,*")
    .SetChildren(
        new Label()
            .SetText("Header")
            .SetGridRow(0),
        new Button()
            .SetText("Click")
            .SetGridRow(1)
    )
```

### Command Binding

**WPF:**
```csharp
public ICommand SaveCommand => new RelayCommand(Save);
```

**PlusUi:**
```csharp
public ICommand SaveCommand { get; } = new SyncCommand(() => Save());
// or
public ICommand SaveCommand { get; } = new AsyncCommand(async () => await SaveAsync());
```

---

## From React Native / Flutter

### Conceptual Similarities

If you're coming from declarative UI frameworks like React Native or Flutter, PlusUi will feel familiar:

| Concept | React/Flutter | PlusUi |
|:--------|:--------------|:-------|
| Components | Function/Widget | UiElement classes |
| Props | Props/Parameters | Set* methods |
| State | useState/setState | ViewModel properties |
| Composition | Children | Constructor params |

### Flutter Comparison

**Flutter:**
```dart
Column(
  children: [
    Text('Hello', style: TextStyle(fontSize: 24)),
    ElevatedButton(
      onPressed: _handleClick,
      child: Text('Click'),
    ),
  ],
)
```

**PlusUi:**
```csharp
new VStack(
    new Label()
        .SetText("Hello")
        .SetTextSize(24),
    new Button()
        .SetText("Click")
        .SetCommand(vm.ClickCommand)
)
```

---

## Control Mapping

### Layouts

| Other Frameworks | PlusUi |
|:-----------------|:-------|
| StackPanel / StackLayout (Vertical) | `VStack` |
| StackPanel / StackLayout (Horizontal) | `HStack` |
| Grid | `Grid` |
| ScrollViewer / ScrollView | `ScrollView` |
| Border | `Border` |

### Basic Controls

| Other Frameworks | PlusUi |
|:-----------------|:-------|
| TextBlock / Label | `Label` |
| Button | `Button` |
| TextBox / Entry / TextField | `Entry` |
| CheckBox | `Checkbox` |
| Image | `Image` |
| Rectangle | `Solid` |

### Containers

| Other Frameworks | PlusUi |
|:-----------------|:-------|
| ContentPage / Page | `UiPageElement` |
| ContentView / UserControl | `UserControl` |

---

## ViewModel Migration

### From CommunityToolkit.Mvvm

PlusUi works seamlessly with CommunityToolkit.Mvvm:

```csharp
// This works directly in PlusUi
public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [RelayCommand]
    private void Save() { }
}
```

### From ViewModelBase

PlusUi provides its own `ViewModelBase`:

```csharp
public class MyViewModel : ViewModelBase
{
    public string Name
    {
        get => field;
        set => SetProperty(ref field, value);
    }
}
```

---

## Common Patterns

### Navigation

**MAUI:**
```csharp
await Navigation.PushAsync(new DetailsPage());
```

**PlusUi:**
```csharp
navigationService.NavigateTo<DetailsPage>();
```

### Dependency Injection

PlusUi uses Microsoft.Extensions.Hosting:

```csharp
public void ConfigureApp(HostApplicationBuilder builder)
{
    builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
    builder.Services.AddSingleton<IMyService, MyService>();
}
```

### Primary Constructors

PlusUi embraces modern C# with primary constructors:

```csharp
public class MainPage(MainPageViewModel vm, IMyService service)
    : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        // vm and service are available here
    }
}
```

---

## Step-by-Step Migration

### 1. Create New Project

```bash
dotnet new console -n MyMigratedApp
cd MyMigratedApp
dotnet add package PlusUi.core
dotnet add package PlusUi.desktop
```

### 2. Create App Configuration

```csharp
public class App : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration config)
    {
        config.Title = "My Migrated App";
        config.Size = new SizeI(800, 600);
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        // Register pages and services
        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
```

### 3. Convert Pages One by One

Start with simple pages and progressively convert more complex ones.

### 4. Convert ViewModels

Most ViewModel code can be reused. Just ensure:
- Properties use `SetProperty` for change notification
- Commands use `SyncCommand` or `AsyncCommand`

### 5. Update Styling

Convert styles to fluent method calls or page-scoped styles.

---

## Tips for Success

{: .tip }
> **Start Small:** Convert one simple page first to understand the patterns.

{: .tip }
> **Reuse ViewModels:** Most ViewModel logic transfers directly.

{: .tip }
> **Embrace C#:** No more context-switching between XAML and C#.

{: .tip }
> **Test on Multiple Platforms:** Your UI now looks identical everywhere.

---

## Need Help?

- Join our [Discord community](https://discord.gg/Je3kNpcmqn)
- Check the [API Reference]({{ site.baseurl }}/api)
- Explore the [samples](https://github.com/BernhardPollerspoeck/PlusUi/tree/main/samples)
