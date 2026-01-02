# Render Loop Analysis - Smart Rendering System

## 🎯 Implementation Steps

### Step 1: Remove platform render loops
- **Aktuell:** Platforms rendern kontinuierlich (Desktop, iOS, Android, Web)
- **Ziel:** Render nur on-demand (Events, Invalidation, Animations)
- **Aufgabe:** Conditional rendering basierend auf InvalidationTracker

### Step 2: Platform layout events implementieren
- **Platform → Core propagation:**
  - Window Resize → InvalidateMeasure + Render
  - DPI Change → InvalidateMeasure + Render
  - Visibility Change → Pause/Resume rendering
  - Screen Rotation → InvalidateMeasure + Render
- **Aufgabe:** Events korrekt zu InvalidationTracker + RenderService propagieren

### Step 3: Animations & Controls (IInvalidator)
- **Core Services/Controls:**
  - TransitionService, ActivityIndicator, Image (GIF)
  - TooltipOverlay, MenuOverlay, ComboBoxDropdown, UiPageElement
- **Aufgabe:** IInvalidator implementieren, bei BuildContent() registrieren

### Step 4: ViewModel Bindings (PropertyChanged)
- **Aktuell:** Bindings einmal evaluiert, nie geupdated
- **Ziel:** Context.PropertyChanged → UpdateBindings → InvalidateMeasure → Render
- **Aufgabe:** PropertyChanged subscription in UiElement

```csharp
// Target Implementation in UiElement:
public virtual INotifyPropertyChanged? Context
{
    get => _context;
    internal set
    {
        if (_context != null)
            _context.PropertyChanged -= OnContextPropertyChanged;

        _context = value;

        if (_context != null)
            _context.PropertyChanged += OnContextPropertyChanged;
    }
}

private void OnContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
{
    UpdateBindings(e.PropertyName);  // Binding update
    InvalidateMeasure();              // Trigger render
}
```

**Flow:**
1. ViewModel feuert PropertyChanged
2. OnContextPropertyChanged wird aufgerufen
3. UpdateBindings evaluiert Bindings für diese Property
4. InvalidateMeasure setzt Flags + triggert Render
5. Platform rendert Frame

---

# Render Loop Analysis - Existing Services

## ✅ Services mit gutem Support

### 1. TransitionService ⭐⭐⭐⭐⭐
**File:** `Services/TransitionService.cs`

**Bereits vorhanden:**
```csharp
public bool IsTransitioning { get; }  // ✅ Perfect!
public void Update();                  // ✅ Called every frame
```

**Integration:**
- `RenderService.Render()` ruft `transitionService.Update()` auf (Zeile 59)
- `IsTransitioning` wird bereits für conditional rendering genutzt
- **KEINE Änderungen nötig** - perfekt für Invalidation Tracking!

---

### 2. Image Control (GIF Support) ⭐⭐⭐⭐
**File:** `Controls/Image.cs`

**Aktuelles Verhalten:**
```csharp
private AnimatedImageInfo? _animatedImageInfo;
private Timer? _animationTimer;

private void AdvanceFrame()
{
    _currentFrameIndex = (_currentFrameIndex + 1) % _animatedImageInfo.FrameCount;
    // This is critical for animated GIFs to actually show animation
    InvalidateMeasure();  // ✅ Triggert Repaint!
    ...
}
```

**Problem:**
- `InvalidateMeasure()` wird aufgerufen
- Aber kein kontinuierlicher Render-Loop → GIF bewegt sich nur bei User-Input

**Lösung:**
- Image muss sich beim InvalidationTracker registrieren wenn `_animatedImageInfo != null`
- Deregistrieren wenn Animation stoppt/disposed

---

### 3. TooltipService ⭐⭐⭐
**File:** `Services/Tooltip/TooltipService.cs`

**Aktuelles Verhalten:**
- Nutzt Timer für Show/Hide delays
- Kein Update() loop, keine Animation tracking

**Benötigt:**
- Falls Tooltips animiert werden sollen (fade in/out)
- Tooltip muss Invalidation während Animation triggern

**Status:** Aktuell keine Animationen → OK

---

### 4. OverlayService ⭐⭐⭐
**File:** `Services/Overlay/OverlayService.cs`

**Aktuelles Verhalten:**
```csharp
private readonly List<UiElement> _overlays = new();
internal void RenderOverlays(SKCanvas canvas) { ... }
```

**Status:** Simple List, keine eigene Animation
- Overlays sind normale UiElements
- Können selbst animiert sein (z.B. TooltipOverlay)
- **Keine direkte Integration nötig**

---

### 5. ActivityIndicator ⭐⭐⭐⭐⭐
**File:** `Controls/ActivityIndicator.cs`

**KRITISCH - Braucht continuous rendering!**
```csharp
private DateTime _startTime;
internal bool IsRunning { get; set; } = true;

public override void Render(SKCanvas canvas)
{
    var elapsedSeconds = (DateTime.Now - _startTime).TotalSeconds;
    var rotationDegrees = (float)((elapsedSeconds * 360 * Speed) % 360);
    // Draws spinning arc based on elapsed time
}
```

**Problem:**
- Berechnet Rotation basierend auf `DateTime.Now` in Render()
- Braucht kontinuierliches Rendering wenn `IsRunning == true`
- Aktuell funktioniert es NICHT ohne continuous loop!

**Lösung:**
- ActivityIndicator implements IInvalidator
- `NeedsRendering => IsRunning`
- **TimeProvider nutzen statt DateTime.Now** (bereits in DI registriert!)

```csharp
public partial class ActivityIndicator : UiElement
{
    private TimeProvider? _timeProvider;
    private DateTimeOffset _startTime;

    public override void BuildContent()
    {
        base.BuildContent();
        _timeProvider = ServiceProviderService.ServiceProvider?
            .GetService<TimeProvider>() ?? TimeProvider.System;
        _startTime = _timeProvider.GetUtcNow();
    }

    public override void Render(SKCanvas canvas)
    {
        var elapsed = (_timeProvider!.GetUtcNow() - _startTime).TotalSeconds;
        var rotationDegrees = (float)((elapsed * 360 * Speed) % 360);
        // ...
    }
}
```

**Vorteile:**
- Testbar (Mock TimeProvider)
- Konsistent mit anderen Services
- Bereits in `AddPlusUiCore` registriert (Zeile 50)

---

### 6. ProgressBar ✅
**File:** `Controls/ProgressBar.cs`

**Status:** Statisch, keine Animation
- Rendert nur aktuellen Progress-Wert
- Kein continuous rendering nötig

---

### 7. Page Appearing/Disappearing 🎬
**File:** `CoreElements/UiPageElement.cs`

**Lifecycle Methods:**
```csharp
public virtual void Appearing() { }
public virtual void Disappearing() { }
public virtual void OnNavigatedTo(object? parameter) { }
public virtual void OnNavigatedFrom() { }
```

**Use Case:** Page kann fade-in Animation bei Appearing machen

**Implementation:**
```csharp
public abstract class UiPageElement : UiElement, IInvalidator
{
    private bool _isAppearingAnimation;

    public bool NeedsRendering => _isAppearingAnimation;
    public event EventHandler? InvalidationChanged;

    public virtual void Appearing()
    {
        // Subclasses können override für Animation
    }

    protected void StartAppearingAnimation()
    {
        _isAppearingAnimation = true;
        InvalidationChanged?.Invoke(this, EventArgs.Empty);

        // Nach Animation fertig:
        Timer.Callback = () =>
        {
            _isAppearingAnimation = false;
            InvalidationChanged?.Invoke(this, EventArgs.Empty);
        };
    }
}
```

**Vorteil:**
- Page selbst kann animiert werden (unabhängig von TransitionService)
- Custom appearing/disappearing effects per Page
- Automatisch tracked via IInvalidator

---

### 8. UserControl ✅
**File:** `Controls/UserControl/UserControl.cs`

**Architektur:**
```csharp
protected abstract UiElement Build();
private UiElement _content;
```

**Wie funktioniert Invalidation?**

**Antwort: Automatisch via Children!** ✅

```csharp
public class MyUserControl : UserControl
{
    protected override UiElement Build() =>
        new VStack(
            new ActivityIndicator().SetIsRunning(true),  // ← Registriert sich selbst!
            new Image().SetSource("animated.gif")         // ← Registriert sich selbst!
        );
}
```

**Warum UserControl NICHTS tun muss:**
1. Children registrieren sich in `BuildContent()`
2. InvalidationTracker hat globale Liste aller Invalidators
3. UserControl ist nur Container
4. Rendering-Bedarf kommt von Children

**UserControl muss NICHT IInvalidator implementieren!**

**Flow:**
```
MyUserControl
  └─ VStack
      ├─ ActivityIndicator (implements IInvalidator) ✅
      │   └─ Register bei BuildContent()
      └─ Image (implements IInvalidator) ✅
          └─ Register bei BuildContent()

InvalidationTracker.NeedsRendering
  = ActivityIndicator.NeedsRendering || Image.NeedsRendering
```

**Clean & Simple!** ✅

---

## 🎯 Pattern Erkenntnisse

### Bereits funktioniert:
1. **TransitionService:** Hat bereits `IsTransitioning` property ✅
2. **Image/GIF:** Ruft `InvalidateMeasure()` auf ✅
3. **RenderService:** Ruft `Update()` auf Services ✅

### Fehlt:
1. **Centralized Tracking:** Kein Service weiß ob rendering nötig ist
2. **Loop Control:** Platforms können Loop nicht stoppen wenn idle

---

## 💡 Empfohlene Architecture

```csharp
// Core Service
public interface IInvalidator
{
    bool NeedsRendering { get; }
    event EventHandler? InvalidationChanged;
}

public class InvalidationTracker
{
    private readonly HashSet<IInvalidator> _invalidators = [];

    public bool NeedsRendering => _invalidators.Any(i => i.NeedsRendering);
    public event EventHandler? RenderingRequiredChanged;

    public void Register(IInvalidator invalidator) { ... }
    public void Unregister(IInvalidator invalidator) { ... }
}
```

### Services die IInvalidator implementieren:
- ✅ **TransitionService** (IsTransitioning → NeedsRendering)
- ✅ **Image** (während GIF animiert)
- ⏳ **Custom Animations** (wenn wir die hinzufügen)
- ⏳ **Loading Indicators** (Spinner etc.)

---

## 🚀 Implementation Plan

1. **Phase 1: Core** (Platform-agnostic)
   - IInvalidator interface
   - InvalidationTracker service
   - TransitionService implements IInvalidator

2. **Phase 2: Controls**
   - Image implements IInvalidator
   - Andere animierte Controls

3. **Phase 3: Platforms**
   - Desktop: WindowManager nutzt InvalidationTracker
   - Mobile: Display Link control
   - Web: RAF Loop control

---

## ✅ Finale Implementation Liste

### Müssen IInvalidator implementieren:

| Type | File | NeedsRendering Logic | Priority |
|------|------|---------------------|----------|
| **TransitionService** | Services/TransitionService.cs | `IsTransitioning` | ⭐⭐⭐⭐⭐ |
| **ActivityIndicator** | Controls/ActivityIndicator.cs | `IsRunning` | ⭐⭐⭐⭐⭐ |
| **Image** | Controls/Image.cs | `_animatedImageInfo != null` | ⭐⭐⭐⭐⭐ |
| **TooltipOverlay** | Controls/Tooltip/TooltipOverlay.cs | `_isAnimating` | ⭐⭐⭐⭐ |
| **UiPageElement** | CoreElements/UiPageElement.cs | `_isAppearingAnimation` | ⭐⭐⭐ |
| **MenuOverlay** | Controls/Menu/MenuOverlay.cs | `_isAnimating` (wenn fade) | ⭐⭐ |
| **ComboBoxDropdown** | Controls/Combobox/ComboBoxDropdownOverlay.cs | `_isAnimating` (wenn fade) | ⭐⭐ |

### Müssen NICHT implementieren:

- ❌ **UserControl** - Children registrieren sich selbst
- ❌ **ProgressBar** - Statisch, keine Animation
- ❌ **OverlayService** - Nur Container
- ❌ **TooltipService** - Nur Management, kein Rendering

### Core Implementation:

```csharp
// IInvalidator.cs
public interface IInvalidator
{
    bool NeedsRendering { get; }
    event EventHandler? InvalidationChanged;
}

// InvalidationTracker.cs
public class InvalidationTracker
{
    private readonly HashSet<IInvalidator> _invalidators = [];
    private readonly ILogger<InvalidationTracker>? _logger;

    public bool NeedsRendering => _invalidators.Any(i => i.NeedsRendering);
    public event EventHandler? RenderingRequiredChanged;

    public void Register(IInvalidator invalidator,
        [CallerMemberName] string? caller = null)
    {
        _logger?.LogTrace("Invalidator registered: {Type} from {Caller}",
            invalidator.GetType().Name, caller);

        _invalidators.Add(invalidator);
        invalidator.InvalidationChanged += OnInvalidatorChanged;
    }

    public void Unregister(IInvalidator invalidator)
    {
        _logger?.LogTrace("Invalidator unregistered: {Type}",
            invalidator.GetType().Name);

        invalidator.InvalidationChanged -= OnInvalidatorChanged;
        _invalidators.Remove(invalidator);
    }

    private void OnInvalidatorChanged(object? sender, EventArgs e)
    {
        // Debounce implementation here
        NotifyRenderingStateChanged();
    }

    private void NotifyRenderingStateChanged()
    {
        var needsRendering = NeedsRendering;
        _logger?.LogDebug("Rendering state: {NeedsRendering}, Active: {Count}",
            needsRendering, _invalidators.Count(i => i.NeedsRendering));

        RenderingRequiredChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

## ✅ Fazit

**Alles designed und bereit für Implementation:**

- ✅ IInvalidator interface klar definiert
- ✅ 7 Controls/Services müssen es implementieren
- ✅ Pattern für Registration in BuildContent() / Dispose()
- ✅ UserControl funktioniert automatisch via Children
- ✅ Logging mit CallerMemberName für Debugging
- ✅ Debounce für Performance
- ✅ Cross-platform (Core + Platform-specific Loop Control)

**Keine "für später" Punkte - alles komplett!**

---

## 🎨 ViewModel-Based Animations (TRANSPARENT!)

### Zwei getrennte Use Cases:

#### 1. **ViewModel-basierte Animationen** (User Code)
User schreibt einfach normales ViewModel mit Timer/Task:

```csharp
public class CountdownViewModel : INotifyPropertyChanged
{
    private int _remainingSeconds = 60;
    private Timer? _timer;

    public int RemainingSeconds
    {
        get => _remainingSeconds;
        set
        {
            _remainingSeconds = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingSeconds)));
        }
    }

    public void StartCountdown()
    {
        _timer = new Timer(_ =>
        {
            if (_remainingSeconds > 0)
                RemainingSeconds--;  // ← Feuert PropertyChanged!
        }, null, 0, 1000);
    }
}
```

**Lösung: Binding System macht das AUTOMATISCH!** ✅

```
PropertyChanged → UpdateBindingsInternal() → Render Request
```

**Kein "Hot Element" Tracking nötig!** Warum?
- User feuert PropertyChanged in Loop (Timer/Task)
- Jedes PropertyChanged → ein Render
- User will animieren? → User feuert PropertyChanged oft
- Framework muss nichts tracken!

#### 2. **System-basierte Animationen** (Framework Code)
Components wie ActivityIndicator, GIF, Transitions:

```csharp
public override void Render(SKCanvas canvas)
{
    var elapsed = (_timeProvider.GetUtcNow() - _startTime).TotalSeconds;
    var rotation = (float)((elapsed * 360 * Speed) % 360);  // ← Berechnet bei jedem Frame!
}
```

**Lösung: IInvalidator** ✅
- Component sagt "ich brauche continuous rendering"
- InvalidationTracker managed Render-Loop
- Kein PropertyChanged involviert

### Vergleich:

| Typ | Trigger | Rendering | Beispiel |
|-----|---------|-----------|----------|
| **ViewModel** | PropertyChanged (User Loop) | Event-driven | Countdown, Progress Simulation |
| **System** | IInvalidator | Continuous Loop | ActivityIndicator, GIF, Transitions |

### Implementation (ViewModel):

**KEINE Änderungen nötig!** ✅

Binding system macht bereits:
```csharp
protected override void UpdateBindingsInternal(string propertyName)
{
    // Update value from ViewModel → UI
    base.UpdateBindingsInternal(propertyName);

    // Trigger render (already exists!)
    InvalidateMeasure();
}
```

User animiert → PropertyChanged Loop → Render Loop
User stoppt → Keine PropertyChanged → Kein Render

**Komplett transparent!** ✅

### Vorteile dieses Ansatzes:

✅ **Einfacher** - Keine "Hot Element" Logik
✅ **Klarer** - Zwei getrennte Mechanismen für zwei Use Cases
✅ **Performanter** - Kein Tracking-Overhead bei jedem PropertyChanged
✅ **Transparent** - User merkt nichts, schreibt normales INotifyPropertyChanged
✅ **Event-driven** - ViewModel triggert genau wenn nötig

---

## 📊 Logging & Diagnostics

### Trace Logging (WICHTIG!)
```csharp
public class InvalidationTracker
{
    private readonly ILogger<InvalidationTracker>? _logger;

    public void Register(IInvalidator invalidator, [CallerMemberName] string? caller = null)
    {
        _logger?.LogTrace("Invalidator registered: {Type} from {Caller}",
            invalidator.GetType().Name, caller);
        // ...
        CheckRenderingState();
    }

    private void CheckRenderingState()
    {
        var needsRendering = NeedsRendering;
        if (needsRendering != _previousState)
        {
            _logger?.LogDebug("Rendering state changed: {NeedsRendering}. Active invalidators: {Count}",
                needsRendering, _invalidators.Count(i => i.NeedsRendering));

            // Log which invalidators are active
            foreach (var inv in _invalidators.Where(i => i.NeedsRendering))
            {
                _logger?.LogTrace("  - {Type} needs rendering", inv.GetType().Name);
            }
        }
    }
}
```

**Wozu:**
- Debuggen welcher Control continuous rendering triggert
- Performance-Probleme identifizieren
- Verstehen wann Loop startet/stoppt

---

## ⏱️ Debounce / Throttle (KRITISCH!)

### Problem:
Mehrere schnelle Invalidation-Events → Unnötige Loop Restarts

### Lösung: Debounce InvalidationChanged
```csharp
public class InvalidationTracker
{
    private Timer? _debounceTimer;
    private bool _pendingNotification;

    private void OnInvalidatorChanged(object? sender, EventArgs e)
    {
        _pendingNotification = true;

        // Debounce: Wait 16ms (1 frame @ 60fps) before notifying
        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(_ =>
        {
            if (_pendingNotification)
            {
                _pendingNotification = false;
                RenderingRequiredChanged?.Invoke(this, EventArgs.Empty);
            }
        }, null, 16, Timeout.Infinite);
    }
}
```

### RAF Throttle (Web-specific)
```csharp
// PlusUiRootComponent.razor
private int? _rafHandle;
private bool _rafPending;

private void StartRenderLoop()
{
    if (_rafHandle.HasValue) return;
    ScheduleNextFrame();
}

private void ScheduleNextFrame()
{
    if (_rafPending) return;
    _rafPending = true;

    _rafHandle = await JsRuntime.InvokeAsync<int>(
        "PlusUiInterop.requestRender",
        DotNetObjectReference.Create(this));
}

[JSInvokable]
public void OnAnimationFrame()
{
    _rafPending = false;

    if (_invalidationTracker.NeedsRendering)
    {
        _canvasView?.Invalidate();
        ScheduleNextFrame(); // Continue loop
    }
    else
    {
        _rafHandle = null; // Stop loop
    }
}
```

**Benefits:**
- Verhindert mehrfache RAF calls pro Frame
- Stoppt Loop automatisch wenn idle
- Startet nur wenn wirklich nötig

---

## 🔄 DI Lifecycle & Registration

### Services (alle Singleton ✅)
```csharp
services.AddSingleton<TransitionService>();
services.AddSingleton<ITransitionService>(sp => sp.GetRequiredService<TransitionService>());
services.AddSingleton<TooltipService>();
services.AddSingleton<OverlayService>();
services.AddSingleton<ImageLoaderService>();
```

**Lifecycle:**
- ✅ Singleton = Ein Service pro App-Lifetime
- ✅ Kann sich einmal bei InvalidationTracker registrieren
- ✅ Event-Handler bleiben aktiv

### Controls (kein DI! ⚠️)
```csharp
// Controls werden direkt erstellt:
new ActivityIndicator().SetIsRunning(true)
new Image().SetSource("animated.gif")
```

**Problem:**
- Keine DI = Keine automatische Registration
- Müssen sich selbst registrieren/deregistrieren

**Lösung:**
```csharp
public partial class ActivityIndicator : UiElement, IInvalidator
{
    private InvalidationTracker? _tracker;
    public bool NeedsRendering => IsRunning;
    public event EventHandler? InvalidationChanged;

    public override void BuildContent()
    {
        base.BuildContent();

        // Get tracker from static ServiceProvider
        _tracker = ServiceProviderService.ServiceProvider?
            .GetService<InvalidationTracker>();

        // Register self
        _tracker?.Register(this);
    }

    internal bool IsRunning
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                InvalidationChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _tracker?.Unregister(this);
        }
        base.Dispose(disposing);
    }
}
```

**Pattern:**
1. In `BuildContent()` → Register
2. Bei Property-Change → Event feuern
3. In `Dispose()` → Unregister

---

## 🖱️ Tooltip bei Mouse Move

### Aktueller Flow:
```csharp
// InputService.cs
public void MouseMove(Vector2 location)
{
    UpdateHoverState(location); // Checks hover enter/leave
}

// TooltipService.cs
public void OnHoverEnter(UiElement? element)
{
    // Start show timer (delayed)
    _showTimer = new Timer(..., tooltip.ShowDelay);
}
```

### Problem:
Mouse Move über hoverable Element → Brauchen wir continuous rendering?

### Antwort: **NEIN!** ✅

**Warum:**
- Tooltip zeigt erst nach Delay (z.B. 500ms)
- Während Mouse Move ohne aktiven Tooltip: kein Rendering nötig
- Nur wenn Tooltip ZEIGT UND animiert ist: rendering nötig

**Lösung:**
```csharp
public class TooltipOverlay : IInvalidator
{
    private bool _isAnimating;
    public bool NeedsRendering => _isAnimating;
    public event EventHandler? InvalidationChanged;

    // Wenn fade-in Animation:
    private void StartFadeIn()
    {
        _isAnimating = true;
        InvalidationChanged?.Invoke(this, EventArgs.Empty);

        // Animation complete nach 200ms
        Timer.Callback = () =>
        {
            _isAnimating = false;
            InvalidationChanged?.Invoke(this, EventArgs.Empty);
        };
    }
}
```

**Implementation:**
```csharp
public class TooltipOverlay : UiElement, IInvalidator
{
    private bool _isAnimating;
    private float _opacity;
    private DateTime _animationStart;

    public bool NeedsRendering => _isAnimating;
    public event EventHandler? InvalidationChanged;

    public TooltipOverlay(...)
    {
        // Register immediately
        var tracker = ServiceProviderService.ServiceProvider?
            .GetService<InvalidationTracker>();
        tracker?.Register(this);

        StartFadeIn();
    }

    private void StartFadeIn()
    {
        _isAnimating = true;
        _animationStart = DateTime.Now;
        InvalidationChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void Render(SKCanvas canvas)
    {
        if (_isAnimating)
        {
            var elapsed = (DateTime.Now - _animationStart).TotalMilliseconds;
            var duration = 200; // ms
            _opacity = Math.Min(1f, (float)(elapsed / duration));

            if (elapsed >= duration)
            {
                _isAnimating = false;
                _opacity = 1f;
                InvalidationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        canvas.Save();
        canvas.SetAlpha((byte)(_opacity * 255));
        // ... render tooltip content
        canvas.Restore();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var tracker = ServiceProviderService.ServiceProvider?
                .GetService<InvalidationTracker>();
            tracker?.Unregister(this);
        }
        base.Dispose(disposing);
    }
}
```
