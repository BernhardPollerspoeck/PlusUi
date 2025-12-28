using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.MenuDemo;

public class MenuDemoPage(MenuDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            // Back button
            new Button()
                .SetText("<- Back")
                .SetTextSize(16)
                .SetCommand(vm.GoBackCommand)
                .SetTextColor(SKColors.White)
                .SetPadding(new Margin(10, 5)),

            // Menu bar
            new Menu()
                .AddItem(new MenuItem()
                    .SetText("File")
                    .AddItem(new MenuItem().SetText("New").SetShortcut("Ctrl+N").SetCommand(vm.NewCommand))
                    .AddItem(new MenuItem().SetText("Open").SetShortcut("Ctrl+O").SetCommand(vm.OpenCommand))
                    .AddItem(new MenuItem().SetText("Save").SetShortcut("Ctrl+S").SetCommand(vm.SaveCommand))
                    .AddSeparator()
                    .AddItem(new MenuItem()
                        .SetText("Recent Files")
                        .AddItem(new MenuItem().SetText("Document1.txt"))
                        .AddItem(new MenuItem().SetText("Document2.txt"))
                        .AddItem(new MenuItem().SetText("Document3.txt")))
                    .AddSeparator()
                    .AddItem(new MenuItem().SetText("Exit").SetCommand(vm.ExitCommand)))
                .AddItem(new MenuItem()
                    .SetText("Edit")
                    .AddItem(new MenuItem().SetText("Undo").SetShortcut("Ctrl+Z").SetCommand(vm.UndoCommand))
                    .AddItem(new MenuItem().SetText("Redo").SetShortcut("Ctrl+Y").SetCommand(vm.RedoCommand))
                    .AddSeparator()
                    .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X").SetCommand(vm.CutCommand))
                    .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C").SetCommand(vm.CopyCommand))
                    .AddItem(new MenuItem().SetText("Paste").SetShortcut("Ctrl+V").SetCommand(vm.PasteCommand))
                    .AddSeparator()
                    .AddItem(new MenuItem().SetText("Delete").SetCommand(vm.DeleteCommand)))
                .AddItem(new MenuItem()
                    .SetText("View")
                    .AddItem(new MenuItem().SetText("Zoom In").SetShortcut("Ctrl++"))
                    .AddItem(new MenuItem().SetText("Zoom Out").SetShortcut("Ctrl+-"))
                    .AddItem(new MenuItem().SetText("Reset Zoom").SetShortcut("Ctrl+0"))
                    .AddSeparator()
                    .AddItem(new MenuItem().SetText("Show Toolbar").SetIsChecked(true))
                    .AddItem(new MenuItem().SetText("Show Statusbar").SetIsChecked(true)))
                .AddItem(new MenuItem()
                    .SetText("Help")
                    .AddItem(new MenuItem().SetText("Documentation").SetShortcut("F1"))
                    .AddSeparator()
                    .AddItem(new MenuItem().SetText("About").SetCommand(vm.AboutCommand))),

            // Main content area
            new VStack(
                new Label()
                    .SetText("Menu Demo")
                    .SetTextSize(24)
                    .SetTextColor(SKColors.White)
                    .SetMargin(new Margin(0, 20)),

                new Label()
                    .SetText("Click on menu items above to test the menu functionality.")
                    .SetTextSize(14)
                    .SetTextColor(SKColors.LightGray)
                    .SetMargin(new Margin(0, 10)),

                new HStack(
                    new Label()
                        .SetText("Last action: ")
                        .SetTextSize(14)
                        .SetTextColor(SKColors.White),
                    new Label()
                        .BindText(nameof(vm.LastAction), () => vm.LastAction)
                        .SetTextSize(14)
                        .SetTextColor(SKColors.LightGreen)
                ).SetMargin(new Margin(0, 20)),

                // ContextMenu demo
                new Label()
                    .SetText("Context Menu Demo")
                    .SetTextSize(20)
                    .SetTextColor(SKColors.White)
                    .SetMargin(new Margin(0, 30, 0, 10)),

                new Label()
                    .SetText("Right-click on the button below to see a context menu:")
                    .SetTextSize(14)
                    .SetTextColor(SKColors.LightGray)
                    .SetMargin(new Margin(0, 10)),

                new Button()
                    .SetText("Right-click me!")
                    .SetTextSize(16)
                    .SetTextColor(SKColors.White)
                    .SetBackground(new SKColor(60, 60, 120))
                    .SetPadding(new Margin(20, 10))
                    .SetContextMenu(new ContextMenu()
                        .AddItem(new MenuItem().SetText("Cut").SetShortcut("Ctrl+X").SetCommand(vm.CutCommand))
                        .AddItem(new MenuItem().SetText("Copy").SetShortcut("Ctrl+C").SetCommand(vm.CopyCommand))
                        .AddItem(new MenuItem().SetText("Paste").SetShortcut("Ctrl+V").SetCommand(vm.PasteCommand))
                        .AddSeparator()
                        .AddItem(new MenuItem().SetText("Delete").SetCommand(vm.DeleteCommand)))
            ).SetMargin(new Margin(20))
        );
    }
}
