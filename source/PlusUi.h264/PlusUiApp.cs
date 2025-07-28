using FFMpegCore.Pipes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlusUi.core;
using PlusUi.h264.Animations;
using System.Threading.Channels;

namespace PlusUi.h264;

public class PlusUiApp(string[] args)
{

    public void CreateApp(Func<HostApplicationBuilder, IVideoAppConfiguration> appBuilder)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var videoApp = appBuilder(builder);
        var app = new InternalApp(videoApp);

        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);

        builder.Services.Configure<VideoConfiguration>(videoApp.ConfigureVideo);

        builder.UsePlusUiInternal(app, args);

        builder.Services.AddSingleton(sp => Channel.CreateUnbounded<IVideoFrame>());
        builder.Services.AddSingleton(sp => sp.GetRequiredService<Channel<IVideoFrame>>().Reader);
        builder.Services.AddSingleton(sp => sp.GetRequiredService<Channel<IVideoFrame>>().Writer);

        builder.Services.AddHostedService<VideoMainHandler>();
        builder.Services.AddSingleton<FrameInformationService>();
        builder.Services.AddSingleton<AudioSequenceConverter>();
        builder.Services.AddHostedService<FrameRenderService>();
        builder.Services.AddSingleton<VideoTimeProvider>();
        builder.Services.AddSingleton<TimeProvider>(sp => sp.GetRequiredService<VideoTimeProvider>());

        builder.Services.AddKeyedTransient<IAnimation, LinearAnimation>(EAnimationType.Linear);

        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();
        host.Run();
    }

}
