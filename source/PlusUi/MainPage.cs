using PlusUi.core;
using System.Windows.Input;

namespace PlusUi;



public class MainPage(MainViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Label()
                .SetText("Hello World !")
                .SetTextSize(50)
                .SetHorizontalAlignment(HorizontalAlignment.Center),
            new Label()
                .BindText(nameof(vm.Count), () => $"I have been clicked {vm.Count} times")
                .SetTextSize(30)
                .SetHorizontalAlignment(HorizontalAlignment.Center),
            new Button()
                .SetText("Click me")
                .SetTextSize(20)
                .SetPadding(new(10,5))
                .SetCommand(vm.CountIncrementCommand)
                .SetHorizontalAlignment(HorizontalAlignment.Center))
            .SetHorizontalAlignment(HorizontalAlignment.Center)
            .SetVerticalAlignment(VerticalAlignment.Center);
    }
}


public class MainViewModel : ViewModelBase
{
    public int Count
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public ICommand CountIncrementCommand { get; }

    public MainViewModel()
    {
        CountIncrementCommand = new SyncCommand(() => Count++);
    }
}






