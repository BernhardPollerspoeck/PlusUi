using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class ControlsPage(
    ControlsViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<ControlsViewModel, DataBindingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "3. Controls";
}
