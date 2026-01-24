using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace PlusUi.Demo.Pages.EntryDemo;

public partial class EntryPageViewModel(INavigationService navigation) : ObservableObject
{
    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _email = "";

    [ObservableProperty]
    private string _notes = "This is a multi-line\ntext entry.\n\nYou can type multiple lines here!";

    [ObservableProperty]
    private string _searchQuery = "";

    [ObservableProperty]
    private int _characterCount;

    [ObservableProperty]
    private string _boundText = "Edit me!";

    partial void OnBoundTextChanged(string value)
    {
        CharacterCount = value.Length;
    }

    [RelayCommand]
    private void ClearAll()
    {
        Username = "";
        Password = "";
        Email = "";
        Notes = "";
        SearchQuery = "";
        BoundText = "";
    }

    [RelayCommand]
    private void GoBack()
    {
        navigation.GoBack();
    }
}
