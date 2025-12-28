namespace PlusUi.core;

public interface IHapticService
{
    void Emit(HapticFeedback feedback);
    bool IsSupported { get; }
}
