---
layout: home
title: Home
nav_order: 1
description: "PlusUi - A fully cross-platform UI Framework for .NET"
permalink: /
---

# PlusUi
{: .fs-9 }

A fully cross-platform UI Framework for .NET, delivering consistent user experiences across iOS, Android, Windows, Mac, and Linux.
{: .fs-6 .fw-300 }

[Get Started]({{ site.baseurl }}/getting-started){: .btn .btn-primary .fs-5 .mb-4 .mb-md-0 .mr-2 }
[View on GitHub](https://github.com/BernhardPollerspoeck/PlusUi){: .btn .fs-5 .mb-4 .mb-md-0 }

---

## Why PlusUi?

Built with **SkiaSharp** as the rendering layer, PlusUi ensures that all platforms look, feel, and behave exactly the same. No more platform-specific quirks or inconsistent styling.

### True Visual Consistency

Unlike frameworks that adapt to native controls, PlusUi renders identically across all platforms. What you design is exactly what users see, regardless of device.

### Pure C# - No XAML

Clean, intuitive MVVM binding with a pure C# approach. No XAML to learn, no complex markup languages - just straightforward code that's familiar to .NET developers.

### Fluent API Design

Method chaining makes UI construction readable and maintainable:

```csharp
new Button()
    .SetText("Save")
    .SetPadding(new Margin(15, 10))
    .SetBackground(new SolidColorBackground(SKColors.ForestGreen))
    .SetCornerRadius(8)
    .SetCommand(vm.SaveCommand)
```

---

## Quick Example

```csharp
public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("Hello PlusUi!")
                .SetTextSize(32)
                .SetTextColor(SKColors.White),

            new Button()
                .SetText("Click Me!")
                .SetCommand(vm.ClickCommand)
                .SetPadding(new Margin(20, 10))
        );
    }
}
```

---

## Features at a Glance

| Feature | Description |
|:--------|:------------|
| **Cross-Platform** | Windows, macOS, Linux, iOS, Android |
| **Consistent Rendering** | SkiaSharp-based, pixel-perfect across platforms |
| **MVVM Support** | Two-way data binding with CommunityToolkit.Mvvm |
| **Fluent API** | Readable method-chaining syntax |
| **Video Rendering** | Export UI to H264 video with PlusUi.h264 |
| **Styling** | Gradients, shadows, rounded corners, custom fonts |
| **Navigation** | Page navigation and modal popups |

---

## Installation

```bash
# For Desktop applications (Windows, Mac, Linux)
dotnet add package PlusUi.core
dotnet add package PlusUi.desktop
```

[View all packages]({{ site.baseurl }}/platform-support){: .btn .btn-outline }

---

## Community

Join our Discord community for support, feedback, and discussions:

[![Discord](https://img.shields.io/discord/1342509628948484287?color=5865F2&logo=discord&logoColor=white&label=Discord)](https://discord.gg/Je3kNpcmqn)

---

## License

PlusUi is distributed under the [MIT License](https://github.com/BernhardPollerspoeck/PlusUi/blob/main/LICENSE.txt).
