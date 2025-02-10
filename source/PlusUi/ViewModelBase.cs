using PlusUi.core.Structures;
using PlusUi.core.UiElements;
using PlusUi.core.ViewModel;
using SkiaSharp;

namespace PlusUi;



public class MainPage(MainViewModel vm) : UiPage(vm)
{
    protected override UiElement Build()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(1000);
                vm.Count++;
            }
        });
        return new VStack(
            new Label()
                .BindText(nameof(vm.Text), () => vm.Text)
                .SetTextSize(20)
             .SetBackgroundColor(SKColors.Green),
            new Button()
                .SetText("Click Set")
                .SetPadding(new Margin(10, 10, 10, 10))
                .SetMargin(new Margin(10, 10, 10, 10)),
            //TODO: implement.SetCommand(vm.ResetCountCommand),
            new Label()
             .BindText(nameof(vm.Count), () => vm.Count.ToString())
             .SetMargin(new Margin(10, 10, 20, 20))
             .SetBackgroundColor(SKColors.Red),
            new Label()
             .BindText(nameof(vm.Count), () => vm.Count.ToString())
             .SetMargin(new Margin(10))
             .SetBackgroundColor(SKColors.Purple),
            new Label()
             .BindText(nameof(vm.Count), () => vm.Count.ToString())
             .SetBackgroundColor(SKColors.SlateGray)
        )
            .SetMargin(new Margin(10))
            .SetSpacing(5);
    }

}

public class MainViewModel : ViewModelBase
{
    private string _text = "Hello World";
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    private int _count = 0;
    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }
}










