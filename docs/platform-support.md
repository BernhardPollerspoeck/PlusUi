---
title: Platform Support
layout: default
nav_order: 6
---

# Platform Support

PlusUi supports multiple platforms through a single codebase.

---

## Supported Platforms

| Platform | Status | Minimum Version | NuGet Package |
|:---------|:-------|:----------------|:--------------|
| Windows | ✅ Stable | Windows 10 (1809+) | `PlusUi.desktop` |
| macOS | ✅ Stable | macOS 11.0+ | `PlusUi.desktop` |
| Linux | ✅ Stable | Ubuntu 20.04+ | `PlusUi.desktop` |
| Android | 🧪 Preview | API 21 (Android 5.0+) | `PlusUi.droid` |
| iOS | 🧪 Preview | iOS 14.0+ | `PlusUi.ios` |
| Web (Blazor) | 🧪 Preview | Modern browsers | `PlusUi.web` |
| Headless | ✅ Stable | .NET 10 | `PlusUi.headless` |

---

## Feature Matrix

### Core Features

| Feature | Windows | macOS | Linux | Android | iOS | Web | Headless |
|:--------|:-------:|:-----:|:-----:|:-------:|:---:|:---:|:--------:|
| Basic Controls | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Data Binding | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Navigation | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Popups/Dialogs | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Custom Fonts | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| SVG Icons | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Theming | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

### Input Features

| Feature | Windows | macOS | Linux | Android | iOS | Web | Headless |
|:--------|:-------:|:-----:|:-----:|:-------:|:---:|:---:|:--------:|
| Mouse Input | ✅ | ✅ | ✅ | N/A | N/A | ✅ | N/A |
| Touch Input | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | N/A |
| Keyboard Input | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | N/A |
| Keyboard Shortcuts | ✅ | ✅ | ✅ | ⚠️ | ⚠️ | ✅ | N/A |
| Scroll Wheel | ✅ | ✅ | ✅ | N/A | N/A | ✅ | N/A |
| Stylus/Pen | ✅ | ✅ | ⚠️ | ✅ | ✅ | ⚠️ | N/A |

### Advanced Features

| Feature | Windows | macOS | Linux | Android | iOS | Web | Headless |
|:--------|:-------:|:-----:|:-----:|:-------:|:---:|:---:|:--------:|
| Hardware Acceleration | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ | N/A |
| High DPI Support | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Dark Mode Detection | ✅ | ✅ | ⚠️ | ✅ | ✅ | ✅ | N/A |
| System Tray | ✅ | ✅ | ✅ | N/A | N/A | N/A | N/A |
| File Dialogs | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ | N/A |
| Clipboard | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | N/A |
| Drag and Drop | ✅ | ✅ | ✅ | ⚠️ | ⚠️ | ⚠️ | N/A |

**Legend:**
- ✅ Fully supported
- ⚠️ Partial support or platform limitations
- ❌ Not yet implemented
- N/A Not applicable to this platform

---

## Platform-Specific Setup

### Windows

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="PlusUi.core" Version="*" />
    <PackageReference Include="PlusUi.desktop" Version="*" />
  </ItemGroup>
</Project>
```

### macOS

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <RuntimeIdentifier>osx-arm64</RuntimeIdentifier> <!-- or osx-x64 -->
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="PlusUi.core" Version="*" />
    <PackageReference Include="PlusUi.desktop" Version="*" />
  </ItemGroup>
</Project>
```

### Linux

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="PlusUi.core" Version="*" />
    <PackageReference Include="PlusUi.desktop" Version="*" />
  </ItemGroup>
</Project>
```

**Note:** Linux requires additional dependencies:
```bash
# Ubuntu/Debian
sudo apt install libfontconfig1 libfreetype6

# Fedora
sudo dnf install fontconfig freetype
```

### Android

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-android</TargetFramework>
    <ApplicationId>com.yourcompany.yourapp</ApplicationId>
    <ApplicationVersion>1</ApplicationVersion>
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="PlusUi.core" Version="*" />
    <PackageReference Include="PlusUi.droid" Version="*" />
  </ItemGroup>
</Project>
```

### iOS

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-ios</TargetFramework>
    <RuntimeIdentifier>ios-arm64</RuntimeIdentifier>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="PlusUi.core" Version="*" />
    <PackageReference Include="PlusUi.ios" Version="*" />
  </ItemGroup>
</Project>
```

### Web (Blazor)

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="PlusUi.core" Version="*" />
    <PackageReference Include="PlusUi.web" Version="*" />
  </ItemGroup>
</Project>
```

### Headless

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="PlusUi.core" Version="*" />
    <PackageReference Include="PlusUi.headless" Version="*" />
  </ItemGroup>
</Project>
```

**Use cases for Headless:**
- Automated UI testing without a display
- Server-side rendering and screenshot generation
- CI/CD pipelines for visual regression testing
- Batch processing of UI exports

---

## Multi-Platform Project Structure

For targeting multiple platforms, use a shared project structure:

```
MyApp/
├── MyApp.Core/              # Shared code
│   ├── ViewModels/
│   ├── Pages/
│   └── MyApp.Core.csproj
├── MyApp.Windows/
│   └── MyApp.Windows.csproj
├── MyApp.Mac/
│   └── MyApp.Mac.csproj
├── MyApp.Android/
│   └── MyApp.Android.csproj
└── MyApp.iOS/
    └── MyApp.iOS.csproj
```

### Shared Project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="PlusUi.core" Version="1.0.0" />
  </ItemGroup>
</Project>
```

### Platform Project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\MyApp.Core\MyApp.Core.csproj" />
    <PackageReference Include="PlusUi.desktop" Version="*" />
  </ItemGroup>
</Project>
```

---

## Platform Detection

```csharp
// Check current platform at runtime
if (PlatformInfo.IsWindows)
{
    // Windows-specific code
}
else if (PlatformInfo.IsMacOS)
{
    // macOS-specific code
}
else if (PlatformInfo.IsAndroid)
{
    // Android-specific code
}
else if (PlatformInfo.IsIOS)
{
    // iOS-specific code
}

// Or use the platform enum
switch (PlatformInfo.Current)
{
    case Platform.Windows:
        break;
    case Platform.MacOS:
        break;
    case Platform.Linux:
        break;
    case Platform.Android:
        break;
    case Platform.iOS:
        break;
    case Platform.Web:
        break;
}
```

---

## Known Platform Limitations

### Web (Blazor)

- No native file system access (uses browser APIs)
- Limited drag-and-drop support
- Hardware acceleration depends on browser WebGL support

### Mobile (Android/iOS)

- Keyboard shortcuts work only with external keyboards
- System tray not available
- Some gestures may conflict with OS navigation

### Linux

- Dark mode detection depends on desktop environment
- Stylus support varies by distribution and hardware

### Headless

- No user input (designed for server-side rendering)
- No window or display output
- Ideal for automated testing, screenshot generation, and CI/CD pipelines
