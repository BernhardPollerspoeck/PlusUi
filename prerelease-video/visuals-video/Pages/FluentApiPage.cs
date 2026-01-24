using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class FluentApiPage(
    FluentApiViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<FluentApiViewModel, StylingPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "5. Fluent API";
}
