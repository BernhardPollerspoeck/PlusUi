using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.DataGridDemo;

public partial class DataGridDemoPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private static readonly string[] Departments = ["Engineering", "Marketing", "HR", "Sales", "Finance"];
    private static readonly string[] Positions = ["Junior Developer", "Senior Developer", "Team Lead", "Manager", "Director", "Analyst", "Consultant", "Specialist"];
    private static readonly string[] FirstNames = ["Alice", "Bob", "Charlie", "Diana", "Eva", "Frank", "Greta", "Hans", "Ingrid", "Jonas", "Karl", "Lisa", "Max", "Nina", "Otto"];
    private static readonly string[] LastNames = ["Schmidt", "Müller", "Weber", "Fischer", "Bauer", "Hoffmann", "Schulz", "Meyer", "Koch", "Wagner", "Becker", "Richter"];

    public DataGridDemoPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        GoBackCommand = new RelayCommand(() => _navigationService.GoBack());

        var random = new Random(42);
        Persons = new ObservableCollection<Person>();
        for (int i = 0; i < 1000; i++)
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var person = new Person
            {
                Id = i + 1,
                Name = $"{firstName} {lastName}",
                Department = Departments[random.Next(Departments.Length)],
                Position = Positions[random.Next(Positions.Length)],
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}@company.com",
                Phone = $"+49 {random.Next(100, 999)} {random.Next(1000000, 9999999)}",
                IsActive = random.Next(100) > 20,
                Salary = random.Next(40000, 120000),
                StartDate = DateTime.Today.AddDays(-random.Next(100, 3000))
            };
            person.Age = random.Next(20, 65);
            Persons.Add(person);
        }
    }

    public ICommand GoBackCommand { get; }
    public ObservableCollection<Person> Persons { get; }

    [ObservableProperty]
    private Person? _selectedPerson;

    [RelayCommand]
    private void DeletePerson(Person person)
    {
        Persons.Remove(person);
    }
}

public partial class Person : ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [ObservableProperty]
    private int _age;

    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal Salary { get; set; }
    public DateTime StartDate { get; set; }

    [RelayCommand]
    private void IncrementAge()
    {
        Age++;
    }
}
