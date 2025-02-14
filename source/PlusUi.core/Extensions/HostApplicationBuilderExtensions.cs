using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core.CoreElements;
using PlusUi.core.Services;
using PlusUi.core.ViewModel;

namespace PlusUi.core.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static HostApplicationBuilder UsePlusUi<TRootPage>(this HostApplicationBuilder builder)
        where TRootPage : UiPageElement
    {
        return builder.UsePlusUi<TRootPage>(_ => { });
    }
    public static HostApplicationBuilder UsePlusUi<TRootPage>(
        this HostApplicationBuilder builder,
        Action<PlusUiConfiguration> configurationAction)
        where TRootPage : UiPageElement
    {
        builder.Services.Configure(configurationAction);

        builder.Services.AddSingleton<RenderService>();
        builder.Services.AddSingleton<UpdateService>();

        builder.Services.AddHostedService<WindowManager>();

        builder.Services.AddSingleton(sp => new CurrentPage
        {
            Page = sp.GetRequiredService<TRootPage>()
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
}
