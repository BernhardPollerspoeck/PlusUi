using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.ios;

namespace PlusUiApp.iOS;

[Register("AppDelegate")]
public class AppDelegate : PlusUiAppDelegate
{
    protected override IAppConfiguration CreateApp(HostApplicationBuilder builder)
    {
        return new App();
    }
}
