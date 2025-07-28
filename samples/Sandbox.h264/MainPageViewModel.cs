using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.h264.Animations;

namespace Sandbox.h264;

public partial class MainPageViewModel(
    TimeProvider timeProvider,
    [FromKeyedServices(EAnimationType.Linear)] IAnimation linearAnimation)
    : ObservableObject
{

    public TimeSpan Timestamp => GetTimestamp();

    private TimeSpan GetTimestamp()
    {
        var val = TimeSpan.FromSeconds(10) - timeProvider.GetUtcNow().TimeOfDay;
        return val < TimeSpan.Zero ? TimeSpan.Zero : val;
    }

    public float Size => linearAnimation.GetLoopValue(80, 100, TimeSpan.FromMilliseconds(1000));

}