using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using Sandbox.Pages.Main;

namespace Sandbox.Pages.DateTimePickerDemo;

public partial class DateTimePickerDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    [RelayCommand]
    private void GoBack() => navigationService.NavigateTo<MainPage>();

    // Basic date/time bindings
    [ObservableProperty]
    private DateOnly? _selectedDate;

    [ObservableProperty]
    private TimeOnly? _selectedTime;

    // Appointment booking demo
    [ObservableProperty]
    private DateOnly? _appointmentDate;

    [ObservableProperty]
    private TimeOnly? _appointmentTime;

    // Birth date demo
    [ObservableProperty]
    private DateOnly? _birthDate;

    // Meeting time demo (business hours)
    [ObservableProperty]
    private TimeOnly? _meetingTime;

    // Two-way bound date for display
    [ObservableProperty]
    private DateOnly? _formDate;

    [ObservableProperty]
    private TimeOnly? _formTime;
}
