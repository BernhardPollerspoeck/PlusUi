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
using Sandbox.Pages.MenuDemo;
using Sandbox.Pages.ImageExportDemo;
using Sandbox.Pages.SvgDemo;
using Sandbox.Pages.UniformGridDemo;
using Sandbox.Pages.WrapDemo;
using Sandbox.Pages.RenderLoopDemo;
using SkiaSharp;

namespace Sandbox.Pages.Main;

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        //SetBackgroundColor(Colors.SlateBlue);
        return new HStack(
            new VStack(
                new HStack(
                    new VStack(
                        new HStack(
                            new Solid().SetBackground(new Color(0, 255, 255)).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new SolidColorBackground(new Color(255, 0, 255))).SetIsVisible(false).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new SolidColorBackground(new Color(255, 255, 0))).SetVisualOffset(new(0, 10)).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new LinearGradient(Colors.White, Colors.Black, 90)).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new SolidColorBackground(new Color(255, 0, 0))).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new SolidColorBackground(new Color(0, 255, 0))).SetDesiredSize(new(75, 75))
                            ),
                        new HStack(
                            new Solid().SetBackground(new SolidColorBackground(new Color(0, 0, 255))).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new LinearGradient(Colors.Red, Colors.Cyan, 45)).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new SolidColorBackground(new Color(255, 255, 255))).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new SolidColorBackground(new Color(128, 128, 128))).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new SolidColorBackground(new Color(50, 50, 50))).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new RadialGradient(Colors.Yellow, Colors.Purple)).SetDesiredSize(new(75, 75))
                            ),
                        new HStack(
                            new Solid().SetBackground(new LinearGradient(Colors.Lime, Colors.Navy, 180)).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new RadialGradient(Colors.White, Colors.Black)).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new MultiStopGradient(
                                90,
                                new GradientStop(Colors.Red, 0),
                                new GradientStop(Colors.Blue, 0.5f),
                                new GradientStop(Colors.Yellow, 1))).SetDesiredSize(new(75, 75)),
                            new Solid().SetBackground(new MultiStopGradient(
                                0,
                                new GradientStop(Colors.Magenta, 0),
                                new GradientStop(Colors.Green, 0.5f),
                                new GradientStop(Colors.Orange, 1))).SetDesiredSize(new(75, 75))
                                )),
                    new VStack(
                        new Label()
                            .SetText("Animated GIFs:")
                            .SetTextColor(Colors.White),
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
                    .SetTextColor(Colors.Yellow)
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
                            new Label().SetText("Rich Tooltip").SetTextColor(Colors.Yellow).SetFontWeight(FontWeight.Bold),
                            new Label().SetText("With multiple lines").SetTextColor(Colors.White),
                            new Label().SetText("And custom styling").SetTextColor(Colors.LightGray)))),

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
                        .SetColor(new Color(255, 0, 0))
                        .SetTooltip("Red checkbox - bound to same value"),
                    new Checkbox()
                        .BindIsChecked(nameof(vm.Checked), () => vm.Checked, isChecked => vm.Checked = isChecked)
                        .SetColor(new Color(0, 255, 0))
                        .SetTooltip("Green checkbox - bound to same value"),

                new Border()
                    .AddChild(new Label().SetText("Solid Border").SetTextColor(Colors.Black))
                    .SetStrokeColor(Colors.Red)
                    .SetStrokeThickness(3f)
                    .SetStrokeType(StrokeType.Solid)
                    .SetBackground(new SolidColorBackground(new Color(255, 255, 0, 100))),

                new Border()
                    .AddChild(new Label().SetText("Dashed").SetTextColor(Colors.Black))
                    .SetStrokeColor(Colors.Blue)
                    .SetStrokeThickness(2f)
                    .SetStrokeType(StrokeType.Dashed)
                    .SetBackground(new SolidColorBackground(new Color(0, 255, 255, 100))),

                new Border()
                    .AddChild(new VStack(
                        new Label().SetText("Dotted").SetTextColor(Colors.White),
                        new Label().SetText("Multi-line").SetTextColor(Colors.White)))
                    .SetStrokeColor(Colors.Green)
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
            new VStack(BuildLabelTest()),
            CreateTestButtons()
                    );
    }

    protected override void ConfigurePageStyles(Style pageStyle)
    {
        pageStyle.AddStyle<UiPageElement>(element
            => element.SetBackground(new SolidColorBackground(Colors.White)));
    }

    private UiElement[] BuildLabelTest()
    {
        var count = 25;
        var result = new UiElement[count];

        for (int i = 0; i < count; i++)
        {
            var size = count - i;
            result[i] = new Label()
                .SetText($"My size is {size}")
                .SetCornerRadius(0)
                .SetTextSize(size)
                .SetTextColor(i%2 ==0 ? Colors.Black : Colors.White)
                .SetBackground(i % 2 == 0 ? Colors.White : Colors.Black)
                .SetMargin(new(0));
        }

        return result;
    }

    private HStack CreateTestButtons()
    {
        return new HStack(
            new Button()
                .SetText("🔄 Render Loop Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(255, 59, 48)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(RenderLoopDemoPage)),
            new Button()
                .SetText("New Controls Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(52, 199, 89)))
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
                .SetCommandParameter(typeof(ScrollViewExamplePage)),
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
                .SetBackground(new SolidColorBackground(new Color(0, 122, 255)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(AccessibilityDemoPage)),
            new Button()
                .SetText("TabControl Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(255, 149, 0)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(TabControlDemoPage)),
            new Button()
                .SetText("Gesture & Haptic Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(175, 82, 222)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(GestureDemoPage)),
            new Button()
                .SetText("DataGrid Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(255, 59, 48)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(DataGridDemoPage)),
            new Button()
                .SetText("TreeView Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(88, 86, 214)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(TreeViewDemoPage)),
            new Button()
                .SetText("Menu Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(52, 120, 246)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(MenuDemoPage)),
            new Button()
                .SetText("Image Export Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(255, 140, 0)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(ImageExportDemoPage)),
            new Button()
                .SetText("SVG Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(0, 191, 255)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(SvgDemoPage)),
            new Button()
                .SetText("UniformGrid Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(138, 43, 226)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(UniformGridDemoPage)),
            new Button()
                .SetText("Wrap Demo")
                .SetPadding(new(20, 5))
                .SetBackground(new SolidColorBackground(new Color(50, 205, 50)))
                .SetCommand(vm.NavigateCommand)
                .SetCommandParameter(typeof(WrapDemoPage))
        ).SetWrap(true);
    }

    public override void Appearing()
    {
        base.Appearing();
        vm.SetColorCommand.Execute(null);
    }
}
