# Link Control - URL Launcher

## Overview

The `Link` control uses the `IUrlLauncherService` to open URLs in the system's default browser. Platform-specific implementations are automatically registered for Desktop, Android, and iOS platforms.

## Automatic Platform Registration

The `IUrlLauncherService` is **automatically registered** for all platforms when you use PlusUi. You don't need to manually register the service in your app.

### Desktop Platforms (Windows, Linux, macOS)

The default `UrlLauncherService` is automatically registered in `PlusUi.desktop.PlusUiApp`:
- Windows: Uses `Process.Start` with `UseShellExecute`
- Linux: Uses `xdg-open`
- macOS: Uses `open`

### Android Platform

The Android-specific implementation is automatically registered in `PlusUi.droid.PlusUiActivity`:
- Uses `Intent.ActionView` to open URLs in the default browser
- Automatically gets the application context from the dependency injection container

### iOS Platform

The iOS-specific implementation is automatically registered in `PlusUi.ios.PlusUiAppDelegate`:
- Uses `UIApplication.SharedApplication.OpenUrl` to open URLs
- Compatible with iOS 10+ with fallback for older versions

#### iOS Info.plist Configuration

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

## Implementation Details

The `Link` control automatically uses dependency injection to get the platform-specific `IUrlLauncherService`:

```csharp
var urlLauncher = ServiceProviderService.ServiceProvider?.GetService<IUrlLauncherService>();
```

If for some reason the service is not registered, it falls back to the default desktop implementation.
