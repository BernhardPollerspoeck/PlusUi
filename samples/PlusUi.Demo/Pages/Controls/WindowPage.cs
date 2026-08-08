using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;
using PlusUi.core.Services;
using PlusUi.Demo.Pages.Shared;

namespace PlusUi.Demo.Pages.Controls;

public partial class WindowPageViewModel : DemoPageViewModel, IDisposable
{
    private readonly IWindowService _windowService;
    private readonly IGlobalInputService _input;

    [ObservableProperty]
    private string _summary = string.Empty;

    [ObservableProperty]
    private string _status = "Nothing done yet.";

    [ObservableProperty]
    private List<DisplayInfo> _displays = [];

    public WindowPageViewModel(
        INavigationService navigation,
        IWindowService windowService,
        IGlobalInputService input)
        : base(navigation)
    {
        _windowService = windowService;
        _input = input;

        // Escape is the safety net, and it is not decoration: a borderless, top-most window
        // spanning every display is exactly the situation where a bug in the restore button
        // would leave no way back to it. The keyboard path does not depend on hit-testing.
        _input.KeyDown += OnKeyDown;

        Refresh();
    }

    private void OnKeyDown(KeyInputEvent e)
    {
        if (e.Key == PlusKey.Escape && _windowService.IsBorderless)
            Restore();
    }

    [RelayCommand]
    private void Refresh()
    {
        Displays = [.. _windowService.GetDisplays()];

        var bounds = _windowService.VirtualDesktopBounds;
        Summary =
            $"IsSupported: {_windowService.IsSupported}   ·   " +
            $"IsBorderless: {_windowService.IsBorderless}   ·   " +
            $"Virtual desktop: {bounds.Width} × {bounds.Height} at ({bounds.X}, {bounds.Y})";
    }

    [RelayCommand]
    private void CoverAllDisplays()
    {
        _windowService.EnterBorderless(_windowService.VirtualDesktopBounds, topMost: true);
        ReportOutcome("all displays");
    }

    [RelayCommand]
    private void CoverDisplay(DisplayInfo? display)
    {
        if (display is null)
            return;

        _windowService.EnterBorderless(display.Bounds, topMost: true);
        ReportOutcome($"display {display.Index} ({display.Name})");
    }

    /// <summary>
    /// Asks for a minimum size well above the current one, then reports the size that came
    /// back. Worth its own button because a size limit is the kind of call that returns
    /// quietly whether or not it did anything — reading the window afterwards is the only way
    /// to tell, and GLFW resizes a window immediately when the new minimum exceeds it.
    /// </summary>
    [RelayCommand]
    private void ApplySizeLimits()
    {
        var before = _windowService.Bounds;
        _windowService.SetSizeLimits(900, 700, maxWidth: null, maxHeight: null);
        var afterLimits = _windowService.Bounds;

        _windowService.Resize(300, 200);
        var afterShrink = _windowService.Bounds;

        Status =
            $"Asked for min 900 × 700.   " +
            $"Before: {before.Width:0} × {before.Height:0}   ·   " +
            $"After the limit: {afterLimits.Width:0} × {afterLimits.Height:0}   ·   " +
            $"After Resize(300, 200): {afterShrink.Width:0} × {afterShrink.Height:0}";

        Refresh();
    }

    [RelayCommand]
    private void Restore()
    {
        _windowService.RestoreNormal();

        Status = _windowService.IsSupported
            ? "Restored to the previous position, size, border and window state."
            : "No-op platform - nothing to restore.";

        Refresh();
    }

    /// <summary>
    /// Reports what actually happened rather than what was requested. On a no-op platform
    /// the call returns just as quietly as on a successful one, so asking the service
    /// afterwards is the only honest way to word the message.
    /// </summary>
    private void ReportOutcome(string target)
    {
        Status = _windowService.IsBorderless
            ? $"Borderless over {target}. Press Escape or use Restore to come back."
            : "Nothing happened - this platform has no window control.";

        Refresh();
    }

    public void Dispose() => _input.KeyDown -= OnKeyDown;
}

public class WindowPage(WindowPageViewModel vm) : DemoPage(vm)
{
    protected override string ControlName => "Window & Displays";

    protected override string Description =>
        "IWindowService switches the window to borderless mode over an arbitrary rectangle and " +
        "reports the connected displays. Registered on every platform - on mobile, web and " +
        "headless it is a deliberate no-op, so calling code needs no platform switch. " +
        "This page also has a transparent background, so the desktop shows through between the cards.";

    /// <summary>
    /// Makes this one page see-through. Page-scoped on purpose: the transparency belongs to
    /// this demo, not to the app, so every other page keeps its opaque default background.
    /// <para>
    /// This only becomes visible because the app sets <c>IsWindowTransparent</c>. That flag is
    /// a window CREATION hint - the framebuffer either has an alpha channel from the start or
    /// it does not - so a transparent page in an opaque window would just render black here.
    /// </para>
    /// </summary>
    protected override void ConfigurePageStyles(Style pageStyle)
    {
        pageStyle.AddStyle<UiPageElement>(page => page.SetBackground(Colors.Transparent));
    }

    protected override IEnumerable<UiElement> BuildSections() =>
    [
        Section("Borderless overlay",
            Note("Covering all displays uses VirtualDesktopBounds - fullscreen would only ever " +
                 "cover one display, which is why the API takes a rectangle instead of a flag."),
            new HStack()
                .SetSpacing(12)
                .AddChild(new Button()
                    .SetText("Cover all displays")
                    .SetCommand(vm.CoverAllDisplaysCommand))
                .AddChild(new Button()
                    .SetText("Restore")
                    .SetCommand(vm.RestoreCommand)),
            Note("Escape also restores, in case the window ends up somewhere the buttons are not.")),

        Section("Size limits",
            Note("Sets a minimum of 900 × 700 and then asks the window to become 300 × 200. " +
                 "The status line reports the size after each step, because a limit that was " +
                 "not applied looks exactly like one that was until you read the window back."),
            new Button()
                .SetText("Apply min 900 × 700, then shrink")
                .SetHorizontalAlignment(HorizontalAlignment.Left)
                .SetCommand(vm.ApplySizeLimitsCommand)),

        Section("Status",
            new Label()
                .BindText(() => vm.Status)
                .SetTextWrapping(TextWrapping.WordWrap),
            new Label()
                .BindText(() => vm.Summary)
                .SetTextSize(12)
                .SetTextColor(PlusUiDefaults.TextSecondary)
                .SetTextWrapping(TextWrapping.WordWrap)),

        Section("Connected displays",
            Note("Bounds are in screen coordinates. Scale is read per display, so it is correct " +
                 "on mixed-DPI setups where a single global factor would be wrong."),
            new Button()
                .SetText("Refresh")
                .SetHorizontalAlignment(HorizontalAlignment.Left)
                .SetCommand(vm.RefreshCommand),
            new ItemsList<DisplayInfo>()
                .BindItemsSource(() => vm.Displays)
                .SetItemTemplate((display, _) => new HStack()
                    .SetSpacing(12)
                    .SetMargin(new Margin(8, 6))
                    .AddChild(new Label()
                        .SetText($"[{display.Index}] {display.Name}")
                        .SetFontWeight(FontWeight.SemiBold))
                    .AddChild(new Label()
                        .SetText($"{display.Width} × {display.Height} at ({display.X}, {display.Y})")
                        .SetTextColor(PlusUiDefaults.TextSecondary))
                    .AddChild(new Label()
                        .SetText($"scale {display.Scale:0.##}×")
                        .SetTextColor(PlusUiDefaults.TextSecondary))
                    .AddChild(new Label()
                        .SetText(display.IsPrimary ? "primary" : string.Empty)
                        .SetTextColor(PlusUiDefaults.AccentPrimary))
                    .AddChild(new Button()
                        .SetText("Cover this one")
                        .SetCommand(vm.CoverDisplayCommand)
                        .SetCommandParameter(display)))
                .SetDesiredHeight(180)),
    ];
}
