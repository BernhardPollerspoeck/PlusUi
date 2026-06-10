using PlusUi.core;

namespace PlusUi.Demo.Pages.Shared;

/// <summary>
/// Base class for control showcase pages. Renders a consistent header, scrollable
/// body and back button. Derived pages only provide the control name, a short
/// description and the demo sections.
///
/// IMPORTANT: demo sections should show the target control with minimal/no styling
/// so its default out-of-the-box appearance is visible. The card chrome around the
/// controls is provided by <see cref="Section"/>, not by the controls themselves.
/// </summary>
public abstract class DemoPage(DemoPageViewModel vm) : UiPageElement(vm)
{
    protected abstract string ControlName { get; }
    protected abstract string Description { get; }
    protected abstract IEnumerable<UiElement> BuildSections();

    protected sealed override UiElement Build()
    {
        var content = new VStack()
            .SetSpacing(24)
            .SetMargin(new Margin(40, 24, 40, 40))
            .AddChild(BuildHeader());

        foreach (var section in BuildSections())
            content.AddChild(section);

        return DemoChrome.WithStickyBack(content, vm.GoBackCommand);
    }

    private UiElement BuildHeader() =>
        new VStack()
            .SetSpacing(8)
            .AddChild(new Label()
                .SetText(ControlName)
                .SetTextSize(32)
                .SetFontWeight(FontWeight.Bold)
                .SetTextColor(PlusUiDefaults.AccentPrimary))
            .AddChild(new Label()
                .SetText(Description)
                .SetTextColor(PlusUiDefaults.TextSecondary)
                .SetTextWrapping(TextWrapping.WordWrap));

    /// <summary>A titled card grouping one or more demo elements.</summary>
    protected static UiElement Section(string title, params UiElement[] children)
    {
        var inner = new VStack()
            .SetSpacing(16)
            .SetMargin(new Margin(16));
        foreach (var child in children)
            inner.AddChild(child);

        return new VStack()
            .SetSpacing(12)
            .AddChild(new Label()
                .SetText(title)
                .SetTextSize(18)
                .SetFontWeight(FontWeight.SemiBold)
                .SetTextColor(PlusUiDefaults.AccentPrimary))
            .AddChild(new Border()
                .SetBackground(PlusUiDefaults.BackgroundSecondary)
                .SetCornerRadius(8)
                .SetStrokeThickness(0)
                .AddChild(inner));
    }

    /// <summary>A captioned single demo: a small grey label above the control.</summary>
    protected static UiElement Demo(string caption, UiElement control) =>
        new VStack()
            .SetSpacing(6)
            .AddChild(new Label()
                .SetText(caption)
                .SetTextSize(12)
                .SetTextColor(PlusUiDefaults.TextSecondary))
            .AddChild(control);

    /// <summary>A small secondary explanatory note.</summary>
    protected static Label Note(string text) =>
        new Label()
            .SetText(text)
            .SetTextSize(12)
            .SetTextColor(PlusUiDefaults.TextSecondary)
            .SetTextWrapping(TextWrapping.WordWrap);

    /// <summary>A simple colored, labelled box - used to make layout arrangement visible.</summary>
    protected static Border Tile(string text) =>
        new Border()
            .SetBackground(PlusUiDefaults.BackgroundControl)
            .SetCornerRadius(6)
            .SetStrokeThickness(0)
            .AddChild(new Label()
                .SetText(text)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetMargin(new Margin(16, 10)));
}
