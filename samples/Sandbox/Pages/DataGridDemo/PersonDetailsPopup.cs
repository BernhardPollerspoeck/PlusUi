using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.core.CoreElements;

namespace Sandbox.Pages.DataGridDemo;

public partial class PersonDetailsPopupViewModel(IPopupService popupService) : ObservableObject
{
    [RelayCommand]
    private void Close()
    {
        popupService.ClosePopup();
    }
}

public class PersonDetailsPopup(PersonDetailsPopupViewModel vm) : UiPopupElement<Person>(vm)
{
    protected override UiElement Build()
    {
        // Use Argument directly since it's set before Build() is called
        var person = Argument;

        return new VStack(
            new Label()
                .SetText("Person Details")
                .SetTextSize(18)
                .SetTextColor(Colors.White)
                .SetMargin(new Margin(0, 0, 0, 10)),
            new Label()
                .SetText($"Name: {person?.Name ?? "-"}")
                .SetTextColor(Colors.White),
            new Label()
                .SetText($"Department: {person?.Department ?? "-"}")
                .SetTextColor(Colors.White),
            new Label()
                .SetText($"Position: {person?.Position ?? "-"}")
                .SetTextColor(Colors.White),
            new Label()
                .SetText($"Email: {person?.Email ?? "-"}")
                .SetTextColor(Colors.White),
            new Label()
                .SetText($"Phone: {person?.Phone ?? "-"}")
                .SetTextColor(Colors.White),
            new Label()
                .SetText($"Salary: {person?.Salary:C0}")
                .SetTextColor(Colors.White),
            new Label()
                .SetText($"Active: {(person?.IsActive == true ? "Yes" : "No")}")
                .SetTextColor(Colors.White),
            new Button()
                .SetText("Close")
                .SetCommand(vm.CloseCommand)
                .SetTextColor(Colors.White)
                .SetBackground(new Color(100, 149, 237))
                .SetPadding(new Margin(20, 8))
                .SetMargin(new Margin(0, 15, 0, 0))
                .SetCornerRadius(4)
        )
        .SetBackground(new Color(50, 50, 50))
        .SetCornerRadius(8)
        .SetMargin(new Margin(20));
    }
}
