using PlusUi.core;

namespace Sandbox.Pages.DataGridDemo;

public class DataGridDemoPage(DataGridDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Button()
                .SetText("<- Back")
                .SetTextSize(16)
                .SetCommand(vm.GoBackCommand)
                .SetTextColor(Colors.White)
                .SetPadding(new Margin(10, 5)),

            new Label()
                .SetText("DataGrid with New Column Types")
                .SetTextSize(18)
                .SetTextColor(Colors.White)
                .SetMargin(new Margin(10, 5)),

            new DataGrid<Person>()
                .SetItemsSource(vm.Persons)
                .SetAlternatingRowStyles(true)
                .SetEvenRowStyle(new SolidColorBackground(new Color(30, 30, 30)), Colors.White)
                .SetOddRowStyle(new SolidColorBackground(new Color(40, 40, 40)), Colors.White)

                // Basic columns
                .AddColumn(new DataGridTextColumn<Person>()
                    .SetHeader("ID")
                    .SetBinding(p => p.Id.ToString())
                    .SetWidth(DataGridColumnWidth.Absolute(50)))
                .AddColumn(new DataGridTextColumn<Person>()
                    .SetHeader("Name")
                    .SetBinding(p => p.Name)
                    .SetWidth(DataGridColumnWidth.Absolute(150)))

                // NEW: ComboBox Column
                .AddColumn(new DataGridComboBoxColumn<Person, string>()
                    .SetHeader("Department")
                    .SetBinding(p => p.Department, (p, v) => p.Department = v ?? "")
                    .SetItemsSource(DataGridDemoPageViewModel.Departments)
                    .SetPlaceholder("Select...")
                    .SetWidth(DataGridColumnWidth.Absolute(140)))

                // NEW: DatePicker Column
                .AddColumn(new DataGridDatePickerColumn<Person>()
                    .SetHeader("Hire Date")
                    .SetBinding(p => p.HireDate, (p, v) => p.HireDate = v)
                    .SetDisplayFormat("dd.MM.yyyy")
                    .SetPlaceholder("Select date")
                    .SetWidth(DataGridColumnWidth.Absolute(140)))

                // NEW: TimePicker Column
                .AddColumn(new DataGridTimePickerColumn<Person>()
                    .SetHeader("Shift Start")
                    .SetBinding(p => p.ShiftStart, (p, v) => p.ShiftStart = v)
                    .SetMinuteIncrement(15)
                    .Set24HourFormat(true)
                    .SetPlaceholder("Select time")
                    .SetWidth(DataGridColumnWidth.Absolute(120)))

                // NEW: Progress Column
                .AddColumn(new DataGridProgressColumn<Person>()
                    .SetHeader("Performance")
                    .SetBinding(p => p.Performance)
                    .SetProgressColor(new Color(0, 200, 100))
                    .SetWidth(DataGridColumnWidth.Absolute(120)))

                // NEW: Slider Column
                .AddColumn(new DataGridSliderColumn<Person>()
                    .SetHeader("Rating")
                    .SetBinding(p => p.Rating, (p, v) => p.Rating = v)
                    .SetRange(1, 5)
                    .SetWidth(DataGridColumnWidth.Absolute(120)))

                // Checkbox Column (existing)
                .AddColumn(new DataGridCheckboxColumn<Person>()
                    .SetHeader("Active")
                    .SetBinding(p => p.IsActive, (p, v) => p.IsActive = v)
                    .SetWidth(DataGridColumnWidth.Absolute(70)))

                // NEW: Link Column
                .AddColumn(new DataGridLinkColumn<Person>()
                    .SetHeader("Details")
                    .SetBinding(p => "View")
                    .SetCommand(vm.ViewDetailsCommand, p => p)
                    .SetLinkColor(new Color(100, 149, 237))
                    .SetWidth(DataGridColumnWidth.Absolute(80)))

                // Button Column (existing)
                .AddColumn(new DataGridButtonColumn<Person>()
                    .SetHeader("Del")
                    .SetButtonText("X")
                    .SetCommand(vm.DeletePersonCommand)
                    .SetWidth(DataGridColumnWidth.Absolute(50)))

                .SetRowHeight(40)
                .SetHeaderHeight(40)
                .SetSelectionMode(SelectionMode.Single)
                .BindSelectedItem(nameof(vm.SelectedPerson),
                    () => vm.SelectedPerson,
                    v => vm.SelectedPerson = v)
        );
    }
}
