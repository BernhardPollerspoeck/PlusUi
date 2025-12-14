using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;
using System.Collections.ObjectModel;

namespace Sandbox.Pages.ComboBoxDemo;

public partial class ComboBoxDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void GoBack() => navigationService.NavigateTo<MainPage>();

    [ObservableProperty]
    private string? _selectedFruit;

    [ObservableProperty]
    private string? _selectedColor;

    [ObservableProperty]
    private Person? _selectedPerson;

    [ObservableProperty]
    private int _selectedCountryIndex = -1;

    public ObservableCollection<string> Fruits { get; } =
    [
        "Apple",
        "Banana",
        "Orange",
        "Strawberry",
        "Mango",
        "Pineapple",
        "Grape",
        "Watermelon"
    ];

    public ObservableCollection<string> Colors { get; } =
    [
        "Red",
        "Green",
        "Blue",
        "Yellow",
        "Purple",
        "Orange",
        "Pink",
        "Cyan"
    ];

    public ObservableCollection<Person> People { get; } =
    [
        new Person("Alice", 28),
        new Person("Bob", 34),
        new Person("Charlie", 22),
        new Person("Diana", 31),
        new Person("Eve", 29)
    ];

    public ObservableCollection<string> Countries { get; } =
    [
        "Austria",
        "Germany",
        "Switzerland",
        "France",
        "Italy",
        "Spain",
        "United Kingdom",
        "United States"
    ];
}

public record Person(string Name, int Age);
