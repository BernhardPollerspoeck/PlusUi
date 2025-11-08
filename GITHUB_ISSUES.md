# GitHub Issues - Migrierte TODOs

Diese Issues wurden aus TODOs im Code migriert. Bitte in GitHub Issues übernehmen und dann TODOs aus dem Code entfernen.

---

## Issue #1: Extract IRenderService interface

**Labels:** `enhancement`, `architecture`
**Milestone:** v2.0
**Priority:** Medium

### Description
The `RenderService` class currently has no interface abstraction, making it harder to test and extend internally.

### Current Code
```csharp
// source/PlusUi.core/Services/RenderService.cs:9
//TODO: interface
public class RenderService(NavigationContainer navigationContainer, ...)
```

### Proposed Solution
Extract `IRenderService` interface to allow for better testability and potential alternative implementations in the future.

```csharp
public interface IRenderService
{
    float DisplayDensity { get; set; }
    void Render(GL? gl, SKCanvas canvas, GRContext? grContext, Vector2 canvasSize);
}
```

### Notes
- This interface should remain **internal** (not public API)
- Only needed for internal architecture improvements
- Not a breaking change as service is already internal

### File Location
- `source/PlusUi.core/Services/RenderService.cs:9`

---

## Issue #2: Improve Popup resize calculations with proper wrapper

**Labels:** `bug`, `popup`, `layout`
**Milestone:** v1.1
**Priority:** High

### Description
The current popup implementation uses a temporary Grid wrapper for centering, which doesn't properly handle resize calculations. This can lead to incorrect layout when the window is resized while a popup is open.

### Current Code
```csharp
// source/PlusUi.core/CoreElements/UiPopupElement.cs:51
internal void BuildPopup()
{
    //TODO: make propper wrapper for working resize calculations
    _tree = new Grid()
        .AddChild(Build())
        .SetHorizontalAlignment(HorizontalAlignment.Center)
        .SetVerticalAlignment(VerticalAlignment.Center);
    ...
}
```

### Steps to Reproduce
1. Open a popup
2. Resize the application window
3. Observe potential layout issues with popup positioning

### Expected Behavior
Popup should remain properly centered and sized when window is resized.

### Proposed Solution
Create a dedicated `PopupWrapper` layout element that:
- Properly handles measure/arrange cycles
- Updates layout on window resize events
- Maintains correct popup positioning

### File Location
- `source/PlusUi.core/CoreElements/UiPopupElement.cs:51`

---

## Issue #3: Hot Reload loses popup background color

**Labels:** `bug`, `hot-reload`, `popup`
**Milestone:** v1.1
**Priority:** Medium

### Description
When using .NET Hot Reload to update a popup, the background color is lost and needs to be reconstructed.

### Current Code
```csharp
// source/PlusUi.core/Services/PlusUiHotReloadManager.cs:42
if (updatedTypes?.Any(t => t.IsAssignableTo(typeof(UiPopupElement))) is true)
{
    var internalPopupService = ServiceProviderService.ServiceProvider?.GetRequiredService<PlusUiPopupService>();
    internalPopupService?.Build();//TODO: Bg color lost => reconstruct
}
```

### Steps to Reproduce
1. Open a popup with a custom background color
2. Make a code change to the popup class
3. Trigger Hot Reload
4. Observe that background color is lost

### Expected Behavior
Popup background color should be preserved during Hot Reload.

### Investigation Needed
- Why is `Background` property not persisted during rebuild?
- Should we cache the original configuration?
- Is this a limitation of Hot Reload metadata updates?

### File Location
- `source/PlusUi.core/Services/PlusUiHotReloadManager.cs:42`

---

## Issue #4: Add keyboard input control support to InputService

**Labels:** `enhancement`, `input`, `keyboard`
**Milestone:** v1.2
**Priority:** Low

### Description
The `InputService` currently only routes keyboard input to text input controls. There's no concept of a general "keyboard input control" that can receive key events.

### Current Code
```csharp
// source/PlusUi.core/Services/Input/InputService.cs:129
public void HandleKeyInput(object? sender, PlusKey key)
{
    //TODO: keyInputControl
    _textInputControl?.HandleInput(key);
}
```

### Use Cases
- Custom controls that need arrow key navigation
- Game-like controls using WASD
- Shortcut keys (Ctrl+S, etc.) handled by specific controls
- Tab navigation between controls

### Proposed Solution
Create `IKeyInputControl` interface:

```csharp
public interface IKeyInputControl
{
    bool HandleKeyInput(PlusKey key);
}
```

Then in InputService, check for this interface:
```csharp
public void HandleKeyInput(object? sender, PlusKey key)
{
    // Try focused control first
    if (_focusedControl is IKeyInputControl keyControl)
    {
        if (keyControl.HandleKeyInput(key))
            return; // Control handled the key
    }

    // Fallback to text input
    _textInputControl?.HandleInput(key);
}
```

### File Location
- `source/PlusUi.core/Services/Input/InputService.cs:129`

---

## Issue #5: Make PlusUiPopupService internal

**Labels:** `refactor`, `api-cleanup`
**Milestone:** v2.0
**Priority:** Low

### Description
`PlusUiPopupService` is currently `public` but should be `internal` as users should only interact through the `IPopupService` interface.

### Current Code
```csharp
// source/PlusUi.core/Services/Popup/PlusUiPopupService.cs:7
//TODO: internal
public class PlusUiPopupService(IServiceProvider serviceProvider, ...) : IPopupService
```

### Reasoning
- Users should only depend on `IPopupService` interface
- Implementation details should be internal
- Reduces public API surface
- Prevents users from depending on implementation specifics

### Breaking Change
This is a **breaking change** and should be done in a major version bump (v2.0).

### File Location
- `source/PlusUi.core/Services/Popup/PlusUiPopupService.cs:7`

---

## Issue #6: Support result values from popup callbacks

**Labels:** `enhancement`, `popup`, `api-design`
**Milestone:** v1.3
**Priority:** Medium

### Description
The current popup API only supports `Action` callbacks (no return value). There's no good way to return a result from a popup.

### Current API
```csharp
// source/PlusUi.core/Services/Popup/IPopupService.cs:9
void ShowPopup<TPopup, TArg>(
    TArg? arg = default,
    Action? onClosed = null, //TODO: is there a good way to have this with a result type?
    Action<IPopupConfiguration>? configure = null)
```

### Use Cases
- Confirmation dialogs returning Yes/No/Cancel
- Input dialogs returning entered text
- Selection dialogs returning selected item
- File picker dialogs returning file path

### Possible Solutions

**Option 1: Generic result callback**
```csharp
void ShowPopup<TPopup, TArg, TResult>(
    TArg? arg = default,
    Action<TResult>? onClosed = null,
    Action<IPopupConfiguration>? configure = null)
```

**Option 2: Task-based async API**
```csharp
Task<TResult?> ShowPopupAsync<TPopup, TArg, TResult>(
    TArg? arg = default,
    Action<IPopupConfiguration>? configure = null)
```

**Option 3: Separate result property**
```csharp
public abstract class UiPopupElement<TArg, TResult>
{
    public TResult? Result { get; protected set; }
}
```

### Discussion Needed
- Which approach fits PlusUi's design philosophy best?
- Should we support both sync callbacks and async patterns?
- How to handle cancellation vs. success with result?

### File Location
- `source/PlusUi.core/Services/Popup/IPopupService.cs:9`

---

## Issue #7: Map browser key codes to PlusKey enum for Web platform

**Labels:** `enhancement`, `web`, `platform-specific`
**Milestone:** Web v1.0
**Priority:** Low (Web is prototype)

### Description
The Web/Blazor platform implementation needs to map browser keyboard event codes to the `PlusKey` enumeration for consistent keyboard handling across platforms.

### Current Code
```csharp
// source/PlusUi.Web/PlusUiRootComponent.razor:141, 148
// TODO: Map browser key codes to PlusKey enum
```

### Implementation Needed
Create a mapping dictionary from JavaScript `KeyboardEvent.code` values to `PlusKey` enum:

```csharp
private static readonly Dictionary<string, PlusKey> BrowserKeyMapping = new()
{
    ["KeyA"] = PlusKey.A,
    ["KeyB"] = PlusKey.B,
    // ... all keys
    ["Enter"] = PlusKey.Enter,
    ["Space"] = PlusKey.Space,
    ["ArrowUp"] = PlusKey.Up,
    // etc.
};
```

### References
- MDN KeyboardEvent.code: https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/code
- Desktop implementation: `source/PlusUi.desktop/KeyboardMapper.cs` (if exists)

### Notes
- Web platform is currently a prototype
- Low priority until Web gets NuGet package
- Should align with desktop/mobile key mappings

### File Locations
- `source/PlusUi.Web/PlusUiRootComponent.razor:141`
- `source/PlusUi.Web/PlusUiRootComponent.razor:148`

---

## Migration Checklist

- [ ] Create all 7 issues in GitHub
- [ ] Assign appropriate labels and milestones
- [ ] Remove TODO comments from source code
- [ ] Reference issue numbers in commit message
- [ ] Close this file or archive it

## Summary

| Priority | Count | Issues |
|----------|-------|--------|
| High | 1 | #2 (Popup resize) |
| Medium | 3 | #1 (RenderService), #3 (Hot Reload), #6 (Popup results) |
| Low | 3 | #4 (KeyInput), #5 (Internal), #7 (Web keys) |

Total: **7 issues**
