using PlusUi.core;

public class IntroPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<PlatformsPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "1. Intro";
}
