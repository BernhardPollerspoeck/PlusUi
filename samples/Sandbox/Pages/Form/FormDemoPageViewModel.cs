using PlusUi.core;
using System.Windows.Input;

namespace Sandbox.Pages.Form;

public class FormDemoPageViewModel : ViewModelBase
{
    public string Name { get; set => SetProperty(ref field, value); } = string.Empty;

    public string Email { get; set => SetProperty(ref field, value); } = string.Empty;

    public string Phone { get; set => SetProperty(ref field, value); } = string.Empty;

    public string Username { get; set => SetProperty(ref field, value); } = string.Empty;

    public string Password { get; set => SetProperty(ref field, value); } = string.Empty;

    public bool ReceiveNotifications { get; set => SetProperty(ref field, value); }

    public bool DarkMode { get; set => SetProperty(ref field, value); }

    public ICommand UploadImageCommand { get; }
    public ICommand SubmitFormCommand { get; }

    public FormDemoPageViewModel()
    {
        UploadImageCommand = new SyncCommand(() => { /* Upload image logic */ });
        SubmitFormCommand = new SyncCommand(() => { /* Submit form logic */ });
    }
}
