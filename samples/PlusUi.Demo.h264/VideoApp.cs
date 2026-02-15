using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.h264;

namespace Sandbox.h264;
public class VideoApp : IVideoAppConfiguration
{
    public void ConfigureApp(IPlusUiAppBuilder builder)
    {
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();
        //builder.Services.AddSingleton<IAudioSequenceProvider>(sp => sp.GetRequiredService<MainPage>());
        builder.Services.AddSingleton<IVideoOverlayProvider, DemoVideoOverlayProvider>();

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

    public IVideoOverlayProvider? GetVideoOverlayProvider(IServiceProvider serviceProvider)
    {
        return new DemoVideoOverlayProvider();
    }

    public void ConfigureVideo(VideoConfiguration videoConfiguration)
    {
        videoConfiguration.Width = 800;
        videoConfiguration.Height = 450;
        videoConfiguration.OutputFilePath = "../output.mp4";
        videoConfiguration.FrameRate = 60;
        videoConfiguration.Duration = TimeSpan.FromSeconds(11);
    }

}
