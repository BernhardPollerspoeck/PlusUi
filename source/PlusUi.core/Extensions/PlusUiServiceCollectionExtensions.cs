using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Binding;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.core.Services.DebugBridge;
using PlusUi.core.Services.Focus;

namespace PlusUi.core;

/// <summary>
/// Core service registration for PlusUi. Used by all platforms.
/// </summary>
public static class PlusUiServiceCollectionExtensions
{
    /// <summary>
    /// Registers all core PlusUi services on the service collection.
    /// This is the shared implementation used by all platforms.
    /// </summary>
    public static IServiceCollection AddPlusUiCore(
        this IServiceCollection services,
        IAppConfiguration appConfiguration,
        string[] args)
    {
        services.AddSingleton(appConfiguration);
        services.AddSingleton<ICommandLineService>(sp => new CommandLineService(args));
        services.AddSingleton<ServiceProviderService>();
        services.AddSingleton<RenderService>();
        services.AddSingleton<InputService>();

        services.AddSingleton<Style>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton(sp => new PlusUiNavigationService(sp));
        services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<PlusUiNavigationService>());

        services.AddSingleton<IFontRegistryService, FontRegistryService>();
        services.AddSingleton<IExpressionPathService, ExpressionPathService>();
        services.AddSingleton<IPaintRegistryService>(sp =>
        {
            var debugBridgeClient = sp.GetService<DebugBridgeClient>();
            return new PaintRegistryService(debugBridgeClient);
        });
        services.AddSingleton<IImageLoaderService, ImageLoaderService>();
        services.AddSingleton<IImageExportService, ImageExportService>();

        services.AddSingleton(sp => TimeProvider.System);
        services.AddSingleton<PlusUiPopupService>();
        services.AddSingleton<IPopupService>(sp => sp.GetRequiredService<PlusUiPopupService>());
        services.AddTransient<IPopupConfiguration, PopupConfiguration>();

        services.AddSingleton<OverlayService>();
        services.AddSingleton<IOverlayService>(sp => sp.GetRequiredService<OverlayService>());

        services.AddSingleton<TooltipService>();
        services.AddSingleton<ITooltipService>(sp => sp.GetRequiredService<TooltipService>());

        services.AddSingleton<TransitionService>();
        services.AddSingleton<ITransitionService>(sp => sp.GetRequiredService<TransitionService>());

        services.AddSingleton<RadioButtonManager>();
        services.AddSingleton<IRadioButtonManager>(sp => sp.GetRequiredService<RadioButtonManager>());

        services.AddSingleton<FocusManager>();
        services.AddSingleton<IFocusManager>(sp => sp.GetRequiredService<FocusManager>());

        // Accessibility services - defaults can be overridden by platform implementations
        services.AddSingleton<IAccessibilityBridge, NoOpAccessibilityBridge>();
        services.AddSingleton<AccessibilityService>();
        services.AddSingleton<IAccessibilityService>(sp => sp.GetRequiredService<AccessibilityService>());
        services.AddSingleton<IAccessibilitySettingsService, AccessibilitySettingsService>();

        services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PlusUiConfiguration>>().Value;
            return new NavigationContainer(configuration);
        });

        // Expose PlusUiConfiguration for navigation service
        services.AddSingleton(sp =>
            sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PlusUiConfiguration>>().Value);

        return services;
    }
}
