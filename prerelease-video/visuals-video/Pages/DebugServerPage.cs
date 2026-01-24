using PlusUi.core;

public class DebugServerPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<OutroPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "10. Debug Server";
}
