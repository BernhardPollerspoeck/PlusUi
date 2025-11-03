using PlusUi.core;
using Sandbox.Controls;
using Sandbox.Pages.BgTest;
using Sandbox.Pages.ButtonDemo;
using Sandbox.Pages.ControlsGrid;
using Sandbox.Pages.FontDemo;
using Sandbox.Pages.Form;
using Sandbox.Pages.ItemsListDemo;
using Sandbox.Pages.LinkDemo;
using Sandbox.Pages.RawControl;
using Sandbox.Pages.ScrollViewDemo;
using Sandbox.Pages.ShadowDemo;
using Sandbox.Pages.TextRendering;
using Sandbox.Pages.TextWrapDemo;
using Sandbox.Pages.NewControlsDemo;
using SkiaSharp;

namespace Sandbox.Pages.Main;

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        //SetBackgroundColor(SKColors.SlateBlue);
        return new HStack(
            new VStack(
                new HStack(
                    new Solid().SetBackground(new SKColor(0, 255, 255)).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new SolidColorBackground(new SKColor(255, 0, 255))).SetIsVisible(false).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new SolidColorBackground(new SKColor(255, 255, 0))).SetVisualOffset(new(0, 10)).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new LinearGradient(SKColors.White, SKColors.Black, 90)).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new SolidColorBackground(new SKColor(255, 0, 0))).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new SolidColorBackground(new SKColor(0, 255, 0))).SetDesiredSize(new(75, 75))
                    ),
                new HStack(
                    new Solid().SetBackground(new SolidColorBackground(new SKColor(0, 0, 255))).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new LinearGradient(SKColors.Red, SKColors.Cyan, 45)).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new SolidColorBackground(new SKColor(255, 255, 255))).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new SolidColorBackground(new SKColor(128, 128, 128))).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new SolidColorBackground(new SKColor(50, 50, 50))).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new RadialGradient(SKColors.Yellow, SKColors.Purple)).SetDesiredSize(new(75, 75))
                    ),
                new HStack(
                    new Solid().SetBackground(new LinearGradient(SKColors.Lime, SKColors.Navy, 180)).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new RadialGradient(SKColors.White, SKColors.Black)).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new MultiStopGradient(
                        90,
                        new MultiStopGradient.GradientStop(SKColors.Red, 0),
                        new MultiStopGradient.GradientStop(SKColors.Blue, 0.5f),
                        new MultiStopGradient.GradientStop(SKColors.Yellow, 1))).SetDesiredSize(new(75, 75)),
                    new Solid().SetBackground(new MultiStopGradient(
                        0,
                        new MultiStopGradient.GradientStop(SKColors.Magenta, 0),
                        new MultiStopGradient.GradientStop(SKColors.Green, 0.5f),
                        new MultiStopGradient.GradientStop(SKColors.Orange, 1))).SetDesiredSize(new(75, 75))
                        ),

                new HelloWorldControl(),
                new Label()
                    .BindText(nameof(vm.Text), () => $"The entry input is: [ {vm.Text} ]"),
                new Entry()
                    .BindText(nameof(vm.Text), () => vm.Text, txt => vm.Text = txt),
                new Entry(),

                new Label()
                    .SetText("Hit the button below to Change my color")
                    .BindTextColor(nameof(vm.Color), () => vm.Color),
                new HStack(
                    new Button()
                        .SetText("Hello World!?")
                        .SetPadding(new(10, 5))
                        .SetCommand(vm.SetColorCommand),
                    new Checkbox()
                        .BindIsChecked(nameof(vm.Checked), () => vm.Checked, isChecked => vm.Checked = isChecked)
                        .SetColor(new SKColor(255, 0, 0)),
                    new Checkbox()
                        .BindIsChecked(nameof(vm.Checked), () => vm.Checked, isChecked => vm.Checked = isChecked)
                        .SetColor(new SKColor(0, 255, 0)),

                new Border()
                    .AddChild(new Label().SetText("Solid Border").SetTextColor(SKColors.Black))
                    .SetStrokeColor(SKColors.Red)
                    .SetStrokeThickness(3f)
                    .SetStrokeType(StrokeType.Solid)
                    .SetBackground(new SolidColorBackground(new SKColor(255, 255, 0, 100))),

                new Border()
                    .AddChild(new Label().SetText("Dashed").SetTextColor(SKColors.Black))
                    .SetStrokeColor(SKColors.Blue)
                    .SetStrokeThickness(2f)
                    .SetStrokeType(StrokeType.Dashed)
                    .SetBackground(new SolidColorBackground(new SKColor(0, 255, 255, 100))),

                new Border()
                    .AddChild(new VStack(
                        new Label().SetText("Dotted").SetTextColor(SKColors.White),
                        new Label().SetText("Multi-line").SetTextColor(SKColors.White)))
                    .SetStrokeColor(SKColors.Green)
                    .SetStrokeThickness(4f)
                    .SetStrokeType(StrokeType.Dotted)
                    .SetCornerRadius(10),

                new HStack(
                    new Image()
                        .SetAspect(Aspect.AspectFit)
                        .SetImageSource("plusui.png")
                        .SetDesiredWidth(300)
                        .SetDesiredHeight(250),
                    new Image()
                        .SetAspect(Aspect.AspectFit)
                        .SetImageSource("https://picsum.photos/100")
                        .SetVerticalAlignment(VerticalAlignment.Bottom)
                        .SetDesiredWidth(100)
                        .SetDesiredHeight(100))))
            ,
            CreateTestButtons()
                    );
    }

    protected override void ConfigurePageStyles(Style pageStyle)
    {
        pageStyle.AddStyle<UiPageElement>(element
            => element.SetBackground(new SolidColorBackground(new SKColor(0, 0, 0, 220))));
    }

    private VStack CreateTestButtons()
    {
        return new VStack(
            new Button()
                .SetText("New Controls Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new SKColor(52, 199, 89)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(NewControlsDemoPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Go to Grid")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(ControlsGridPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Go to TextRendering")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(TextRenderPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Text Wrap & Truncation Demo")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(TextWrapDemoPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Custom Font Demo")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(FontDemoPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Go to Form")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(FormDemoPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Go to Button Demo")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(ButtonDemoPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Go to Link Demo")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(LinkDemoPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Popup")
                .SetPadding(new(20, 5))
                .SetCommand(vm.PopupCommand),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("ScrollView Example")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(ScrollViewExamplePage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("ItemsList Example")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(ItemsListDemoPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("Shadow Demo")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(ShadowDemoPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("RawUserControl Example")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(RawUserControlPage)),
            new Solid().SetDesiredHeight(10).IgnoreStyling(),
            new Button()
                .SetText("BgTest Example")
                .SetPadding(new(20, 5))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(BgTestPage))

        );
    }

    public override void Appearing()
    {
        base.Appearing();
        vm.SetColorCommand.Execute(null);
    }
}
