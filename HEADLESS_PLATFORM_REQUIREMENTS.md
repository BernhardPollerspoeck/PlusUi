# Headless Platform - Anforderungsdokument

## Übersicht

Die Headless Platform ermöglicht die Ausführung von PlusUi-Anwendungen ohne grafisches Display. Das Framework rendert UI-Elemente in den Speicher und stellt diese als Bilddaten zur Verfügung. Eingaben (Maus, Tastatur) werden programmatisch injiziert.

### Hauptanwendungsfälle

1. **Automatisierte UI-Tests**: Integration Tests mit Screenshot-Vergleich (Visual Regression Testing)
2. **Remote/Cloud Usage**: UI in der Cloud rendern und als Image-Stream bereitstellen
3. **CI/CD Integration**: Headless UI-Tests in Build-Pipelines
4. **Documentation/Screenshots**: Automatische Screenshot-Generierung für Dokumentation

---

## Interface-Namenskonzept

Das zentrale Interface umfasst **Rendering + Input + Konfiguration** - es ist das Haupt-Interface für alle Headless-Operationen.

### Name: `IPlusUiHeadlessService`

**Begründung:**
- ✅ Macht klar: DAS zentrale Interface für Headless-Interaktion mit PlusUi
- ✅ Umfasst Rendering und Input-Handling
- ✅ Folgt PlusUi Naming-Konvention mit `IPlusUi*` Präfix
- ✅ Generisch genug für alle Headless-Szenarien (Tests, Remote, CI/CD)
- ✅ Schlank und fokussiert: Konfiguration erfolgt beim Startup, nicht zur Laufzeit

**Alternative Namen (verworfen):**
- `IHeadlessRenderService` - Zu eingeschränkt (nur Rendering im Namen, aber Interface macht auch Input)
- `IFrameCaptureService` - Zu fokussiert auf Capturing (Input fehlt im Namen)
- `IHeadlessUiService` - Weniger klar in der Zugehörigkeit zu PlusUi

---

## Architektur-Übersicht

```
┌─────────────────────────────────────────────────────┐
│                Test/Client Code                      │
│  - Ruft IPlusUiHeadlessService auf                  │
│  - Simuliert Input (Mouse, Keyboard)                │
│  - Erhält Frames als PNG                            │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│            IPlusUiHeadlessService                    │
│  ┌─────────────────────────────────────────────┐    │
│  │ Rendering                                   │    │
│  │  + Task<byte[]> GetCurrentFrameAsync()      │    │
│  └─────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────┐    │
│  │ Input                                       │    │
│  │  + MouseMove(float x, float y)              │    │
│  │  + MouseDown()                              │    │
│  │  + MouseUp()                                │    │
│  │  + MouseWheel(float deltaX, float deltaY)   │    │
│  │  + KeyPress(PlusKey key)                    │    │
│  │  + CharInput(char c)                        │    │
│  └─────────────────────────────────────────────┘    │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│         PlusUiHeadlessService (Implementation)       │
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

### IPlusUiHeadlessService Interface

```csharp
namespace PlusUi.Headless.Services;

/// <summary>
/// Zentrales Interface für Headless-Betrieb von PlusUi-Anwendungen.
/// Ermöglicht programmatische Input-Simulation und Frame-Capturing.
/// Frame-Größe und Format werden beim Startup konfiguriert.
/// </summary>
public interface IPlusUiHeadlessService
{
    /// <summary>
    /// Rendert den aktuellen Frame on-demand und gibt ihn als Bilddaten zurück.
    /// Führt intern Measure, Arrange und Render aus.
    /// </summary>
    /// <returns>Frame als Byte-Array im konfigurierten Format (PNG/JPEG/etc.)</returns>
    Task<byte[]> GetCurrentFrameAsync();

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
}
```

### HeadlessConfiguration Klasse

```csharp
namespace PlusUi.Headless;

public class HeadlessConfiguration
{
    /// <summary>Frame-Breite in Pixel</summary>
    public int Width { get; set; } = 1280;

    /// <summary>Frame-Höhe in Pixel</summary>
    public int Height { get; set; } = 720;

    /// <summary>Ausgabeformat der Frames</summary>
    public ImageFormat Format { get; set; } = ImageFormat.Png;
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

### IHeadlessAppConfiguration Interface

```csharp
namespace PlusUi.Headless;

/// <summary>
/// Konfigurationsinterface für Headless-Anwendungen.
/// Erweitert das Standard-Pattern um headless-spezifische Konfiguration.
/// </summary>
public interface IHeadlessAppConfiguration
{
    /// <summary>
    /// Konfiguriert die Headless-spezifischen Einstellungen (Frame-Größe, Format).
    /// </summary>
    void ConfigureHeadless(HeadlessConfiguration configuration);

    /// <summary>
    /// Konfiguriert zusätzliche Services und Dependencies.
    /// </summary>
    void ConfigureApp(HostApplicationBuilder builder);

    /// <summary>
    /// Gibt die Root-Page der Anwendung zurück.
    /// </summary>
    UiPageElement GetRootPage(IServiceProvider serviceProvider);
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

### PlusUiHeadlessService

**Verantwortlichkeiten:**
- Frame-Rendering on-demand (kein kontinuierlicher Loop)
- Input-Event-Propagation über bestehenden `InputService`
- Memory-basiertes Rendering (kein GPU/OpenGL erforderlich)
- Format-Konvertierung (SKBitmap → PNG/JPEG bytes)

**Technischer Ansatz:**
```csharp
public class PlusUiHeadlessService : IPlusUiHeadlessService
{
    private readonly RenderService _renderService;
    private readonly InputService _inputService;
    private readonly HeadlessPlatformService _platformService;
    private readonly HeadlessKeyboardHandler _keyboardHandler;

    private Vector2 _currentMousePosition;
    private readonly Size _frameSize;
    private readonly ImageFormat _format;

    public PlusUiHeadlessService(
        RenderService renderService,
        InputService inputService,
        HeadlessPlatformService platformService,
        HeadlessKeyboardHandler keyboardHandler,
        Size frameSize,
        ImageFormat format)
    {
        _renderService = renderService;
        _inputService = inputService;
        _platformService = platformService;
        _keyboardHandler = keyboardHandler;
        _frameSize = frameSize;
        _format = format;
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

    // Private helper für Format-Konvertierung
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
}
```

---

## 5. PlusUiApp & Service Registration

### PlusUiApp Klasse (analog zu Desktop/H.264)

```csharp
namespace PlusUi.Headless;

/// <summary>
/// Entry-Point für Headless PlusUi-Anwendungen.
/// Folgt dem gleichen Pattern wie Desktop und H.264 Plattformen.
/// </summary>
public class PlusUiApp(string[] args)
{
    public void CreateApp(Func<HostApplicationBuilder, IHeadlessAppConfiguration> appBuilder)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // User-Konfiguration holen
        var headlessApp = appBuilder(builder);

        // Headless-Konfiguration via Options-Pattern
        builder.Services.Configure<HeadlessConfiguration>(headlessApp.ConfigureHeadless);

        // Core PlusUi Services registrieren (Navigation, Rendering, etc.)
        var internalApp = new InternalAppWrapper(headlessApp);
        builder.UsePlusUiInternal(internalApp, args);

        // Headless Platform Service
        var config = new HeadlessConfiguration();
        headlessApp.ConfigureHeadless(config);

        var platformService = new HeadlessPlatformService(new Size(config.Width, config.Height));
        builder.Services.AddSingleton<HeadlessPlatformService>(platformService);
        builder.Services.AddSingleton<IPlatformService>(platformService);

        // Headless Keyboard Handler
        var keyboardHandler = new HeadlessKeyboardHandler();
        builder.Services.AddSingleton<HeadlessKeyboardHandler>(keyboardHandler);
        builder.Services.AddSingleton<IKeyboardHandler>(keyboardHandler);

        // Headless Service (zentrale Schnittstelle)
        builder.Services.AddSingleton<IPlusUiHeadlessService>(sp =>
            new PlusUiHeadlessService(
                sp.GetRequiredService<RenderService>(),
                sp.GetRequiredService<InputService>(),
                sp.GetRequiredService<HeadlessPlatformService>(),
                sp.GetRequiredService<HeadlessKeyboardHandler>(),
                new Size(config.Width, config.Height),
                config.Format
            )
        );

        // User-App konfigurieren lassen
        builder.ConfigurePlusUiApp(internalApp);

        var host = builder.Build();

        // Headless läuft NICHT automatisch (kein host.Run())
        // User hat volle Kontrolle über Lifecycle
    }

    public IHost Build(Func<HostApplicationBuilder, IHeadlessAppConfiguration> appBuilder)
    {
        var builder = Host.CreateApplicationBuilder(args);
        var headlessApp = appBuilder(builder);

        // ... gleiche Registrierung wie oben ...

        return builder.Build();
    }
}
```

### InternalAppWrapper (Helper-Klasse)

```csharp
internal class InternalAppWrapper(IHeadlessAppConfiguration headlessApp) : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        // Headless braucht kein Window - no-op
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        headlessApp.ConfigureApp(builder);
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return headlessApp.GetRootPage(serviceProvider);
    }
}
```

### Usage Example

```csharp
// Headless-App Konfiguration
public class MyHeadlessApp : IHeadlessAppConfiguration
{
    public void ConfigureHeadless(HeadlessConfiguration configuration)
    {
        configuration.Width = 1920;
        configuration.Height = 1080;
        configuration.Format = ImageFormat.Png;
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        // Pages und ViewModels registrieren
        builder.AddPage<MainPage>().WithViewModel<MainViewModel>();
        builder.StylePlusUi<MyAppStyles>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}

// Test oder Headless-Nutzung
var app = new PlusUiApp(args);
var host = app.Build(builder => new MyHeadlessApp());

await host.StartAsync();

// Headless-Service holen
var headless = host.Services.GetRequiredService<IPlusUiHeadlessService>();

// Navigation zur Startseite (passiert automatisch durch GetRootPage)
var navigation = host.Services.GetRequiredService<INavigationService>();

// Frame rendern
var frameBytes = await headless.GetCurrentFrameAsync();
File.WriteAllBytes("screenshot.png", frameBytes);

// Input simulieren
headless.MouseMove(100, 200);
headless.MouseDown();
headless.MouseUp();

// Neuen Frame nach Interaktion
frameBytes = await headless.GetCurrentFrameAsync();

await host.StopAsync();
```

---

## 6. Testing-Szenarien

### 6.1 Visual Regression Testing

```csharp
[TestClass]
public class VisualRegressionTests
{
    private IHost? _host;
    private IPlusUiHeadlessService? _headless;

    [TestInitialize]
    public async Task Setup()
    {
        var app = new PlusUiApp(Array.Empty<string>());
        _host = app.Build(builder => new TestHeadlessApp());

        await _host.StartAsync();

        _headless = _host.Services.GetRequiredService<IPlusUiHeadlessService>();
    }
}

// Test-App Konfiguration
public class TestHeadlessApp : IHeadlessAppConfiguration
{
    public void ConfigureHeadless(HeadlessConfiguration configuration)
    {
        configuration.Width = 1280;
        configuration.Height = 720;
        configuration.Format = ImageFormat.Png;
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        builder.AddPage<MainPage>().WithViewModel<MainViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
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
    private readonly IPlusUiHeadlessService _headless;

    public RemoteUiController(IPlusUiHeadlessService headless)
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
│   ├── IPlusUiHeadlessService.cs        ← Zentrales Interface
│   ├── PlusUiHeadlessService.cs         ← Implementation
│   ├── HeadlessPlatformService.cs
│   └── HeadlessKeyboardHandler.cs
├── Enumerations/
│   └── ImageFormat.cs
├── HeadlessConfiguration.cs             ← Konfigurationsklasse
├── IHeadlessAppConfiguration.cs         ← App-Konfig-Interface
├── PlusUiApp.cs                         ← Entry-Point (wie Desktop/H.264)
├── InternalAppWrapper.cs                ← Helper für IAppConfiguration
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

## 11. Implementierungsplan (Phasen)

### Phase 1: Grundstruktur (Minimal Viable Product)
1. PlusUi.Headless Projekt anlegen
2. `ImageFormat` Enum erstellen
3. `HeadlessConfiguration` Klasse erstellen
4. `IHeadlessAppConfiguration` Interface definieren
5. `IPlusUiHeadlessService` Interface definieren
6. `HeadlessPlatformService` implementieren
7. `HeadlessKeyboardHandler` implementieren
8. `PlatformType.Headless` zu Enum hinzufügen

### Phase 2: Core Rendering & App Setup
9. `PlusUiHeadlessService` Grundgerüst (Implementation von `IPlusUiHeadlessService`)
10. `GetCurrentFrameAsync()` mit PNG-Encoding
11. Integration mit bestehendem `RenderService`
12. `PlusUiApp` Klasse erstellen (analog zu Desktop/H.264)
13. `InternalAppWrapper` Helper-Klasse

### Phase 3: Input-System
14. Mouse-Input-Methoden implementieren
15. Keyboard-Input-Methoden implementieren
16. Integration mit bestehendem `InputService`

### Phase 4: Testing & Validation
17. Erstes Test-Projekt erstellen
18. Sample-App für Headless-Tests (mit `IHeadlessAppConfiguration`)
19. Visual Regression Test schreiben
20. Interaction Test schreiben

### Phase 5: Polish & Documentation
21. JPEG/WebP Format-Support
22. Performance-Optimierungen
23. Dokumentation & Beispiele

---

## 12. Zusammenfassung

Die Headless Platform ist ein **schlankes Add-on** zum bestehenden PlusUi-Framework:

✅ **Wiederverwendet bestehende Abstractions** (IPlatformService, IKeyboardHandler, RenderService, InputService)
✅ **Minimaler Code-Aufwand** (~300-400 LOC geschätzt)
✅ **Klare Separation of Concerns**
✅ **On-Demand Rendering** (kein kontinuierlicher Loop)
✅ **Perfekt für Tests und Remote-Szenarien**

**Kern-Konzept**: Die UI wird normal aufgebaut und verwaltet, nur das Rendering geht in den Speicher statt auf ein Display.
