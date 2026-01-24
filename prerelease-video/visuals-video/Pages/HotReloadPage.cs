using PlusUi.core;

public class HotReloadPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<DebugServerPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "9. Hot Reload";
}
