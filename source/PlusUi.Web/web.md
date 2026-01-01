# PlusUi.Web - TODO & Research Notes

## Open Issues

### Render Loop (LOW PRIORITY)
**Status:** TODO
**Symptom:** Rendering nur bei User-Input, nicht flüssig (GIFs, Animationen)
**Cause:** Kein kontinuierlicher Render-Loop, nur on-demand bei Events
**Solution:** `requestAnimationFrame` Loop oder `EnableRenderLoop="true"` auf SKCanvasView

---

## Fixed Issues

### 1. Resize
**Fix:** `_canvasView?.Invalidate()` statt `StateHasChanged()`

### 2. Input/Mouse Click
**Fix:** `if (!_isInitialized) return;` Guards in allen Event-Handlers

### 3. Navigation stack empty
**Fix:** Guards verhindern Events vor Initialisierung

---

## Research Notes

### SKCanvasView in Blazor
- `SKCanvasView.Invalidate()` triggers repaint via `requestAnimationFrame`
- `StateHasChanged()` alone does NOT repaint the canvas
- `EnableRenderLoop="true"` für kontinuierliches Rendering (CPU-intensiv)
- Source: https://github.com/mono/SkiaSharp/blob/main/source/SkiaSharp.Views/SkiaSharp.Views.Blazor/SKCanvasView.razor.cs

---

## Completed
- [x] InitializePlusUi - Styles work (no HostedService for WASM)
- [x] RequestRender fix - Invalidate() statt StateHasChanged()
- [x] Event Guards - Keine Events vor Initialisierung
