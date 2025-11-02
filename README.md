# PlusUi

A fully cross-platform UI Framework for .NET, delivering consistent user experiences across iOS, Android, Windows, Mac, and Linux. Built with SkiaSharp as the rendering layer, PlusUi ensures that all platforms look, feel, and behave exactly the same.

[![Build Status](https://github.com/BernhardPollerspoeck/PlusUi/actions/workflows/main.yml/badge.svg)](https://github.com/BernhardPollerspoeck/PlusUi/actions/workflows/main.yml)

## Supported Platforms

| Platform | Package | Status | NuGet |
|----------|---------|--------|-------|
| Windows | PlusUi.desktop | ✅ Stable | [![NuGet](https://img.shields.io/nuget/v/PlusUi.desktop.svg)](https://www.nuget.org/packages/PlusUi.desktop) |
| macOS | PlusUi.desktop | ✅ Stable | [![NuGet](https://img.shields.io/nuget/v/PlusUi.desktop.svg)](https://www.nuget.org/packages/PlusUi.desktop) |
| Linux | PlusUi.desktop | ✅ Stable | [![NuGet](https://img.shields.io/nuget/v/PlusUi.desktop.svg)](https://www.nuget.org/packages/PlusUi.desktop) |
| iOS | PlusUi.ios | 🚧 In Development | [![NuGet](https://img.shields.io/nuget/v/PlusUi.ios.svg)](https://www.nuget.org/packages/PlusUi.ios) |
| Android | PlusUi.droid | 🚧 In Development | [![NuGet](https://img.shields.io/nuget/v/PlusUi.droid.svg)](https://www.nuget.org/packages/PlusUi.droid) |
| **Video Rendering** | **PlusUi.h264** | ✅ **Stable** | [![NuGet](https://img.shields.io/nuget/v/PlusUi.h264.svg)](https://www.nuget.org/packages/PlusUi.h264) |

**Core Library:** [![NuGet](https://img.shields.io/nuget/v/PlusUi.core.svg)](https://www.nuget.org/packages/PlusUi.core) (Required by all platforms)

---

## Quick Start

### Prerequisites

- .NET 9.0 SDK
- **Windows:** Windows 11 or later
- **Android:** Android 8.0 (API 26) or later
- **iOS:** iOS 11 or later
- **Linux/Mac:** No specific version requirements

### Installation

Install the PlusUi packages via NuGet:

```bash
# For Desktop applications (Windows, Mac, Linux)
dotnet add package PlusUi.core
dotnet add package PlusUi.desktop

# For iOS
dotnet add package PlusUi.core
dotnet add package PlusUi.ios

# For Android
dotnet add package PlusUi.core
dotnet add package PlusUi.droid

# For Video Rendering (H264)
dotnet add package PlusUi.core
dotnet add package PlusUi.h264
```

### Hello World Example

Create a simple counter application:

**App.cs**
```csharp
using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace MyFirstApp;

public class App : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration config)
    {
        config.Title = "My First PlusUi App";
        config.Size = new SizeI(800, 600);
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        // Register your pages
        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}
```

**MainPage.cs**
```csharp
using PlusUi.core;
using SkiaSharp;

namespace MyFirstApp;

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            // Label showing the counter
            new Label()
                .SetText("Counter App")
                .SetTextSize(32)
                .SetTextColor(SKColors.White)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                .SetMargin(new Margin(0, 20)),

            // Display current count with data binding
            new Label()
                .BindText(nameof(vm.Count), () => $"Count: {vm.Count}")
                .SetTextSize(24)
                .SetTextColor(SKColors.LightGray)
                .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                .SetMargin(new Margin(0, 10)),

            // Button to increment counter
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

**MainPageViewModel.cs**
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyFirstApp;

public partial class MainPageViewModel : ObservableObject
{
    public int Count
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    [RelayCommand]
    private void Increment()
    {
        Count++;
    }
}
```

### Build and Run

```bash
# Build the solution
dotnet build

# Run on Desktop
dotnet run --project YourDesktopProject

# Run tests
dotnet test
```

For detailed setup instructions, platform-specific configurations, and more examples, visit the [GitHub Wiki](https://github.com/BernhardPollerspoeck/PlusUi/wiki).

---

## Why Choose PlusUi?

### True Visual Consistency
Unlike frameworks that adapt to native controls, PlusUi renders identically across all platforms through SkiaSharp. What you design is exactly what users see, regardless of device.

### Simple Programming Model
Clean, intuitive MVVM binding with a pure C# approach. No XAML to learn, no complex markup languages - just straightforward code that's familiar to .NET developers.

### Fluent API Design
Method chaining makes UI construction readable and maintainable:

```csharp
new Button()
    .SetText("Save")
    .SetIcon("save.png")
    .SetPadding(new Margin(15, 10))
    .SetBackground(new SolidColorBackground(SKColors.ForestGreen))
    .SetCornerRadius(8)
    .SetCommand(vm.SaveCommand)
```

### Freedom from Platform Limitations
By using a consistent rendering approach, PlusUi isn't constrained by platform-specific UI capabilities or limitations.

---

## Features

### UI Components
- **Layouts:** `VStack`, `HStack`, `Grid`, `ScrollView`, `Border`
- **Controls:** `Label`, `Button`, `Entry`, `Checkbox`, `Image`
- **Custom Drawing:** `Solid`, `UserControl`, `RawUserControl`
- **Lists:** `ItemsList` for dynamic, data-bound lists

### Styling & Theming
- Solid colors, linear gradients, radial gradients, multi-stop gradients
- Corner radius, borders (solid, dashed, dotted)
- Shadows with customizable blur and offset
- Custom fonts with `FontRegistryService`
- Theme support with `IApplicationStyle`

### Data Binding
- Two-way MVVM binding
- Property change notifications
- Command binding with parameters
- Uses CommunityToolkit.Mvvm

### Navigation & Popups
- Page-based navigation with `INavigationService`
- Modal popups with `IPopupService`
- Configurable popup behavior (background click, escape key)

### Cross-Platform Input
- Mouse and touch input
- Keyboard support with customizable handlers
- Scroll gestures

### Asset Loading
- Local images (embedded resources)
- Web images (async loading with caching)
- Custom fonts

---

## Video Rendering (PlusUi.h264)

The **PlusUi.h264** package provides a unique capability: rendering your PlusUi application directly to an H264 video file. This is perfect for creating animated demos, promotional videos, tutorials, or any scenario where you need to convert your UI into video format.

### Features

- **Video Output:** Renders UI to MP4 files with configurable resolution, frame rate, and duration
- **Audio Support:** Synchronize audio tracks with your UI animations using `AudioDefinition`
- **Animation System:** Built-in animation support for smooth transitions
- **FFmpeg Integration:** Includes FFmpeg binaries for Windows, macOS, and Linux
- **Progress Tracking:** Real-time rendering progress with Spectre.Console

### Example Usage

**VideoApp.cs**
```csharp
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.h264;

public class VideoApp : IVideoAppConfiguration
{
    public void ConfigureVideo(VideoConfiguration config)
    {
        config.Width = 1920;
        config.Height = 1080;
        config.OutputFilePath = "output.mp4";
        config.FrameRate = 60;
        config.Duration = TimeSpan.FromSeconds(10);
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        builder.AddPage<MainPage>().WithViewModel<MainPageViewModel>();
        builder.StylePlusUi<MyCustomStyle>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }

    public IAudioSequenceProvider? GetAudioSequenceProvider(IServiceProvider serviceProvider)
    {
        // Return null for no audio, or implement IAudioSequenceProvider
        return null;
    }
}
```

**Program.cs**
```csharp
using PlusUi.h264;

var app = new PlusUiApp(args);
app.CreateApp(builder => new VideoApp());
```

### Adding Audio

Implement `IAudioSequenceProvider` to add synchronized audio:

```csharp
public class MyAudioProvider : IAudioSequenceProvider
{
    public IEnumerable<AudioDefinition> GetAudioSequence()
    {
        yield return new AudioDefinition(
            FilePath: "background-music.mp3",
            StartTime: TimeSpan.Zero,
            Volume: 0.8f
        );

        yield return new AudioDefinition(
            FilePath: "sound-effect.wav",
            StartTime: TimeSpan.FromSeconds(2.5),
            Volume: 1.0f
        );
    }
}
```

### Use Cases

- **Promotional Videos:** Create product demos and marketing materials
- **Tutorials:** Generate step-by-step tutorial videos
- **Automated Testing:** Record UI test runs
- **Documentation:** Visual documentation of UI features
- **Social Media:** Export UI animations for social media posts

---

## Examples

The following example demonstrates how PlusUi code translates into UI. This shows the complete flow from code to visual output:

### 1. Page Definition

![Page Code](https://github.com/user-attachments/assets/5ad0c960-35c1-409a-878e-1237d8c32925)

### 2. ViewModel

![ViewModel Code](https://github.com/user-attachments/assets/cdb360c7-e6ed-440e-981e-fb62fb0eacab)

### 3. Styling

![Styling Code](https://github.com/user-attachments/assets/d01227b8-b556-4316-887d-ecd5aba9f54c)

### 4. Resulting UI

![Final UI](https://github.com/user-attachments/assets/f96d219c-9a22-416b-9e6e-4e8ba3fc8ea0)

For more examples and interactive demos, explore the `samples/Sandbox` project in this repository.

---

## Documentation

Comprehensive documentation is available in the [GitHub Wiki](https://github.com/BernhardPollerspoeck/PlusUi/wiki):

- Getting Started Guide
- Control Reference
- Data Binding Tutorial
- Styling & Themes
- Navigation Patterns
- Platform-Specific Setup
- Migration Guides
- Advanced Topics

---

## Building from Source

### Clone the Repository
```bash
git clone https://github.com/BernhardPollerspoeck/PlusUi.git
cd PlusUi
```

### Build Commands
```bash
# Build entire solution
dotnet build PlusUi.sln

# Build with minimal output
dotnet build PlusUi.sln -v q

# Run all tests
dotnet test PlusUi.sln

# Run a specific test
dotnet test PlusUi.sln --filter "FullyQualifiedName~UiPlus.core.Tests.LabelTests"
```

### Project Structure
```
PlusUi/
├── source/
│   ├── PlusUi.core/          # Core framework (cross-platform)
│   ├── PlusUi.desktop/       # Desktop implementation
│   ├── PlusUi.ios/           # iOS implementation
│   ├── PlusUi.droid/         # Android implementation
│   └── PlusUi.SourceGenerators/  # Code generation tools
├── samples/
│   └── Sandbox/              # Example application
└── tests/
    └── UiPlus.core.Tests/    # Unit tests
```

---

## Roadmap

We're actively working on the following features for upcoming releases:

### Planned Features
- **GridControl Enhancement:** Advanced grid capabilities with sorting and filtering
- **dotnet new Templates:** Quick project scaffolding with `dotnet new plusui`
- **Rendering Optimization:** Internal rendering engine improvements for better performance

### Future Considerations
- Additional UI controls (DatePicker, Slider, ProgressBar)
- Animation framework
- Accessibility improvements
- Hot reload enhancements

Want to see something specific? Open an issue and let us know!

---

## Technical Architecture

PlusUi implements a custom UI framework with a clean separation of concerns:

### Core Rendering Pipeline

1. **SkiaSharp Rendering Layer:** All rendering is handled by SkiaSharp, providing direct access to the 2D graphics canvas. This ensures consistent visual output across all platforms.

2. **Custom Layout Engine:** The framework implements a measurement and arrangement system similar to WPF/MAUI, where every UI element:
   - Measures its desired size based on content and constraints
   - Arranges itself and its children within an allocated space
   - Supports flexible layouts with alignment, margins, and padding

3. **Platform Abstraction:** Platform-specific hosts (Desktop, iOS, Android) provide:
   - Window/View creation and management
   - Input event handling (mouse, touch, keyboard)
   - Integration with SkiaSharp's rendering context

### Component Model

- **Base Elements:** All UI components inherit from `UiElement` or `UiLayoutElement`
- **Pages:** Top-level containers inherit from `UiPageElement`
- **Data Binding:** Property change notifications integrated with MVVM pattern
- **Fluent API:** Method chaining with `.Set*()` methods for readable UI construction

This architecture allows PlusUi to deliver consistent user experiences across platforms while maintaining flexibility and performance.

---

## Community & Support

Join our Discord community to get support, share feedback, or discuss your experiences with PlusUi:

[![Discord Banner 3](https://discord.com/api/guilds/1342509628948484287/widget.png?style=banner3)](https://discord.gg/Je3kNpcmqn)

---

## Contributing

We welcome contributions! Please open an issue to discuss any planned changes before submitting a pull request. This helps ensure that your contribution aligns with the project's goals and avoids duplication of effort.

### Contribution Guidelines

1. **Discuss First:** Open an issue to propose changes before starting work
2. **Code Standards:** Follow the project's coding standards (see CLAUDE.md for details)
3. **Tests:** Include appropriate tests for new features or bug fixes
4. **Documentation:** Update documentation for any user-facing changes
5. **Small PRs:** Keep pull requests focused and manageable

We also welcome contributions to:
- Documentation and wiki pages
- Sample applications
- Bug reports and feature requests
- Code reviews and discussions

Thank you for your interest in contributing to PlusUi!

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

The MIT License allows for wide usage and contribution while remaining open and accessible to developers.

---

## About the Project

After years of working with various cross-platform UI frameworks and encountering inconsistencies between platforms, PlusUi was born from curiosity: what if consistency was the absolute priority? This project represents a journey to create something where what you design is exactly what users see, regardless of device.
