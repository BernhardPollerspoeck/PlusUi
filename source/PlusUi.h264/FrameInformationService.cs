using Microsoft.Extensions.Options;
using PlusUi.core.Services;

namespace PlusUi.h264;

internal class FrameInformationService(
    IOptions<VideoConfiguration> videoOptions,
    ICommandLineService commandLineService)
{
    public int CurrentFrame { get; private set; } = 0;

    internal IEnumerable<int> GetNextFrame()
    {
        //special case: if args has the flag --frameOutput and also we get a value for --frameTimestamp
        //then we only return that 1 specific frame and are done
        if (commandLineService.HasFlag("--frameOutput"))
        {
            var frameTimestamp = commandLineService.GetOptionValue("--frameTimestamp");
            if(frameTimestamp is not null && TimeSpan.TryParseExact(frameTimestamp, "c", null, out var parsedTimestamp))
            {
                // Convert TimeSpan to frame number
                var frameNumber = (int)(parsedTimestamp.TotalSeconds * videoOptions.Value.FrameRate);
                CurrentFrame = frameNumber;
                yield return CurrentFrame;
                yield break; // Exit after yielding the specific frame
            }
        }


        var totalFrames = (int)(videoOptions.Value.FrameRate * videoOptions.Value.Duration.TotalSeconds);

        while (CurrentFrame < totalFrames)
        {
            yield return CurrentFrame++;
        }

        // Reset current frame if we've reached the end
        CurrentFrame = 0;
    }
}
