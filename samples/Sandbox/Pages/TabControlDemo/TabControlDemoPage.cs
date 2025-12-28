using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.TabControlDemo;

public class TabControlDemoPage(TabControlDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new ScrollView(
            new VStack(
                // Header with back button
                new HStack(
                    new Button()
                        .SetText("<- Back")
                        .SetTextSize(16)
                        .SetCommand(vm.GoBackCommand)
                        .SetTextColor(Colors.White)
                        .SetPadding(new Margin(10, 5)),
                    new Label()
                        .SetText("TabControl Demo")
                        .SetTextSize(24)
                        .SetTextColor(Colors.White)
                        .SetMargin(new Margin(20, 0, 0, 0))
                ).SetMargin(new Margin(10, 10, 0, 10)),

                // Section: Basic TabControl
                CreateSection("Basic TabControl (Tabs on Top)",
                    new TabControl()
                        .AddTab(new TabItem()
                            .SetHeader("General")
                            .SetContent(CreateTabContent("General Settings", "This is the general settings tab.")))
                        .AddTab(new TabItem()
                            .SetHeader("Advanced")
                            .SetContent(CreateTabContent("Advanced Settings", "Configure advanced options here.")))
                        .AddTab(new TabItem()
                            .SetHeader("About")
                            .SetContent(CreateTabContent("About", "TabControl Demo v1.0")))
                        .SetDesiredSize(new Size(500, 200))
                        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                        .SetCornerRadius(8)
                ),

                // Section: TabControl with Binding
                CreateSection("TabControl with Index Binding",
                    new VStack(
                        new HStack(
                            new Label()
                                .SetText("Selected Tab Index: ")
                                .SetTextSize(14)
                                .SetTextColor(Colors.Gray),
                            new Label()
                                .BindText(nameof(vm.SelectedTabIndex), () => vm.SelectedTabIndex.ToString())
                                .SetTextSize(14)
                                .SetTextColor(Colors.LimeGreen)
                        ).SetMargin(new Margin(0, 0, 0, 10)),
                        new TabControl()
                            .AddTab(new TabItem()
                                .SetHeader("Tab 1")
                                .SetContent(CreateTabContent("First Tab", "Content for the first tab.")))
                            .AddTab(new TabItem()
                                .SetHeader("Tab 2")
                                .SetContent(CreateTabContent("Second Tab", "Content for the second tab.")))
                            .AddTab(new TabItem()
                                .SetHeader("Tab 3")
                                .SetContent(CreateTabContent("Third Tab", "Content for the third tab.")))
                            .BindSelectedIndex(nameof(vm.SelectedTabIndex), () => vm.SelectedTabIndex, i => vm.SelectedTabIndex = i)
                            .SetDesiredSize(new Size(500, 180))
                            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                            .SetCornerRadius(8)
                    )
                ),

                // Section: Tabs on Bottom
                CreateSection("Tabs on Bottom",
                    new TabControl()
                        .AddTab(new TabItem()
                            .SetHeader("Home")
                            .SetContent(CreateTabContent("Home", "Welcome to the home screen!")))
                        .AddTab(new TabItem()
                            .SetHeader("Search")
                            .SetContent(CreateTabContent("Search", "Search for items here.")))
                        .AddTab(new TabItem()
                            .SetHeader("Profile")
                            .SetContent(CreateTabContent("Profile", "Your profile information.")))
                        .SetTabPosition(TabPosition.Bottom)
                        .SetDesiredSize(new Size(500, 180))
                        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                        .SetCornerRadius(8)
                ),

                // Section: Vertical Tabs (Left)
                CreateSection("Vertical Tabs (Left Side)",
                    new TabControl()
                        .AddTab(new TabItem()
                            .SetHeader("Dashboard")
                            .SetContent(CreateTabContent("Dashboard", "Overview of your data.")))
                        .AddTab(new TabItem()
                            .SetHeader("Reports")
                            .SetContent(CreateTabContent("Reports", "View and generate reports.")))
                        .AddTab(new TabItem()
                            .SetHeader("Settings")
                            .SetContent(CreateTabContent("Settings", "Application settings.")))
                        .SetTabPosition(TabPosition.Left)
                        .BindSelectedIndex(nameof(vm.VerticalTabIndex), () => vm.VerticalTabIndex, i => vm.VerticalTabIndex = i)
                        .SetDesiredSize(new Size(500, 200))
                        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                        .SetCornerRadius(8)
                ),

                // Section: Vertical Tabs (Right)
                CreateSection("Vertical Tabs (Right Side)",
                    new TabControl()
                        .AddTab(new TabItem()
                            .SetHeader("Files")
                            .SetContent(CreateTabContent("Files", "Browse your files.")))
                        .AddTab(new TabItem()
                            .SetHeader("Shared")
                            .SetContent(CreateTabContent("Shared", "Files shared with you.")))
                        .AddTab(new TabItem()
                            .SetHeader("Trash")
                            .SetContent(CreateTabContent("Trash", "Deleted files.")))
                        .SetTabPosition(TabPosition.Right)
                        .SetDesiredSize(new Size(500, 200))
                        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                        .SetCornerRadius(8)
                ),

                // Section: Custom Styled TabControl
                CreateSection("Custom Styled TabControl",
                    new TabControl()
                        .AddTab(new TabItem()
                            .SetHeader("Primary")
                            .SetContent(CreateTabContent("Primary", "Main content area.")))
                        .AddTab(new TabItem()
                            .SetHeader("Secondary")
                            .SetContent(CreateTabContent("Secondary", "Additional content.")))
                        .AddTab(new TabItem()
                            .SetHeader("Tertiary")
                            .SetContent(CreateTabContent("Tertiary", "More content here.")))
                        .SetHeaderTextColor(Colors.LightGray)
                        .SetActiveHeaderTextColor(Colors.Cyan)
                        .SetHeaderBackgroundColor(new Color(20, 40, 60))
                        .SetActiveTabBackgroundColor(new Color(30, 60, 90))
                        .SetHoverTabBackgroundColor(new Color(25, 50, 75))
                        .SetTabIndicatorColor(Colors.Cyan)
                        .SetTabIndicatorHeight(4)
                        .SetHeaderTextSize(16)
                        .SetTabPadding(new Margin(20, 12))
                        .SetDesiredSize(new Size(500, 200))
                        .SetBackground(new SolidColorBackground(new Color(15, 30, 45)))
                        .SetCornerRadius(12)
                ),

                // Section: TabControl with Disabled Tab
                CreateSection("TabControl with Disabled Tab",
                    new TabControl()
                        .AddTab(new TabItem()
                            .SetHeader("Enabled")
                            .SetContent(CreateTabContent("Enabled Tab", "This tab is enabled.")))
                        .AddTab(new TabItem()
                            .SetHeader("Disabled")
                            .SetIsEnabled(false)
                            .SetContent(CreateTabContent("Disabled Tab", "You shouldn't see this!")))
                        .AddTab(new TabItem()
                            .SetHeader("Also Enabled")
                            .SetContent(CreateTabContent("Also Enabled", "This tab is also enabled.")))
                        .SetDesiredSize(new Size(500, 180))
                        .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                        .SetCornerRadius(8)
                ),

                // Section: Dynamic Tab Position
                CreateSection("Dynamic Tab Position",
                    new VStack(
                        new HStack(
                            new Label()
                                .SetText("Current Position: ")
                                .SetTextSize(14)
                                .SetTextColor(Colors.Gray),
                            new Label()
                                .BindText(nameof(vm.DynamicTabPosition), () => vm.DynamicTabPosition.ToString())
                                .SetTextSize(14)
                                .SetTextColor(Colors.Orange),
                            new Button()
                                .SetText("Cycle Position")
                                .SetCommand(vm.CycleTabPositionCommand)
                                .SetTextColor(Colors.White)
                                .SetPadding(new Margin(12, 6))
                                .SetBackground(new SolidColorBackground(new Color(80, 80, 80)))
                                .SetCornerRadius(4)
                                .SetMargin(new Margin(20, 0, 0, 0))
                        ).SetMargin(new Margin(0, 0, 0, 10)),
                        new TabControl()
                            .AddTab(new TabItem()
                                .SetHeader("Alpha")
                                .SetContent(CreateTabContent("Alpha", "First dynamic tab.")))
                            .AddTab(new TabItem()
                                .SetHeader("Beta")
                                .SetContent(CreateTabContent("Beta", "Second dynamic tab.")))
                            .AddTab(new TabItem()
                                .SetHeader("Gamma")
                                .SetContent(CreateTabContent("Gamma", "Third dynamic tab.")))
                            .BindTabPosition(nameof(vm.DynamicTabPosition), () => vm.DynamicTabPosition)
                            .SetDesiredSize(new Size(500, 200))
                            .SetBackground(new SolidColorBackground(new Color(30, 30, 30)))
                            .SetCornerRadius(8)
                    )
                ),

                // Bottom padding
                new Solid().SetDesiredHeight(50).IgnoreStyling()
            )
        )
        .SetCanScrollHorizontally(false);
    }

    private UiElement CreateSection(string title, UiElement content)
    {
        return new VStack(
            new Label()
                .SetText(title)
                .SetTextSize(20)
                .SetTextColor(Colors.LightGray)
                .SetMargin(new Margin(0, 15, 0, 10)),
            new Border()
                .AddChild(
                    new VStack(content)
                        .SetMargin(new Margin(16))
                )
                .SetBackground(new SolidColorBackground(new Color(40, 40, 40)))
                .SetCornerRadius(12)
                .SetMargin(new Margin(0, 0, 0, 10))
        ).SetMargin(new Margin(20, 0));
    }

    private UiElement CreateTabContent(string title, string description)
    {
        return new VStack(
            new Label()
                .SetText(title)
                .SetTextSize(18)
                .SetTextColor(Colors.White)
                .SetMargin(new Margin(0, 0, 0, 8)),
            new Label()
                .SetText(description)
                .SetTextSize(14)
                .SetTextColor(Colors.Gray)
        ).SetMargin(new Margin(16));
    }
}
