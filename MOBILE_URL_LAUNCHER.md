# Mobile URL Launcher Setup

## Overview

The `Link` control uses the `IUrlLauncherService` to open URLs in the system's default browser. Platform-specific implementations are provided for Android and iOS.

## Desktop Platforms (Windows, Linux, macOS)

The default `UrlLauncherService` implementation works automatically for desktop platforms. No additional setup is required.

## Android Setup

To enable URL launching on Android, register the Android-specific implementation in your app's startup code:

```csharp
// In your MainActivity.cs or application startup
using PlusUi.droid;
using PlusUi.core.Services;
using Microsoft.Extensions.DependencyInjection;

// During service registration
services.AddSingleton<IUrlLauncherService>(sp =>
    new UrlLauncherService(Android.App.Application.Context));
```

The Android implementation uses `Intent.ActionView` to open URLs in the default browser.

## iOS Setup

To enable URL launching on iOS, register the iOS-specific implementation in your app's startup code:

```csharp
// In your AppDelegate.cs or application startup
using PlusUi.ios;
using PlusUi.core.Services;
using Microsoft.Extensions.DependencyInjection;

// During service registration
services.AddSingleton<IUrlLauncherService, UrlLauncherService>();
```

The iOS implementation uses `UIApplication.SharedApplication.OpenUrl` to open URLs.

### iOS Info.plist Configuration

For iOS 9+, you may need to declare the URL schemes you want to open in your `Info.plist`:

```xml
<key>LSApplicationQueriesSchemes</key>
<array>
    <string>http</string>
    <string>https</string>
</array>
```

## Usage Example

```csharp
var link = new Link()
    .SetText("Visit our website")
    .SetUrl("https://example.com")
    .SetTextColor(SKColors.Blue);
```

When the link is clicked (via `IInputControl.InvokeCommand()`), it will automatically use the appropriate platform-specific implementation to open the URL.

## Fallback Behavior

If no `IUrlLauncherService` is registered in the dependency injection container, the `Link` control will fall back to the default desktop implementation, which may or may not work on mobile platforms depending on the .NET runtime capabilities.

For best results, always register the appropriate platform-specific implementation for mobile apps.
