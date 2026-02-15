using System.Globalization;

namespace PlusUi.h264;

internal class VideoOverlayFilterBuilder
{
    public record FilterResult(string VideoFilter, string AudioFilter, List<string> AudioLabels);

    public FilterResult BuildFilters(
        IReadOnlyList<VideoOverlayDefinition> overlays,
        int firstInputIndex,
        int outputWidth,
        int outputHeight)
    {
        if (overlays.Count == 0)
            return new FilterResult(string.Empty, string.Empty, []);

        var videoParts = new List<string>();
        var audioParts = new List<string>();
        var audioLabels = new List<string>();

        var baseLabel = "base";
        videoParts.Add($"[0:v]scale={outputWidth}:{outputHeight}[{baseLabel}]");

        for (int i = 0; i < overlays.Count; i++)
        {
            var overlay = overlays[i];
            var inputIdx = firstInputIndex + i;
            var ovLabel = $"ov{i}";
            var nextBaseLabel = i < overlays.Count - 1 ? $"base{i + 1}" : "vout";
            var startSec = overlay.StartTime.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture);

            var videoChain = BuildVideoChain(overlay, inputIdx, ovLabel);
            videoParts.Add(videoChain);

            var dest = overlay.DestRect;
            var enableExpr = overlay.Duration is { } dur
                ? $"between(t,{startSec},{(overlay.StartTime + dur).TotalSeconds.ToString("F3", CultureInfo.InvariantCulture)})"
                : $"gte(t,{startSec})";
            videoParts.Add(
                $"[{baseLabel}][{ovLabel}]overlay={dest.X}:{dest.Y}:enable='{enableExpr}'[{nextBaseLabel}]");

            baseLabel = nextBaseLabel;

            var audioLabel = $"ova{i}";
            var audioChain = BuildAudioChain(overlay, inputIdx, audioLabel);
            if (audioChain is not null)
            {
                audioParts.Add(audioChain);
                audioLabels.Add($"[{audioLabel}]");
            }
        }

        var videoFilter = string.Join(";", videoParts);
        var audioFilter = string.Join(";", audioParts);

        return new FilterResult(videoFilter, audioFilter, audioLabels);
    }

    private static string BuildVideoChain(VideoOverlayDefinition overlay, int inputIdx, string outputLabel)
    {
        var filters = new List<string>();

        if (overlay.SourceRect is { } src)
        {
            filters.Add($"crop={src.Width}:{src.Height}:{src.X}:{src.Y}");
        }

        if (Math.Abs(overlay.PlaybackSpeed - 1.0f) > 0.001f)
        {
            var speed = overlay.PlaybackSpeed.ToString("F3", CultureInfo.InvariantCulture);
            filters.Add($"setpts=PTS/{speed}");
        }

        var dest = overlay.DestRect;
        filters.Add($"scale={dest.Width}:{dest.Height}");

        if (overlay.FadeInDuration is { TotalSeconds: > 0 } || overlay.FadeOutDuration is { TotalSeconds: > 0 })
        {
            filters.Add("format=yuva420p");
        }

        if (overlay.FadeInDuration is { TotalSeconds: > 0 } fadeIn)
        {
            var fadeInStart = overlay.StartTime.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture);
            filters.Add($"fade=t=in:st={fadeInStart}:d={fadeIn.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture)}:alpha=1");
        }

        if (overlay is { FadeOutDuration.TotalSeconds: > 0, Duration.TotalSeconds: > 0 })
        {
            var fadeOutSec = overlay.FadeOutDuration!.Value.TotalSeconds;
            var fadeOutStart = (overlay.StartTime.TotalSeconds + overlay.Duration!.Value.TotalSeconds - fadeOutSec)
                .ToString("F3", CultureInfo.InvariantCulture);
            filters.Add($"fade=t=out:st={fadeOutStart}:d={fadeOutSec.ToString("F3", CultureInfo.InvariantCulture)}:alpha=1");
        }

        return $"[{inputIdx}:v]{string.Join(",", filters)}[{outputLabel}]";
    }

    private static string? BuildAudioChain(VideoOverlayDefinition overlay, int inputIdx, string outputLabel)
    {
        if (overlay.Volume <= 0f)
            return null;

        var filters = new List<string>();

        if (Math.Abs(overlay.PlaybackSpeed - 1.0f) > 0.001f)
        {
            var speed = overlay.PlaybackSpeed.ToString("F3", CultureInfo.InvariantCulture);
            filters.Add($"atempo={speed}");
        }

        var delayMs = (int)overlay.StartTime.TotalMilliseconds;
        if (delayMs > 0)
        {
            filters.Add($"adelay={delayMs}|{delayMs}");
        }

        if (Math.Abs(overlay.Volume - 1.0f) > 0.001f)
        {
            var vol = overlay.Volume.ToString("F2", CultureInfo.InvariantCulture);
            filters.Add($"volume={vol}");
        }

        if (overlay.FadeInDuration is { TotalSeconds: > 0 } fadeIn)
        {
            var fadeInStart = overlay.StartTime.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture);
            filters.Add($"afade=t=in:st={fadeInStart}:d={fadeIn.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture)}");
        }

        if (overlay is { FadeOutDuration.TotalSeconds: > 0, Duration.TotalSeconds: > 0 })
        {
            var fadeOutSec = overlay.FadeOutDuration!.Value.TotalSeconds;
            var fadeOutStart = (overlay.StartTime.TotalSeconds + overlay.Duration!.Value.TotalSeconds - fadeOutSec)
                .ToString("F3", CultureInfo.InvariantCulture);
            filters.Add($"afade=t=out:st={fadeOutStart}:d={fadeOutSec.ToString("F3", CultureInfo.InvariantCulture)}");
        }

        if (filters.Count == 0)
            return $"[{inputIdx}:a]anull[{outputLabel}]";

        return $"[{inputIdx}:a]{string.Join(",", filters)}[{outputLabel}]";
    }
}
