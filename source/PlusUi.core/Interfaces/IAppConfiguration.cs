using Microsoft.Extensions.Hosting;

namespace PlusUi.core;

public interface IAppConfiguration
{
    void ConfigureWindow(PlusUiConfiguration configuration);
    void ConfigureApp(HostApplicationBuilder builder);
}