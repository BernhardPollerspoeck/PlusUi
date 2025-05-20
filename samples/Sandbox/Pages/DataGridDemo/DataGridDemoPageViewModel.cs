using PlusUi.core;
using Sandbox.Pages.Main;
using System.Windows.Input;

namespace Sandbox.Pages.DataGridDemo;

public class Person
{
    [DataGridColumn("ID", Order = 0)]
    public int Id { get; set; }
    
    [DataGridColumn("Full Name", Order = 1)]
    public string Name { get; set; } = string.Empty;
    
    [DataGridColumn("Email Address", Order = 2)]
    public string Email { get; set; } = string.Empty;
    
    [DataGridColumn("Active", Order = 3, CellTemplate = DataGridCellTemplate.Checkbox)]
    public bool IsActive { get; set; }
    
    [DataGridColumn("Birth Date", Order = 4)]
    public DateTime DateOfBirth { get; set; }
    
    [DataGridColumn("Avatar", Order = 5, CellTemplate = DataGridCellTemplate.Image)]
    public string ImageUrl { get; set; } = string.Empty;
}

internal class DataGridDemoPageViewModel : ViewModelBase
{
    public List<Person> People { 
        get => field;
        set => SetProperty(ref field, value);
    }
    
    public List<Person> CustomPeople { 
        get => field;
        set => SetProperty(ref field, value);
    }
    
    public bool AutoGenerateColumns { 
        get => field;
        set => SetProperty(ref field, value);
    } = true;
    
    public ICommand NavCommand { get; }
    
    public DataGridDemoPageViewModel(INavigationService navigationService)
    {
        NavCommand = new SyncCommand(() => navigationService.NavigateTo<MainPage>());
        
        // Initialize sample data
        People = GenerateSampleData();
        CustomPeople = GenerateSampleData();
    }
    
    private List<Person> GenerateSampleData()
    {
        return new List<Person>
        {
            new Person 
            { 
                Id = 1, 
                Name = "John Doe", 
                Email = "john.doe@example.com", 
                IsActive = true, 
                DateOfBirth = new DateTime(1985, 5, 15),
                ImageUrl = "plusui.png"
            },
            new Person 
            { 
                Id = 2, 
                Name = "Jane Smith", 
                Email = "jane.smith@example.com", 
                IsActive = false, 
                DateOfBirth = new DateTime(1990, 8, 22),
                ImageUrl = "plusui.png"
            },
            new Person 
            { 
                Id = 3, 
                Name = "Bob Johnson", 
                Email = "bob.johnson@example.com", 
                IsActive = true, 
                DateOfBirth = new DateTime(1978, 3, 10),
                ImageUrl = "plusui.png"
            },
            new Person 
            { 
                Id = 4, 
                Name = "Alice Brown", 
                Email = "alice.brown@example.com", 
                IsActive = true, 
                DateOfBirth = new DateTime(1992, 11, 7),
                ImageUrl = "plusui.png"
            },
            new Person 
            { 
                Id = 5, 
                Name = "Charlie Wilson", 
                Email = "charlie.wilson@example.com", 
                IsActive = false, 
                DateOfBirth = new DateTime(1980, 1, 30),
                ImageUrl = "plusui.png"
            }
        };
    }
}