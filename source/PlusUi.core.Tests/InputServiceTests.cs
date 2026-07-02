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
        public event EventHandler<PlusKey>? RawKeyDown { add { } remove { } }
        public event EventHandler<PlusKey>? RawKeyUp { add { } remove { } }
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

    private sealed class TrackingKeyboardHandler : IKeyboardHandler
    {
        public int ShowCount { get; private set; }
        public int HideCount { get; private set; }
        public event EventHandler<PlusKey> KeyInput { add { } remove { } }
        public event EventHandler<char> CharInput { add { } remove { } }
        public event EventHandler<bool>? ShiftStateChanged;
        public event EventHandler<bool>? CtrlStateChanged;
        public event EventHandler<PlusKey>? RawKeyDown;
        public event EventHandler<PlusKey>? RawKeyUp;
        public void Show() => ShowCount++;
        public void Hide() => HideCount++;
        public void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword) => ShowCount++;

        public void RaiseRawKeyDown(PlusKey key) => RawKeyDown?.Invoke(this, key);
        public void RaiseRawKeyUp(PlusKey key) => RawKeyUp?.Invoke(this, key);
        public void RaiseShift(bool pressed) => ShiftStateChanged?.Invoke(this, pressed);
        public void RaiseCtrl(bool pressed) => CtrlStateChanged?.Invoke(this, pressed);
    }

    /// <summary>Minimal hoverable element with no font/render dependencies.</summary>
    private sealed class FakeHoverable : UiElement, IHoverableControl
    {
        public bool IsHovered { get; set; }
        protected internal override bool IsFocusable => false;
        public override AccessibilityRole AccessibilityRole => AccessibilityRole.None;

        public FakeHoverable()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            SetDesiredSize(new Size(100, 100));
        }
    }

    /// <summary>Minimal text-input element that records its selection state.</summary>
    private sealed class FakeTextInput : UiElement, ITextInputControl
    {
        public bool Selected { get; private set; }
        protected internal override bool IsFocusable => false;
        public override AccessibilityRole AccessibilityRole => AccessibilityRole.None;
        public void SetSelectionStatus(bool isSelected) => Selected = isSelected;
        public void HandleInput(PlusKey key, bool shift, bool ctrl) { }
        public void HandleInput(char chr) { }
        public void HandleClick(float localX, float localY) { }

        public FakeTextInput()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            SetDesiredSize(new Size(100, 100));
        }
    }

    private sealed class FakeCursorService : IPlatformCursorService
    {
        public CursorType Current { get; private set; } = CursorType.Default;
        public void SetCursor(CursorType cursor) => Current = cursor;
    }

    private sealed record InputContext(InputService Input, NavigationContainer Nav, TrackingKeyboardHandler Keyboard);

    /// <summary>Like <see cref="Setup"/> but exposes the nav container and keyboard so tests
    /// can drive page changes and assert keyboard side-effects.</summary>
    private static InputContext SetupContext(UiElement content, IPlatformCursorService? cursor = null)
    {
        var sp = ServiceProviderService.ServiceProvider!;
        var config = sp.GetRequiredService<PlusUiConfiguration>();

        var nav = new NavigationContainer(config);
        var page = new InputTestPage(new Vm(), content);
        nav.Push(page);
        page.BuildPage();
        page.Measure(new Size(400, 400));
        page.Arrange(new Rect(0, 0, 400, 400));

        var keyboard = new TrackingKeyboardHandler();
        var input = new InputService(
            nav,
            new PlusUiPopupService(sp),
            new OverlayService(),
            keyboard,
            sp.GetRequiredService<IFocusManager>(),
            sp.GetRequiredService<ITooltipService>(),
            cursorService: cursor);

        return new InputContext(input, nav, keyboard);
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

    // Leak fix: the InputService must not keep references to elements of a page that has been
    // navigated away from. ClearTransientState() releases the hovered element and resets its flag.
    [TestMethod]
    public void ClearTransientState_AfterHover_ReleasesHoveredElement()
    {
        var hoverable = new FakeHoverable();
        var ctx = SetupContext(hoverable);
        var center = new Vector2(hoverable.Position.X + 50, hoverable.Position.Y + 50);

        ctx.Input.MouseMove(center);
        Assert.IsTrue(hoverable.IsHovered, "Sanity: moving over the element should set hover.");

        ctx.Input.ClearTransientState();

        Assert.IsFalse(hoverable.IsHovered,
            "Clearing transient state must reset the hovered element's flag so it isn't kept alive.");
    }

    // Leak fix wiring: a page change (navigation) must trigger ClearTransientState automatically.
    [TestMethod]
    public void PageChanged_ReleasesHoveredElement()
    {
        var hoverable = new FakeHoverable();
        var ctx = SetupContext(hoverable);
        var center = new Vector2(hoverable.Position.X + 50, hoverable.Position.Y + 50);

        ctx.Input.MouseMove(center);
        Assert.IsTrue(hoverable.IsHovered);

        var next = new InputTestPage(new Vm(), new Solid(10, 10));
        ctx.Nav.RaisePageChangedForPush(next, ctx.Nav.CurrentPage);

        Assert.IsFalse(hoverable.IsHovered,
            "Navigating to a new page must clear hover so the old page isn't retained by the InputService.");
    }

    // Leak fix: an active text input must be released (deselected + keyboard hidden) on clear.
    [TestMethod]
    public void ClearTransientState_AfterTextInputFocus_HidesKeyboardAndDeselects()
    {
        var textInput = new FakeTextInput();
        var ctx = SetupContext(textInput);
        var center = new Vector2(textInput.Position.X + 50, textInput.Position.Y + 50);

        ctx.Input.MouseDown(center);
        ctx.Input.MouseUp(center);
        Assert.IsTrue(textInput.Selected, "Sanity: clicking the text input should select it.");

        ctx.Input.ClearTransientState();

        Assert.IsFalse(textInput.Selected,
            "Clearing transient state must deselect the active text input.");
        Assert.AreEqual(1, ctx.Keyboard.HideCount,
            "Clearing transient state must hide the keyboard for the released text input.");
    }

    // Hover commands: moving onto an element fires its hover-enter command.
    [TestMethod]
    public void Hover_MovingOntoElement_FiresEnterCommand()
    {
        var enter = new TestCommand();
        var exit = new TestCommand();
        var el = new FakeHoverable();
        el.SetOnHoverEnterCommand(enter);
        el.SetOnHoverExitCommand(exit);
        var ctx = SetupContext(el);

        ctx.Input.MouseMove(new Vector2(el.Position.X + 50, el.Position.Y + 50));

        Assert.IsTrue(enter.Executed, "Moving onto the element should fire the hover-enter command.");
        Assert.IsFalse(exit.Executed, "The hover-exit command must not fire while still hovering.");
    }

    // Hover commands: moving off an element fires its hover-exit command.
    [TestMethod]
    public void Hover_MovingOffElement_FiresExitCommand()
    {
        var enter = new TestCommand();
        var exit = new TestCommand();
        var el = new FakeHoverable();
        el.SetOnHoverEnterCommand(enter);
        el.SetOnHoverExitCommand(exit);
        var ctx = SetupContext(el);

        ctx.Input.MouseMove(new Vector2(el.Position.X + 50, el.Position.Y + 50));
        // Move off the element (and off the page) so nothing is hovered.
        ctx.Input.MouseMove(new Vector2(el.Position.X + 500, el.Position.Y + 500));

        Assert.IsTrue(exit.Executed, "Moving off the element should fire the hover-exit command.");
    }

    // Hover commands: page navigation while hovering does NOT fire the exit command
    // (the VM may be tearing down); it only releases state.
    [TestMethod]
    public void ClearTransientState_WhileHovering_DoesNotFireExitCommand()
    {
        var exit = new TestCommand();
        var el = new FakeHoverable();
        el.SetOnHoverExitCommand(exit);
        var ctx = SetupContext(el);

        ctx.Input.MouseMove(new Vector2(el.Position.X + 50, el.Position.Y + 50));
        ctx.Input.ClearTransientState();

        Assert.IsFalse(exit.Executed,
            "ClearTransientState releases hover state but must not invoke the hover-exit command.");
    }

    // Cursor control: hovering an element applies its configured cursor.
    [TestMethod]
    public void Hover_MovingOntoElement_AppliesItsCursor()
    {
        var cursor = new FakeCursorService();
        var el = new FakeHoverable();
        el.SetCursor(CursorType.Hand);
        var ctx = SetupContext(el, cursor);

        ctx.Input.MouseMove(new Vector2(el.Position.X + 50, el.Position.Y + 50));

        Assert.AreEqual(CursorType.Hand, cursor.Current, "Hovering an element should apply its cursor.");
    }

    // Cursor control: moving off the element resets the cursor to default.
    [TestMethod]
    public void Hover_MovingOffElement_ResetsCursorToDefault()
    {
        var cursor = new FakeCursorService();
        var el = new FakeHoverable();
        el.SetCursor(CursorType.Hand);
        var ctx = SetupContext(el, cursor);

        ctx.Input.MouseMove(new Vector2(el.Position.X + 50, el.Position.Y + 50));
        Assert.AreEqual(CursorType.Hand, cursor.Current, "Sanity: hovering applies the cursor.");

        ctx.Input.MouseMove(new Vector2(el.Position.X + 500, el.Position.Y + 500));

        Assert.AreEqual(CursorType.Default, cursor.Current,
            "Moving off the element should reset the cursor to default.");
    }

    // Cursor control: clearing transient state (e.g. on navigation) resets the cursor.
    [TestMethod]
    public void ClearTransientState_ResetsCursorToDefault()
    {
        var cursor = new FakeCursorService();
        var el = new FakeHoverable();
        el.SetCursor(CursorType.Hand);
        var ctx = SetupContext(el, cursor);

        ctx.Input.MouseMove(new Vector2(el.Position.X + 50, el.Position.Y + 50));
        Assert.AreEqual(CursorType.Hand, cursor.Current, "Sanity: hovering applies the cursor.");

        ctx.Input.ClearTransientState();

        Assert.AreEqual(CursorType.Default, cursor.Current,
            "Clearing transient state should reset the cursor to default.");
    }

    // Global input bus: pointer, scroll and raw keyboard are broadcast (with modifier state),
    // independently of hit-testing/focus.
    [TestMethod]
    public void GlobalInputService_BroadcastsPointerScrollAndRawKey()
    {
        var sp = ServiceProviderService.ServiceProvider!;
        var config = sp.GetRequiredService<PlusUiConfiguration>();
        var nav = new NavigationContainer(config);
        var page = new InputTestPage(new Vm(), new Solid(100, 100));
        nav.Push(page);
        page.BuildPage();
        page.Measure(new Size(400, 400));
        page.Arrange(new Rect(0, 0, 400, 400));

        var keyboard = new TrackingKeyboardHandler();
        var bus = new GlobalInputService();
        var input = new InputService(
            nav,
            new PlusUiPopupService(sp),
            new OverlayService(),
            keyboard,
            sp.GetRequiredService<IFocusManager>(),
            sp.GetRequiredService<ITooltipService>(),
            globalInput: bus);

        PointerInputEvent? down = null;
        ScrollInputEvent? scroll = null;
        KeyInputEvent? keyDown = null;
        bus.PointerDown += e => down = e;
        bus.Scrolled += e => scroll = e;
        bus.KeyDown += e => keyDown = e;

        input.MouseDown(new Vector2(10, 20));
        input.MouseWheel(new Vector2(30, 40), 1f, -2f);
        keyboard.RaiseShift(true);
        keyboard.RaiseRawKeyDown(PlusKey.W);

        Assert.IsNotNull(down, "PointerDown should be broadcast.");
        Assert.AreEqual(PointerButton.Left, down.Value.Button);
        Assert.AreEqual(10f, down.Value.Position.X);

        Assert.IsNotNull(scroll, "Scrolled should be broadcast.");
        Assert.AreEqual(-2f, scroll.Value.DeltaY);

        Assert.IsNotNull(keyDown, "Raw KeyDown should be broadcast with the full key set.");
        Assert.AreEqual(PlusKey.W, keyDown.Value.Key);
        Assert.IsTrue(keyDown.Value.Modifiers.HasFlag(KeyModifiers.Shift),
            "Modifier state should be stamped onto raw key events.");
    }
}
