using Microsoft.Extensions.DependencyInjection;
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
    public static HostApplicationBuilder UsePlusUiInternal<TRootPage>(
        this HostApplicationBuilder builder)
        where TRootPage : UiPageElement
    {
        builder.Services.AddSingleton<ServiceProviderService>();

        builder.Services.AddSingleton<RenderService>();
        builder.Services.AddSingleton<UpdateService>();
        
        builder.Services.AddSingleton<Style>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddHostedService<StartupStyleService>();

        builder.Services.AddSingleton(sp => new PlusUiNavigationService(sp));
        builder.Services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<PlusUiNavigationService>());

        builder.Services.AddSingleton(sp => new NavigationContainer(sp.GetRequiredService<TRootPage>()));
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

    public static HostApplicationBuilder StylePlusUi<TApplicationStyle>(this HostApplicationBuilder builder)
        where TApplicationStyle : class, IApplicationStyle
    {
        builder.Services.AddSingleton<IApplicationStyle, TApplicationStyle>();
        return builder;
    }
}
