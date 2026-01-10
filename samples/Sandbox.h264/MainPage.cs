using PlusUi.core;
using PlusUi.h264;
using SkiaSharp;

namespace Sandbox.h264;

public class TestControl(MainPageViewModel vm) : UserControl
{
    protected override UiElement Build()
    {
        return new Label()
                    .BindText(() => $"PlusUi: {(int)vm.Timestamp.TotalSeconds}.{vm.Timestamp.Milliseconds:000}")
                    .SetTextSize(90)
                    .SetVerticalAlignment(VerticalAlignment.Center);
    }
}

public class MainPage(
    MainPageViewModel vm)
    : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return
            new Grid()
                .AddColumn(Column.Absolute, 100)
                .AddColumn(Column.Auto, 1)
                .AddChild(new Solid()
                    .SetBackground(new SolidColorBackground(Colors.Red))
                    .BindDesiredHeight(() => vm.Size)
                    .BindDesiredWidth(() => vm.Size)
                    .BindCornerRadius(() => vm.Size / 2)
                    .SetHorizontalAlignment(HorizontalAlignment.Center)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                )
                .AddChild(new TestControl(vm)
                , column: 1)
                .SetDesiredHeight(100)
                .SetHorizontalAlignment(HorizontalAlignment.Center)

            ;
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
