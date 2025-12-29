using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core.CoreElements;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.core.Services.Focus;
using System.ComponentModel;

namespace PlusUi.core;

public static class HostApplicationBuilderExtensions
{
    public static HostApplicationBuilder ConfigurePlusUiApp(
        this HostApplicationBuilder builder,
        IAppConfiguration appConfiguration)
    {
        builder.Services.Configure<PlusUiConfiguration>(appConfiguration.ConfigureWindow);
        appConfiguration.ConfigureApp(builder);
        return builder;
    }

    public static HostApplicationBuilder UsePlusUiInternal(
        this HostApplicationBuilder builder,
        IAppConfiguration appConfiguration,
        string[] args)
    {
        builder.Services.AddSingleton(appConfiguration);
        builder.Services.AddSingleton<ICommandLineService>(sp => new CommandLineService(args));
        builder.Services.AddSingleton<ServiceProviderService>();
        builder.Services.AddSingleton<RenderService>();
        builder.Services.AddSingleton<InputService>();

        builder.Services.AddSingleton<Style>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddHostedService<StartupStyleService>();
        builder.Services.AddSingleton(sp => new PlusUiNavigationService(sp));
        builder.Services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<PlusUiNavigationService>());

        builder.Services.AddSingleton<IFontRegistryService, FontRegistryService>();
        builder.Services.AddSingleton<IImageLoaderService, ImageLoaderService>();
        builder.Services.AddSingleton<IImageExportService, ImageExportService>();

        builder.Services.AddSingleton(sp => TimeProvider.System);
        builder.Services.AddSingleton<PlusUiPopupService>();
        builder.Services.AddSingleton<IPopupService>(sp => sp.GetRequiredService<PlusUiPopupService>());
        builder.Services.AddTransient<IPopupConfiguration, PopupConfiguration>();

        builder.Services.AddSingleton<OverlayService>();
        builder.Services.AddSingleton<IOverlayService>(sp => sp.GetRequiredService<OverlayService>());

        builder.Services.AddSingleton<TooltipService>();
        builder.Services.AddSingleton<ITooltipService>(sp => sp.GetRequiredService<TooltipService>());

        builder.Services.AddSingleton<TransitionService>();
        builder.Services.AddSingleton<ITransitionService>(sp => sp.GetRequiredService<TransitionService>());

        builder.Services.AddSingleton<RadioButtonManager>();
        builder.Services.AddSingleton<IRadioButtonManager>(sp => sp.GetRequiredService<RadioButtonManager>());

        builder.Services.AddSingleton<FocusManager>();
        builder.Services.AddSingleton<IFocusManager>(sp => sp.GetRequiredService<FocusManager>());

        // Accessibility services - defaults can be overridden by platform implementations
        builder.Services.AddSingleton<IAccessibilityBridge, NoOpAccessibilityBridge>();
        builder.Services.AddSingleton<AccessibilityService>();
        builder.Services.AddSingleton<IAccessibilityService>(sp => sp.GetRequiredService<AccessibilityService>());
        builder.Services.AddSingleton<IAccessibilitySettingsService, AccessibilitySettingsService>();

        builder.Services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PlusUiConfiguration>>().Value;
            return new NavigationContainer(configuration);
        });

        // Expose PlusUiConfiguration for navigation service
        builder.Services.AddSingleton(sp =>
            sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PlusUiConfiguration>>().Value);

        return builder;

    }


    public static HostApplicationBuilder AddPage<TPage>(this HostApplicationBuilder builder)
        where TPage : UiPageElement
    {
        builder.Services.AddTransient<TPage>();
        return builder;
    }
    public static HostApplicationBuilder WithViewModel<TViewModel>(this HostApplicationBuilder builder)
        where TViewModel : class, INotifyPropertyChanged
    {
        builder.Services.AddTransient<TViewModel>();
        return builder;
    }
    public static HostApplicationBuilder AddPopup<TPopup>(this HostApplicationBuilder builder)
       where TPopup : UiPopupElement
    {
        builder.Services.AddTransient<TPopup>();
        return builder;
    }

    public static HostApplicationBuilder StylePlusUi<TApplicationStyle>(this HostApplicationBuilder builder)
        where TApplicationStyle : class, IApplicationStyle
    {
        builder.Services.AddSingleton<IApplicationStyle, TApplicationStyle>();
        return builder;
    }

    public static HostApplicationBuilder RegisterFont(
        this HostApplicationBuilder builder,
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
