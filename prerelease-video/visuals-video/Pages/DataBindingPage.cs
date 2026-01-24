using PlusUi.core;

public class DataBindingPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<FluentApiPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "4. Data Binding";
}
