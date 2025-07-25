﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.h264;

namespace Sandbox.h264;
public class VideoApp : IVideoAppConfiguration
{
    public void ConfigureApp(HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();
        //builder.Services.AddSingleton<IAudioSequenceProvider>(sp => sp.GetRequiredService<MainPage>());

        builder.Services.AddSingleton<IApplicationStyle, ApplicationStyle>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }

    public IAudioSequenceProvider? GetAudioSequenceProvider(IServiceProvider serviceProvider)
    {
        return null;// serviceProvider.GetRequiredService<MainPage>();
    }

    public void ConfigureVideo(VideoConfiguration videoConfiguration)
    {
        videoConfiguration.Width = 800;
        videoConfiguration.Height = 100;
        videoConfiguration.OutputFilePath = "../output.mp4";
        videoConfiguration.FrameRate = 60;
        videoConfiguration.Duration = TimeSpan.FromSeconds(11);
    }

}
