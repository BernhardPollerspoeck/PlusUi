using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class AccessibilityPage(
    AccessibilityViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<AccessibilityViewModel, HotReloadPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "8. Accessibility";
}
