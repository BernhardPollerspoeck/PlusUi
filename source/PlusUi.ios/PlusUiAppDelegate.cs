using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.ios.Accessibility;

namespace PlusUi.ios;

public abstract class PlusUiAppDelegate : UIApplicationDelegate
{
    private IHost? _host;

    protected abstract IAppConfiguration CreateApp(HostApplicationBuilder builder);


    public override UIWindow? Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        _host = CreateAndStartHost();

        Window = CreateWindow();
        Window.RootViewController = _host.Services.GetRequiredService<OpenGlViewController>();
        Window.MakeKeyAndVisible();
        return true;
    }

    private static UIWindow CreateWindow()
    {
        if (OperatingSystem.IsIOSVersionAtLeast(26))
        {
            var windowScene = UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIWindowScene>()
                .FirstOrDefault();

            if (windowScene != null)
            {
                return new UIWindow(windowScene);
            }
        }

#pragma warning disable CA1422 // Required for iOS < 26 backward compatibility
        return new UIWindow(UIScreen.MainScreen.Bounds);
#pragma warning restore CA1422
    }

    private IHost CreateAndStartHost()
    {
        var builder = Host.CreateApplicationBuilder();

        var app = CreateApp(builder);

        builder.UsePlusUiInternal(app, []);
        builder.Services.AddSingleton<IosPlatformService>();
        builder.Services.AddSingleton<IPlatformService>(sp => sp.GetRequiredService<IosPlatformService>());
        builder.Services.AddSingleton<IosHapticService>();
        builder.Services.AddSingleton<IHapticService>(sp => sp.GetRequiredService<IosHapticService>());
        builder.Services.AddSingleton<OpenGlViewController>();
        builder.Services.AddSingleton<KeyboardTextField>();
        builder.Services.AddSingleton<IKeyboardHandler>(sp => sp.GetRequiredService<KeyboardTextField>());

        // Register iOS accessibility bridge (VoiceOver support)
        builder.Services.AddSingleton<IAccessibilityBridge, IosAccessibilityBridge>();

        // Register iOS accessibility settings service (Dynamic Type, high contrast, etc.)
        builder.Services.AddSingleton<IAccessibilitySettingsService, IosAccessibilitySettingsService>();

        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();
        host.InitializePlusUi();
        host.Start();
        return host;
    }
}
