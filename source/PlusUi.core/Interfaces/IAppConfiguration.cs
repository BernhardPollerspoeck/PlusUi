using Microsoft.Extensions.Hosting;

namespace PlusUi.core;

public interface IAppConfiguration
{
    void ConfigureWindow(PlusUiConfiguration configuration);
    void ConfigureApp(HostApplicationBuilder builder);
    UiPageElement GetRootPage(IServiceProvider serviceProvider);//TODO: only return type instead of instance => navigation uses type for serviceProvider to create instance
}