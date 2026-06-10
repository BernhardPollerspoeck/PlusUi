using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;
using PlusUi.core.Services.Focus;

namespace PlusUi.core.Tests;

/// <summary>
/// Integration tests that drive the real <see cref="InputService"/> against a built page,
/// to prove/diagnose the input-pipeline bugs in uifix.md (swipe, context menu, long press).
/// </summary>
[TestClass]
public sealed class InputServiceTests
{
    private sealed class Vm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }
    }

    private sealed class FakeKeyboardHandler : IKeyboardHandler
    {
        public event EventHandler<PlusKey> KeyInput { add { } remove { } }
        public event EventHandler<char> CharInput { add { } remove { } }
        public event EventHandler<bool>? ShiftStateChanged { add { } remove { } }
        public event EventHandler<bool>? CtrlStateChanged { add { } remove { } }
        public void Show() { }
        public void Hide() { }
        public void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword) { }
    }

    private sealed class TestCommand : ICommand
    {
        public bool Executed { get; private set; }
        public object? Parameter { get; private set; }
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) { Executed = true; Parameter = parameter; }
    }

    private sealed class FakeTime : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = DateTimeOffset.UnixEpoch;
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class InputTestPage(Vm vm, UiElement content) : UiPageElement(vm)
    {
        protected override UiElement Build() => content;
    }

    private static InputService Setup(UiElement content, TimeProvider? timeProvider = null)
    {
        var sp = ServiceProviderService.ServiceProvider!;
        var config = sp.GetRequiredService<PlusUiConfiguration>();

        // Fresh navigation container per test so pushed pages don't leak between tests.
        var nav = new NavigationContainer(config);
        var page = new InputTestPage(new Vm(), content);
        nav.Push(page);
        page.BuildPage();
        page.Measure(new Size(400, 400));
        page.Arrange(new Rect(0, 0, 400, 400));

        // Fresh overlay/popup services per test so leftover overlays/popups from other tests
        // don't pollute HitTestAll (which checks overlays/popups before the page).
        return new InputService(
            nav,
            new PlusUiPopupService(sp),
            new OverlayService(),
            new FakeKeyboardHandler(),
            sp.GetRequiredService<IFocusManager>(),
            sp.GetRequiredService<ITooltipService>(),
            timeProvider);
    }

    // uifix.md > Gestures: swipe ending the drag outside the control is not recognized.
    [TestMethod]
    public void Swipe_StartedInsideEndedOutside_IsRecognized()
    {
        var command = new TestCommand();
        var detector = new SwipeGestureDetector(new Solid(100, 100))
            .SetAllowedDirections(SwipeDirection.All)
            .SetCommand(command);
        var input = Setup(detector);

        var inside = new Vector2(detector.Position.X + 10, detector.Position.Y + 10);
        var outside = new Vector2(detector.Position.X + detector.ElementSize.Width + 120, detector.Position.Y + 10);

        input.MouseDown(inside);
        input.MouseUp(outside);

        Assert.IsTrue(command.Executed,
            "A swipe that starts inside the control but ends outside should still be recognized.");
    }

    // uifix.md > ContextMenu: right-click does nothing. Diagnostic: does RightClick open the menu
    // at the InputService level? (separates a logic bug from a render-only issue)
    [TestMethod]
    public void ContextMenu_RightClickOverElement_Opens()
    {
        var box = new Solid(100, 100);
        box.SetContextMenu(new ContextMenu()
            .AddItem(new MenuItem().SetText("Cut"))
            .AddItem(new MenuItem().SetText("Copy")));
        var input = Setup(box);

        var center = new Vector2(box.Position.X + 50, box.Position.Y + 50);
        input.RightClick(center);

        Assert.IsTrue(box.ContextMenu!.IsOpen,
            "Right-clicking an element with a context menu should open it.");
    }

    // uifix.md > Gestures: long press does nothing (desktop has no press-and-hold timer).
    [TestMethod]
    public void LongPress_PressHeldPastThreshold_IsRecognized()
    {
        var command = new TestCommand();
        var detector = new LongPressGestureDetector(new Solid(100, 100)).SetCommand(command);
        var time = new FakeTime();
        var input = Setup(detector, time);

        var point = new Vector2(detector.Position.X + 50, detector.Position.Y + 50);
        input.MouseDown(point);
        time.Now = time.Now.AddMilliseconds(700); // hold past the long-press threshold
        input.MouseUp(point);

        Assert.IsTrue(command.Executed,
            "Holding the press past the long-press threshold should fire the long-press command.");
    }
}
