using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.Demo;
using PlusUi.ios;

namespace PlusUi.Demo.ios;

[Register("AppDelegate")]
public class AppDelegate : PlusUiAppDelegate
{
    protected override IAppConfiguration CreateApp(HostApplicationBuilder builder)
    {
        return new App();
    }
}