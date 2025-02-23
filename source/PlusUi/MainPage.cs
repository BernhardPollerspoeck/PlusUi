using PlusUi.core;
using SkiaSharp;
using System.Diagnostics;

namespace PlusUi;



public class MainPage(MainViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("I have been clicked {vm.Count} times")
                .SetTextSize(20)
                .SetHorizontalAlignment(HorizontalAlignment.Center),
            new TestControl(),
            new Entry()
                .BindText(nameof(vm.Text), () => vm.Text, v => vm.Text = v)
                .SetTextSize(20)
                .SetDesiredWidth(300)
                .SetBackgroundColor(SKColors.Green)
                .SetHorizontalAlignment(HorizontalAlignment.Center),
            new Label()
                .SetText("I have been clicked {vm.Count} times")
                .SetTextSize(20)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
            )
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);
    }
}


public class MainViewModel : ViewModelBase
{
    public string? Text
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            Debug.WriteLine(value);
        }
    }

}






