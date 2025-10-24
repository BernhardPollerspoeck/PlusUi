using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace PlusUi.h264;

public interface IVideoAppConfiguration
{
    void ConfigureVideo(VideoConfiguration videoConfiguration);
    void ConfigureApp(HostApplicationBuilder builder);
    UiPageElement GetRootPage(IServiceProvider serviceProvider);
    IAudioSequenceProvider? GetAudioSequenceProvider(IServiceProvider serviceProvider);
}
