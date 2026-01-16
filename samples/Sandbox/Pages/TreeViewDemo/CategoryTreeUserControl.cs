using PlusUi.core;

namespace Sandbox.Pages.TreeViewDemo;

internal class CategoryTreeUserControl : UserControl
{
    private readonly TreeViewDemoPageViewModel _viewModel;

    public CategoryTreeUserControl(TreeViewDemoPageViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    protected override UiElement Build()
    {
        return new VStack()
            .AddChild(
                new HStack(
                    new Button()
                        .SetText("Action 2")
                        .SetTextColor(Colors.White)
                        .SetTextSize(12)
                        .SetBackground(new Color(60, 60, 60))
                        .SetCornerRadius(4)
                        .SetPadding(new Margin(8, 4)))
                .SetMargin(new Margin(8))
                .SetHorizontalAlignment(HorizontalAlignment.Left))
            .AddChild(
                new TreeView()
                    .SetItemsSource(_viewModel.CategoryItems)
                    .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>().Concat(c.Products.Cast<object>()))
                    .SetItemTemplate((item, depth) => item switch
                    {
                        Category c => new HStack(
                            new Label()
                                .SetText("[#] " + c.Name)
                                .SetTextColor(Colors.LightGreen)
                                .SetVerticalAlignment(VerticalAlignment.Center),
                            new Button()
                                .SetText("X")
                                .SetTextColor(Colors.Gray)
                                .SetBackground(Colors.Transparent)
                                .SetDesiredWidth(20)
                                .SetDesiredHeight(20))
                            .SetSpacing(4)
                            .SetVerticalAlignment(VerticalAlignment.Center),
                        Product p => new HStack(
                            new Label()
                                .SetText(p.Name)
                                .SetTextColor(Colors.Orange)
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
                    .SetLineColor(new Color(80, 80, 60)));
    }
}
