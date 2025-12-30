---
layout: default
title: Getting Started
nav_order: 2
description: "Get started with PlusUi - Installation, setup, and your first app"
permalink: /getting-started
---

# Getting Started
{: .fs-9 }

Create your first PlusUi application in minutes.
{: .fs-6 .fw-300 }

---

## Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 10.0 SDK** or later
- An IDE (Visual Studio 2022, VS Code, or Rider)

### Platform Requirements

| Platform | Minimum Version |
|:---------|:----------------|
| Windows | Windows 11 or later |
| macOS | No specific version |
| Linux | No specific version |
| Android | Android 8.0 (API 26) |
| iOS | iOS 11 |

---

## Installation

### 1. Create a New Project

```bash
# Create a new console application
dotnet new console -n MyPlusUiApp
cd MyPlusUiApp
```

### 2. Add NuGet Packages

For **Desktop** applications (Windows, macOS, Linux):

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.desktop
```

For **iOS**:

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.ios
```

For **Android**:

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.droid
```

{: .note }
> All platforms require the `PlusUi.core` package as the foundation.

---

## Your First App

Let's create a simple counter application to understand the basics.

### Step 1: Create the App Configuration

Create a file named `App.cs`:

```csharp
using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace MyPlusUiApp;

public class App : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration config)
    {
        config.Title = "My First PlusUi App";
        config.Size = new SizeI(800, 600);
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        // Register your pages with their ViewModels
        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
```

### Step 2: Create the ViewModel

Create a file named `MainPageViewModel.cs`:

```csharp
using PlusUi.core;
using System.Windows.Input;

namespace MyPlusUiApp;

public class MainPageViewModel : ViewModelBase
{
    public int Count
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public ICommand IncrementCommand { get; }

    public MainPageViewModel()
    {
        IncrementCommand = new SyncCommand(() => Count++);
    }
}
```

### Step 3: Create the Page

Create a file named `MainPage.cs`:

```csharp
using PlusUi.core;
using SkiaSharp;

namespace MyPlusUiApp;

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        SetBackground(new SolidColorBackground(new SKColor(30, 30, 30)));

        return new VStack(
            // Title
            new Label()
                .SetText("Counter App")
                .SetTextSize(32)
                .SetTextColor(SKColors.White)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                .SetMargin(new Margin(0, 20)),

            // Counter display with data binding
            new Label()
                .BindText(nameof(vm.Count), () => $"Count: {vm.Count}")
                .SetTextSize(24)
                .SetTextColor(SKColors.LightGray)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                .SetMargin(new Margin(0, 10)),

            // Increment button
            new Button()
                .SetText("Click Me!")
                .SetPadding(new Margin(20, 10))
                .SetBackground(new SolidColorBackground(SKColors.DodgerBlue))
                .SetCornerRadius(8)
                .SetCommand(vm.IncrementCommand)
                .SetMargin(new Margin(20))

        ).SetHorizontalAlignment(HorizontalAlignment.Center)
         .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
```

### Step 4: Update Program.cs

Replace the contents of `Program.cs`:

```csharp
using PlusUi.desktop;
using MyPlusUiApp;

var app = new PlusUiApp(args);
app.CreateApp(builder => new App());
```

### Step 5: Run Your App

```bash
dotnet run
```

You should see a window with a counter that increments when you click the button.

---

## Understanding the Code

### The MVVM Pattern

PlusUi uses the **Model-View-ViewModel** pattern:

| Component | Purpose | Example |
|:----------|:--------|:--------|
| **Model** | Business logic and data | Your domain classes |
| **View** | UI definition | `MainPage.cs` |
| **ViewModel** | State and commands | `MainPageViewModel.cs` |

### Data Binding

PlusUi supports two-way data binding using `Set*` and `Bind*` methods:

```csharp
// Static value - doesn't change
new Label().SetText("Hello World")

// Bound value - updates when property changes
new Label().BindText(nameof(vm.Name), () => vm.Name)

// Two-way binding (for input controls)
new Entry().BindText(
    nameof(vm.Input),
    () => vm.Input,           // getter
    value => vm.Input = value // setter
)
```

### The Fluent API

Every UI element uses method chaining for configuration:

```csharp
new Button()
    .SetText("Submit")           // Set the button text
    .SetPadding(new Margin(15))  // Add padding
    .SetCornerRadius(8)          // Round corners
    .SetBackground(...)          // Set background
    .SetCommand(vm.SubmitCmd)    // Bind command
```

{: .tip }
> Every `Set*` method has a corresponding `Bind*` method for data binding.

---

## Next Steps

Now that you have a running app, explore more:

- [API Reference]({{ site.baseurl }}/api) - Full API documentation
- [Platform Support]({{ site.baseurl }}/platform-support) - Detailed platform information
- [Migration Guide]({{ site.baseurl }}/migration) - Coming from other frameworks?

---

## Sample Projects

Explore the `samples/Sandbox` project in the repository for more examples:

```bash
git clone https://github.com/BernhardPollerspoeck/PlusUi.git
cd PlusUi/samples/Sandbox
dotnet run
```
