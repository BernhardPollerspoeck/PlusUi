using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.ToolbarDemo;

public class ToolbarDemoPage(ToolbarDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Page Title
                new Label()
                    .SetText("Toolbar Demo")
                    .SetTextSize(32)
                    .SetTextColor(SKColors.White)
                    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
                    .SetMargin(new Margin(0, 20, 0, 10)),

                // Status display for interactive toolbars
                new Border()
                    .AddChild(
                        new VStack(
                            new Label()
                                .SetText("Last Action:")
                                .SetTextSize(14)
                                .SetTextColor(SKColors.Gray),
                            new Label()
                                .BindText(nameof(vm.LastAction), () => vm.LastAction)
                                .SetTextSize(18)
                                .SetTextColor(SKColors.LimeGreen)
                        ).SetMargin(new Margin(16, 8))
                    )
                    .SetBackground(new SolidColorBackground(new SKColor(30, 30, 30)))
                    .SetCornerRadius(8)
                    .SetMargin(new Margin(20, 0, 20, 20)),

                // Section: Interactive Toolbar with Commands
                CreateSection("Interactive Toolbar (Click the buttons!)",
                    new Toolbar()
                        .SetTitle("Interactive")
                        .AddLeft(
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(20)
                                .SetPadding(new Margin(8))
                                .SetBackground(new SolidColorBackground(SKColors.Transparent))
                                .SetCommand(vm.MenuCommand)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                        )
                        .AddRight(
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(20)
                                .SetPadding(new Margin(8))
                                .SetBackground(new SolidColorBackground(SKColors.Transparent))
                                .SetCommand(vm.SearchCommand)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                        )
                        .AddRight(
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(20)
                                .SetPadding(new Margin(8))
                                .SetBackground(new SolidColorBackground(SKColors.Transparent))
                                .SetCommand(vm.SettingsCommand)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                        )
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.DodgerBlue))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Social Actions Toolbar
                CreateSection("Social Actions (Like, Bookmark, Share)",
                    new Toolbar()
                        .SetTitle("Social Demo")
                        .AddRight(
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(20)
                                .SetPadding(new Margin(8))
                                .SetBackground(new SolidColorBackground(SKColors.Transparent))
                                .SetCommand(vm.ToggleLikeCommand)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                        )
                        .AddRight(
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(20)
                                .SetPadding(new Margin(8))
                                .SetBackground(new SolidColorBackground(SKColors.Transparent))
                                .SetCommand(vm.ToggleBookmarkCommand)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                        )
                        .AddRight(
                            new Button()
                                .SetIcon("plusui.png")
                                .SetTextSize(20)
                                .SetPadding(new Margin(8))
                                .SetBackground(new SolidColorBackground(SKColors.Transparent))
                                .SetCommand(vm.ShareCommand)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                        )
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.DeepPink))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Click Counter Toolbar
                CreateSection("Click Counter",
                    new Toolbar()
                        .SetTitle("Click the buttons!")
                        .AddLeft(
                            new Button()
                                .SetText("+1")
                                .SetTextSize(16)
                                .SetTextColor(SKColors.White)
                                .SetPadding(new Margin(12, 8))
                                .SetBackground(new SolidColorBackground(SKColors.Green))
                                .SetCornerRadius(4)
                                .SetCommand(vm.IncrementClickCommand)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                        )
                        .AddLeft(
                            new Button()
                                .SetText("+1")
                                .SetTextSize(16)
                                .SetTextColor(SKColors.White)
                                .SetPadding(new Margin(12, 8))
                                .SetBackground(new SolidColorBackground(SKColors.Orange))
                                .SetCornerRadius(4)
                                .SetCommand(vm.IncrementClickCommand)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                        )
                        .AddRight(
                            new Button()
                                .SetText("+1")
                                .SetTextSize(16)
                                .SetTextColor(SKColors.White)
                                .SetPadding(new Margin(12, 8))
                                .SetBackground(new SolidColorBackground(SKColors.Purple))
                                .SetCornerRadius(4)
                                .SetCommand(vm.IncrementClickCommand)
                                .SetVerticalAlignment(VerticalAlignment.Center)
                        )
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(new SKColor(50, 50, 50)))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Basic Toolbar with Title
                CreateSection("Basic Toolbar with Title",
                    new Toolbar()
                        .SetTitle("My Application")
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.DodgerBlue))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Toolbar with Left Icon
                CreateSection("Toolbar with Left Icon",
                    new Toolbar()
                        .SetTitle("Navigation")
                        .AddLeft(CreateIconButton("plusui.png"))
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.MediumSeaGreen))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Toolbar with Right Icons
                CreateSection("Toolbar with Right Icons",
                    new Toolbar()
                        .SetTitle("Actions")
                        .AddRight(CreateIconButton("plusui.png"))
                        .AddRight(CreateIconButton("plusui.png"))
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.OrangeRed))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Toolbar Title Alignment - Left
                CreateSection("Title Alignment - Left",
                    new Toolbar()
                        .SetTitle("Left Aligned Title")
                        .SetTitleAlignment(TitleAlignment.Left)
                        .AddLeft(CreateIconButton("plusui.png"))
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.Teal))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Toolbar Title Alignment - Center
                CreateSection("Title Alignment - Center",
                    new Toolbar()
                        .SetTitle("Center Aligned Title")
                        .SetTitleAlignment(TitleAlignment.Center)
                        .AddLeft(CreateIconButton("plusui.png"))
                        .AddRight(CreateIconButton("plusui.png"))
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.Crimson))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Toolbar with Icon Groups
                CreateSection("Toolbar with Icon Groups",
                    new Toolbar()
                        .SetTitle("Editor Toolbar")
                        .AddLeftGroup(
                            new ToolbarIconGroup(
                                CreateIconButton("plusui.png"),
                                CreateIconButton("plusui.png")
                            )
                            .SetSeparator(true)
                            .SetSpacing(4)
                            .SetPriority(10)
                        )
                        .AddLeftGroup(
                            new ToolbarIconGroup(
                                CreateIconButton("plusui.png"),
                                CreateIconButton("plusui.png"),
                                CreateIconButton("plusui.png")
                            )
                            .SetSeparator(true)
                            .SetSpacing(4)
                            .SetPriority(5)
                        )
                        .AddRightGroup(
                            new ToolbarIconGroup(
                                CreateIconButton("plusui.png"),
                                CreateIconButton("plusui.png")
                            )
                            .SetSpacing(4)
                        )
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.DarkSlateGray))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Toolbar with Custom Title Styling
                CreateSection("Custom Title Styling",
                    new Toolbar()
                        .SetTitle("Styled Title")
                        .SetTitleFontSize(28)
                        .SetTitleColor(SKColors.Gold)
                        .SetDesiredHeight(64)
                        .SetBackground(new SolidColorBackground(new SKColor(40, 40, 60)))
                ),

                // Section: Toolbar with Item Spacing
                CreateSection("Custom Item Spacing",
                    new Toolbar()
                        .SetTitle("Spaced Items")
                        .SetItemSpacing(24)
                        .AddLeft(CreateIconButton("plusui.png"))
                        .AddLeft(CreateIconButton("plusui.png"))
                        .AddRight(CreateIconButton("plusui.png"))
                        .AddRight(CreateIconButton("plusui.png"))
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.Indigo))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Toolbar with Custom Content Padding
                CreateSection("Custom Content Padding",
                    new Toolbar()
                        .SetTitle("Wide Padding")
                        .SetContentPadding(new Margin(40, 8, 40, 8))
                        .AddLeft(CreateIconButton("plusui.png"))
                        .AddRight(CreateIconButton("plusui.png"))
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.DarkOliveGreen))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Toolbar with Overflow Behavior
                // Overflow menu appears when toolbar width < threshold and items don't fit
                // Click the "..." button to see the overflow menu!
                CreateSection("Overflow Behavior - Click the overflow button (...)!",
                    new Toolbar()
                        .SetTitle("Overflow Demo")
                        .SetOverflowBehavior(OverflowBehavior.CollapseToMenu)
                        .SetOverflowThreshold(1400)
                        .AddLeft(new Button().SetText("File").SetPadding(new Margin(12, 8)).SetBackground(new SolidColorBackground(SKColors.Transparent)).SetTextColor(SKColors.White).SetVerticalAlignment(VerticalAlignment.Center))
                        .AddLeft(new Button().SetText("Edit").SetPadding(new Margin(12, 8)).SetBackground(new SolidColorBackground(SKColors.Transparent)).SetTextColor(SKColors.White).SetVerticalAlignment(VerticalAlignment.Center))
                        .AddLeft(new Button().SetText("View").SetPadding(new Margin(12, 8)).SetBackground(new SolidColorBackground(SKColors.Transparent)).SetTextColor(SKColors.White).SetVerticalAlignment(VerticalAlignment.Center))
                        .AddLeft(new Button().SetText("Tools").SetPadding(new Margin(12, 8)).SetBackground(new SolidColorBackground(SKColors.Transparent)).SetTextColor(SKColors.White).SetVerticalAlignment(VerticalAlignment.Center))
                        .AddRight(new Button().SetText("Search").SetPadding(new Margin(12, 8)).SetBackground(new SolidColorBackground(SKColors.Transparent)).SetTextColor(SKColors.White).SetVerticalAlignment(VerticalAlignment.Center))
                        .AddRight(new Button().SetText("Settings").SetPadding(new Margin(12, 8)).SetBackground(new SolidColorBackground(SKColors.Transparent)).SetTextColor(SKColors.White).SetVerticalAlignment(VerticalAlignment.Center))
                        .AddRight(new Button().SetText("Help").SetPadding(new Margin(12, 8)).SetBackground(new SolidColorBackground(SKColors.Transparent)).SetTextColor(SKColors.White).SetVerticalAlignment(VerticalAlignment.Center))
                        .AddRight(new Button().SetText("About").SetPadding(new Margin(12, 8)).SetBackground(new SolidColorBackground(SKColors.Transparent)).SetTextColor(SKColors.White).SetVerticalAlignment(VerticalAlignment.Center))
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.DarkSlateBlue))
                        .SetTitleColor(SKColors.White)
                ),

                // Section: Toolbar with Center Content
                CreateSection("Toolbar with Center Content",
                    new Toolbar()
                        .SetCenterContent(
                            new HStack(
                                new Label()
                                    .SetText("Custom Center")
                                    .SetTextSize(16)
                                    .SetTextColor(SKColors.White),
                                new Button()
                                    .SetIcon("plusui.png")
                                    .SetPadding(new Margin(8))
                                    .SetBackground(new SolidColorBackground(SKColors.Transparent))
                            )
                        )
                        .AddLeft(CreateIconButton("plusui.png"))
                        .AddRight(CreateIconButton("plusui.png"))
                        .SetDesiredHeight(56)
                        .SetBackground(new SolidColorBackground(SKColors.CadetBlue))
                ),

                // Section: Multiple Toolbars Stacked (with vertical centering fix)
                CreateSection("Multiple Toolbars",
                    new VStack(
                        new Toolbar()
                            .SetTitle("Primary")
                            .AddLeft(CreateIconButton("plusui.png"))
                            .SetDesiredHeight(48)
                            .SetBackground(new SolidColorBackground(SKColors.RoyalBlue))
                            .SetTitleColor(SKColors.White),
                        new Toolbar()
                            .SetTitle("Secondary")
                            .AddRight(CreateIconButton("plusui.png"))
                            .AddRight(CreateIconButton("plusui.png"))
                            .SetDesiredHeight(40)
                            .SetContentPadding(new Margin(16, 4, 16, 4))
                            .SetBackground(new SolidColorBackground(SKColors.LightGray))
                            .SetTitleColor(SKColors.Black)
                            .SetTitleFontSize(14)
                    )
                ),

                // Bottom padding
                new Solid().SetDesiredHeight(50).IgnoreStyling()
            )
        )
            .SetCanScrollHorizontally(false)
            ;
    }

    private Button CreateIconButton(string icon)
    {
        return new Button()
            .SetIcon(icon)
            .SetTextSize(20)
            .SetPadding(new Margin(8))
            .SetBackground(new SolidColorBackground(SKColors.Transparent))
            .SetVerticalAlignment(VerticalAlignment.Center);
    }

    private UiElement CreateSection(string title, UiElement content)
    {
        return new VStack(
            new Label()
                .SetText(title)
                .SetTextSize(20)
                .SetTextColor(SKColors.LightGray)
                .SetMargin(new Margin(0, 15, 0, 10)),
            new Border()
                .AddChild(content)
                .SetBackground(new SolidColorBackground(new SKColor(40, 40, 40)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }
}
