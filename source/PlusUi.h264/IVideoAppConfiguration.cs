using PlusUi.core;

namespace PlusUi.h264;

public interface IVideoAppConfiguration
{
    void ConfigureVideo(VideoConfiguration videoConfiguration);
    void ConfigureApp(IPlusUiAppBuilder builder);
    UiPageElement GetRootPage(IServiceProvider serviceProvider);
    IAudioSequenceProvider? GetAudioSequenceProvider(IServiceProvider serviceProvider);
    IVideoOverlayProvider? GetVideoOverlayProvider(IServiceProvider serviceProvider);
}
