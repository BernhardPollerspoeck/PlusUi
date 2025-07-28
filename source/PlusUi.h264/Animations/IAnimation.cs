namespace PlusUi.h264.Animations;
public interface IAnimation
{
    float GetLoopValue(float start, float end, TimeSpan duration);
    bool IsLoopValueAboveThreshold(float start, float end, TimeSpan duration, float value);

    float GetSegmentValue(float start, float end, TimeSpan duration, TimeSpan segmentStart, float outsideSegmentValue);
    bool IsSegmentValueAboveThreshold(float start, float end, TimeSpan duration, TimeSpan segmentStart, float outsideSegmentValue, float value);
}
