using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class HoverPageViewModel(INavigationService navigation) : DemoPageViewModel(navigation)
{
    private int _enterCount;

    [ObservableProperty]
    private string _hoverState = "Move the pointer over the box";

    [RelayCommand] private void HoverEnter() => HoverState = $"Pointer entered (count: {++_enterCount})";
    [RelayCommand] private void HoverLeave() => HoverState = "Pointer left the box";
}

public class HoverPage(HoverPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Hover & Cursor";

    protected override string Description =>
        "Any element can fire hover enter/leave commands and set the mouse cursor (desktop). " +
        "Both act on the element directly under the pointer - no interface to implement.";

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Hover enter / leave commands",
            Note("Move the pointer onto the box - enter and leave fire ICommands on the ViewModel."),
            new Button()
                .SetText("Hover me")
                .SetDesiredSize(new Size(170, 90))
                .SetHorizontalAlignment(HorizontalAlignment.Left)
                .SetOnHoverEnterCommand(vm.HoverEnterCommand)
                .SetOnHoverExitCommand(vm.HoverLeaveCommand)),

        Section("Hover status",
            new Label().BindText(() => vm.HoverState).SetFontWeight(FontWeight.Bold)),

        Section("Mouse cursor (desktop only)",
            Note("Hover each box - on desktop the cursor changes to match its CursorType. " +
                 "Standard shapes use the native OS cursor (via GLFW); PlusUi, Wait and Not-allowed " +
                 "are self-drawn so they render even where the OS/GLFW has no such cursor. " +
                 "On touch platforms this is a no-op."),
            new HStack().SetSpacing(12)
                .AddChild(CursorBox("PlusUi", CursorType.PlusUi))
                .AddChild(CursorBox("Default", CursorType.Default))
                .AddChild(CursorBox("Arrow", CursorType.Arrow)),
            new HStack().SetSpacing(12)
                .AddChild(CursorBox("Hand", CursorType.Hand))
                .AddChild(CursorBox("Text", CursorType.Text))
                .AddChild(CursorBox("Crosshair", CursorType.Crosshair)),
            new HStack().SetSpacing(12)
                .AddChild(CursorBox("Wait", CursorType.Wait))
                .AddChild(CursorBox("Progress", CursorType.Progress))
                .AddChild(CursorBox("Not allowed", CursorType.NotAllowed)),
            new HStack().SetSpacing(12)
                .AddChild(CursorBox("Resize ↔", CursorType.ResizeHorizontal))
                .AddChild(CursorBox("Resize ↕", CursorType.ResizeVertical))
                .AddChild(CursorBox("Move (all)", CursorType.ResizeAll)),
            new HStack().SetSpacing(12)
                .AddChild(CursorBox("Resize ↘↖", CursorType.ResizeNwse))
                .AddChild(CursorBox("Resize ↙↗", CursorType.ResizeNesw))),
    ];

    private static UiElement CursorBox(string label, CursorType cursor) =>
        new Button()
            .SetText(label)
            .SetDesiredSize(new Size(130, 70))
            .SetCursor(cursor);
}
