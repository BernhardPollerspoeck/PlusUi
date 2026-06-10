using CommunityToolkit.Mvvm.ComponentModel;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class SliderPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValueText))]
    private float _value = 50f;

    public string ValueText => $"{Value:F0}";
}

public class SliderPage(SliderPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Slider";

    protected override string Description =>
        "Selects a value within a range by dragging the thumb. Stretches to fill the available width.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Default (0-100)",
            new Slider()),

        Section("Interactive",
            new Slider().BindValue(() => vm.Value, v => vm.Value = v),
            new Label().BindText(() => vm.ValueText)),

        Section("Custom range (0-10)",
            new Slider().SetMinimum(0).SetMaximum(10).SetValue(7)),
    ];
}
