using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.MenuDemo;

public partial class MenuDemoPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public MenuDemoPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        GoBackCommand = new RelayCommand(() => _navigationService.GoBack());
        NewCommand = new RelayCommand(() => LastAction = "New clicked");
        OpenCommand = new RelayCommand(() => LastAction = "Open clicked");
        SaveCommand = new RelayCommand(() => LastAction = "Save clicked");
        ExitCommand = new RelayCommand(() => LastAction = "Exit clicked");
        UndoCommand = new RelayCommand(() => LastAction = "Undo clicked");
        RedoCommand = new RelayCommand(() => LastAction = "Redo clicked");
        CutCommand = new RelayCommand(() => LastAction = "Cut clicked");
        CopyCommand = new RelayCommand(() => LastAction = "Copy clicked");
        PasteCommand = new RelayCommand(() => LastAction = "Paste clicked");
        DeleteCommand = new RelayCommand(() => LastAction = "Delete clicked");
        AboutCommand = new RelayCommand(() => LastAction = "About clicked");
    }

    public ICommand GoBackCommand { get; }
    public ICommand NewCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ExitCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand CutCommand { get; }
    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand AboutCommand { get; }

    [ObservableProperty]
    private string _lastAction = "(no action yet)";
}
