using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.ios;

namespace Sandbox.ios;

[Register("AppDelegate")]
public class AppDelegate : PlusUiAppDelegate
{
    protected override IAppConfiguration CreateApp(HostApplicationBuilder builder)
    {
        return new App();
    }
}