---
title: First App
layout: default
parent: Getting Started
nav_order: 2
---

# Creating Your First App

In this guide, you'll create a simple PlusUi desktop application from scratch.

---

## Step 1: Create the Project

```bash
dotnet new console -n MyFirstPlusUiApp
cd MyFirstPlusUiApp
dotnet add package PlusUi.core
dotnet add package PlusUi.desktop
```

---

## Step 2: Create the App Configuration

Create `App.cs` - this is the entry point for your PlusUi application:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace MyFirstPlusUiApp;

public class App : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Title = "My First PlusUi App";
        configuration.Size = new SizeI(800, 600);
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        // Register your pages
        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();

        // Optional: Add a custom style
        // builder.StylePlusUi<MyCustomStyle>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
```

---

## Step 3: Create a ViewModel

Create `MainPageViewModel.cs`:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyFirstPlusUiApp;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _greeting = "Hello, PlusUi!";

    [ObservableProperty]
    private int _clickCount;

    [RelayCommand]
    private void IncrementCounter()
    {
        ClickCount++;
        Greeting = $"Clicked {ClickCount} times!";
    }
}
```

{: .tip }
> We use [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm) for easy MVVM support. Add it with `dotnet add package CommunityToolkit.Mvvm`.

---

## Step 4: Create the Page

Create `MainPage.cs`:

```csharp
using PlusUi.core;

namespace MyFirstPlusUiApp;

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .BindText(nameof(vm.Greeting), () => vm.Greeting)
                .SetTextSize(32)
                .SetTextColor(Colors.White)
                .SetHorizontalAlignment(HorizontalAlignment.Center),

            new Button()
                .SetText("Click Me!")
                .SetPadding(new Margin(20, 10))
                .SetCommand(vm.IncrementCounterCommand)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
        )
        .SetVerticalAlignment(VerticalAlignment.Center);
    }
}
```

---

## Step 5: Update Program.cs

Replace the contents of `Program.cs`:

```csharp
using PlusUi.desktop;
using MyFirstPlusUiApp;

var app = new PlusUiApp(args);
app.CreateApp(builder =>
{
    return new App();
});
```

---

## Step 6: Run the Application

```bash
dotnet run
```

You should see a window with your greeting and a clickable button!

---

## What's Next?

- [Project Setup](../guides/project-setup.html) - Learn about proper project structure
- [Theming](../guides/theming.html) - Customize the look of your app
- [Controls Reference](../controls/) - Explore all available controls
