using PlusUi.core;
using PrereleaseVideo.ViewModels;

public class DataBindingPage(
    DataBindingViewModel vm,
    INavigationService navigationService,
    TimeProvider timeProvider)
    : PlaceholderPage<DataBindingViewModel, FluentApiPage>(vm, navigationService, timeProvider)
{
    protected override string SectionTitle => "4. Data Binding";
}
