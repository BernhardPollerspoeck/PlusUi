using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace PlusUi.desktop;

public class PlusUiApp(string[] args)
{

    public void CreateApp(Func<HostApplicationBuilder, IAppConfiguration> appBuilder)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var app = appBuilder(builder);
        var rootPage = app.ConfigureRootPage();

        builder.UsePlusUiInternal(rootPage);
        
        builder.Services.AddSingleton<DesktopKeyboardHandler>();
        builder.Services.AddSingleton<IKeyboardHandler>(sp => sp.GetRequiredService<DesktopKeyboardHandler>());
        builder.Services.AddHostedService<WindowManager>();

        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();
        host.Run();
    }

}
