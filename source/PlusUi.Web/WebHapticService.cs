using Microsoft.JSInterop;
using PlusUi.core;

namespace PlusUi.Web;

/// <summary>
/// Web implementation of haptic feedback using the Vibration API.
/// Note: The Vibration API is primarily supported on mobile browsers.
/// Desktop browsers generally don't support haptic feedback.
/// </summary>
public class WebHapticService(IJSRuntime jsRuntime) : IHapticService
{
    private bool? _isSupported;

    /// <summary>
    /// Checks if the Vibration API is supported in the current browser.
    /// This is typically only true on mobile devices.
    /// </summary>
    public bool IsSupported
    {
        get
        {
            if (_isSupported.HasValue)
            {
                return _isSupported.Value;
            }

            // Check support asynchronously on first access
            _ = CheckSupportAsync();
            return false; // Return false until we know for sure
        }
    }

    /// <summary>
    /// Emits haptic feedback based on the specified feedback type.
    /// </summary>
    public void Emit(HapticFeedback feedback)
    {
        var feedbackType = feedback switch
        {
            HapticFeedback.Light => "light",
            HapticFeedback.Medium => "medium",
            HapticFeedback.Heavy => "heavy",
            HapticFeedback.Selection => "selection",
            HapticFeedback.Success => "success",
            HapticFeedback.Warning => "warning",
            HapticFeedback.Error => "error",
            _ => "medium"
        };

        _ = jsRuntime.InvokeVoidAsync("PlusUiInterop.haptics.emit", feedbackType);
    }

    private async Task CheckSupportAsync()
    {
        try
        {
            _isSupported = await jsRuntime.InvokeAsync<bool>("PlusUiInterop.haptics.isSupported");
        }
        catch
        {
            _isSupported = false;
        }
    }
}
