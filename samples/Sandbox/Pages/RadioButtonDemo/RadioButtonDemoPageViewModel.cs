using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.RadioButtonDemo;

public partial class RadioButtonDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void GoBack() => navigationService.GoBack();

    // String group selection
    [ObservableProperty]
    private bool _isOptionASelected;

    [ObservableProperty]
    private bool _isOptionBSelected;

    [ObservableProperty]
    private bool _isOptionCSelected;

    // Size selection
    [ObservableProperty]
    private bool _isSizeSmall;

    [ObservableProperty]
    private bool _isSizeMedium = true; // Default selection

    [ObservableProperty]
    private bool _isSizeLarge;

    // Payment method
    [ObservableProperty]
    private bool _isCreditCard;

    [ObservableProperty]
    private bool _isPayPal;

    [ObservableProperty]
    private bool _isBankTransfer;
}

// Enum for grouping demo
public enum PaymentGroup
{
    Payment
}

public enum SizeGroup
{
    Size
}
