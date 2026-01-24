using PlusUi.core;

namespace PlusUi.Demo.Pages.Main;

public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new Grid()
            .AddColumn(Column.Auto)
            .AddColumn(Column.Star)
            .AddRow(Row.Star)
            .AddChild(
                new Border()
                    .SetBackground(PlusUiDefaults.BackgroundPrimary)
                    .SetStrokeThickness(0)
                    .AddChild(
                        new VStack()
                            .AddChild(
                                new VStack()
                                    .SetSpacing(0)
                                    .AddChild(
                                        new Label()
                                            .SetText("Controls")
                                            .SetTextSize(PlusUiDefaults.FontSizeLarge)
                                            .SetFontWeight(FontWeight.SemiBold)
                                            .SetMargin(new Margin(16, 12)))
                                    .AddChild(
                                        new Border()
                                            .SetDesiredHeight(2)
                                            .SetBackground(PlusUiDefaults.AccentPrimary)
                                            .SetStrokeThickness(0)
                                            .SetHorizontalAlignment(HorizontalAlignment.Stretch)))
                            .AddChild(
                                new ItemsList<string>()
                                    .BindItemsSource(() => vm.Controls)
                                    .SetItemTemplate((name, _) =>
                                        new Button()
                                            .SetText(name)
                                            .SetMargin(new Margin(4))
                                            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                                            .SetCommand(vm.NavigateToDemoCommand)
                                            .SetCommandParameter(name)))),
                row: 0, column: 0)
            .AddChild(
                new VStack()
                    .SetHorizontalAlignment(HorizontalAlignment.Center)
                    .SetVerticalAlignment(VerticalAlignment.Center)
                    .AddChild(
                        new Image()
                            .SetImageSource("plusui.png")
                            .SetDesiredHeight(200)
                            .SetHorizontalAlignment(HorizontalAlignment.Center))
                    .AddChild(
                        new Label()
                            .SetText("Welcome to PlusUi")
                            .SetTextSize(20)
                            .SetMargin(new(0,0,0,10))
                            .SetHorizontalAlignment(HorizontalAlignment.Center))
                    .AddChild(
                        new Label()
                            .SetText("""
                                PlusUi is a cross-platform UI framework that draws every control itself instead of wrapping native platform controls. This means your UI renders identically on Windows, macOS, Linux, Android, iOS, and Web - no platform-specific quirks, no "it works differently on iOS" surprises, no visual inconsistencies between platforms.

                                If you've worked with cross-platform UI frameworks before, you know the pain: controls that look different on each platform, layout bugs that only appear on specific devices, regressions with every update that force you to retest everything. PlusUi eliminates these problems by taking full control of rendering - what you see on one platform is exactly what your users will see on every other platform.

                                All controls are implemented from scratch with a focus on consistency and predictability. The layout system follows clear rules. Data binding just works. When something doesn't behave as expected, you can debug it - no black box native controls, no platform abstraction layers hiding the actual behavior.

                                PlusUi uses a fluent API with method chaining that makes building UIs straightforward and readable. No XAML, no markup languages, no stringly-typed bindings - everything is pure, type-safe C# code with full IntelliSense and compile-time checking. Styling and theming are built in from the ground up. The framework is designed to get out of your way and let you ship your application.

                                Select a control from the sidebar to see it in action.
                                """)
                            .SetTextWrapping(TextWrapping.WordWrap)
                            .SetMargin(new(40,0)))
                    .AddChild(
                        new Label()
                            .SetText("built by Bernhard Pollerspöck - QSP")
                            .SetHorizontalAlignment(HorizontalAlignment.Right)
                            .SetMargin(new(0,0,20,0))),
                row: 0, column: 1);
    }
}
