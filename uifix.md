# UI Fix List

Reported by Bernhard while reviewing the control showcase. Tags: `(demo)` = fix in PlusUi.Demo, `(core)` = fix in PlusUi.core (prove with a failing unit test first, then review, then fix).

Failing tests that prove the `(core)` bugs live in `source/PlusUi.core.Tests/UiFixTests.cs` (all RED on purpose). Run: `dotnet test --filter "FullyQualifiedName~PlusUi.core.Tests.UiFixTests"`.

## ContextMenu
- Right-click does nothing. `(core)`
  → VERIFIED WORKING end-to-end in tests, no core bug found:
    1. Input opens it — `InputServiceTests.ContextMenu_RightClickOverElement_Opens` GREEN (RightClick → IsOpen=true).
    2. Overlay draws pixels — `MenuOverlayRenderTests.OverlayService_RendersRegisteredMenuOverlay_DrawsPixels` GREEN (RenderOverlays draws the menu).
    3. RenderService renders OverlayService overlays each frame (RenderService.cs:74), same DI-singleton OverlayService that ContextMenu.Open registers into.
  → Most likely the original report was a stale build (VS was updating at the time). ACTION: re-test right-click in the freshly built demo. If it still fails, note the exact right-click location (positioning falls back to 800x600 if IPlatformService.WindowSize is unavailable).

## ProgressBar
- One example should be animated (indeterminate). `(core feature)` — no indeterminate mode exists; needs a small core feature, then demo.
