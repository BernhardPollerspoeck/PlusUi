using Android.Opengl;
using Android.OS;
using Android.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlusUi.core;
using PlusUi.core.Services;
using PlusUi.core.Services.Accessibility;
using PlusUi.droid.Accessibility;

namespace PlusUi.droid;

public abstract class PlusUiActivity : Activity
{
    private GLSurfaceView? _glSurfaceView;
    private IHost? _host;

    protected abstract IAppConfiguration CreateApp(HostApplicationBuilder builder);


    protected override void OnCreate(Bundle? savedInstanceState)
    {
        _host = CreateAndStartHost();

        RequestWindowFeature(WindowFeatures.NoTitle);

        base.OnCreate(savedInstanceState);

        if (OperatingSystem.IsAndroidVersionAtLeast(21) && !OperatingSystem.IsAndroidVersionAtLeast(35))
        {
            Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FF5722"));
        }

        _glSurfaceView = new GLSurfaceView(this);
        _glSurfaceView.SetEGLContextClientVersion(3); // OpenGL ES 3.0

        var renderer = _host.Services.GetRequiredService<SilkRenderer>();
        _glSurfaceView.SetRenderer(renderer);

        // Enable on-demand rendering instead of continuous (battery optimization)
        _glSurfaceView.RenderMode = Rendermode.WhenDirty;

        // Give renderer reference to surface view for RequestRender() calls
        renderer.SetView(_glSurfaceView);

        _glSurfaceView.SetOnTouchListener(_host.Services.GetRequiredService<TapGestureListener>());



        var frameLayout = new FrameLayout(this);
        frameLayout.AddView(_glSurfaceView, new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent));

        var keyCapture = _host.Services.GetRequiredService<KeyCaptureEditText>();
        frameLayout.AddView(keyCapture, new FrameLayout.LayoutParams(
            1, 1));

        SetContentView(frameLayout);

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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_host is not null)
        {
            var keyboardHandler = _host.Services.GetRequiredService<IKeyboardHandler>();
            keyboardHandler.Hide();
            _host.StopAsync().Wait();
            _host.Dispose();
        }

    }

    private IHost CreateAndStartHost()
    {
        var builder = Host.CreateApplicationBuilder();

        var app = CreateApp(builder);

        builder.UsePlusUiInternal(app, []);

        if (ApplicationContext is null)
        {
            throw new InvalidOperationException("ApplicationContext is null");
        }
        builder.Services.AddSingleton<Activity>(this);
        builder.Services.AddSingleton(ApplicationContext);
        builder.Services.AddSingleton<AndroidPlatformService>();
        builder.Services.AddSingleton<IPlatformService>(sp => sp.GetRequiredService<AndroidPlatformService>());
        builder.Services.AddSingleton<SilkRenderer>();
        builder.Services.AddSingleton<AndroidHapticService>();
        builder.Services.AddSingleton<IHapticService>(sp => sp.GetRequiredService<AndroidHapticService>());
        builder.Services.AddSingleton<TapGestureListener>(sp =>
            new TapGestureListener(
                sp.GetRequiredService<InputService>(),
                sp.GetRequiredService<RenderService>(),
                sp.GetRequiredService<Android.Content.Context>()));
        builder.Services.AddSingleton<KeyCaptureEditText>();
        builder.Services.AddSingleton<IKeyboardHandler>(sp => sp.GetRequiredService<KeyCaptureEditText>());

        // Register Android accessibility bridge (TalkBack support)
        builder.Services.AddSingleton<IAccessibilityBridge, AndroidAccessibilityBridge>();

        // Register Android accessibility settings service (font scaling, high contrast, etc.)
        builder.Services.AddSingleton<IAccessibilitySettingsService>(sp =>
            new AndroidAccessibilitySettingsService(sp.GetRequiredService<Android.Content.Context>()));

        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();
        host.InitializePlusUi();
        host.Start();
        return host;
    }

}
