# IPlatformService

## Overview

The `IPlatformService` provides a unified interface for accessing platform-specific functionality and information across all PlusUi platforms. This service is automatically registered for each platform and provides consistent access to platform capabilities.

## Features

The `IPlatformService` interface provides:

- **Platform**: The current platform type (Desktop, Android, iOS, Web)
- **WindowSize**: Current window or screen size
- **DisplayDensity**: Screen density/scale factor
- **OpenUrl**: Opens URLs in the system's default browser

## Platform Types

```csharp
public enum PlatformType
{
    Desktop,  // Windows, Linux, macOS
    Android,  // Android devices
    iOS,      // iPhone, iPad
    Web       // Blazor WebAssembly
}
```

## Automatic Registration

The `IPlatformService` is **automatically registered** for all platforms. No manual setup is required.

### Desktop (Windows, Linux, macOS)
- Registered in: `PlusUi.desktop.PlusUiApp`
- Implementation: `DesktopPlatformService`
- URL opening: Uses `Process.Start` with platform-specific commands

### Android
- Registered in: `PlusUi.droid.PlusUiActivity`
- Implementation: `AndroidPlatformService`
- URL opening: Uses `Intent.ActionView`

### iOS
- Registered in: `PlusUi.ios.PlusUiAppDelegate`
- Implementation: `iOSPlatformService`
- URL opening: Uses `UIApplication.SharedApplication.OpenUrl`

### H.264 Video Rendering
- Registered in: `PlusUi.h264.PlusUiApp`
- Implementation: `H264PlatformService`
- URL opening: Not supported (returns false)

### Web (Blazor WebAssembly)
- Registered in: `PlusUi.Web.PlusUiWebApp`
- Implementation: `WebPlatformService`
- URL opening: Uses JavaScript `window.open`

## Usage Examples

### Access Platform Information

```csharp
using PlusUi.core.Services;

public class MyControl : UiElement
{
    private readonly IPlatformService _platformService;

    public MyControl(IPlatformService platformService)
    {
        _platformService = platformService;
    }

    public void ShowPlatformInfo()
    {
        // Get platform type
        var platform = _platformService.Platform;
        Console.WriteLine($"Running on: {platform}");

        // Get window size
        var size = _platformService.WindowSize;
        Console.WriteLine($"Window size: {size.Width}x{size.Height}");

        // Get display density
        var density = _platformService.DisplayDensity;
        Console.WriteLine($"Display density: {density}");
    }
}
```

### Open URLs

```csharp
public class MyControl
{
    private readonly IPlatformService _platformService;

    public MyControl(IPlatformService platformService)
    {
        _platformService = platformService;
    }

    public void OpenWebsite()
    {
        var success = _platformService.OpenUrl("https://example.com");
        if (!success)
        {
            Console.WriteLine("Failed to open URL");
        }
    }
}
```

### Using Dependency Injection

```csharp
// The service is automatically available through DI
var platformService = ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();

if (platformService != null)
{
    var platform = platformService.Platform;
    var windowSize = platformService.WindowSize;
}
```

## Link Control Integration

The `Link` control automatically uses `IPlatformService` to open URLs when clicked:

```csharp
var link = new Link()
    .SetText("Visit our website")
    .SetUrl("https://example.com")
    .SetTextColor(SKColors.Blue);
```

When clicked, the link automatically calls `IPlatformService.OpenUrl()` using the platform-specific implementation.

## Platform-Specific Notes

### iOS URL Scheme Configuration

For iOS 9+, declare URL schemes in your `Info.plist`:

```xml
<key>LSApplicationQueriesSchemes</key>
<array>
    <string>http</string>
    <string>https</string>
</array>
```

### H.264 Video Rendering

The H.264 platform service provides window size based on the `VideoConfiguration` but does not support URL opening as it runs in a non-interactive rendering context.

## Benefits

- **No RuntimeInformation checks**: All platform detection is handled by DI
- **Consistent API**: Same interface across all platforms
- **Discoverable**: Users can easily find available platform capabilities
- **Extensible**: New platform-specific features can be added to the interface
- **Testable**: Easy to mock for unit testing
