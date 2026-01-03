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


        Window = new UIWindow(UIScreen.MainScreen.Bounds)
        {
            RootViewController = _host.Services.GetRequiredService<OpenGlViewController>()
        };
        Window.MakeKeyAndVisible();
        return true;
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
