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

---

## Feature Matrix

### Core Features

| Feature | Windows | macOS | Linux | Android | iOS | Web |
|:--------|:-------:|:-----:|:-----:|:-------:|:---:|:---:|
| Basic Controls | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Data Binding | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Navigation | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Popups/Dialogs | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Custom Fonts | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| SVG Icons | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Theming | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

### Input Features

| Feature | Windows | macOS | Linux | Android | iOS | Web |
|:--------|:-------:|:-----:|:-----:|:-------:|:---:|:---:|
| Mouse Input | ✅ | ✅ | ✅ | N/A | N/A | ✅ |
| Touch Input | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Keyboard Input | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Keyboard Shortcuts | ✅ | ✅ | ✅ | ⚠️ | ⚠️ | ✅ |
| Scroll Wheel | ✅ | ✅ | ✅ | N/A | N/A | ✅ |
| Stylus/Pen | ✅ | ✅ | ⚠️ | ✅ | ✅ | ⚠️ |

### Advanced Features

| Feature | Windows | macOS | Linux | Android | iOS | Web |
|:--------|:-------:|:-----:|:-----:|:-------:|:---:|:---:|
| Hardware Acceleration | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ |
| High DPI Support | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Dark Mode Detection | ✅ | ✅ | ⚠️ | ✅ | ✅ | ✅ |
| System Tray | ✅ | ✅ | ✅ | N/A | N/A | N/A |
| File Dialogs | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ |
| Clipboard | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Drag and Drop | ✅ | ✅ | ✅ | ⚠️ | ⚠️ | ⚠️ |

**Legend:**
- ✅ Fully supported
- ⚠️ Partial support or platform limitations
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
