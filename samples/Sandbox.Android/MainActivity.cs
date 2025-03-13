using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.droid;

namespace Sandbox.Android;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : PlusUiActivity
{
    protected override IAppConfiguration CreateApp(HostApplicationBuilder builder)
    {
        return new App();
    }

}