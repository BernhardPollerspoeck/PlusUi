---
title: Headless Mode
layout: default
parent: Guides
nav_order: 4
---

# Headless Mode

PlusUi Headless allows you to run your UI application without a display. This is ideal for automated testing, screenshot generation, CI/CD pipelines, and server-side rendering.

---

## Installation

```bash
dotnet add package PlusUi.core
dotnet add package PlusUi.headless
```

---

## Project Setup

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="PlusUi.core" Version="*" />
    <PackageReference Include="PlusUi.headless" Version="*" />
  </ItemGroup>
</Project>
```

---

## Basic Usage

Create a headless instance using your existing `App` class:

```csharp
using PlusUi.Headless;
using PlusUi.Headless.Enumerations;

// Create headless instance with PNG output format
using var headless = PlusUiHeadless.Create(new App(), ImageFormat.Png);

// Capture a screenshot of the current frame
var frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot.png", frame);
```

---

## Simulating User Input

Headless mode supports simulating mouse and keyboard interactions:

### Mouse Input

```csharp
// Move mouse to position (x, y)
headless.MouseMove(100, 200);

// Click (move, press, release)
headless.MouseDown();
headless.MouseUp();

// Combined: move and click
headless.MouseMove(100, 200);
headless.MouseDown();
headless.MouseUp();
```

### Keyboard Input

```csharp
// Type text character by character
foreach (char c in "Hello World")
{
    headless.CharInput(c);
}
```

---

## Complete Example

This example demonstrates a full workflow: capturing screenshots after each interaction.

```csharp
using PlusUi.Headless;
using PlusUi.Headless.Enumerations;

// Create headless instance
using var headless = PlusUiHeadless.Create(new App(loadImagesSynchronously: true), ImageFormat.Png);

// 1. Capture initial state
var frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_01_initial.png", frame);

// 2. Click on a text entry and type
headless.MouseMove(65, 437);
headless.MouseDown();
headless.MouseUp();

foreach (char c in "Hello World")
{
    headless.CharInput(c);
}

frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_02_text_entered.png", frame);

// 3. Click a checkbox
headless.MouseMove(117, 533);
headless.MouseDown();
headless.MouseUp();

frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_03_checkbox_clicked.png", frame);

// 4. Click a button
headless.MouseMove(51, 535);
headless.MouseDown();
headless.MouseUp();

frame = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("screenshot_04_button_clicked.png", frame);
```

---

## Image Formats

The `ImageFormat` enum supports the following output formats:

| Format | Description |
|:-------|:------------|
| `ImageFormat.Png` | PNG format (lossless, with transparency) |
| `ImageFormat.Jpeg` | JPEG format (lossy, smaller file size) |
| `ImageFormat.Webp` | WebP format (modern, efficient compression) |

```csharp
// PNG (default, best for UI screenshots)
using var headless = PlusUiHeadless.Create(new App(), ImageFormat.Png);

// JPEG (smaller files, no transparency)
using var headless = PlusUiHeadless.Create(new App(), ImageFormat.Jpeg);

// WebP (modern format, good compression)
using var headless = PlusUiHeadless.Create(new App(), ImageFormat.Webp);
```

---

## Use Cases

### Automated UI Testing

```csharp
using var headless = PlusUiHeadless.Create(new App(), ImageFormat.Png);

// Navigate to a specific state
headless.MouseMove(buttonX, buttonY);
headless.MouseDown();
headless.MouseUp();

// Capture and compare with baseline
var screenshot = await headless.GetCurrentFrameAsync();
await File.WriteAllBytesAsync("test_result.png", screenshot);

// Compare with expected baseline image
Assert.IsTrue(CompareImages("baseline.png", "test_result.png"));
```

### CI/CD Visual Regression Testing

Run headless tests in your CI pipeline to detect visual regressions:

```yaml
# Example GitHub Actions step
- name: Run Visual Tests
  run: dotnet run --project MyApp.HeadlessTests
```

### Screenshot Generation for Documentation

```csharp
// Generate documentation screenshots automatically
var pages = new[] { "home", "settings", "profile" };

foreach (var page in pages)
{
    // Navigate to page
    NavigateToPage(headless, page);

    // Capture screenshot
    var frame = await headless.GetCurrentFrameAsync();
    await File.WriteAllBytesAsync($"docs/images/{page}.png", frame);
}
```

### Server-Side Rendering

Generate images on the server without requiring a display:

```csharp
public async Task<byte[]> GeneratePreviewImage(UserData data)
{
    using var headless = PlusUiHeadless.Create(new PreviewApp(data), ImageFormat.Png);
    return await headless.GetCurrentFrameAsync();
}
```

---

## Tips

{: .tip }
> **Synchronous Image Loading** - When generating screenshots, pass `loadImagesSynchronously: true` to your App constructor to ensure all images are loaded before capturing.

{: .tip }
> **Dispose Properly** - Always use `using` or call `Dispose()` on the headless instance to free resources.

{: .warning }
> **No User Input** - Headless mode doesn't have actual user input. All interactions must be simulated programmatically.

---

## Next Steps

- [Project Setup](project-setup.html) - App configuration and DI
- [Best Practices](best-practices.html) - Write better PlusUi code
