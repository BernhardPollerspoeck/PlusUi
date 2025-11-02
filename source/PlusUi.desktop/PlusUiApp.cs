using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.core.Services;

namespace PlusUi.desktop;

public class PlusUiApp(string[] args)
{

    public void CreateApp(Func<HostApplicationBuilder, IAppConfiguration> appBuilder)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var app = appBuilder(builder);

        builder.UsePlusUiInternal(app, args);

        builder.Services.AddSingleton<IUrlLauncherService, UrlLauncherService>();
        builder.Services.AddSingleton<DesktopKeyboardHandler>();
        builder.Services.AddSingleton<IKeyboardHandler>(sp => sp.GetRequiredService<DesktopKeyboardHandler>());
        builder.Services.AddHostedService<WindowManager>();

        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();
        host.Run();
    }

}
