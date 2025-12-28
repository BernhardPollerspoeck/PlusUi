using PlusUi.core;
using SkiaSharp;

namespace Sandbox.Pages.TreeViewDemo;

public class TreeViewDemoPage(TreeViewDemoPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        return new VStack(
            new Button()
                .SetText("<- Back")
                .SetTextSize(16)
                .SetCommand(vm.GoBackCommand)
                .SetTextColor(SKColors.White)
                .SetPadding(new Margin(10, 5)),

            new Label()
                .SetText("TreeView Demo - File System Explorer")
                .SetTextColor(SKColors.White)
                .SetTextSize(20)
                .SetFontWeight(FontWeight.Bold)
                .SetMargin(new Margin(0, 10)),

            new HStack(
                // TreeView
                new TreeView()
                    .SetItemsSource(vm.RootItems)
                    .SetChildrenSelector<Drive>(d => d.Folders.Cast<object>())
                    .SetChildrenSelector<Folder>(f => f.SubFolders.Cast<object>().Concat(f.Files.Cast<object>()))
                    .SetItemTemplate((item, depth) => item switch
                    {
                        Drive d => new Label()
                            .SetText(":: " + d.Name)
                            .SetTextColor(SKColors.Yellow)
                            .SetFontWeight(FontWeight.Bold),
                        Folder f => new Label()
                            .SetText("[+] " + f.Name)
                            .SetTextColor(SKColors.LightBlue),
                        FileItem file => new Label()
                            .SetText("    " + file.Name)
                            .SetTextColor(SKColors.White),
                        _ => new Label()
                            .SetText(item?.ToString() ?? "")
                            .SetTextColor(SKColors.Gray)
                    })
                    .BindSelectedItem(nameof(vm.SelectedItem),
                        () => vm.SelectedItem,
                        v => vm.SelectedItem = v)
                    .SetItemHeight(28)
                    .SetIndentation(24)
                    .SetExpanderSize(16)
                    .SetDesiredWidth(400)
                    .SetDesiredHeight(400)
                    .SetBackground(new SolidColorBackground(new SKColor(30, 30, 30))),

                // Selection info panel
                new VStack(
                    new Label()
                        .SetText("Selected:")
                        .SetTextColor(SKColors.Gray)
                        .SetTextSize(14),
                    new Label()
                        .BindText(nameof(vm.SelectedItemName), () => vm.SelectedItemName)
                        .SetTextColor(SKColors.White)
                        .SetTextSize(16)
                        .SetFontWeight(FontWeight.Bold),

                    new Label()
                        .SetText("Features:")
                        .SetTextColor(SKColors.Gray)
                        .SetTextSize(14)
                        .SetMargin(new Margin(0, 20, 0, 5)),
                    new Label()
                        .SetText("- Heterogeneous child types (Drive, Folder, File)")
                        .SetTextColor(SKColors.LightGray)
                        .SetTextSize(12),
                    new Label()
                        .SetText("- Lazy loading of children on expand")
                        .SetTextColor(SKColors.LightGray)
                        .SetTextSize(12),
                    new Label()
                        .SetText("- Click expander to expand/collapse")
                        .SetTextColor(SKColors.LightGray)
                        .SetTextSize(12),
                    new Label()
                        .SetText("- Click item to select")
                        .SetTextColor(SKColors.LightGray)
                        .SetTextSize(12),
                    new Label()
                        .SetText("- Virtualized rendering")
                        .SetTextColor(SKColors.LightGray)
                        .SetTextSize(12)
                ).SetMargin(new Margin(20, 0, 0, 0))
            )
        ).SetMargin(new Margin(20));
    }
}
