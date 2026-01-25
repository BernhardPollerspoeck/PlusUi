using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.h264;
using PrereleaseVideo.ViewModels;

public class VideoApp : IVideoAppConfiguration
{
    public void ConfigureVideo(VideoConfiguration config)
    {
        config.Width = 1920;
        config.Height = 1080;
        config.OutputFilePath = "prerelease-video.mp4";
        config.FrameRate = 60;
        config.Duration = TimeSpan.FromSeconds(55);
    }

    public void ConfigureApp(IPlusUiAppBuilder builder)
    {
        builder.AddPage<IntroPage>().WithViewModel<IntroViewModel>();
        builder.AddPage<PlatformsPage>().WithViewModel<PlatformsViewModel>();
        builder.AddPage<ControlsPage>().WithViewModel<ControlsViewModel>();
        builder.AddPage<DataBindingPage>().WithViewModel<DataBindingViewModel>();
        builder.AddPage<FluentApiPage>().WithViewModel<FluentApiViewModel>();
        builder.AddPage<StylingPage>().WithViewModel<StylingViewModel>();
        builder.AddPage<ThemingPage>().WithViewModel<ThemingViewModel>();
        builder.AddPage<AccessibilityPage>().WithViewModel<AccessibilityViewModel>();
        builder.AddPage<HotReloadPage>().WithViewModel<HotReloadViewModel>();
        builder.AddPage<DebugServerPage>().WithViewModel<DebugServerViewModel>();
        builder.AddPage<OutroPage>().WithViewModel<OutroViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IntroPage>();
    }

    public IAudioSequenceProvider? GetAudioSequenceProvider(IServiceProvider serviceProvider)
    {
        return null;
    }
}
