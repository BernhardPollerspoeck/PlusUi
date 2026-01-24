using PlusUi.core;

public class StylingPage(
    PlaceholderViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<ThemingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "6. Styling & Animations";
}
