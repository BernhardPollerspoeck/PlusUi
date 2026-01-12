using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PlusUi.core;

namespace Sandbox.Pages.AccessibilityDemo;

public class AccessibilityDemoPageViewModel(INavigationService navigationService) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isChecked;
    private string _entryText = "";
    private double _sliderValue = 50;
    private string _lastAction = "Keine Aktion";

    public List<string> ComboItems { get; } =
    [
        "Deutschland",
        "Oesterreich",
        "Schweiz",
        "Frankreich",
        "Italien",
        "Spanien",
        "Portugal",
        "Niederlande",
        "Belgien",
        "Polen"
    ];

    public bool IsChecked
    {
        get => _isChecked;
        set { _isChecked = value; OnPropertyChanged(); LastAction = $"Checkbox: {(value ? "aktiviert" : "deaktiviert")}"; }
    }

    public string EntryText
    {
        get => _entryText;
        set { _entryText = value; OnPropertyChanged(); }
    }

    public double SliderValue
    {
        get => _sliderValue;
        set { _sliderValue = value; OnPropertyChanged(); LastAction = $"Slider: {value:F0}%"; }
    }

    public string LastAction
    {
        get => _lastAction;
        set { _lastAction = value; OnPropertyChanged(); }
    }

    public ICommand GoBackCommand => new RelayCommand(() => navigationService.GoBack());
    public ICommand ButtonCommand => new RelayCommand(() => LastAction = "Button geklickt!");

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class RelayCommand(Action execute) : ICommand
{
    public event EventHandler? CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => execute();
}
