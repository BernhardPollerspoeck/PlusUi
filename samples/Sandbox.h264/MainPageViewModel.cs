using CommunityToolkit.Mvvm.ComponentModel;
using PlusUi.core.Services;

namespace Sandbox.h264;

public partial class MainPageViewModel(
    IApplicationTimeProvider timeProvider)
    : ObservableObject
{

    public TimeSpan Timestamp => GetTimestamp();

    private TimeSpan GetTimestamp()
    {
        var val = TimeSpan.FromSeconds(10) - timeProvider.Now.TimeOfDay;
        return val < TimeSpan.Zero ? TimeSpan.Zero : val;
    }

}