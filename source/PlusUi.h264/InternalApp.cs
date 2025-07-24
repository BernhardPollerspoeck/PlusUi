using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace PlusUi.h264;

internal class InternalApp(IVideoAppConfiguration videoAppConfiguration) : IAppConfiguration
{
    public void ConfigureApp(HostApplicationBuilder builder)
    {
        videoAppConfiguration.ConfigureApp(builder);
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return videoAppConfiguration.GetRootPage(serviceProvider);
    }

    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
    }
}
