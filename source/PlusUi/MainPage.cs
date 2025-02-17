using PlusUi.core.Controls;
using PlusUi.core.CoreElements;
using PlusUi.core.ViewModel;
using PlusUi.core.Enumerations;
using SkiaSharp;
using PlusUi.core.Interfaces;

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

        return new Label()
            .SetText("Hello World !")
            .SetTextSize(50)
            .SetBackgroundColor(SKColors.Aqua)
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);

        //return new VStack(
        //    new Solid(height: 10, color: SKColors.Green)
        //        .BindDesiredWidth(nameof(vm.Count), () => vm.Count * 3)
        //        .SetMargin(new(0, 10, 0, 0)),
        //    new Solid(50, 10, SKColors.Red),
        //    new VStack(
        //        new Solid(10, 10, SKColors.Blue),
        //        new Solid(10, 10, SKColors.Purple))
        //        .SetHorizontalAlignment(HorizontalAlignment.Right)
        //        .SetMargin(new(10, 0, 0, 0)),
        //    new Label()
        //        .SetText("Hello World !"),
        //    new Button()
        //        .SetCommand(new SyncCommand(() => vm.Count = 0))
        //        .SetText("Click me")
        //        .SetMargin(new(10, 0, 0, 0)),
        //    new Label()
        //        .BindText(nameof(vm.Count), () => vm.Count.ToString())
        //        .SetTextColor(SKColors.Black)
        //        .SetBackgroundColor(SKColors.Aqua))
        //    .SetMargin(new(10, 0, 0, 0));
    }


}


public class MainViewModel : ViewModelBase
{
    public string Text
    {
        get;
        set => SetProperty(ref field, value);
    } = "Hello World";
    public int Count
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;
}










