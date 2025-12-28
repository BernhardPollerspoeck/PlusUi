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
using Sandbox.Pages.ToolbarDemo;
using Sandbox.Pages.ComboBoxDemo;
using Sandbox.Pages.DateTimePickerDemo;
using Sandbox.Pages.RadioButtonDemo;
using Sandbox.Pages.AccessibilityDemo;
using Sandbox.Pages.TabControlDemo;
using Sandbox.Pages.GestureDemo;
using Sandbox.Pages.DataGridDemo;
using Sandbox.Pages.TreeViewDemo;
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
                                )),
                    new VStack(
                        new Label()
                            .SetText("Animated GIFs:")
                            .SetTextColor(SKColors.White),
                        new Image()
                            .SetAspect(Aspect.AspectFit)
                            .SetImageSource("https://media.giphy.com/media/3oEjI6SIIHBdRxXI40/giphy.gif")
                            .SetDesiredWidth(150)
                            .SetDesiredHeight(150),
                        new Image()
                            .SetAspect(Aspect.AspectFit)
                            .SetImageSource("https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExaXBvaHU3dDE3aXpobncxY3R1YzZxMm90ZWR6azF2YjZzZGd0MTkxdCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/a8v7h37XBEj865P0fH/giphy.gif")
                            .SetDesiredWidth(150)
                            .SetDesiredHeight(150))
                    ),

                new HelloWorldControl(),
                new Label()
                    .BindText(nameof(vm.Text), () => $"The entry input is: [ {vm.Text} ]"),
                new Entry()
                    .BindText(nameof(vm.Text), () => vm.Text, txt => vm.Text = txt)
                    .SetTooltip("Type some text here"),
                new Entry()
                    .SetTooltip("Another input field"),

                // Tooltip Demo Section
                new Label()
                    .SetText("Tooltip Demo (hover over buttons):")
                    .SetTextColor(SKColors.Yellow)
                    .SetTextSize(14),
                new HStack(
                    new Button()
                        .SetText("Auto")
                        .SetPadding(new(8, 4))
                        .SetTooltip("Placement: Auto (default)"),
                    new Button()
                        .SetText("Top")
                        .SetPadding(new(8, 4))
                        .SetTooltip("Tooltip appears on top")
                        .SetTooltipPlacement(TooltipPlacement.Top),
                    new Button()
                        .SetText("Bottom")
                        .SetPadding(new(8, 4))
                        .SetTooltip("Tooltip appears at bottom")
                        .SetTooltipPlacement(TooltipPlacement.Bottom),
                    new Button()
                        .SetText("Left")
                        .SetPadding(new(8, 4))
                        .SetTooltip("Tooltip appears on left")
                        .SetTooltipPlacement(TooltipPlacement.Left),
                    new Button()
                        .SetText("Right")
                        .SetPadding(new(8, 4))
                        .SetTooltip("Tooltip appears on right")
                        .SetTooltipPlacement(TooltipPlacement.Right)),
                new HStack(
                    new Button()
                        .SetText("Fast (100ms)")
                        .SetPadding(new(8, 4))
                        .SetTooltip("Shows quickly!")
                        .SetTooltipShowDelay(100),
                    new Button()
                        .SetText("Slow (1s)")
                        .SetPadding(new(8, 4))
                        .SetTooltip("Takes a second to show")
                        .SetTooltipShowDelay(1000),
                    new Button()
                        .SetText("Rich Content")
                        .SetPadding(new(8, 4))
                        .SetTooltip(new VStack(
                            new Label().SetText("Rich Tooltip").SetTextColor(SKColors.Yellow).SetFontWeight(FontWeight.Bold),
                            new Label().SetText("With multiple lines").SetTextColor(SKColors.White),
                            new Label().SetText("And custom styling").SetTextColor(SKColors.LightGray)))),

                new Label()
                    .SetText("Hit the button below to Change my color")
                    .BindTextColor(nameof(vm.Color), () => vm.Color),
                new HStack(
                    new Button()
                        .SetText("Hello World!?")
                        .SetPadding(new(10, 5))
                        .SetCommand(vm.SetColorCommand)
                        .SetTooltip("Click to change the label color above"),
                    new Checkbox()
                        .BindIsChecked(nameof(vm.Checked), () => vm.Checked, isChecked => vm.Checked = isChecked)
                        .SetColor(new SKColor(255, 0, 0))
                        .SetTooltip("Red checkbox - bound to same value"),
                    new Checkbox()
                        .BindIsChecked(nameof(vm.Checked), () => vm.Checked, isChecked => vm.Checked = isChecked)
                        .SetColor(new SKColor(0, 255, 0))
                        .SetTooltip("Green checkbox - bound to same value"),

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
            => element.SetBackground(new SolidColorBackground(SKColors.White)));
    }

    private HStack CreateTestButtons()
    {
        return new HStack(
            // Spalte 1
            new VStack(
                new Button()
                    .SetText("New Controls Demo")
                    .SetPadding(new(20, 5))
                    .SetBackground(new SolidColorBackground(new SKColor(52, 199, 89)))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(NewControlsDemoPage)),
                new Button()
                    .SetText("Go to Grid")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(ControlsGridPage)),
                new Button()
                    .SetText("Go to TextRendering")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(TextRenderPage)),
                new Button()
                    .SetText("Text Wrap & Truncation")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(TextWrapDemoPage)),
                new Button()
                    .SetText("Custom Font Demo")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(FontDemoPage)),
                new Button()
                    .SetText("Go to Form")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(FormDemoPage)),
                new Button()
                    .SetText("Go to Button Demo")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(ButtonDemoPage)),
                new Button()
                    .SetText("Go to Link Demo")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(LinkDemoPage)),
                new Button()
                    .SetText("Popup")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.PopupCommand),
                new Button()
                    .SetText("ScrollView Example")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(ScrollViewExamplePage))
            ),

            // Spalte 2
            new VStack(
                new Button()
                    .SetText("ItemsList Example")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(ItemsListDemoPage)),
                new Button()
                    .SetText("Shadow Demo")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(ShadowDemoPage)),
                new Button()
                    .SetText("RawUserControl")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(RawUserControlPage)),
                new Button()
                    .SetText("BgTest Example")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(BgTestPage)),
                new Button()
                    .SetText("Toolbar Demo")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(ToolbarDemoPage)),
                new Button()
                    .SetText("ComboBox Demo")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(ComboBoxDemoPage)),
                new Button()
                    .SetText("DatePicker & TimePicker")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(DateTimePickerDemoPage)),
                new Button()
                    .SetText("RadioButton Demo")
                    .SetPadding(new(20, 5))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(RadioButtonDemoPage)),
                new Button()
                    .SetText("Accessibility Demo")
                    .SetPadding(new(20, 5))
                    .SetBackground(new SolidColorBackground(new SKColor(0, 122, 255)))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(AccessibilityDemoPage)),
                new Button()
                    .SetText("TabControl Demo")
                    .SetPadding(new(20, 5))
                    .SetBackground(new SolidColorBackground(new SKColor(255, 149, 0)))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(TabControlDemoPage)),
                new Button()
                    .SetText("Gesture & Haptic Demo")
                    .SetPadding(new(20, 5))
                    .SetBackground(new SolidColorBackground(new SKColor(175, 82, 222)))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(GestureDemoPage)),
                new Button()
                    .SetText("DataGrid Demo")
                    .SetPadding(new(20, 5))
                    .SetBackground(new SolidColorBackground(new SKColor(255, 59, 48)))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(DataGridDemoPage)),
                new Button()
                    .SetText("TreeView Demo")
                    .SetPadding(new(20, 5))
                    .SetBackground(new SolidColorBackground(new SKColor(88, 86, 214)))
                    .SetCommand(vm.NavigateCommand)
                    .SetCommandParameter(typeof(TreeViewDemoPage))
            ).SetMargin(new Margin(20, 0, 0, 0))
        );
    }

    public override void Appearing()
    {
        base.Appearing();
        vm.SetColorCommand.Execute(null);
    }
}
