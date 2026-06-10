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
            .AddChild(BuildSidebar(), row: 0, column: 0)
            .AddChild(BuildWelcome(), row: 0, column: 1);
    }

    private UiElement BuildSidebar()
    {
        return new Border()
            .SetBackground(PlusUiDefaults.BackgroundPrimary)
            .SetStrokeThickness(0)
            .SetDesiredWidth(240)
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
                        new ItemsList<SidebarRow>()
                            .BindItemsSource(() => vm.Rows)
                            .SetItemTemplate((row, _) => row switch
                            {
                                SidebarHeader header => BuildSidebarHeader(header),
                                SidebarItem item => BuildSidebarItem(item),
                                _ => new Label()
                            })));
    }

    private static UiElement BuildSidebarHeader(SidebarHeader header) =>
        new Label()
            .SetText(header.Title.ToUpperInvariant())
            .SetTextSize(11)
            .SetFontWeight(FontWeight.SemiBold)
            .SetTextColor(PlusUiDefaults.TextSecondary)
            .SetMargin(new Margin(12, 14, 12, 4));

    private UiElement BuildSidebarItem(SidebarItem item)
    {
        var button = new Button()
            .SetText(item.Name)
            .SetMargin(new Margin(4, 2))
            .SetHorizontalAlignment(HorizontalAlignment.Stretch);

        if (item.Available)
        {
            button.SetCommand(vm.NavigateToDemoCommand).SetCommandParameter(item.Name);
        }
        else
        {
            button.SetTextColor(PlusUiDefaults.TextDisabled);
        }

        return button;
    }

    private UiElement BuildWelcome()
    {
        return new VStack()
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
                    .SetMargin(new(0, 0, 0, 10))
                    .SetHorizontalAlignment(HorizontalAlignment.Center))
            .AddChild(
                new Label()
                    .SetText("""
                        PlusUi is a cross-platform UI framework that draws every control itself instead of wrapping native platform controls. This means your UI renders identically on Windows, macOS, Linux, Android, iOS, and Web - no platform-specific quirks, no "it works differently on iOS" surprises, no visual inconsistencies between platforms.

                        All controls are implemented from scratch with a focus on consistency and predictability. The layout system follows clear rules. Data binding just works. PlusUi uses a fluent API with method chaining - no XAML, no markup languages, no stringly-typed bindings - everything is pure, type-safe C# code with full IntelliSense and compile-time checking.

                        Select a control from the sidebar to see it in action.
                        """)
                    .SetTextWrapping(TextWrapping.WordWrap)
                    .SetMargin(new(40, 0)))
            .AddChild(
                new Label()
                    .SetText("built by Bernhard Pollerspöck - QSP")
                    .SetHorizontalAlignment(HorizontalAlignment.Right)
                    .SetMargin(new(0, 0, 20, 0)));
    }
}
