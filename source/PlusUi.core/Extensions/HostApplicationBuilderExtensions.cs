﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        Type mainPageType)
    {
        builder.Services.AddSingleton<ServiceProviderService>();
        builder.Services.AddSingleton<RenderService>();
        builder.Services.AddSingleton<InputService>();

        builder.Services.AddSingleton<Style>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddHostedService<StartupStyleService>();
        builder.Services.AddSingleton(sp => new PlusUiNavigationService(sp));
        builder.Services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<PlusUiNavigationService>());

        builder.Services.AddSingleton<PlusUiPopupService>();
        builder.Services.AddSingleton<IPopupService>(sp => sp.GetRequiredService<PlusUiPopupService>());
        builder.Services.AddTransient<IPopupConfiguration, PopupConfiguration>();

        builder.Services.AddSingleton(sp =>
        {
            var mainPage = sp.GetRequiredService(mainPageType) as UiPageElement
                ?? throw new Exception("MainPage not found");
            return new NavigationContainer(mainPage);
        });
        return builder;

    }


    public static HostApplicationBuilder AddPage<TPage>(this HostApplicationBuilder builder)
        where TPage : UiPageElement
    {
        builder.Services.AddTransient<TPage>();
        return builder;
    }
    public static HostApplicationBuilder WithViewModel<TViewModel>(this HostApplicationBuilder builder)
        where TViewModel : ViewModelBase
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
}
