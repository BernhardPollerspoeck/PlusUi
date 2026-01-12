using System.ComponentModel;
using System.Windows.Input;
using PlusUi.core;

namespace Sandbox.Pages.RenderLoopDemo;

public class RenderLoopDemoViewModel : INotifyPropertyChanged
{
    private readonly PlusUiNavigationService _navigationService;
    private System.Threading.Timer? _countdownTimer;
    private int _remainingSeconds = 30;
    private bool _isActivityRunning;

    public RenderLoopDemoViewModel(PlusUiNavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand BackCommand => new RelayCommand(() => _navigationService.GoBack());

    public int RemainingSeconds
    {
        get => _remainingSeconds;
        private set
        {
            if (_remainingSeconds != value)
            {
                _remainingSeconds = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingSeconds)));
            }
        }
    }

    public bool IsActivityRunning
    {
        get => _isActivityRunning;
        private set
        {
            if (_isActivityRunning != value)
            {
                _isActivityRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActivityRunning)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActivityStatusText)));
            }
        }
    }

    public string ActivityStatusText => IsActivityRunning ? "Running (60 FPS)" : "Stopped (0 FPS)";

    public ICommand StartCountdownCommand => new RelayCommand(() =>
    {
        StopCountdown();
        RemainingSeconds = 30;
        StartCountdown();
    });

    public ICommand StopCountdownCommand => new RelayCommand(StopCountdown);

    public ICommand StartActivityCommand => new RelayCommand(() => IsActivityRunning = true);

    public ICommand StopActivityCommand => new RelayCommand(() => IsActivityRunning = false);

    public ICommand StartAllCommand => new RelayCommand(() =>
    {
        StopCountdown();
        RemainingSeconds = 30;
        StartCountdown();
        IsActivityRunning = true;
    });

    public ICommand StopAllCommand => new RelayCommand(() =>
    {
        StopCountdown();
        IsActivityRunning = false;
    });

    private void StartCountdown()
    {
        _countdownTimer = new System.Threading.Timer(_ =>
        {
            if (RemainingSeconds > 0)
            {
                RemainingSeconds--;
            }
            else
            {
                StopCountdown();
            }
        }, null, 1000, 1000);
    }

    private void StopCountdown()
    {
        _countdownTimer?.Dispose();
        _countdownTimer = null;
    }

    private class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();

        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
