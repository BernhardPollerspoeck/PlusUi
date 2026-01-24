using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.h264;

public class VideoApp : IVideoAppConfiguration
{
    public void ConfigureVideo(VideoConfiguration config)
    {
        config.Width = 1920;
        config.Height = 1080;
        config.OutputFilePath = "prerelease-video.mp4";
        config.FrameRate = 60;
        config.Duration = TimeSpan.FromMinutes(20);
    }

    public void ConfigureApp(IPlusUiAppBuilder builder)
    {
        // Pages
        builder.AddPage<IntroPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<PlatformsPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<ControlsPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<DataBindingPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<FluentApiPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<StylingPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<ThemingPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<AccessibilityPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<HotReloadPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<DebugServerPage>().WithViewModel<PlaceholderViewModel>();
        builder.AddPage<OutroPage>().WithViewModel<PlaceholderViewModel>();
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
