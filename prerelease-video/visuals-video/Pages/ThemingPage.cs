using PlusUi.core;

public class ThemingPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<AccessibilityPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "7. Theming";
}
