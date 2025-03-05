# PlusUi

PlusUi is a cutting-edge, fully cross-platform UI Framework designed to deliver a seamless and consistent user experience across iOS, Android, Windows, Mac, and Linux. Leveraging hardware acceleration through Silk.NET and utilizing SkiaSharp as the rendering layer, PlusUi ensures that all platforms look, feel, and behave exactly the same. This approach guarantees high performance and visual fidelity, making PlusUi an ideal choice for modern, responsive applications.

## The Journey Behind PlusUi

After years of working with various cross-platform UI frameworks and repeatedly encountering inconsistencies between platforms, I decided to explore a different path. PlusUi wasn't born from a belief that I could outdo established solutions, but from genuine curiosity: what if I approached the cross-platform challenge with consistency as the absolute priority? This project represents that personal journey—an attempt to create something where what you design is exactly what users see, regardless of device. It's about seeing the results emerge when building things to work as I imagined they should.

## Why Choose PlusUi?

In a landscape filled with cross-platform solutions like MAUI, Uno, Avalonia, and Blazor Desktop, PlusUi stands out for several key reasons:

- **True Visual Consistency**: Unlike frameworks that adapt to native controls, PlusUi renders identically across all platforms through SkiaSharp, ensuring your app looks and behaves exactly the same everywhere.
  
- **Performance First**: Built with hardware acceleration at its core, PlusUi delivers superior performance for graphics-intensive applications and smooth animations.

- **Lightweight Footprint**: PlusUi maintains a smaller resource footprint than alternatives that need to incorporate multiple platform-specific rendering engines.

- **Simple Programming Model**: Clean, intuitive MVVM binding with a pure C# approach. No XAML to learn, no complex markup languages or designers - just straightforward code that's familiar to .NET developers.

- **Freedom from Platform Limitations**: By using a consistent rendering approach, PlusUi isn't constrained by platform-specific UI capabilities or limitations.

----
### To get in touch regarding Support, Feedback or just a quick chat about your experiences the discord server would be the best place. See you there!

[![Discord Banner 3](https://discord.com/api/guilds/1342509628948484287/widget.png?style=banner3)](https://discord.gg/Je3kNpcmqn)


## Releases
- [![nuget](https://github.com/BernhardPollerspoeck/PlusUi/actions/workflows/main.yml/badge.svg)](https://github.com/BernhardPollerspoeck/PlusUi/actions/workflows/main.yml)
- ![NuGet Version](https://img.shields.io/nuget/v/PlusUi.core?label=PlusUi.core&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FPlusUi.core)
- ![NuGet Version](https://img.shields.io/nuget/v/PlusUi.desktop?&label=PlusUi.desktop&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FPlusUi.desktop)


## Features

The following is a list of some of the features currently available in PlusUi. This section will be further expanded as the project evolves and may not list all features.

- Cross-platform support: 
  - iOS (TODO)
  - Android (TODO)
  - Windows
  - Mac (TODO)
  - Linux (TODO)
- Easy to use and extend
- Mvvm Data Binding
- Modern UI components
- Customizable themes

## Documentation

All the controls, samples, and project setup instructions can be found in the [GitHub Wiki](https://github.com/BernhardPollerspoeck/PlusUi/wiki). For more detailed information, including advanced usage and troubleshooting, please refer to the comprehensive guides and FAQs available in the wiki.

## Example
This quick Example shows a simple Sample on how the code written for this framework translates into UI.
![image](https://github.com/user-attachments/assets/5ad0c960-35c1-409a-878e-1237d8c32925)
![image](https://github.com/user-attachments/assets/cdb360c7-e6ed-440e-981e-fb62fb0eacab)
![image](https://github.com/user-attachments/assets/d01227b8-b556-4316-887d-ecd5aba9f54c)
![image](https://github.com/user-attachments/assets/f96d219c-9a22-416b-9e6e-4e8ba3fc8ea0)


## Technical Architecture

### How PlusUi Works Under the Hood

PlusUi implements a custom UI framework with a clean separation of concerns across multiple architectural layers:

#### Core Rendering Pipeline

1. **Silk.NET Foundation**: PlusUi uses Silk.NET for its windowing and input capabilities. This provides low-level access to platform-specific windowing systems and input devices through a unified API.

2. **SkiaSharp Rendering Layer**: The rendering is handled by SkiaSharp, coordinating between OpenGL contexts and SkiaSharp's canvas. Every UI element utilizes SkiaSharp's drawing primitives for consistent cross-platform visuals.

3. **Custom Layout Engine**: The framework implements a measurement and arrangement system.

This architecture allows PlusUi to deliver a consistent user experience across platforms while maintaining good performance through direct rendering. The component model and binding system provide a familiar programming pattern for .NET developers without requiring XAML or other markup languages.


## Contributing

We welcome contributions! Please open an issue to discuss any planned changes before submitting a pull request. This helps us ensure that your contribution aligns with the project's goals and avoids duplication of effort. 

When submitting a pull request, please ensure that your code follows the project's coding standards and includes appropriate tests. We also encourage you to update the documentation if your changes affect it. Contributions to documentation, samples, the README, or the wiki are also welcome.

Thank you for your interest in contributing to PlusUi!

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. The MIT License was chosen for its permissiveness and simplicity, allowing for wide usage and contribution. It ensures that the project remains open and accessible to developers while protecting the original authors.
