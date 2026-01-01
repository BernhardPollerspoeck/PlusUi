using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core.CoreElements;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.core.Services.Focus;
using System.ComponentModel;

namespace PlusUi.core;

/// <summary>
/// Default wrapper for HostApplicationBuilder implementing IPlusUiAppBuilder.
/// Used by Desktop, Headless, h264, iOS, and Android platforms.
/// </summary>
public class HostAppBuilder(HostApplicationBuilder builder) : IPlusUiAppBuilder
{
    public IServiceCollection Services => builder.Services;
    public HostApplicationBuilder Inner => builder;
}

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

public static class HostApplicationBuilderExtensions
{
    public static HostApplicationBuilder ConfigurePlusUiApp(
        this HostApplicationBuilder builder,
        IAppConfiguration appConfiguration)
    {
        builder.Services.Configure<PlusUiConfiguration>(appConfiguration.ConfigureWindow);
        appConfiguration.ConfigureApp(new HostAppBuilder(builder));
        return builder;
    }

    public static HostApplicationBuilder UsePlusUiInternal(
        this HostApplicationBuilder builder,
        IAppConfiguration appConfiguration,
        string[] args)
    {
        builder.Services.AddPlusUiCore(appConfiguration, args);
        return builder;
    }
}

/// <summary>
/// Extension methods for IPlusUiAppBuilder used in app configuration.
/// </summary>
public static class PlusUiAppBuilderExtensions
{
    public static IPlusUiAppBuilder AddPage<TPage>(this IPlusUiAppBuilder builder)
        where TPage : UiPageElement
    {
        builder.Services.AddTransient<TPage>();
        return builder;
    }

    public static IPlusUiAppBuilder WithViewModel<TViewModel>(this IPlusUiAppBuilder builder)
        where TViewModel : class, INotifyPropertyChanged
    {
        builder.Services.AddTransient<TViewModel>();
        return builder;
    }

    public static IPlusUiAppBuilder AddPopup<TPopup>(this IPlusUiAppBuilder builder)
       where TPopup : UiPopupElement
    {
        builder.Services.AddTransient<TPopup>();
        return builder;
    }

    public static IPlusUiAppBuilder StylePlusUi<TApplicationStyle>(this IPlusUiAppBuilder builder)
        where TApplicationStyle : class, IApplicationStyle
    {
        builder.Services.AddSingleton<IApplicationStyle, TApplicationStyle>();
        return builder;
    }

    public static IPlusUiAppBuilder RegisterFont(
        this IPlusUiAppBuilder builder,
        string resourcePath,
        string fontFamily,
        FontWeight fontWeight = FontWeight.Regular,
        FontStyle fontStyle = FontStyle.Normal)
    {
        // Store font registrations to be executed when the service provider is built
        builder.Services.AddSingleton<IFontRegistration>(new FontRegistration
        {
            ResourcePath = resourcePath,
            FontFamily = fontFamily,
            FontWeight = fontWeight,
            FontStyle = fontStyle
        });
        return builder;
    }
}
