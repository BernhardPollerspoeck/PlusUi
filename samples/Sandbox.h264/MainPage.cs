using PlusUi.core;
using PlusUi.h264;

namespace Sandbox.h264;

public class MainPage(
    MainPageViewModel vm)
    : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new HStack(
            new Label()
                .BindText(nameof(vm.Timestamp), () => $"PlusUi: {(int)vm.Timestamp.TotalSeconds}.{vm.Timestamp.Milliseconds:000}")
                .SetTextSize(90))
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);
    }

    public IEnumerable<AudioDefinition> GetAudioSequence()
    {
        yield return new("9.wav", TimeSpan.FromSeconds(0.2), 1.0f);
        yield return new("8.wav", TimeSpan.FromSeconds(1.2), 1.0f);
        yield return new("7.wav", TimeSpan.FromSeconds(2.2), 1.0f);
        yield return new("6.wav", TimeSpan.FromSeconds(3.2), 1.0f);
        yield return new("5.wav", TimeSpan.FromSeconds(4.2), 1.0f);
        yield return new("4.wav", TimeSpan.FromSeconds(5.2), 1.0f);
        yield return new("3.wav", TimeSpan.FromSeconds(6.2), 1.0f);
        yield return new("2.wav", TimeSpan.FromSeconds(7.2), 1.0f);
        yield return new("1.wav", TimeSpan.FromSeconds(8.2), 1.0f);
        yield return new("0.wav", TimeSpan.FromSeconds(9.2), 1.0f);
    }
}
