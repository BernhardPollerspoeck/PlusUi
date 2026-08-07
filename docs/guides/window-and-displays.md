---
title: Window & Displays
layout: default
parent: Guides
nav_order: 5
---

# Window & Displays

`IWindowService` controls the application window and reports the connected displays. It is used for borderless overlays — region selectors, presentation modes, kiosk views — and for anything that needs to know how many screens there are and where they sit.

---

## One interface, everywhere

The service is registered on **every** platform. Where there is no window to speak of — mobile, web, headless — the implementation is a deliberate no-op rather than a missing registration.

That is a design decision, not an oversight. A capability that exists only on some platforms forces `#if` blocks or partial classes into application code, and those spread. A shared surface that quietly does nothing in places is easier to write against than one that only exists in places.

```csharp
public partial class MyViewModel(IWindowService windowService) : ObservableObject
{
    [RelayCommand]
    private void GoFullscreen()
    {
        // No platform check, no null check. On Android this returns without doing anything.
        windowService.EnterBorderless(windowService.VirtualDesktopBounds, topMost: true);
    }
}
```

`IsSupported` reports whether anything actually happens. It exists so you can **hide UI** that would be inconsequential — not so you can branch your logic again:

```csharp
new Button()
    .SetText("Fullscreen")
    .SetCommand(vm.GoFullscreenCommand)
    .SetIsVisible(windowService.IsSupported);
```

---

## API

| Member | Type | Notes |
|:-------|:-----|:------|
| `IsSupported` | `bool` | `false` on no-op platforms, and on desktop before the window exists. |
| `IsBorderless` | `bool` | Whether `EnterBorderless` is currently in effect. |
| `EnterBorderless(Rect bounds, bool topMost)` | `void` | Undecorated window at `bounds`. Ignored if already borderless. |
| `RestoreNormal()` | `void` | Restores position, size, border, window state and top-most. |
| `Bounds` | `Rect` | Current position and size, in screen coordinates. |
| `MoveTo(x, y)` | `void` | Moves the window; size, border and borderless state untouched. |
| `Resize(w, h)` | `void` | Resizes it; position untouched. Sizes below 1px are ignored. |
| `GetDisplays()` | `IReadOnlyList<DisplayInfo>` | Connected displays, in system order. |
| `VirtualDesktopBounds` | `Rect` | Bounding rectangle across all displays. |

`DisplayInfo` carries `Index`, `Name`, `X`, `Y`, `Width`, `Height`, `Scale`, `IsPrimary` and a convenience `Bounds` rectangle.

---

## Borderless overlays

`EnterBorderless` takes a **rectangle**, not a fullscreen flag. That is the important part of the design:

{: .warning }
> Fullscreen occupies exactly one display on every common windowing system. An overlay meant to span several displays — a region selector, for instance — is only possible as a borderless window covering the combined area.

```csharp
// Cover every display
windowService.EnterBorderless(windowService.VirtualDesktopBounds, topMost: true);

// Cover just the primary display
var primary = windowService.GetDisplays().First(d => d.IsPrimary);
windowService.EnterBorderless(primary.Bounds, topMost: true);

// Back to where the window was before
windowService.RestoreNormal();
```

`RestoreNormal` remembers the previous state itself — position, size, border style, window state and the top-most flag are captured on the way in. The caller does not have to store anything.

Calling `EnterBorderless` while already borderless **re-targets** the window and keeps the state saved on the first call, so `RestoreNormal` still returns to where the user had it. That makes "move out of the way, do something to the screen, then cover the desktop" one continuous operation instead of a round trip through `RestoreNormal` that flashes the window back at its old position.

{: .tip }
> Always give the user a keyboard way out. A borderless, top-most window covering every display is exactly the situation where a mis-placed button leaves no route back. Subscribe to `IGlobalInputService.KeyDown` and restore on `Escape`.

{: .warning }
> **Transparency and borderless do not automatically combine.** With `IsWindowTransparent` enabled, a borderless window that covers a whole display can lose its transparency and render on black instead. Desktop compositors commonly stop compositing — "unredirect" — a window that exactly matches a screen, and per-pixel alpha goes away with the compositing. Observed on Windows.
>
> If you need a see-through overlay rather than an opaque one, inset the bounds slightly so the window no longer exactly matches the display, and check whether the compositor keeps composing it.

---

{: .tip }
> For a canvas that pans, the middle mouse button arrives on `IGlobalInputService` as a
> `PointerDown`/`PointerUp` pair with `PointerButton.Middle`. It is raised on the global bus
> only — it does no hit testing, changes no focus and fires no click, because PlusUi controls
> have no middle-button behaviour to inherit.

## Windows with their own chrome

Setting `WindowBorder.Hidden` removes the system title bar, and with it the two things that bar provided: a close button and a way to move and resize the window. Rebuild both, or the user ends up with a window nailed to the screen at a fixed size.

```csharp
// Drag: accumulate the pointer delta onto the current position.
windowService.MoveTo(
    windowService.Bounds.X + deltaX,
    windowService.Bounds.Y + deltaY);

// Resize grip in the bottom-right corner.
windowService.Resize(
    pointer.X - windowService.Bounds.X,
    pointer.Y - windowService.Bounds.Y);
```

{: .warning }
> Pointer positions arrive in layout units, window geometry is in screen coordinates. On a display scaled above 100% the two differ, and using the raw delta makes the window lag behind or outrun the cursor. Multiply by `IPlatformService.DisplayDensity`.

## Displays

```csharp
foreach (var display in windowService.GetDisplays())
{
    Console.WriteLine(
        $"[{display.Index}] {display.Name} " +
        $"{display.Width}x{display.Height} at ({display.X},{display.Y}) " +
        $"scale {display.Scale}x{(display.IsPrimary ? " (primary)" : "")}");
}
```

`X` and `Y` can be **negative** — a display placed to the left of the primary one starts at a negative X within the virtual desktop.

`Scale` is read **per display**, so it is correct on mixed-DPI setups where a single global factor would be wrong for every display but one.

On no-op platforms `GetDisplays()` returns exactly **one** entry describing the drawing surface. It is never empty there, so code that just wants "the screen" finds it without a special case. On desktop an empty list means something different: GLFW is not up yet because the window has not been created — a temporary state, not a platform property.

---

## Screen coordinates vs. layout units

{: .warning }
> `Rect` values in this API are in **screen coordinates** — the same unit as window positions. They are *not* layout units. PlusUi divides pointer positions by the display density before they reach layout, so on a display scaled to 150% the two differ by a factor of 1.5.

This matters as soon as you map between the two — mapping a selection rectangle drawn in the UI onto the pixels of a screen capture, for example. Use `DisplayInfo.Scale` for the conversion.

Mixed-DPI multi-monitor setups are a known rough edge: a single window spanning displays with different scale factors has one framebuffer scale, so one of the two displays will be off.

---

## Platform support

| Platform | Behaviour |
|:---------|:----------|
| Windows | Full support |
| Linux | Supported; see the note below |
| macOS | Untested, same code path as Linux/Windows |
| Android, iOS, Web, Headless, h264 | No-op; `GetDisplays()` reports the drawing surface |

{: .warning }
> **Linux:** some window managers ignore a decoration change made *after* the window was created. Position and size are unaffected. If borderless mode does not drop the title bar on a given desktop environment, that is the cause.

---

## Trying it out

The demo application has a **Window & Displays** page under *Advanced* that toggles borderless mode across all displays or a single one, and lists what `GetDisplays()` reports.
