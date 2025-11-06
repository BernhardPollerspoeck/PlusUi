# Headless Platform - Anforderungsdokument

## Übersicht

Die Headless Platform ermöglicht die Ausführung von PlusUi-Anwendungen ohne grafisches Display. Das Framework rendert UI-Elemente in den Speicher und stellt diese als Bilddaten zur Verfügung. Eingaben (Maus, Tastatur) werden programmatisch injiziert.

### Hauptanwendungsfälle

1. **Automatisierte UI-Tests**: Integration Tests mit Screenshot-Vergleich (Visual Regression Testing)
2. **Remote/Cloud Usage**: UI in der Cloud rendern und als Image-Stream bereitstellen
3. **CI/CD Integration**: Headless UI-Tests in Build-Pipelines
4. **Documentation/Screenshots**: Automatische Screenshot-Generierung für Dokumentation

---

## Architektur-Übersicht

```
┌─────────────────────────────────────────────────────┐
│                Test/Client Code                      │
│  - Ruft IHeadlessRenderService auf                  │
│  - Simuliert Input (Mouse, Keyboard)                │
│  - Erhält Frames als PNG                            │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│            IHeadlessRenderService                    │
│  + Task<byte[]> GetCurrentFrameAsync()              │
│  + MouseMove(float x, float y)                      │
│  + MouseDown()                                       │
│  + MouseUp()                                         │
│  + MouseWheel(float deltaX, float deltaY)           │
│  + KeyPress(PlusKey key)                            │
│  + CharInput(char c)                                │
│  + Size FrameSize { get; set; }                     │
│  + ImageFormat Format { get; set; }                 │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│         HeadlessRenderService (Implementation)       │
│  - Verwaltet aktuellen Frame-State                  │
│  - Nutzt RenderService für Layout/Rendering         │
│  - Nutzt InputService für Input-Propagation         │
│  - Rendert on-demand in Memory (SKBitmap)           │
└────────────────┬────────────────────────────────────┘
                 │
       ┌─────────┴─────────┐
       ▼                   ▼
┌──────────────┐    ┌──────────────────┐
│ RenderService│    │  InputService    │
│ (Core)       │    │  (Core)          │
└──────────────┘    └──────────────────┘
       │                   │
       └─────────┬─────────┘
                 ▼
┌─────────────────────────────────────────────────────┐
│         HeadlessPlatformService                      │
│  (IPlatformService Implementation)                   │
│  - Platform = Headless                              │
│  - WindowSize = konfigurierbar                      │
│  - DisplayDensity = 1.0 (default)                   │
│  - OpenUrl() = false (nicht interaktiv)             │
└─────────────────────────────────────────────────────┘
```

---

## 1. Service-Interface Design

### IHeadlessRenderService Interface

```csharp
namespace PlusUi.Headless.Services;

/// <summary>
/// Service für Headless-Rendering von PlusUi-Anwendungen.
/// Ermöglicht programmatische Input-Simulation und Frame-Capturing.
/// </summary>
public interface IHeadlessRenderService
{
    /// <summary>
    /// Rendert den aktuellen Frame on-demand und gibt ihn als Bilddaten zurück.
    /// Führt intern Measure, Arrange und Render aus.
    /// </summary>
    /// <returns>Frame als Byte-Array im konfigurierten Format (PNG/JPEG/etc.)</returns>
    Task<byte[]> GetCurrentFrameAsync();

    /// <summary>
    /// Frame-Größe (Breite x Höhe in Pixel).
    /// Änderungen triggern neues Layout.
    /// </summary>
    Size FrameSize { get; set; }

    /// <summary>
    /// Ausgabeformat der Frames (PNG, JPEG, WebP).
    /// </summary>
    ImageFormat Format { get; set; }

    /// <summary>
    /// Bewegt die Maus zur angegebenen Position.
    /// </summary>
    void MouseMove(float x, float y);

    /// <summary>
    /// Erzeugt MouseDown-Event an der aktuellen Mausposition.
    /// </summary>
    void MouseDown();

    /// <summary>
    /// Erzeugt MouseUp-Event an der aktuellen Mausposition.
    /// </summary>
    void MouseUp();

    /// <summary>
    /// Erzeugt Mouse-Wheel-Event an der aktuellen Mausposition.
    /// </summary>
    void MouseWheel(float deltaX, float deltaY);

    /// <summary>
    /// Simuliert Tastendruck (Enter, Backspace, etc.).
    /// </summary>
    void KeyPress(PlusKey key);

    /// <summary>
    /// Simuliert Zeichen-Eingabe (für Text-Input).
    /// </summary>
    void CharInput(char c);

    /// <summary>
    /// Wartet auf das Abschließen aller laufenden Animationen.
    /// Nützlich für stabile Screenshots.
    /// </summary>
    Task WaitForAnimationsAsync(TimeSpan? timeout = null);

    /// <summary>
    /// Wartet auf das Abschließen aller laufenden Property-Bindings.
    /// Nützlich nach ViewModel-Änderungen.
    /// </summary>
    Task WaitForBindingUpdatesAsync();
}
```

### ImageFormat Enumeration

```csharp
namespace PlusUi.Headless.Enumerations;

public enum ImageFormat
{
    /// <summary>PNG - Verlustfrei, größere Dateigröße</summary>
    Png,

    /// <summary>JPEG - Komprimiert, kleinere Dateigröße</summary>
    Jpeg,

    /// <summary>WebP - Modern, gute Kompression</summary>
    WebP
}
```

---

## 2. Platform-Implementation

### HeadlessPlatformService

```csharp
namespace PlusUi.Headless.Services;

public class HeadlessPlatformService : IPlatformService
{
    private Size _windowSize;

    public HeadlessPlatformService(Size initialSize)
    {
        _windowSize = initialSize;
    }

    public PlatformType Platform => PlatformType.Headless;

    public Size WindowSize
    {
        get => _windowSize;
        set => _windowSize = value;
    }

    public float DisplayDensity => 1.0f;

    public bool OpenUrl(string url)
    {
        // Headless-Umgebung hat keinen Browser
        return false;
    }
}
```

### PlatformType Erweiterung

```csharp
// In: PlusUi.core/Services/IPlatformService.cs

public enum PlatformType
{
    Desktop,
    Android,
    iOS,
    Web,
    Headless  // NEU
}
```

---

## 3. Keyboard-Handler Implementation

### HeadlessKeyboardHandler

```csharp
namespace PlusUi.Headless.Services;

/// <summary>
/// Mock-Implementation für Headless-Umgebung.
/// Keyboard wird programmatisch gesteuert (kein natives Keyboard).
/// </summary>
public class HeadlessKeyboardHandler : IKeyboardHandler
{
    public event EventHandler<PlusKey>? KeyInput;
    public event EventHandler<char>? CharInput;

    // Show/Hide sind no-ops in Headless-Umgebung
    public void Show() { }
    public void Hide() { }
    public void Show(KeyboardType keyboardType, ReturnKeyType returnKeyType, bool isPassword) { }

    // Interne Methoden für programmatische Steuerung
    internal void RaiseKeyInput(PlusKey key)
    {
        KeyInput?.Invoke(this, key);
    }

    internal void RaiseCharInput(char c)
    {
        CharInput?.Invoke(this, c);
    }
}
```

---

## 4. Render-Service Implementation

### HeadlessRenderService

**Verantwortlichkeiten:**
- Frame-Rendering on-demand (kein kontinuierlicher Loop)
- Input-Event-Propagation über bestehenden `InputService`
- Memory-basiertes Rendering (kein GPU/OpenGL erforderlich)
- Format-Konvertierung (SKBitmap → PNG/JPEG bytes)

**Technischer Ansatz:**
```csharp
public class HeadlessRenderService : IHeadlessRenderService
{
    private readonly RenderService _renderService;
    private readonly InputService _inputService;
    private readonly HeadlessPlatformService _platformService;
    private readonly HeadlessKeyboardHandler _keyboardHandler;

    private Vector2 _currentMousePosition;
    private Size _frameSize;
    private ImageFormat _format = ImageFormat.Png;

    public Size FrameSize
    {
        get => _frameSize;
        set
        {
            _frameSize = value;
            _platformService.WindowSize = value;
        }
    }

    public ImageFormat Format
    {
        get => _format;
        set => _format = value;
    }

    public Task<byte[]> GetCurrentFrameAsync()
    {
        return Task.Run(() =>
        {
            // 1. Create offscreen surface
            var bitmap = new SKBitmap(
                (int)_frameSize.Width,
                (int)_frameSize.Height,
                SKColorType.Rgba8888,
                SKAlphaType.Premul
            );

            using var canvas = new SKCanvas(bitmap);

            // 2. Render current frame (Measure + Arrange + Render)
            _renderService.Render(
                gl: null,  // Kein OpenGL
                canvas: canvas,
                grContext: null,  // Kein GPU-Context
                canvasSize: new Vector2(_frameSize.Width, _frameSize.Height)
            );

            // 3. Encode to format
            return EncodeToFormat(bitmap, _format);
        });
    }

    public void MouseMove(float x, float y)
    {
        _currentMousePosition = new Vector2(x, y);
        _inputService.MouseMove(_currentMousePosition);
    }

    public void MouseDown()
    {
        _inputService.MouseDown(_currentMousePosition);
    }

    public void MouseUp()
    {
        _inputService.MouseUp(_currentMousePosition);
    }

    public void MouseWheel(float deltaX, float deltaY)
    {
        _inputService.MouseWheel(_currentMousePosition, deltaX, deltaY);
    }

    public void KeyPress(PlusKey key)
    {
        _keyboardHandler.RaiseKeyInput(key);
    }

    public void CharInput(char c)
    {
        _keyboardHandler.RaiseCharInput(c);
    }

    private byte[] EncodeToFormat(SKBitmap bitmap, ImageFormat format)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = format switch
        {
            ImageFormat.Png => image.Encode(SKEncodedImageFormat.Png, 100),
            ImageFormat.Jpeg => image.Encode(SKEncodedImageFormat.Jpeg, 90),
            ImageFormat.WebP => image.Encode(SKEncodedImageFormat.Webp, 90),
            _ => throw new ArgumentException($"Unsupported format: {format}")
        };

        return data.ToArray();
    }

    public async Task WaitForAnimationsAsync(TimeSpan? timeout = null)
    {
        // TODO: Hook into Animation-System
        // For now: simple delay
        await Task.Delay(timeout ?? TimeSpan.FromSeconds(1));
    }

    public async Task WaitForBindingUpdatesAsync()
    {
        // TODO: Hook into Binding-System
        // For now: single frame delay
        await Task.Yield();
    }
}
```

---

## 5. Service Registration

### Extension Method

```csharp
namespace PlusUi.Headless.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder UsePlusUiHeadless(
        this IHostApplicationBuilder builder,
        IAppConfiguration appConfiguration,
        Size initialFrameSize)
    {
        // Core PlusUi Services
        builder.UsePlusUiInternal(appConfiguration);

        // Headless Platform Service
        var platformService = new HeadlessPlatformService(initialFrameSize);
        builder.Services.AddSingleton<HeadlessPlatformService>(platformService);
        builder.Services.AddSingleton<IPlatformService>(platformService);

        // Headless Keyboard Handler
        var keyboardHandler = new HeadlessKeyboardHandler();
        builder.Services.AddSingleton<HeadlessKeyboardHandler>(keyboardHandler);
        builder.Services.AddSingleton<IKeyboardHandler>(keyboardHandler);

        // Headless Render Service
        builder.Services.AddSingleton<HeadlessRenderService>();
        builder.Services.AddSingleton<IHeadlessRenderService>(
            sp => sp.GetRequiredService<HeadlessRenderService>()
        );

        return builder;
    }
}
```

### Usage Example

```csharp
// In Test-Projekt oder Headless-App

var builder = Host.CreateApplicationBuilder();

builder.UsePlusUiHeadless(
    appConfiguration: new MyAppConfiguration(),
    initialFrameSize: new Size(1920, 1080)
);

var host = builder.Build();

// Headless-Service holen
var headless = host.Services.GetRequiredService<IHeadlessRenderService>();

// Navigation zur Startseite
var navigation = host.Services.GetRequiredService<INavigationService>();
navigation.NavigateTo<MainPage>();

// Frame rendern
var frameBytes = await headless.GetCurrentFrameAsync();
File.WriteAllBytes("screenshot.png", frameBytes);

// Input simulieren
headless.MouseMove(100, 200);
headless.MouseDown();
headless.MouseUp();

// Neuen Frame nach Interaktion
frameBytes = await headless.GetCurrentFrameAsync();
```

---

## 6. Testing-Szenarien

### 6.1 Visual Regression Testing

```csharp
[TestClass]
public class VisualRegressionTests
{
    private IHost? _host;
    private IHeadlessRenderService? _headless;

    [TestInitialize]
    public async Task Setup()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.UsePlusUiHeadless(
            new TestAppConfiguration(),
            new Size(1280, 720)
        );

        _host = builder.Build();
        await _host.StartAsync();

        _headless = _host.Services.GetRequiredService<IHeadlessRenderService>();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        if (_host is not null)
            await _host.StopAsync();
    }

    [TestMethod]
    public async Task MainPage_InitialState_MatchesBaseline()
    {
        // Arrange
        var navigation = _host!.Services.GetRequiredService<INavigationService>();
        navigation.NavigateTo<MainPage>();

        await _headless!.WaitForBindingUpdatesAsync();

        // Act
        var frame = await _headless.GetCurrentFrameAsync();

        // Assert
        var baselinePath = "Baselines/MainPage_InitialState.png";
        AssertImageMatches(frame, baselinePath);
    }

    [TestMethod]
    public async Task Button_OnHover_ChangesColor()
    {
        // Arrange
        var navigation = _host!.Services.GetRequiredService<INavigationService>();
        navigation.NavigateTo<MainPage>();

        // Act - Move mouse to button
        _headless!.MouseMove(640, 360); // Center of 1280x720
        await _headless.WaitForAnimationsAsync();

        var frame = await _headless.GetCurrentFrameAsync();

        // Assert
        AssertImageMatches(frame, "Baselines/Button_Hover.png");
    }

    private void AssertImageMatches(byte[] actual, string baselinePath)
    {
        // Image comparison logic (z.B. mit ImageSharp)
        // ...
    }
}
```

### 6.2 Interaction Testing

```csharp
[TestMethod]
public async Task Entry_TypeText_UpdatesValue()
{
    // Arrange
    var navigation = _host!.Services.GetRequiredService<INavigationService>();
    navigation.NavigateTo<LoginPage>();

    var viewModel = _host.Services.GetRequiredService<LoginViewModel>();

    // Act - Click on entry field
    _headless!.MouseMove(400, 300);
    _headless.MouseDown();
    _headless.MouseUp();

    // Type text
    foreach (var c in "testuser")
    {
        _headless.CharInput(c);
    }

    await _headless.WaitForBindingUpdatesAsync();

    // Assert
    Assert.AreEqual("testuser", viewModel.Username);

    // Visual verification
    var frame = await _headless.GetCurrentFrameAsync();
    File.WriteAllBytes("test_output.png", frame);
}
```

### 6.3 Scroll Testing

```csharp
[TestMethod]
public async Task ScrollView_MouseWheel_ScrollsContent()
{
    // Arrange
    var navigation = _host!.Services.GetRequiredService<INavigationService>();
    navigation.NavigateTo<ScrollTestPage>();

    // Capture initial state
    var frameBefore = await _headless!.GetCurrentFrameAsync();

    // Act - Scroll down
    _headless.MouseMove(640, 360);
    _headless.MouseWheel(0, -100); // Negative = scroll down

    await Task.Delay(100); // Allow scroll animation

    var frameAfter = await _headless.GetCurrentFrameAsync();

    // Assert - Frames should be different
    Assert.IsFalse(frameBefore.SequenceEqual(frameAfter));
}
```

---

## 7. Remote/Cloud Usage Scenario

### Frame-Streaming Server (Konzept)

```csharp
// WebAPI Endpoint für Remote-Rendering

[ApiController]
[Route("api/ui")]
public class RemoteUiController : ControllerBase
{
    private readonly IHeadlessRenderService _headless;

    public RemoteUiController(IHeadlessRenderService headless)
    {
        _headless = headless;
    }

    [HttpGet("frame")]
    public async Task<IActionResult> GetFrame()
    {
        var frame = await _headless.GetCurrentFrameAsync();
        return File(frame, "image/png");
    }

    [HttpPost("input/mouse/move")]
    public IActionResult MouseMove([FromBody] MousePosition pos)
    {
        _headless.MouseMove(pos.X, pos.Y);
        return Ok();
    }

    [HttpPost("input/mouse/click")]
    public IActionResult MouseClick()
    {
        _headless.MouseDown();
        _headless.MouseUp();
        return Ok();
    }

    [HttpPost("input/keyboard")]
    public IActionResult KeyPress([FromBody] KeyInput input)
    {
        if (input.Char.HasValue)
            _headless.CharInput(input.Char.Value);
        else if (input.Key.HasValue)
            _headless.KeyPress(input.Key.Value);

        return Ok();
    }
}
```

---

## 8. Projektstruktur

```
PlusUi.Headless/
├── Services/
│   ├── IHeadlessRenderService.cs
│   ├── HeadlessRenderService.cs
│   ├── HeadlessPlatformService.cs
│   └── HeadlessKeyboardHandler.cs
├── Enumerations/
│   └── ImageFormat.cs
├── Extensions/
│   └── HostApplicationBuilderExtensions.cs
└── PlusUi.Headless.csproj

PlusUi.Headless.Tests/
├── VisualRegressionTests.cs
├── InteractionTests.cs
├── NavigationTests.cs
├── Baselines/
│   ├── MainPage_InitialState.png
│   └── ...
└── PlusUi.Headless.Tests.csproj
```

---

## 9. Dependencies

**PlusUi.Headless.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <!-- Core Framework -->
    <ProjectReference Include="..\PlusUi.core\PlusUi.core.csproj" />

    <!-- SkiaSharp für CPU-basiertes Rendering -->
    <PackageReference Include="SkiaSharp" Version="3.116.1" />

    <!-- Microsoft Hosting -->
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.0" />
  </ItemGroup>
</Project>
```

**PlusUi.Headless.Tests.csproj:**
```xml
<ItemGroup>
  <ProjectReference Include="..\PlusUi.Headless\PlusUi.Headless.csproj" />

  <!-- Testing Frameworks -->
  <PackageReference Include="MSTest.TestFramework" Version="3.6.3" />
  <PackageReference Include="MSTest.TestAdapter" Version="3.6.3" />

  <!-- Image Comparison (optional) -->
  <PackageReference Include="SixLabors.ImageSharp" Version="3.1.5" />
</ItemGroup>
```

---

## 10. Performance-Überlegungen

### Memory Management
- **SKBitmap Disposal**: Jeder Frame muss ordnungsgemäß disposed werden
- **Bitmap Pooling**: Für häufige Frame-Captures wiederverwendbare Bitmaps nutzen
- **Lazy Rendering**: Nur rendern wenn Frame angefordert wird (✅ bereits geplant)

### Concurrency
- **Thread-Safety**: RenderService ist nicht thread-safe → Lock bei GetCurrentFrameAsync
- **Parallel Testing**: Jeder Test braucht eigene Host-Instanz

### Optimierungen
- **Format-Caching**: JPEG für schnelles Streaming, PNG für Tests
- **Dirty-Flag**: Nur neu rendern wenn UI sich geändert hat (optional)
- **Partial Updates**: Nur geänderte Regionen rendern (fortgeschritten)

---

## 11. Offene Fragen / TODOs

### Muss entschieden werden:
- [ ] **Animation-Wartemechanismus**: Wie erkennt man, dass Animationen fertig sind?
- [ ] **Binding-Propagation**: Wie wartet man auf abgeschlossene Binding-Updates?
- [ ] **Multi-Instance**: Unterstützt RenderService mehrere parallele Instanzen?
- [ ] **Dirty-Tracking**: Brauchen wir einen Mechanismus um zu wissen, wann neu gerendert werden muss?

### Nice-to-have Features:
- [ ] **Frame-Differencing**: Nur Änderungen zwischen Frames berechnen
- [ ] **Recording-Mode**: Sequenz von Frames aufzeichnen (für GIF/Video)
- [ ] **Element-Highlighting**: Test-Hilfsmethode um Element-Bounds zu zeichnen
- [ ] **Debug-Overlay**: Zeige Layout-Bounds, HitTest-Bereiche, etc.

---

## 12. Implementierungsplan (Phasen)

### Phase 1: Grundstruktur (Minimal Viable Product)
1. PlusUi.Headless Projekt anlegen
2. `ImageFormat` Enum erstellen
3. `IHeadlessRenderService` Interface definieren
4. `HeadlessPlatformService` implementieren
5. `HeadlessKeyboardHandler` implementieren
6. `PlatformType.Headless` zu Enum hinzufügen

### Phase 2: Core Rendering
7. `HeadlessRenderService` Grundgerüst
8. `GetCurrentFrameAsync()` mit PNG-Encoding
9. Integration mit bestehendem `RenderService`
10. Extension Method `UsePlusUiHeadless()`

### Phase 3: Input-System
11. Mouse-Input-Methoden implementieren
12. Keyboard-Input-Methoden implementieren
13. Integration mit bestehendem `InputService`

### Phase 4: Testing & Validation
14. Erstes Test-Projekt erstellen
15. Sample-App für Headless-Tests
16. Visual Regression Test schreiben
17. Interaction Test schreiben

### Phase 5: Polish & Documentation
18. JPEG/WebP Format-Support
19. `WaitForAnimations()` implementieren
20. `WaitForBindingUpdates()` implementieren
21. Performance-Optimierungen
22. Dokumentation & Beispiele

---

## 13. Risiken & Mitigations

| Risiko | Wahrscheinlichkeit | Impact | Mitigation |
|--------|-------------------|--------|------------|
| RenderService nicht thread-safe | Hoch | Hoch | Lock-Mechanismus in HeadlessRenderService |
| Memory Leaks bei häufigen Frames | Mittel | Hoch | Strikte Disposal-Pattern, Pooling |
| Animation-Timing instabil | Mittel | Mittel | Configurable Delays, Event-basiertes Warten |
| Multi-Instance Support fehlt | Niedrig | Mittel | Pro Test eigene Host-Instanz |

---

## Zusammenfassung

Die Headless Platform ist ein **schlankes Add-on** zum bestehenden PlusUi-Framework:

✅ **Wiederverwendet bestehende Abstractions** (IPlatformService, IKeyboardHandler, RenderService, InputService)
✅ **Minimaler Code-Aufwand** (~300-400 LOC geschätzt)
✅ **Klare Separation of Concerns**
✅ **On-Demand Rendering** (kein kontinuierlicher Loop)
✅ **Perfekt für Tests und Remote-Szenarien**

**Kern-Konzept**: Die UI wird normal aufgebaut und verwaltet, nur das Rendering geht in den Speicher statt auf ein Display.
