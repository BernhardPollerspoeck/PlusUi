using CommunityToolkit.Mvvm.ComponentModel;

namespace Sandbox.h264;

public partial class MainPageViewModel(
    TimeProvider timeProvider)
    : ObservableObject
{

    public TimeSpan Timestamp => GetTimestamp();

    private TimeSpan GetTimestamp()
    {
        var val = TimeSpan.FromSeconds(10) - timeProvider.GetUtcNow().TimeOfDay;
        return val < TimeSpan.Zero ? TimeSpan.Zero : val;
    }

}