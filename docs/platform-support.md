---
layout: default
title: Platform Support
nav_order: 3
description: "PlusUi platform support matrix - Windows, macOS, Linux, iOS, Android"
permalink: /platform-support
---

# Platform Support
{: .fs-9 }

PlusUi runs on all major platforms with consistent rendering.
{: .fs-6 .fw-300 }

---

## Support Matrix

| Platform | Package | Status | Min Version | NuGet |
|:---------|:--------|:-------|:------------|:------|
| **Windows** | `PlusUi.desktop` | Stable | Windows 11 | [![NuGet](https://img.shields.io/nuget/v/PlusUi.desktop.svg)](https://www.nuget.org/packages/PlusUi.desktop) |
| **macOS** | `PlusUi.desktop` | Stable | - | [![NuGet](https://img.shields.io/nuget/v/PlusUi.desktop.svg)](https://www.nuget.org/packages/PlusUi.desktop) |
| **Linux** | `PlusUi.desktop` | Stable | - | [![NuGet](https://img.shields.io/nuget/v/PlusUi.desktop.svg)](https://www.nuget.org/packages/PlusUi.desktop) |
| **iOS** | `PlusUi.ios` | In Development | iOS 11 | [![NuGet](https://img.shields.io/nuget/v/PlusUi.ios.svg)](https://www.nuget.org/packages/PlusUi.ios) |
| **Android** | `PlusUi.droid` | In Development | API 26 (8.0) | [![NuGet](https://img.shields.io/nuget/v/PlusUi.droid.svg)](https://www.nuget.org/packages/PlusUi.droid) |

### Special Packages

| Package | Purpose | Status | NuGet |
|:--------|:--------|:-------|:------|
| **PlusUi.core** | Core framework (required) | Stable | [![NuGet](https://img.shields.io/nuget/v/PlusUi.core.svg)](https://www.nuget.org/packages/PlusUi.core) |
| **PlusUi.h264** | Video rendering | Stable | [![NuGet](https://img.shields.io/nuget/v/PlusUi.h264.svg)](https://www.nuget.org/packages/PlusUi.h264) |

---

## Desktop Platforms

### Windows

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.desktop
```

**Requirements:**
- Windows 11 or later
- .NET 10.0 SDK

**Features:**
- Full mouse and keyboard support
- Window resize and positioning
- System clipboard integration
- Native file dialogs

### macOS

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.desktop
```

**Requirements:**
- macOS (no specific version requirement)
- .NET 10.0 SDK

**Features:**
- Retina display support
- Trackpad gestures
- Menu bar integration

### Linux

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.desktop
```

**Requirements:**
- Any modern Linux distribution
- .NET 10.0 SDK
- X11 or Wayland display server

**Tested Distributions:**
- Ubuntu 22.04+
- Debian 12+
- Fedora 38+
- Arch Linux

---

## Mobile Platforms

{: .warning }
> Mobile platforms are currently in development. APIs may change.

### iOS

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.ios
```

**Requirements:**
- iOS 11 or later
- Xcode (for building)
- macOS development machine

**Features:**
- Touch input
- Screen rotation
- Safe area handling

### Android

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.droid
```

**Requirements:**
- Android 8.0 (API level 26) or later
- Android SDK

**Features:**
- Touch input
- Screen rotation
- Back button handling

---

## Video Rendering

The **PlusUi.h264** package enables rendering your UI to video files.

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.h264
```

### Supported Output Formats

| Format | Codec | Container |
|:-------|:------|:----------|
| H.264 | x264 | MP4 |

### Platform Support for Video Rendering

| Platform | Status | Notes |
|:---------|:-------|:------|
| Windows | Supported | FFmpeg included |
| macOS | Supported | FFmpeg included |
| Linux | Supported | FFmpeg included |

### Configuration Options

```csharp
public void ConfigureVideo(VideoConfiguration config)
{
    config.Width = 1920;           // Video width
    config.Height = 1080;          // Video height
    config.OutputFilePath = "output.mp4";
    config.FrameRate = 60;         // Frames per second
    config.Duration = TimeSpan.FromSeconds(10);
}
```

---

## Architecture Support

### Desktop (PlusUi.desktop)

| Architecture | Windows | macOS | Linux |
|:-------------|:--------|:------|:------|
| x64 | Yes | Yes | Yes |
| ARM64 | Yes | Yes (Apple Silicon) | Yes |

### Mobile

| Architecture | iOS | Android |
|:-------------|:----|:--------|
| ARM64 | Yes | Yes |
| x64 | Simulator | Emulator |

---

## .NET Version Requirements

| PlusUi Version | .NET Version |
|:---------------|:-------------|
| 1.x | .NET 10.0+ |

{: .note }
> PlusUi targets .NET 10.0 and uses the latest C# language features.

---

## Rendering Engine

All platforms use **SkiaSharp** for rendering, which provides:

- Consistent pixel-perfect rendering
- Hardware acceleration where available
- Support for complex paths and gradients
- Custom font rendering
- Image manipulation

### SkiaSharp Version

PlusUi uses SkiaSharp 3.x for optimal performance and platform support.

---

## Feature Availability

| Feature | Desktop | iOS | Android | Video |
|:--------|:--------|:----|:--------|:------|
| **Layouts** |
| VStack/HStack | Yes | Yes | Yes | Yes |
| Grid | Yes | Yes | Yes | Yes |
| ScrollView | Yes | Yes | Yes | Yes |
| **Controls** |
| Label | Yes | Yes | Yes | Yes |
| Button | Yes | Yes | Yes | Yes |
| Entry | Yes | Yes | Yes | Yes |
| Checkbox | Yes | Yes | Yes | Yes |
| Image | Yes | Yes | Yes | Yes |
| **Input** |
| Mouse | Yes | - | - | - |
| Touch | - | Yes | Yes | - |
| Keyboard | Yes | Yes | Yes | - |
| **Styling** |
| Solid Colors | Yes | Yes | Yes | Yes |
| Gradients | Yes | Yes | Yes | Yes |
| Shadows | Yes | Yes | Yes | Yes |
| Custom Fonts | Yes | Yes | Yes | Yes |
| **Navigation** |
| Page Navigation | Yes | Yes | Yes | Yes |
| Popups | Yes | Yes | Yes | Yes |

---

## Known Limitations

### Desktop
- Windows: Requires Windows 11 for best experience

### iOS
- In development - API may change
- Requires macOS for building

### Android
- In development - API may change
- Some advanced features pending

### Video Rendering
- No real-time interaction
- Fixed duration required upfront
