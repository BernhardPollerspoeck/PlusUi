---
title: Home
layout: home
nav_order: 1
---

# PlusUi Documentation

PlusUi is a **cross-platform UI framework** built with SkiaSharp, designed for creating beautiful, performant applications across Desktop (Windows, Linux, macOS), Mobile (Android, iOS), and Web platforms.

{: .highlight }
> Currently targeting **.NET 10** with C# preview features.

---

## Key Features

- **Cross-Platform** - Write once, run on Desktop, Mobile, and Web
- **SkiaSharp Rendering** - Hardware-accelerated graphics
- **Fluent API** - Chainable, readable control configuration
- **MVVM Support** - Built-in data binding with `INotifyPropertyChanged`
- **Theming** - Customizable styles and themes
- **Accessibility** - Screen reader support and keyboard navigation

---

## Quick Example

```csharp
public class MyPage(MyPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("Hello, PlusUi!")
                .SetTextSize(24)
                .SetTextColor(Colors.White),

            new Button()
                .SetText("Click Me")
                .SetPadding(new Margin(20, 10))
                .SetCommand(vm.MyCommand)
        );
    }
}
```

---

## Get Started

<div class="code-example" markdown="1">
1. [Installation]({% link getting-started/installation.md %}) - Add PlusUi to your project
2. [First App]({% link getting-started/first-app.md %}) - Create your first application
3. [Project Setup]({% link guides/project-setup.md %}) - Configure your app properly
</div>

---

## Platform Support

| Platform | Status | Package |
|:---------|:-------|:--------|
| Windows Desktop | Stable | `PlusUi.desktop` |
| Linux Desktop | Stable | `PlusUi.desktop` |
| macOS Desktop | Stable | `PlusUi.desktop` |
| Android | Preview | `PlusUi.droid` |
| iOS | Preview | `PlusUi.ios` |
| Web (Blazor) | Preview | `PlusUi.web` |
| Headless | Stable | `PlusUi.headless` |

[View full Platform Support Matrix]({% link platform-support.md %})

---

## License

PlusUi is licensed under the [MIT License](https://github.com/BernhardPollerspoeck/PlusUi/blob/master/LICENSE).
