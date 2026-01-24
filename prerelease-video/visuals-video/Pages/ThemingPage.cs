using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class ThemingPage(
    ThemingViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<ThemingViewModel, AccessibilityPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "7. Theming";
}
