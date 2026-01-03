using PlusUi.core;

namespace Sandbox.Pages.RawControl;

internal class RawUserControlPage(RawUserControlPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new TestRawUserControl()
            .SetDebug();
    }
}