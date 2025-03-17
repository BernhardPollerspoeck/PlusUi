using PlusUi.core;
using SkiaSharp;
using System.Windows.Input;

namespace Sandbox.Popups;

public class TestPopupViewModel : ViewModelBase
{
    private readonly IPopupService _popupService;

    public ICommand CloseCommand { get; }

    public TestPopupViewModel(IPopupService popupService)
    {
        _popupService = popupService;
        CloseCommand = new SyncCommand(Close);
    }

    private void Close()
    {
        _popupService.ClosePopup();
    }
}

public class TestPopup(TestPopupViewModel vm) : UiPopupElement<string>(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("Hello Popup")
                .SetTextColor(SKColors.Black)
                .SetHorizontalAlignment(HorizontalAlignment.Stretch),
            new Button()
                .SetText("Close")
                .SetCommand(vm.CloseCommand)
                .SetPadding(new(10, 5))
                .SetMargin(new(10))
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetCornerRadius(1)

                )

            .SetBackgroundColor(SKColors.White);//TODO: this BG color has been lost on HotReload

    }
}
