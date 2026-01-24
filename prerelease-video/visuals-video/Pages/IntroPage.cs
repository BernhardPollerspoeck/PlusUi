using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class IntroPage(
    IntroViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<IntroViewModel, PlatformsPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "1. Intro";
}
