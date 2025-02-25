using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.Main;

internal class MainPage(MainViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new HStack(
                new Solid().SetBackgroundColor(new SKColor(0, 255, 255)),
                new Solid().SetBackgroundColor(new SKColor(255, 0, 255)),
                new Solid().SetBackgroundColor(new SKColor(255, 255, 0))),
            new HStack(
                new Solid().SetBackgroundColor(new SKColor(255, 0, 0)),
                new Solid().SetBackgroundColor(new SKColor(0, 255, 0)),
                new Solid().SetBackgroundColor(new SKColor(0, 0, 255))),
            new HStack(
                new Solid().SetBackgroundColor(new SKColor(255, 255, 255)),
                new Solid().SetBackgroundColor(new SKColor(128, 128, 128)),
                new Solid().SetBackgroundColor(new SKColor(50, 50, 50))),

            new Label()
                .BindText(nameof(vm.Text), () => $"The entry input is: [ {vm.Text} ]"),
            new Entry()
                .BindText(nameof(vm.Text), () => vm.Text, txt => vm.Text = txt),

            new Label()
                .SetText("Hit the button below to Change my color")
                .BindTextColor(nameof(vm.Color), () => vm.Color),
            new Button()
                .SetText("Hello World!")
                .SetPadding(new(10, 5))
                .SetCommand(vm.SetColorCommand));
    }

    public override void Appearing()
    {
        base.Appearing();
        vm.SetColorCommand.Execute(null);
    }
}






