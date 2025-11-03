using Microsoft.JSInterop;
using PlusUi.core;
using PlusUi.core.Services;

namespace PlusUi.Web;

/// <summary>
/// Web platform service implementation for Blazor WebAssembly
/// </summary>
public class WebPlatformService : IPlatformService
{
    private readonly RenderService _renderService;
    private readonly IJSRuntime _jsRuntime;
    private Size _windowSize;

    public WebPlatformService(RenderService renderService, IJSRuntime jsRuntime)
    {
        _renderService = renderService;
        _jsRuntime = jsRuntime;
        _windowSize = new Size(0, 0);
    }

    /// <summary>
    /// Updates the window size (called when browser window is resized)
    /// </summary>
    public void SetWindowSize(float width, float height)
    {
        _windowSize = new Size(width, height);
    }

    public PlatformType Platform => PlatformType.Web;

    public Size WindowSize => _windowSize;

    public float DisplayDensity => _renderService.DisplayDensity;

    public bool OpenUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        try
        {
            // Use JavaScript to open URL in new tab
            _jsRuntime.InvokeVoidAsync("open", url, "_blank");
            return true;
        }
        catch
        {
            return false;
        }
    }
}
