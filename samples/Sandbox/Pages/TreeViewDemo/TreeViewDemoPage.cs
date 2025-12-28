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

            new TreeView()
                .SetItemsSource(vm.RootItems)
                .SetChildrenSelector<Drive>(d => d.Folders.Cast<object>())
                .SetChildrenSelector<Folder>(f => f.SubFolders.Cast<object>().Concat(f.Files.Cast<object>()))
                .SetItemTemplate((item, depth) => item switch
                {
                    Drive d => new Label()
                        .SetText(":: " + d.Name)
                        .SetTextColor(SKColors.Yellow)
                        .SetVerticalAlignment(VerticalAlignment.Center),
                    Folder f => new Label()
                        .SetText("[+] " + f.Name)
                        .SetTextColor(SKColors.LightBlue)
                        .SetVerticalAlignment(VerticalAlignment.Center),
                    FileItem file => new Label()
                        .SetText(file.Name)
                        .SetTextColor(SKColors.White)
                        .SetVerticalAlignment(VerticalAlignment.Center),
                    _ => new Label()
                        .SetText(item?.ToString() ?? "")
                        .SetTextColor(SKColors.Gray)
                        .SetVerticalAlignment(VerticalAlignment.Center)
                })
                .SetItemHeight(28)
                .SetIndentation(24)
                .SetExpanderSize(16)
                .SetShowLines(true)
                .SetLineColor(new SKColor(60, 60, 60))
        );
    }
}
