---
title: Project Setup
layout: default
parent: Guides
nav_order: 1
---

# Project Setup

This guide covers how to properly structure and configure a PlusUi application.

---

## The App Class

Every PlusUi application needs an `App` class implementing `IAppConfiguration`:

```csharp
public class App : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        // Window settings
    }

    public void ConfigureApp(IPlusUiAppBuilder builder)
    {
        // Register pages, ViewModels, services
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        // Return the first page to display
    }
}
```

---

## Window Configuration

Configure your application window in `ConfigureWindow`:

```csharp
public void ConfigureWindow(PlusUiConfiguration configuration)
{
    // Basic settings
    configuration.Title = "My App";
    configuration.Size = new SizeI(1200, 800);

    // Navigation
    configuration.EnableNavigationStack = true;  // Enable back navigation
    configuration.PreservePageState = true;      // Keep page state when navigating

    // Accessibility
    configuration.EnableHighContrastSupport = true;
    configuration.RespectReducedMotion = true;

    // Advanced window settings (Desktop only)
    // configuration.IsWindowTransparent = true;
    // configuration.WindowBorder = WindowBorder.Hidden;
    // configuration.WindowState = WindowState.Maximized;
}
```

---

## Registering Pages

Register all your pages in `ConfigureApp`:

```csharp
public void ConfigureApp(IPlusUiAppBuilder builder)
{
    // Register pages with their ViewModels
    builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
    builder.AddPage<SettingsPage>().WithViewModel<SettingsPageViewModel>();
    builder.AddPage<DetailPage>().WithViewModel<DetailPageViewModel>();

    // Register popups
    builder.AddPopup<ConfirmPopup>().WithViewModel<ConfirmPopupViewModel>();

    // Apply a style
    builder.StylePlusUi<DefaultStyle>();

    // Register custom services
    builder.Services.AddSingleton<IMyService, MyService>();
}
```

---

## Dependency Injection

PlusUi uses Microsoft.Extensions.DependencyInjection. Services are injected via primary constructors:

### In ViewModels

```csharp
public partial class MainPageViewModel(
    INavigationService navigationService,
    IMyService myService) : ObservableObject
{
    [RelayCommand]
    private void GoToSettings()
    {
        navigationService.NavigateTo<SettingsPage>();
    }
}
```

### In Pages

```csharp
public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        // Access vm.SomeProperty
    }
}
```

---

## Navigation

### Basic Navigation

```csharp
// Inject INavigationService in your ViewModel
public partial class MyViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void NavigateToDetails()
    {
        navigationService.NavigateTo<DetailPage>();
    }

    [RelayCommand]
    private void GoBack()
    {
        navigationService.GoBack();
    }
}
```

### Navigation with Parameters

```csharp
// Navigate with a parameter
navigationService.NavigateTo<DetailPage>(selectedItem);

// In DetailPage ViewModel, implement INavigationAware
public partial class DetailPageViewModel : ObservableObject, INavigationAware
{
    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is MyItem item)
        {
            CurrentItem = item;
        }
    }
}
```

---

## Popups

### Registering Popups

```csharp
builder.AddPopup<ConfirmPopup>().WithViewModel<ConfirmPopupViewModel>();
```

### Showing Popups

```csharp
public partial class MyViewModel(IPopupService popupService) : ObservableObject
{
    [RelayCommand]
    private async Task ShowConfirmation()
    {
        var result = await popupService.ShowPopup<ConfirmPopup, bool>();
        if (result)
        {
            // User confirmed
        }
    }
}
```

---

## Recommended Project Structure

```
MyApp/
├── App.cs                    # App configuration
├── Program.cs                # Entry point
├── Styles/
│   └── MyCustomStyle.cs      # Custom styles
├── Services/
│   ├── IMyService.cs
│   └── MyService.cs
├── Pages/
│   ├── Main/
│   │   ├── MainPage.cs
│   │   └── MainPageViewModel.cs
│   ├── Settings/
│   │   ├── SettingsPage.cs
│   │   └── SettingsPageViewModel.cs
│   └── ...
├── Popups/
│   ├── ConfirmPopup.cs
│   └── ConfirmPopupViewModel.cs
└── Controls/
    └── MyCustomControl.cs    # Custom controls
```

---

## Next Steps

- [Theming](theming.html) - Customize your app's appearance
- [Best Practices](best-practices.html) - Write better PlusUi code
