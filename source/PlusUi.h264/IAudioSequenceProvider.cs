namespace PlusUi.h264;

public interface IAudioSequenceProvider
{
    IEnumerable<AudioDefinition> GetAudioSequence();
}
