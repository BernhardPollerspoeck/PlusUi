using Android.Opengl;
using Android.OS;
using Android.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;

namespace PlusUi.droid;

public abstract class PlusUiActivity : Activity
{
    private GLSurfaceView? _glSurfaceView;
    private IHost? _host;

    protected abstract IAppConfiguration CreateApp(HostApplicationBuilder builder);

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        //TODO: SetupInputHandling();
        _host = CreateAndStartHost();

        RequestWindowFeature(WindowFeatures.NoTitle);

        base.OnCreate(savedInstanceState);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CA1422 // Validate platform compatibility
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FF5722"));
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore IDE0079 // Remove unnecessary suppression
        }

        _glSurfaceView = new GLSurfaceView(this);
        _glSurfaceView.SetEGLContextClientVersion(3); // OpenGL ES 3.0
        _glSurfaceView.SetRenderer(_host.Services.GetRequiredService<SilkRenderer>());

        SetContentView(_glSurfaceView);
    }

    protected override void OnPause()
    {
        base.OnPause();
        _glSurfaceView?.OnPause();
    }

    protected override void OnResume()
    {
        base.OnResume();
        _glSurfaceView?.OnResume();
    }

    private IHost CreateAndStartHost()
    {
        var builder = Host.CreateApplicationBuilder();

        var app = CreateApp(builder);
        var rootPage = app.ConfigureRootPage();

        builder.UsePlusUiInternal(rootPage);

        if (ApplicationContext is null)
        {
            throw new InvalidOperationException("ApplicationContext is null");
        }
        builder.Services.AddSingleton(ApplicationContext);
        builder.Services.AddSingleton<SilkRenderer>();

        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();
        host.Start();
        return host;
    }

}
