using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sandbox.Pages.Form;

public partial class FormDemoPageViewModel : ObservableObject
{
    public string Name { get; set => SetProperty(ref field, value); } = string.Empty;

    public string Email { get; set => SetProperty(ref field, value); } = string.Empty;

    public string Phone { get; set => SetProperty(ref field, value); } = string.Empty;

    public string Username { get; set => SetProperty(ref field, value); } = string.Empty;

    public string Password { get; set => SetProperty(ref field, value); } = string.Empty;

    public bool ReceiveNotifications { get; set => SetProperty(ref field, value); }

    public bool DarkMode { get; set => SetProperty(ref field, value); }


    [RelayCommand]
    private void UploadImage()
    {
        // Logic to upload an image
    }
    [RelayCommand]
    private void SubmitForm()
    {
        // Logic to submit the form
        // For example, you might want to validate the fields and then send them to a server or save them locally
    }
}
