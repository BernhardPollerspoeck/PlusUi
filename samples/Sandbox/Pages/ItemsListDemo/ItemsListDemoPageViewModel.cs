using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Sandbox.Pages.ItemsListDemo;

internal partial class ItemsListDemoPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    
    public class ItemModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SKColor Color { get; set; }
    }

    public ObservableCollection<ItemModel> Items
    {
        get => field;
        set => SetProperty(ref field, value);
    } = new ObservableCollection<ItemModel>();

    public ObservableCollection<ItemModel> HorizontalItems
    {
        get => field;
        set => SetProperty(ref field, value);
    } = new ObservableCollection<ItemModel>();

    private int _itemCounter = 0;
    private readonly Random _random = new Random();

    public ItemsListDemoPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        
        // Initialize with many items to demonstrate virtualization
        for (int i = 0; i < 100; i++)
        {
            Items.Add(new ItemModel
            {
                Title = $"Item {i + 1}",
                Description = $"This is item number {i + 1}",
                Color = GetRandomColor()
            });
        }

        // Initialize horizontal items
        for (int i = 0; i < 20; i++)
        {
            HorizontalItems.Add(new ItemModel
            {
                Title = $"Card {i + 1}",
                Description = $"Card {i + 1}",
                Color = GetRandomColor()
            });
        }

        _itemCounter = 100;
    }

    [RelayCommand]
    private void AddItem()
    {
        _itemCounter++;
        Items.Add(new ItemModel
        {
            Title = $"Item {_itemCounter}",
            Description = $"This is item number {_itemCounter}",
            Color = GetRandomColor()
        });

        HorizontalItems.Add(new ItemModel
        {
            Title = $"Card {_itemCounter}",
            Description = $"Card {_itemCounter}",
            Color = GetRandomColor()
        });
    }

    [RelayCommand]
    private void RemoveItem()
    {
        if (Items.Count > 0)
        {
            Items.RemoveAt(Items.Count - 1);
        }

        if (HorizontalItems.Count > 0)
        {
            HorizontalItems.RemoveAt(HorizontalItems.Count - 1);
        }
    }

    [RelayCommand]
    private void Nav()
    {
        _navigationService.NavigateTo<MainPage>();
    }

    private SKColor GetRandomColor()
    {
        return new SKColor(
            (byte)_random.Next(100, 256),
            (byte)_random.Next(100, 256),
            (byte)_random.Next(100, 256)
        );
    }
}
