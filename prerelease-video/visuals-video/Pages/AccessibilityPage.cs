using PlusUi.core;

public class AccessibilityPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<HotReloadPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "8. Accessibility";
}
