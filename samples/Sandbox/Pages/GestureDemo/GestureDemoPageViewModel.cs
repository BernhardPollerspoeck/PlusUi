using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.GestureDemo;

public partial class GestureDemoPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IHapticService _hapticService;

    public GestureDemoPageViewModel(INavigationService navigationService, IHapticService hapticService)
    {
        _navigationService = navigationService;
        _hapticService = hapticService;

        GoBackCommand = new RelayCommand(() => _navigationService.GoBack());
        LongPressCommand = new RelayCommand(OnLongPress);
        DoubleTapCommand = new RelayCommand(OnDoubleTap);
        SwipeCommand = new RelayCommand<SwipeDirection>(OnSwipe);
        PinchCommand = new RelayCommand<float>(OnPinch);
        ResetCountersCommand = new RelayCommand(ResetCounters);
        TestHapticLightCommand = new RelayCommand(() => _hapticService.Emit(HapticFeedback.Light));
        TestHapticMediumCommand = new RelayCommand(() => _hapticService.Emit(HapticFeedback.Medium));
        TestHapticHeavyCommand = new RelayCommand(() => _hapticService.Emit(HapticFeedback.Heavy));
        TestHapticSuccessCommand = new RelayCommand(() => _hapticService.Emit(HapticFeedback.Success));
        TestHapticWarningCommand = new RelayCommand(() => _hapticService.Emit(HapticFeedback.Warning));
        TestHapticErrorCommand = new RelayCommand(() => _hapticService.Emit(HapticFeedback.Error));
    }

    public ICommand GoBackCommand { get; }
    public ICommand LongPressCommand { get; }
    public ICommand DoubleTapCommand { get; }
    public ICommand SwipeCommand { get; }
    public ICommand PinchCommand { get; }
    public ICommand ResetCountersCommand { get; }
    public ICommand TestHapticLightCommand { get; }
    public ICommand TestHapticMediumCommand { get; }
    public ICommand TestHapticHeavyCommand { get; }
    public ICommand TestHapticSuccessCommand { get; }
    public ICommand TestHapticWarningCommand { get; }
    public ICommand TestHapticErrorCommand { get; }

    [ObservableProperty]
    private string _lastGesture = "None";

    [ObservableProperty]
    private int _longPressCount;

    [ObservableProperty]
    private int _doubleTapCount;

    [ObservableProperty]
    private string _swipeDirection = "-";

    [ObservableProperty]
    private float _pinchScale = 1.0f;

    private void OnLongPress()
    {
        LongPressCount++;
        LastGesture = $"LongPress ({LongPressCount}x)";
        _hapticService.Emit(HapticFeedback.Heavy);
    }

    private void OnDoubleTap()
    {
        DoubleTapCount++;
        LastGesture = $"DoubleTap ({DoubleTapCount}x)";
        _hapticService.Emit(HapticFeedback.Medium);
    }

    private void OnSwipe(SwipeDirection direction)
    {
        SwipeDirection = direction.ToString();
        LastGesture = $"Swipe {direction}";
        _hapticService.Emit(HapticFeedback.Light);
    }

    private void OnPinch(float scale)
    {
        PinchScale *= scale;
        PinchScale = Math.Clamp(PinchScale, 0.5f, 3.0f);
        LastGesture = $"Pinch (scale: {PinchScale:F2})";
        _hapticService.Emit(HapticFeedback.Selection);
    }

    private void ResetCounters()
    {
        LongPressCount = 0;
        DoubleTapCount = 0;
        SwipeDirection = "-";
        PinchScale = 1.0f;
        LastGesture = "Reset";
        _hapticService.Emit(HapticFeedback.Success);
    }

    public bool HapticsSupported => _hapticService.IsSupported;
}
