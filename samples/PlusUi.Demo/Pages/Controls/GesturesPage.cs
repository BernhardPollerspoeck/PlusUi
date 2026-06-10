using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class GesturesPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    [ObservableProperty]
    private string _lastGesture = "Interact with the boxes above";

    [RelayCommand] private void Tap() => LastGesture = "Tapped";
    [RelayCommand] private void DoubleTap() => LastGesture = "Double-tapped";
    [RelayCommand] private void LongPress() => LastGesture = "Long-pressed";
    [RelayCommand] private void Swipe(SwipeDirection direction) => LastGesture = $"Swiped {direction}";
}

public class GesturesPage(GesturesPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Gestures";

    protected override string Description =>
        "Gesture detectors wrap a child element and fire a command on tap, double-tap, long-press, swipe or pinch.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Tap & double-tap",
            new HStack()
                .SetSpacing(12)
                .AddChild(new TapGestureDetector(Box("Tap me")).SetCommand(vm.TapCommand))
                .AddChild(new DoubleTapGestureDetector(Box("Double-tap me")).SetCommand(vm.DoubleTapCommand))),

        Section("Long press",
            new LongPressGestureDetector(Box("Long-press me")).SetCommand(vm.LongPressCommand)),

        Section("Swipe (any direction)",
            Note("Swipe across the square - the detected direction is shown below."),
            new SwipeGestureDetector(Box("Swipe me"))
                .SetAllowedDirections(SwipeDirection.All)
                .SetCommand(vm.SwipeCommand)),

        Section("Last gesture",
            new Label().BindText(() => vm.LastGesture).SetFontWeight(FontWeight.Bold)),
    ];

    private static UiElement Box(string text) =>
        new Border()
            .SetBackground(PlusUiDefaults.BackgroundControl)
            .SetCornerRadius(8)
            .SetStrokeThickness(0)
            .SetDesiredSize(new Size(150, 110))
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .AddChild(new Label()
                .SetText(text)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetVerticalAlignment(VerticalAlignment.Center));
}
