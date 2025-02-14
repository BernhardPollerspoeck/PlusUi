using PlusUi.core.Controls;
using PlusUi.core.CoreElements;
using PlusUi.core.Structures;
using PlusUi.core.ViewModel;
using PlusUi.core.Enumerations;
using SkiaSharp;

namespace PlusUi;



public class MainPage(MainViewModel vm) : UiPageElement(vm)
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
            new Solid(height: 10, color: SKColors.Green)
                .BindDesiredWidth(nameof(vm.Count), () => vm.Count * 3)
                .SetMargin(new Margin(0, 10, 0, 0)),
            new Solid(50, 10, SKColors.Red),
            new VStack(
                new Solid(10, 10, SKColors.Blue),
                new Solid(10, 10, SKColors.Purple)
                )
                .SetSpacing(10)
                .SetHorizontalAlignment(HorizontalAlignment.Right)//TODO: this does not yet work.
                .SetMargin(new Margin(10, 0, 0, 0)),
            new Label()
                .SetText("Hello World !"),
            new Label()
                .BindText(nameof(vm.Count), () => vm.Count.ToString())
                .SetTextColor(SKColors.Black)
                .SetBackgroundColor(SKColors.Aqua))
            .SetMargin(new Margin(10, 0, 0, 0)


            );
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










