using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class HotReloadPage(
    HotReloadViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<HotReloadViewModel, DebugServerPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "9. Hot Reload";
}
