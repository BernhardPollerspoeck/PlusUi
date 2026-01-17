using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.Demo;
using PlusUi.droid;

namespace PlusUi.Demo.Android;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : PlusUiActivity
{
    protected override IAppConfiguration CreateApp(HostApplicationBuilder builder)
    {
        return new App();
    }
}