using PlusUi.core;

public class FluentApiPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<StylingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "5. Fluent API";
}
