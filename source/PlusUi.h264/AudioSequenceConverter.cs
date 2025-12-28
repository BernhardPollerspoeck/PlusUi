namespace PlusUi.h264;

internal class AudioSequenceConverter
{
    public string GetComplexFilter(IEnumerable<AudioDefinition> audioDefinitions)
    {
        if (audioDefinitions == null || !audioDefinitions.Any())
        {
            return string.Empty;
        }
        var filterParts = new List<string>();
        var delayParts = new List<string>();
        int index = 0;
        foreach (var audio in audioDefinitions)
        {
            var delay = (int)audio.StartTime.TotalMilliseconds;
            filterParts.Add($"[{index + 1}:a]adelay={delay}|{delay}[a{index}]");
            delayParts.Add($"[a{index}]");
            index++;
        }
        var mixPart = string.Join("", delayParts);
        return $"{string.Join(";", filterParts)};{mixPart}amix=inputs={index}[aout]";
    }
}
