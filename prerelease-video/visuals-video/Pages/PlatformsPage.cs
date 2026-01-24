using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class PlatformsPage(
    PlatformsViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<PlatformsViewModel, ControlsPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "2. Platforms";
}
