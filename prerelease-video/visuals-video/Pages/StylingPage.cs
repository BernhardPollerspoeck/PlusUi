using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class StylingPage(
    StylingViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<StylingViewModel, ThemingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "6. Styling & Animations";
}
