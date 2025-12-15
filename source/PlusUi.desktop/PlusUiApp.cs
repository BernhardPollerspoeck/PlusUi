using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.desktop.Accessibility;
using System.Runtime.InteropServices;

namespace PlusUi.desktop;

public class PlusUiApp(string[] args)
{

    public void CreateApp(Func<HostApplicationBuilder, IAppConfiguration> appBuilder)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var app = appBuilder(builder);

        builder.UsePlusUiInternal(app, args);

        builder.Services.AddSingleton<DesktopPlatformService>();
        builder.Services.AddSingleton<IPlatformService>(sp => sp.GetRequiredService<DesktopPlatformService>());
        builder.Services.AddSingleton<DesktopKeyboardHandler>();
        builder.Services.AddSingleton<IKeyboardHandler>(sp => sp.GetRequiredService<DesktopKeyboardHandler>());
        builder.Services.AddHostedService<WindowManager>();

        // Register platform-specific accessibility bridge
        RegisterAccessibilityBridge(builder);

        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();
        host.Run();
    }

    private static void RegisterAccessibilityBridge(HostApplicationBuilder builder)
    {
        // Override the default NoOpAccessibilityBridge with platform-specific implementation
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            builder.Services.AddSingleton<IAccessibilityBridge, WindowsAccessibilityBridge>();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            builder.Services.AddSingleton<IAccessibilityBridge, MacOSAccessibilityBridge>();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            builder.Services.AddSingleton<IAccessibilityBridge, LinuxAccessibilityBridge>();
        }
        // If no platform matches, the NullAccessibilityBridge from core is used

        // Override the default AccessibilitySettingsService with platform-specific implementation
        builder.Services.AddSingleton<IAccessibilitySettingsService, DesktopAccessibilitySettingsService>();
    }

}
