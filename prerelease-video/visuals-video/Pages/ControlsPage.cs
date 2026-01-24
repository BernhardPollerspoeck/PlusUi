using PlusUi.core;

public class ControlsPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<DataBindingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "3. Controls";
}
