namespace PlusUi.h264;

internal class AudioSequenceConverter
{
    public string GetComplexFilter(
        IEnumerable<AudioDefinition> audioDefinitions,
        List<string>? additionalAudioLabels = null,
        string? additionalAudioFilter = null)
    {
        var filterParts = new List<string>();
        var mixLabels = new List<string>();
        int index = 0;

        if (audioDefinitions != null)
        {
            foreach (var audio in audioDefinitions)
            {
                var delay = (int)audio.StartTime.TotalMilliseconds;
                var volFilter = Math.Abs(audio.Volume - 1.0f) > 0.001f
                    ? $",volume={audio.Volume:F2}"
                    : "";
                filterParts.Add($"[{index + 1}:a]adelay={delay}|{delay}{volFilter}[a{index}]");
                mixLabels.Add($"[a{index}]");
                index++;
            }
        }

        if (!string.IsNullOrEmpty(additionalAudioFilter))
        {
            filterParts.Add(additionalAudioFilter);
        }

        if (additionalAudioLabels is not null)
        {
            mixLabels.AddRange(additionalAudioLabels);
        }

        if (mixLabels.Count == 0)
            return string.Empty;

        var mixPart = string.Join("", mixLabels);
        var totalInputs = mixLabels.Count;
        return $"{string.Join(";", filterParts)};{mixPart}amix=inputs={totalInputs}[aout]";
    }
}
