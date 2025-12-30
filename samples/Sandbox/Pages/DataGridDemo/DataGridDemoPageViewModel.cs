using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.DataGridDemo;

public partial class DataGridDemoPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IPopupService _popupService;
    public static readonly string[] Departments = ["Engineering", "Marketing", "HR", "Sales", "Finance"];
    private static readonly string[] Positions = ["Junior Developer", "Senior Developer", "Team Lead", "Manager", "Director", "Analyst", "Consultant", "Specialist"];
    private static readonly string[] FirstNames = ["Alice", "Bob", "Charlie", "Diana", "Eva", "Frank", "Greta", "Hans", "Ingrid", "Jonas", "Karl", "Lisa", "Max", "Nina", "Otto"];
    private static readonly string[] LastNames = ["Schmidt", "Müller", "Weber", "Fischer", "Bauer", "Hoffmann", "Schulz", "Meyer", "Koch", "Wagner", "Becker", "Richter"];

    public DataGridDemoPageViewModel(INavigationService navigationService, IPopupService popupService)
    {
        _navigationService = navigationService;
        _popupService = popupService;
        GoBackCommand = new RelayCommand(() => _navigationService.GoBack());
        ViewDetailsCommand = new RelayCommand<Person>(p =>
        {
            if (p != null)
            {
                _popupService.ShowPopup<PersonDetailsPopup, Person>(
                    arg: p,
                    configure: cfg => cfg.CloseOnBackgroundClick = true);
            }
        });

        var random = new Random(42);
        Persons = new ObservableCollection<Person>();
        for (int i = 0; i < 50; i++) // Reduced for demo
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var startDate = DateTime.Today.AddDays(-random.Next(100, 3000));
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
                StartDate = startDate,
                HireDate = DateOnly.FromDateTime(startDate),
                ShiftStart = new TimeOnly(8 + random.Next(0, 4), random.Next(0, 4) * 15),
                Performance = (float)random.NextDouble(),
                Rating = random.Next(1, 6)
            };
            person.Age = random.Next(20, 65);
            Persons.Add(person);
        }
    }

    public ICommand ViewDetailsCommand { get; }

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

    [ObservableProperty]
    private string _department = string.Empty;

    public string Position { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isActive;

    public decimal Salary { get; set; }
    public DateTime StartDate { get; set; }

    [ObservableProperty]
    private DateOnly? _hireDate;

    [ObservableProperty]
    private TimeOnly? _shiftStart;

    [ObservableProperty]
    private float _performance = 0.5f;

    [ObservableProperty]
    private float _rating = 3f;

    public string? AvatarUrl { get; set; }

    [RelayCommand]
    private void IncrementAge()
    {
        Age++;
    }
}
