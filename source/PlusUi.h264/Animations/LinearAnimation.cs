namespace PlusUi.h264.Animations;

internal class LinearAnimation(TimeProvider timeProvider)
    : IAnimation
{
    public float GetLoopValue(float min, float max, TimeSpan duration)
    {
        var elapsed = timeProvider.GetUtcNow().TimeOfDay.TotalMilliseconds % duration.TotalMilliseconds;
        var progress = (float)(elapsed / duration.TotalMilliseconds);
        var loopProgress = progress < 0.5f ? progress * 2 : 2 - (progress * 2);        
        return min + ((max - min) * loopProgress);
    }
    public bool IsLoopValueAboveThreshold(float min, float max, TimeSpan duration, float value)
    {
        var loopValue = GetLoopValue(min, max, duration);
        return loopValue >= value;
    }

    public float GetSegmentValue(float min, float max, TimeSpan duration, TimeSpan segmentStart, float outsideSegmentValue)
    {
        var elapsed = timeProvider.GetUtcNow().TimeOfDay - segmentStart;
        if (elapsed < TimeSpan.Zero || elapsed > duration)
        {
            return outsideSegmentValue;
        }
        var progress = (float)(elapsed.TotalMilliseconds / duration.TotalMilliseconds);
        return min + ((max - min) * progress);
    }
    public bool IsSegmentValueAboveThreshold(float min, float max, TimeSpan duration, TimeSpan segmentStart, float outsideSegmentValue, float value)
    {
        var segmentValue = GetSegmentValue(min, max, duration, segmentStart, outsideSegmentValue);
        return segmentValue >= value;
    }
}
