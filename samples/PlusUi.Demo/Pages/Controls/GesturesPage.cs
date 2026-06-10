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
    [RelayCommand] private void Swipe() => LastGesture = "Swiped";
}

public class GesturesPage(GesturesPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Gestures";

    protected override string Description =>
        "Gesture detectors wrap a child element and fire a command on tap, double-tap, long-press, swipe or pinch.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Tap & double-tap",
            new TapGestureDetector(Box("Tap me")).SetCommand(vm.TapCommand),
            new DoubleTapGestureDetector(Box("Double-tap me")).SetCommand(vm.DoubleTapCommand)),

        Section("Long press & swipe",
            new LongPressGestureDetector(Box("Long-press me")).SetCommand(vm.LongPressCommand),
            new SwipeGestureDetector(Box("Swipe me horizontally"))
                .SetAllowedDirections(SwipeDirection.Left | SwipeDirection.Right)
                .SetCommand(vm.SwipeCommand)),

        Section("Last gesture",
            new Label().BindText(() => vm.LastGesture)),
    ];

    private static UiElement Box(string text) =>
        new Border()
            .SetBackground(PlusUiDefaults.BackgroundControl)
            .SetCornerRadius(8)
            .SetStrokeThickness(0)
            .SetHorizontalAlignment(HorizontalAlignment.Left)
            .AddChild(new Label().SetText(text).SetMargin(new Margin(20)));
}
