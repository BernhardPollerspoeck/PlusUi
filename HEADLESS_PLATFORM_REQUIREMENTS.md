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
- ✅ Nutzt Standard `IAppConfiguration` wie Desktop (maximale Konsistenz!)

**Design-Entscheidung:**
- **Keine** custom `IHeadlessAppConfiguration` - wir nutzen Standard `IAppConfiguration`!
- **Keine** `HeadlessConfiguration` Klasse - nicht benötigt!
- **Factory-Pattern**: `PlusUiHeadless.Create()` - einfache statische Factory-Methode
- Frame-Größe kommt aus `PlusUiConfiguration.Size` (wie Desktop Window-Size)
- ImageFormat als optionaler Parameter bei `Create()`
- **Vollständige Isolation**: Eigener interner ServiceProvider - keine Host-Integration nötig
- **IDisposable**: Sauberes Cleanup des internen Hosts
- **Maximal einfach**: Eine Zeile zum Erstellen, funktioniert überall!

---

## Architektur-Übersicht

```
┌─────────────────────────────────────────────────────┐
│                Test/Client Code                      │
│                                                      │
│  using var headless = PlusUiHeadless.Create(config);│
│  var frame = await headless.GetCurrentFrameAsync();│
│  headless.MouseMove(x, y);                         │
└────────────────┬────────────────────────────────────┘
                 │
                 │ PlusUiHeadless.Create()
                 ▼
┌─────────────────────────────────────────────────────┐
│        PlusUiHeadless (Factory - static)             │
│  + Create(IAppConfiguration, ImageFormat)           │
│                                                      │
│  1. Erstellt Host.CreateApplicationBuilder()       │
│  2. Registriert alle Services                       │
│  3. Baut und startet Host                          │
│  4. Gibt PlusUiHeadlessWrapper zurück              │
└────────────────┬────────────────────────────────────┘
                 │
                 │ returns
                 ▼
┌─────────────────────────────────────────────────────┐
│  PlusUiHeadlessWrapper (IDisposable)                │
│  implements IPlusUiHeadlessService                  │
│                                                      │
│  - Hält Referenz zum internen Host                 │
│  - Delegiert Calls an PlusUiHeadlessService        │
│  - Dispose() stoppt und disposed Host              │
└────────────────┬────────────────────────────────────┘
                 │
                 │ delegates to
                 ▼
┌─────────────────────────────────────────────────────┐
│     PlusUiHeadlessService (internal)                │
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
│  - WindowSize = Frame-Size                          │
│  - DisplayDensity = 1.0 (default)                   │
│  - OpenUrl() = false (nicht interaktiv)             │
└─────────────────────────────────────────────────────┘

Alles läuft in eigenem isolierten ServiceProvider!
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

### PlusUiHeadlessService

**Verantwortlichkeiten:**
- Frame-Rendering on-demand (kein kontinuierlicher Loop)
- Input-Event-Propagation über bestehenden `InputService`
- Memory-basiertes Rendering (kein GPU/OpenGL erforderlich)
- Format-Konvertierung (SKBitmap → PNG/JPEG bytes)

**Technischer Ansatz:**
```csharp
internal class PlusUiHeadlessService
{
    private readonly RenderService _renderService;
    private readonly InputService _inputService;
    private readonly HeadlessPlatformService _platformService;
    private readonly HeadlessKeyboardHandler _keyboardHandler;

    private Vector2 _currentMousePosition;
    private Size _frameSize;
    private ImageFormat _format;

    public PlusUiHeadlessService(
        RenderService renderService,
        InputService inputService,
        HeadlessPlatformService platformService,
        HeadlessKeyboardHandler keyboardHandler)
    {
        _renderService = renderService;
        _inputService = inputService;
        _platformService = platformService;
        _keyboardHandler = keyboardHandler;
    }

    // Called by wrapper after construction
    internal void Initialize(Size frameSize, ImageFormat format)
    {
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

## 5. Factory-Pattern & Service Registration

### PlusUiHeadless Factory-Klasse

Die zentrale Factory-Klasse erstellt eine vollständig isolierte Headless-Instanz mit eigenem internen ServiceProvider.

```csharp
namespace PlusUi.Headless;

/// <summary>
/// Factory-Klasse für Erstellung von isolierten PlusUi Headless-Instanzen.
/// Jede Instanz hat ihren eigenen internen ServiceProvider und ist komplett isoliert.
/// </summary>
public static class PlusUiHeadless
{
    /// <summary>
    /// Erstellt eine neue isolierte Headless-Instanz.
    /// </summary>
    /// <param name="appConfiguration">App-Konfiguration (Standard IAppConfiguration)</param>
    /// <param name="format">Ausgabeformat der Frames (Standard: PNG)</param>
    /// <returns>Headless-Instanz mit eigenem isolierten ServiceProvider</returns>
    public static IPlusUiHeadlessService Create(
        IAppConfiguration appConfiguration,
        ImageFormat format = ImageFormat.Png)
    {
        // Internen Host erstellen
        var builder = Host.CreateApplicationBuilder();

        // Core PlusUi Services registrieren
        builder.UsePlusUiInternal(appConfiguration, args: null);

        // Frame-Größe aus PlusUiConfiguration extrahieren
        var plusUiConfig = new PlusUiConfiguration();
        appConfiguration.ConfigureWindow(plusUiConfig);
        var frameSize = new Size(plusUiConfig.Size.X, plusUiConfig.Size.Y);

        // Headless Platform Service
        var platformService = new HeadlessPlatformService(frameSize);
        builder.Services.AddSingleton<HeadlessPlatformService>(platformService);
        builder.Services.AddSingleton<IPlatformService>(platformService);

        // Headless Keyboard Handler
        var keyboardHandler = new HeadlessKeyboardHandler();
        builder.Services.AddSingleton<HeadlessKeyboardHandler>(keyboardHandler);
        builder.Services.AddSingleton<IKeyboardHandler>(keyboardHandler);

        // Headless Service Implementation (intern)
        builder.Services.AddSingleton<PlusUiHeadlessService>();

        // User-App konfigurieren lassen
        builder.ConfigurePlusUiApp(appConfiguration);

        // Host bauen und starten
        var host = builder.Build();
        host.Start(); // Synchron starten

        // Wrapper zurückgeben der IPlusUiHeadlessService implementiert
        // und intern den Host managed
        var headlessService = host.Services.GetRequiredService<PlusUiHeadlessService>();
        return new PlusUiHeadlessWrapper(host, headlessService, frameSize, format);
    }
}

/// <summary>
/// Wrapper-Klasse die IPlusUiHeadlessService implementiert und den internen Host managed.
/// </summary>
internal class PlusUiHeadlessWrapper : IPlusUiHeadlessService, IDisposable
{
    private readonly IHost _host;
    private readonly PlusUiHeadlessService _headlessService;

    internal PlusUiHeadlessWrapper(
        IHost host,
        PlusUiHeadlessService headlessService,
        Size frameSize,
        ImageFormat format)
    {
        _host = host;
        _headlessService = headlessService;
        // Initialize with frame size and format
        _headlessService.Initialize(frameSize, format);
    }

    public Task<byte[]> GetCurrentFrameAsync() => _headlessService.GetCurrentFrameAsync();
    public void MouseMove(float x, float y) => _headlessService.MouseMove(x, y);
    public void MouseDown() => _headlessService.MouseDown();
    public void MouseUp() => _headlessService.MouseUp();
    public void MouseWheel(float deltaX, float deltaY) => _headlessService.MouseWheel(deltaX, deltaY);
    public void KeyPress(PlusKey key) => _headlessService.KeyPress(key);
    public void CharInput(char c) => _headlessService.CharInput(c);

    public void Dispose()
    {
        _host?.StopAsync().GetAwaiter().GetResult();
        _host?.Dispose();
    }
}
```

### PlusUiHeadlessService Implementation (angepasst)

---

### Usage Example (Factory-Pattern)

```csharp
// Test-App Konfiguration (Standard IAppConfiguration!)
public class TestHeadlessApp : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Size = new SizeI(1280, 720);
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        builder.AddPage<MainPage>().WithViewModel<MainViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
    }
}

// Einfache Nutzung - eine Zeile!
using var headless = PlusUiHeadless.Create(new TestHeadlessApp());

// Frame rendern
var frame = await headless.GetCurrentFrameAsync();
File.WriteAllBytes("screenshot.png", frame);

// Input simulieren
headless.MouseMove(640, 360);
headless.MouseDown();
headless.MouseUp();

// Cleanup automatisch via using

---

## 6. Testing-Szenarien

### 6.1 Visual Regression Testing

```csharp
[TestClass]
public class VisualRegressionTests
{
    private IPlusUiHeadlessService? _headless;

    [TestInitialize]
    public void Setup()
    {
        // Einfache Factory-Erstellung
        _headless = PlusUiHeadless.Create(new TestHeadlessApp());
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Dispose räumt internen Host auf
        (_headless as IDisposable)?.Dispose();
    }
}

// Test-App Konfiguration (Standard IAppConfiguration!)
public class TestHeadlessApp : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Size = new SizeI(1280, 720);
    }

    public void ConfigureApp(HostApplicationBuilder builder)
    {
        builder.AddPage<MainPage>().WithViewModel<MainViewModel>();
    }

    public UiPageElement GetRootPage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainPage>();
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

---

## 7. Projektstruktur

```
PlusUi.Headless/
├── Services/
