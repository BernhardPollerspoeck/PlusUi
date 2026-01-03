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
        RootItems = new ObservableCollection<object>
        {
            new Drive
            {
                Name = "C:",
                Folders = new List<Folder>
                {
                    new Folder
                    {
                        Name = "Program Files",
                        SubFolders = new List<Folder>
                        {
                            new Folder { Name = "Microsoft", SubFolders = new List<Folder>(), Files = new List<FileItem> { new FileItem { Name = "readme.txt" } } },
                            new Folder { Name = "Common Files", SubFolders = new List<Folder>(), Files = new List<FileItem>() }
                        },
                        Files = new List<FileItem>()
                    },
                    new Folder
                    {
                        Name = "Users",
                        SubFolders = new List<Folder>
                        {
                            new Folder
                            {
                                Name = "Admin",
                                SubFolders = new List<Folder>
                                {
                                    new Folder { Name = "Documents", SubFolders = new List<Folder>(), Files = new List<FileItem> { new FileItem { Name = "report.docx" }, new FileItem { Name = "notes.txt" } } },
                                    new Folder { Name = "Downloads", SubFolders = new List<Folder>(), Files = new List<FileItem> { new FileItem { Name = "setup.exe" } } }
                                },
                                Files = new List<FileItem>()
                            }
                        },
                        Files = new List<FileItem>()
                    },
                    new Folder
                    {
                        Name = "Windows",
                        SubFolders = new List<Folder>
                        {
                            new Folder { Name = "System32", SubFolders = new List<Folder>(), Files = new List<FileItem> { new FileItem { Name = "cmd.exe" }, new FileItem { Name = "notepad.exe" } } },
                            new Folder { Name = "Fonts", SubFolders = new List<Folder>(), Files = new List<FileItem>() }
                        },
                        Files = new List<FileItem> { new FileItem { Name = "explorer.exe" } }
                    }
                }
            },
            new Drive
            {
                Name = "D:",
                Folders = new List<Folder>
                {
                    new Folder
                    {
                        Name = "Projects",
                        SubFolders = new List<Folder>
                        {
                            new Folder { Name = "PlusUi", SubFolders = new List<Folder>(), Files = new List<FileItem> { new FileItem { Name = "PlusUi.sln" }, new FileItem { Name = "README.md" } } },
                            new Folder { Name = "OtherProject", SubFolders = new List<Folder>(), Files = new List<FileItem>() }
                        },
                        Files = new List<FileItem>()
                    },
                    new Folder
                    {
                        Name = "Backups",
                        SubFolders = new List<Folder>(),
                        Files = new List<FileItem> { new FileItem { Name = "backup_2024.zip" } }
                    }
                }
            }
        };
    }

    public ICommand GoBackCommand { get; }
    public ObservableCollection<object> RootItems { get; }

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
