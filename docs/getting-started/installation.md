---
title: Installation
layout: default
parent: Getting Started
nav_order: 1
---

# Installation

## NuGet Packages

Add the required packages to your project:

### For Desktop Applications (Windows/Linux/macOS)

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.desktop
```

### For Android

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.droid
```

### For iOS

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.ios
```

### For Web (Blazor)

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.web
```

### For Headless (Testing, Automation, Server-side Rendering)

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.headless
```

---

## Project File Setup

Your `.csproj` should target .NET 10:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
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

{: .note }
> For Android, use `net10.0-android`. For iOS, use `net10.0-ios`.

---

## Verify Installation

Create a simple test to verify everything works:

```csharp
using PlusUi.core;

// If this compiles, you're ready!
var label = new Label().SetText("PlusUi is installed!");
```

---

## Next Steps

Continue to [First App](first-app.html) to create your first PlusUi application.
