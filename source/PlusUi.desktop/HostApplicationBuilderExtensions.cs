using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace PlusUi.desktop;

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
        builder.UsePlusUiInternal<TRootPage>(configurationAction);

        builder.Services.AddHostedService<WindowManager>();

        return builder;
    }
}
