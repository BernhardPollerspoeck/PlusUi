# Memory Analysis Report - PlusUi Sandbox (TODO 999.5)

**Issue:** Sandbox requires approximately 350MB on startup
**Date:** 2026-01-25
**Status:** Analysis complete

---

## Executive Summary

The high memory usage (~350MB) at startup is caused by a combination of factors:

| Category | Estimated Memory | Severity |
|----------|-----------------|----------|
| **GPU Context (GRContext)** | 1-5 MB | Low |
| **SKSurface (1200x800)** | ~4 MB | Medium |
| **Inter Variable Font** | ~1 MB (loaded) | Low |
| **SkiaSharp Native Libraries** | ~50-100 MB | High (unavoidable) |
| **.NET Runtime + JIT** | ~80-150 MB | High (unavoidable) |
| **Debug Features (if enabled)** | Variable | High |
| **Memory Leaks (identified)** | Cumulative | **Critical** |

---

## 1. Identified Memory Leaks

### 1.1 RawUserControl.cs - SKBitmap Memory Leak 🔴 CRITICAL

**Location:** `source/PlusUi.core/Controls/UserControl/RawUserControl.cs:57-59`

```csharp
public override void Render(SKCanvas canvas)
{
    // ...
    var bitmap = new SKBitmap((int)Size.Width, (int)Size.Height);  // ALLOCATED
    RenderControl(bitmap);
    canvas.DrawBitmap(bitmap, ...);
    // bitmap is NEVER disposed!
}
```

**Impact:** Every frame creates a new SKBitmap that is never released. Memory grows continuously.

**Recommendation:** Add `using` statement or call `bitmap.Dispose()` after drawing.

---

### 1.2 PlusUiHeadlessService.cs - SKCanvas Not Disposed 🔴 CRITICAL

**Location:** `source/PlusUi.Headless/Services/PlusUiHeadlessService.cs:45`

```csharp
public Task<byte[]> GetCurrentFrameAsync()
{
    var bitmap = new SKBitmap(...);
    var canvas = new SKCanvas(bitmap);  // NEVER disposed!
    // ...
    return Task.FromResult(EncodeToFormat(bitmap, _format));
    // bitmap also not disposed!
}
```

**Impact:** Both SKCanvas and SKBitmap leak native resources on every frame capture.

**Recommendation:** Add `using` statements for both canvas and bitmap.

---

## 2. Framework Baseline Memory

### 2.1 .NET Runtime Overhead

| Component | Memory Range | Notes |
|-----------|-------------|-------|
| .NET 10 Runtime | 40-80 MB | Includes GC heap, JIT compiler, type metadata |
| Initial JIT compilation | 20-50 MB | First-time method compilation |
| Assembly loading | 10-30 MB | All referenced assemblies |

### 2.2 SkiaSharp Native Libraries

| Component | Memory Range | Notes |
|-----------|-------------|-------|
| libSkiaSharp.so/dll | 30-50 MB | Native rendering library |
| OpenGL driver | 20-50 MB | GPU driver memory |
| Font subsystem | 5-10 MB | FreeType/HarfBuzz |

**Unavoidable baseline: ~100-200 MB** before any application code runs.

---

## 3. GPU and Rendering Resources

### 3.1 GRContext (OpenGL)

**Location:** `source/PlusUi.desktop/WindowManager.cs:129-132`

```csharp
_glContext = _window.CreateOpenGL();
var glInterface = GRGlInterface.Create();
_grContext = GRContext.CreateGl(glInterface);
```

**Memory:** 1-5 MB (persistent, one-time allocation)

- Manages GPU resources and shader programs
- Cached for application lifetime
- Properly disposed on window close

### 3.2 SKSurface (Main Render Target)

**Location:** `source/PlusUi.desktop/WindowManager.cs:263-280`

```csharp
private void CreateSurface(Vector2D<int> size)
{
    var frameBufferInfo = new GRGlFramebufferInfo(0, 0x8058);  // GL_RGBA8
    var backendRenderTarget = new GRBackendRenderTarget(
        size.X, size.Y, 0, 0, frameBufferInfo);
    _surface = SKSurface.Create(_grContext, backendRenderTarget, ...);
}
```

**Memory Formula:** `width × height × 4 bytes`

| Resolution | Memory |
|------------|--------|
| 1200×800 (Demo default) | ~3.8 MB |
| 1920×1080 | ~8.3 MB |
| 2560×1440 | ~14.7 MB |
| 3840×2160 (4K) | ~33.2 MB |

**Status:** Properly managed - disposed on resize and close.

---

## 4. Font System

### 4.1 Embedded Font

**Location:** `source/PlusUi.core/Fonts/InterVariable.ttf`
**Size:** 860 KB (on disk), ~1 MB loaded

- Loaded once at first use (lazy initialization)
- Cached in `FontRegistryService._fontCache`
- Never released during runtime (by design)

### 4.2 Paint/Font Registry

**Location:** `source/PlusUi.core/Services/PaintRegistryService.cs`

```csharp
private readonly ConcurrentDictionary<PaintCacheKey, PaintEntry> _cache = new();
```

- Uses reference counting for SKPaint/SKFont objects
- Entries disposed when RefCount reaches 0
- **Well-implemented** - no leaks detected

---

## 5. Image Caching System

### 5.1 Cache Structure

**Location:** `source/PlusUi.core/Services/ImageLoaderService.cs:17-19`

```csharp
private static readonly ConcurrentDictionary<string, WeakReference<SKImage>> _imageCache = new();
private static readonly ConcurrentDictionary<string, WeakReference<AnimatedImageInfo>> _animatedImageCache = new();
private static readonly ConcurrentDictionary<string, WeakReference<SvgImageInfo>> _svgImageCache = new();
```

**Status:** Uses `WeakReference<T>` - allows GC to reclaim unused images.

### 5.2 Animated GIF Concern 🟡 MEDIUM

**Location:** `source/PlusUi.core/Services/ImageLoaderService.cs:345-432`

```csharp
var frames = new SKImage[frameCount];  // All frames in memory
```

- Large animated GIFs load ALL frames into memory
- No streaming or on-demand frame loading
- Could cause spikes with large animations

---

## 6. Debug Features (High Impact When Enabled)

### 6.1 DebugBridgeClient Screenshots

**Location:** `source/PlusUi.core/Services/DebugBridge/DebugBridgeClient.cs:431-491`

When capturing a screenshot:

| Step | Memory |
|------|--------|
| SKBitmap creation | width × height × 4 bytes |
| Base64 encoding | +33% overhead |
| Message serialization | Another copy |

**Example (1920×1080):** ~8.3 MB + ~11 MB = **~19.3 MB per screenshot**

### 6.2 Log Queue

**Location:** `source/PlusUi.core/Services/DebugBridge/DebugBridgeLoggerProvider.cs:15`

```csharp
private static readonly ConcurrentQueue<LogMessageDto> _logQueue = new();
```

- Batches logs every 500ms (max 50 per batch)
- Minimal impact under normal use
- Could accumulate if WebSocket disconnected

---

## 7. Static Memory Holders

### 7.1 PlusUiDefaults

**Location:** `source/PlusUi.core/Styling/PlusUiDefaults.cs`

Contains ~40 static readonly Color values - negligible impact (~200 bytes total).

### 7.2 HttpClient (Singleton)

**Location:** `source/PlusUi.core/Services/ImageLoaderService.cs:20-27`

```csharp
private static readonly System.Net.Http.HttpClient _httpClient = new()
{
    Timeout = TimeSpan.FromSeconds(30),
};
```

- Single instance, properly configured
- Minimal memory (~10 KB)

---

## 8. Memory Breakdown Estimate

### Startup Memory (Clean Launch)

| Component | Memory (MB) | % of Total |
|-----------|-------------|------------|
| .NET 10 Runtime | 80-100 | 23-29% |
| SkiaSharp Native | 50-80 | 14-23% |
| OpenGL Context | 30-50 | 9-14% |
| GRContext + Surface | 5-10 | 1-3% |
| Font (Inter) | ~1 | <1% |
| Application Code | 20-40 | 6-11% |
| Demo Pages (UI Tree) | 10-30 | 3-9% |
| **Baseline Total** | **~200-300 MB** | **60-85%** |
| *Unaccounted / Fragmentation* | *~50-100 MB* | *15-30%* |
| **Observed Total** | **~350 MB** | **100%** |

---

## 9. Recommendations

### 9.1 Critical Fixes (Memory Leaks)

1. **RawUserControl.cs** - Add bitmap disposal:
   ```csharp
   using var bitmap = new SKBitmap((int)Size.Width, (int)Size.Height);
   RenderControl(bitmap);
   canvas.DrawBitmap(bitmap, ...);
   ```

2. **PlusUiHeadlessService.cs** - Add proper disposal:
   ```csharp
   using var bitmap = new SKBitmap(...);
   using var canvas = new SKCanvas(bitmap);
   ```

### 9.2 Optimization Opportunities

| Priority | Action | Expected Savings |
|----------|--------|------------------|
| High | Fix memory leaks | Prevents continuous growth |
| Medium | Lazy-load fonts on first use | Already implemented |
| Medium | Consider smaller default font | 500-800 KB |
| Low | Implement animated GIF streaming | Variable |
| Low | Compress debug screenshots | ~50% reduction |

### 9.3 Measurement Tools

For accurate memory profiling, use:
- **dotMemory** (JetBrains) - Managed heap analysis
- **VTune** - Native memory analysis
- **GPU-Z** - GPU memory monitoring
- Built-in: `GC.GetTotalMemory(false)` - Only managed heap!

---

## 10. Files Analyzed

| Path | Purpose | Memory Impact |
|------|---------|---------------|
| `PlusUi.desktop/WindowManager.cs` | Window, GL context, surface | High |
| `PlusUi.core/Services/PaintRegistryService.cs` | Paint/Font caching | Low (well-managed) |
| `PlusUi.core/Services/FontRegistryService.cs` | Font loading | Low |
| `PlusUi.core/Services/ImageLoaderService.cs` | Image caching | Medium (WeakRef) |
| `PlusUi.core/Controls/UserControl/RawUserControl.cs` | Custom rendering | **MEMORY LEAK** |
| `PlusUi.Headless/Services/PlusUiHeadlessService.cs` | Headless rendering | **MEMORY LEAK** |
| `PlusUi.core/Services/DebugBridge/*` | Debug features | High when active |
| `PlusUi.core/Fonts/InterVariable.ttf` | Default font | 860 KB |

---

## Conclusion

The ~350MB startup memory is primarily due to:

1. **~200-250 MB:** Framework baseline (.NET runtime + SkiaSharp + OpenGL)
2. **~50-100 MB:** Application-specific allocations (UI tree, caches, surfaces)
3. **~50 MB:** Memory fragmentation and alignment

The identified **memory leaks in RawUserControl and PlusUiHeadlessService** will cause memory to grow over time but are not responsible for the initial 350MB.

**The baseline memory cannot be significantly reduced** without switching frameworks. The identified leaks should still be fixed to prevent long-running memory growth.
