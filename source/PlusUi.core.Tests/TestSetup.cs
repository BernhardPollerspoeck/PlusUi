using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlusUi.core;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.core.Services.Focus;

namespace PlusUi.core.Tests;

[TestClass]
public static class TestSetup
{
    [AssemblyInitialize]
    public static void Initialize(TestContext context)
    {
        var services = new ServiceCollection();

        // Configuration
        var config = new PlusUiConfiguration();
        services.AddSingleton(config);
        services.AddSingleton<IOptions<PlusUiConfiguration>>(new OptionsWrapper<PlusUiConfiguration>(config));

        // Core services
        services.AddSingleton<Style>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        services.AddSingleton<IPaintRegistryService, PaintRegistryService>();
        services.AddSingleton<IImageLoaderService, ImageLoaderService>();
        services.AddSingleton<IImageExportService, ImageExportService>();

        // Navigation
        services.AddSingleton(sp => new NavigationContainer(config));
        services.AddSingleton(sp => new PlusUiNavigationService(sp));
        services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<PlusUiNavigationService>());

        // Popup/Overlay
        services.AddSingleton(sp => TimeProvider.System);
        services.AddSingleton<PlusUiPopupService>();
        services.AddSingleton<IPopupService>(sp => sp.GetRequiredService<PlusUiPopupService>());
        services.AddTransient<IPopupConfiguration, PopupConfiguration>();
        services.AddSingleton<OverlayService>();
        services.AddSingleton<IOverlayService>(sp => sp.GetRequiredService<OverlayService>());
        services.AddSingleton<TooltipService>();
        services.AddSingleton<ITooltipService>(sp => sp.GetRequiredService<TooltipService>());

        // Transition/Animation
        services.AddSingleton<TransitionService>();
        services.AddSingleton<ITransitionService>(sp => sp.GetRequiredService<TransitionService>());

        // Input - Note: RadioButtonManager not registered globally to avoid test interference
        // Tests that need it create their own instance
        services.AddSingleton<FocusManager>();
        services.AddSingleton<IFocusManager>(sp => sp.GetRequiredService<FocusManager>());

        // Accessibility
        services.AddSingleton<IAccessibilityBridge, NoOpAccessibilityBridge>();
        services.AddSingleton<AccessibilityService>();
        services.AddSingleton<IAccessibilityService>(sp => sp.GetRequiredService<AccessibilityService>());
        services.AddSingleton<IAccessibilitySettingsService, AccessibilitySettingsService>();

        var serviceProvider = services.BuildServiceProvider();
        _ = new ServiceProviderService(serviceProvider);
    }
}
