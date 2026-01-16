using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.TreeViewDemo;

public partial class TreeViewDemoPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public TreeViewDemoPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        GoBackCommand = new RelayCommand(() => _navigationService.GoBack());

        // Create file system tree
        RootItems =
        [
            new Drive
            {
                Name = "C:",
                Folders =
                [
                    new Folder
                    {
                        Name = "Program Files",
                        SubFolders =
                        [
                            new Folder { Name = "Microsoft", SubFolders = [], Files = [new FileItem { Name = "readme.txt" }] },
                            new Folder { Name = "Common Files", SubFolders = [], Files = [] }
                        ],
                        Files = []
                    },
                    new Folder
                    {
                        Name = "Users",
                        SubFolders =
                        [
                            new Folder
                            {
                                Name = "Admin",
                                SubFolders =
                                [
                                    new Folder { Name = "Documents", SubFolders = [], Files = [new FileItem { Name = "report.docx" }, new FileItem { Name = "notes.txt" }] },
                                    new Folder { Name = "Downloads", SubFolders = [], Files = [new FileItem { Name = "setup.exe" }] }
                                ],
                                Files = []
                            }
                        ],
                        Files = []
                    },
                    new Folder
                    {
                        Name = "Windows",
                        SubFolders =
                        [
                            new Folder { Name = "System32", SubFolders = [], Files = [new FileItem { Name = "cmd.exe" }, new FileItem { Name = "notepad.exe" }] },
                            new Folder { Name = "Fonts", SubFolders = [], Files = [] }
                        ],
                        Files = [new FileItem { Name = "explorer.exe" }]
                    }
                ]
            },
            new Drive
            {
                Name = "D:",
                Folders =
                [
                    new Folder
                    {
                        Name = "Projects",
                        SubFolders =
                        [
                            new Folder { Name = "PlusUi", SubFolders = [], Files = [new FileItem { Name = "PlusUi.sln" }, new FileItem { Name = "README.md" }] },
                            new Folder { Name = "OtherProject", SubFolders = [], Files = [] }
                        ],
                        Files = []
                    },
                    new Folder
                    {
                        Name = "Backups",
                        SubFolders = [],
                        Files = [new FileItem { Name = "backup_2024.zip" }]
                    }
                ]
            }
        ];

        CategoryItems =
        [
            new Category
            {
                Name = "Electronics",
                SubCategories =
                [
                    new Category
                    {
                        Name = "Computers",
                        SubCategories = [],
                        Products = [new Product { Name = "Laptop Pro 15" }, new Product { Name = "Desktop Tower" }]
                    },
                    new Category
                    {
                        Name = "Phones",
                        SubCategories = [],
                        Products = [new Product { Name = "SmartPhone X" }, new Product { Name = "BasicPhone" }]
                    }
                ],
                Products = []
            },
            new Category
            {
                Name = "Books",
                SubCategories =
                [
                    new Category
                    {
                        Name = "Programming",
                        SubCategories = [],
                        Products = [new Product { Name = "C# in Depth" }, new Product { Name = "Clean Code" }]
                    },
                    new Category
                    {
                        Name = "Fiction",
                        SubCategories = [],
                        Products = [new Product { Name = "The Hobbit" }]
                    }
                ],
                Products = []
            },
            new Category
            {
                Name = "Clothing",
                SubCategories =
                [
                    new Category { Name = "Men", SubCategories = [], Products = [new Product { Name = "T-Shirt" }] },
                    new Category { Name = "Women", SubCategories = [], Products = [new Product { Name = "Dress" }] }
                ],
                Products = []
            }
        ];
    }

    public ICommand GoBackCommand { get; }
    public ObservableCollection<object> RootItems { get; }
    public ObservableCollection<object> CategoryItems { get; }

    [ObservableProperty]
    private object? _selectedItem;

    partial void OnSelectedItemChanged(object? value)
    {
        if (value != null)
        {
            SelectedItemName = value switch
            {
                Drive d => $"Drive: {d.Name}",
                Folder f => $"Folder: {f.Name}",
                FileItem file => $"File: {file.Name}",
                _ => value.ToString() ?? ""
            };
        }
        else
        {
            SelectedItemName = "(nothing selected)";
        }
    }

    [ObservableProperty]
    private string _selectedItemName = "(nothing selected)";
}
