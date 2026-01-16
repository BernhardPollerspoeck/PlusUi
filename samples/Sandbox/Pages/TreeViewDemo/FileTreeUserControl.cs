using PlusUi.core;

namespace Sandbox.Pages.TreeViewDemo;

internal class FileTreeUserControl : UserControl
{
    private readonly TreeViewDemoPageViewModel _viewModel;

    public FileTreeUserControl(TreeViewDemoPageViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        return new VStack()
            .AddChild(
                new HStack(
                    new Button()
                        .SetText("Action")
                        .SetTextColor(Colors.White)
                        .SetTextSize(12)
                        .SetBackground(new Color(60, 60, 60))
                        .SetCornerRadius(4)
                        .SetPadding(new Margin(8, 4)))
                .SetMargin(new Margin(8))
                .SetHorizontalAlignment(HorizontalAlignment.Left))
            .AddChild(
                new TreeView()
                    .SetItemsSource(_viewModel.RootItems)
                    .SetChildrenSelector<Drive>(d => d.Folders.Cast<object>())
                    .SetChildrenSelector<Folder>(f => f.SubFolders.Cast<object>().Concat(f.Files.Cast<object>()))
                    .SetItemTemplate((item, depth) => item switch
                    {
                        Drive d => new HStack(
                            new Label()
                                .SetText(":: " + d.Name)
                                .SetTextColor(Colors.Yellow)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Button()
                                .SetText("X")
                                .SetTextColor(Colors.Gray)
                                .SetBackground(Colors.Transparent)
                                .SetDesiredWidth(20)
                                .SetDesiredHeight(20))
                            .SetSpacing(4)
                            .SetVerticalAlignment(VerticalAlignment.Center),
                        Folder f => new HStack(
                            new Label()
                                .SetText("[+] " + f.Name)
                                .SetTextColor(Colors.LightBlue)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Button()
                                .SetText("X")
                                .SetTextColor(Colors.Gray)
                                .SetBackground(Colors.Transparent)
                                .SetDesiredWidth(20)
                                .SetDesiredHeight(20))
                            .SetSpacing(4)
                            .SetVerticalAlignment(VerticalAlignment.Center),
                        FileItem file => new HStack(
                            new Label()
                                .SetText(file.Name)
                                .SetTextColor(Colors.White)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Button()
                                .SetText("X")
                                .SetTextColor(Colors.Gray)
                                .SetBackground(Colors.Transparent)
                                .SetDesiredWidth(20)
                                .SetDesiredHeight(20))
                            .SetSpacing(4)
                            .SetVerticalAlignment(VerticalAlignment.Center),
                        _ => new Label()
                            .SetText(item?.ToString() ?? "")
                            .SetTextColor(Colors.Gray)
                            .SetVerticalAlignment(VerticalAlignment.Center)
                    })
                    .SetItemHeight(28)
                    .SetIndentation(24)
                    .SetExpanderSize(16)
                    .SetShowLines(true)
                    .SetLineColor(new Color(60, 60, 60)));
    }
}
