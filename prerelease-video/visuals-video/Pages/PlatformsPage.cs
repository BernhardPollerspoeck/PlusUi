using PlusUi.core;

public class PlatformsPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<ControlsPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "2. Platforms";
}
