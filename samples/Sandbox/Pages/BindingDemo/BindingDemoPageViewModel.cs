using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.BindingDemo;

public partial class BindingDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    // Simple property (no nesting)
    public bool SimpleValue
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    // One-level nested property
    public Level1ViewModel Level1
    {
        get => field ??= new();
        set => SetProperty(ref field, value);
    }

    [RelayCommand]
    private void GoBack()
    {
        navigationService.GoBack();
    }

    [RelayCommand]
    private void SwapLevel1()
    {
        Level1 = new Level1ViewModel
        {
            Checked = !Level1.Checked,
            Level2 = new Level2ViewModel
            {
                DeepChecked = !Level1.Level2.DeepChecked,
                Level3 = new Level3ViewModel
                {
                    Value = !Level1.Level2.Level3.Value,
                    Level4 = new Level4ViewModel
                    {
                        Value = !Level1.Level2.Level3.Level4.Value,
                        Level5 = new Level5ViewModel
                        {
                            UltraDeepValue = !Level1.Level2.Level3.Level4.Level5.UltraDeepValue
                        }
                    }
                }
            }
        };
    }
}

public partial class Level1ViewModel : ObservableObject
{
    public bool Checked
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Level2ViewModel Level2
    {
        get => field ??= new();
        set => SetProperty(ref field, value);
    }
}

public partial class Level2ViewModel : ObservableObject
{
    public bool DeepChecked
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Level3ViewModel Level3
    {
        get => field ??= new();
        set => SetProperty(ref field, value);
    }
}

public partial class Level3ViewModel : ObservableObject
{
    public bool Value
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Level4ViewModel Level4
    {
        get => field ??= new();
        set => SetProperty(ref field, value);
    }
}

public partial class Level4ViewModel : ObservableObject
{
    public bool Value
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Level5ViewModel Level5
    {
        get => field ??= new();
        set => SetProperty(ref field, value);
    }
}

public partial class Level5ViewModel : ObservableObject
{
    public bool UltraDeepValue
    {
        get => field;
        set => SetProperty(ref field, value);
    }
}
