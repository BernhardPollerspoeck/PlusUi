using CommunityToolkit.Mvvm.ComponentModel;

namespace Sandbox.Pages.WrapDemo;

public partial class WrapDemoPageViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _hStackWrapEnabled = true;

    [ObservableProperty]
    private bool _vStackWrapEnabled = true;
}
