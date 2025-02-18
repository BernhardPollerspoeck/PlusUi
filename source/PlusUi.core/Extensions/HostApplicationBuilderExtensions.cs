using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PlusUi.core;

public static class HostApplicationBuilderExtensions
{
    public static HostApplicationBuilder UsePlusUiInternal<TRootPage>(
        this HostApplicationBuilder builder,
        Action<PlusUiConfiguration> configurationAction)
        where TRootPage : UiPageElement
    {
        builder.Services.Configure(configurationAction);

        builder.Services.AddSingleton<RenderService>();
        builder.Services.AddSingleton<UpdateService>();

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
}
