using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class DebugServerPage(
    DebugServerViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<DebugServerViewModel, OutroPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "10. Debug Server";
}
