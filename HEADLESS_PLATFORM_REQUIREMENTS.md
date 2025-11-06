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

**Alternative Namen (verworfen):**
- `IHeadlessRenderService` - Zu eingeschränkt (nur Rendering im Namen, aber Interface macht auch Input)
- `IFrameCaptureService` - Zu fokussiert auf Capturing (Input fehlt im Namen)
- `IHeadlessUiService` - Weniger klar in der Zugehörigkeit zu PlusUi

**Design-Entscheidung:**
- **Keine** custom `IHeadlessAppConfiguration` - wir nutzen Standard `IAppConfiguration`!
- **Keine** `HeadlessConfiguration` Klasse - nicht benötigt!
- **Zwei Nutzungsmuster**: `AddPlusUiHeadless()` Extension (Hauptweg) + `PlusUiApp` (optional)
- Frame-Größe kommt aus `PlusUiConfiguration.Size` (wie Desktop Window-Size)
- ImageFormat als einfacher Parameter bei `AddPlusUiHeadless()/Build()`
- **Integration First**: Extension Method für Einbetten in bestehende Apps (Tests, Services)
- Desktop-Pattern: **Maximal einfach, maximale Konsistenz!**

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

## 5. Integration & Service Registration

### Zwei Nutzungs-Szenarien

**Szenario 1: Standalone** (einfache Nutzung, eigener Host)
- Für schnelle Tests, Prototyping, einfache Szenarien
- `PlusUiApp` erstellt eigenen Host

**Szenario 2: Integration** (in bestehende App einbetten)
- Für komplexe Test-Infrastruktur, Web-Services, CI/CD
- Extension Method registriert Services in bestehendem Host

---

### Integration: Extension Method (empfohlen für Tests)

```csharp
namespace PlusUi.Headless.Extensions;

/// <summary>
/// Extension Methods für Integration von PlusUi Headless in bestehende Hosts.
/// </summary>
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Registriert PlusUi Headless in einem bestehenden HostApplicationBuilder.
    /// Für Integration in Test-Infrastruktur, Web-Services, etc.
    /// </summary>
    public static IHostApplicationBuilder AddPlusUiHeadless(
        this IHostApplicationBuilder builder,
        IAppConfiguration appConfiguration,
        ImageFormat format = ImageFormat.Png)
    {
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

        // Headless Service (zentrale Schnittstelle)
        builder.Services.AddSingleton<IPlusUiHeadlessService>(sp =>
            new PlusUiHeadlessService(
                sp.GetRequiredService<RenderService>(),
                sp.GetRequiredService<InputService>(),
                sp.GetRequiredService<HeadlessPlatformService>(),
                sp.GetRequiredService<HeadlessKeyboardHandler>(),
                frameSize,
                format
            )
        );

        // User-App konfigurieren lassen
        builder.ConfigurePlusUiApp(appConfiguration);

        return builder;
    }

    /// <summary>
    /// Registriert PlusUi Headless mit Func-basierter App-Konfiguration.
    /// </summary>
    public static IHostApplicationBuilder AddPlusUiHeadless(
        this IHostApplicationBuilder builder,
        Func<HostApplicationBuilder, IAppConfiguration> appBuilder,
        ImageFormat format = ImageFormat.Png)
    {
        var appConfig = appBuilder(builder);
        return builder.AddPlusUiHeadless(appConfig, format);
    }
}
```

### Integration: Usage Example

```csharp
// In Test-Projekt - Integration in bestehenden Host
public class VisualRegressionTests
{
    private IHost? _host;
    private IPlusUiHeadlessService? _headless;

    [TestInitialize]
    public async Task Setup()
    {
        // Bestehender Builder (z.B. aus Test-Infrastruktur)
        var builder = Host.CreateApplicationBuilder();

        // Weitere Test-Services registrieren
        builder.Services.AddSingleton<ITestLogger, TestLogger>();
        builder.Services.AddSingleton<IScreenshotComparer, ScreenshotComparer>();

        // PlusUi Headless integrieren
        builder.AddPlusUiHeadless(
            appConfiguration: new TestHeadlessApp(),
            format: ImageFormat.Png
        );

        _host = builder.Build();
        await _host.StartAsync();

        _headless = _host.Services.GetRequiredService<IPlusUiHeadlessService>();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        if (_host is not null)
            await _host.StopAsync();
    }
}

// Oder mit Func-basiertem Builder
var builder = Host.CreateApplicationBuilder();
builder.AddPlusUiHeadless(b => new MyHeadlessApp());
var host = builder.Build();
```

### Standalone: PlusUiApp Klasse (für einfache Szenarien)

```csharp
namespace PlusUi.Headless;

/// <summary>
/// Entry-Point für Headless PlusUi-Anwendungen.
/// Folgt dem gleichen Pattern wie Desktop-Plattform.
/// </summary>
public class PlusUiApp(string[] args)
{
    public void CreateApp(
        Func<HostApplicationBuilder, IAppConfiguration> appBuilder,
        ImageFormat format = ImageFormat.Png)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // User-Konfiguration holen (Standard IAppConfiguration!)
        var app = appBuilder(builder);

        // Core PlusUi Services registrieren
        builder.UsePlusUiInternal(app, args);

        // Frame-Größe aus PlusUiConfiguration extrahieren
        var plusUiConfig = new PlusUiConfiguration();
        app.ConfigureWindow(plusUiConfig);
        var frameSize = new Size(plusUiConfig.Size.X, plusUiConfig.Size.Y);

        // Headless Platform Service
        var platformService = new HeadlessPlatformService(frameSize);
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
                frameSize,
                format
            )
        );

        // User-App konfigurieren lassen
        builder.ConfigurePlusUiApp(app);

        var host = builder.Build();

        // Headless läuft NICHT automatisch (kein host.Run())
        // User hat volle Kontrolle über Lifecycle
    }

    public IHost Build(
        Func<HostApplicationBuilder, IAppConfiguration> appBuilder,
        ImageFormat format = ImageFormat.Png)
    {
        var builder = Host.CreateApplicationBuilder(args);
        var app = appBuilder(builder);

        builder.UsePlusUiInternal(app, args);

        var plusUiConfig = new PlusUiConfiguration();
        app.ConfigureWindow(plusUiConfig);
        var frameSize = new Size(plusUiConfig.Size.X, plusUiConfig.Size.Y);

        var platformService = new HeadlessPlatformService(frameSize);
        builder.Services.AddSingleton<HeadlessPlatformService>(platformService);
        builder.Services.AddSingleton<IPlatformService>(platformService);

        var keyboardHandler = new HeadlessKeyboardHandler();
        builder.Services.AddSingleton<HeadlessKeyboardHandler>(keyboardHandler);
        builder.Services.AddSingleton<IKeyboardHandler>(keyboardHandler);

        builder.Services.AddSingleton<IPlusUiHeadlessService>(sp =>
            new PlusUiHeadlessService(
                sp.GetRequiredService<RenderService>(),
                sp.GetRequiredService<InputService>(),
                sp.GetRequiredService<HeadlessPlatformService>(),
                sp.GetRequiredService<HeadlessKeyboardHandler>(),
                frameSize,
                format
            )
        );

        builder.ConfigurePlusUiApp(app);

        return builder.Build();
    }
}
```

### Usage Example (Integration Pattern)

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

// Integration in Test-Framework
[TestClass]
public class MyTests
{
    private IHost? _host;
    private IPlusUiHeadlessService? _headless;

    [TestInitialize]
    public async Task Setup()
    {
        var builder = Host.CreateApplicationBuilder();

        // PlusUi Headless integrieren
        builder.AddPlusUiHeadless(new TestHeadlessApp(), ImageFormat.Png);

        _host = builder.Build();
        await _host.StartAsync();

        _headless = _host.Services.GetRequiredService<IPlusUiHeadlessService>();
    }

    [TestCleanup]

**Empfehlung:** Für Tests und komplexe Szenarien → `AddPlusUiHeadless()` Extension
Für schnelle Prototypen → `PlusUiApp`

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
        var builder = Host.CreateApplicationBuilder();

        // PlusUi Headless integrieren
        builder.AddPlusUiHeadless(new TestHeadlessApp());

        _host = builder.Build();
        await _host.StartAsync();

        _headless = _host.Services.GetRequiredService<IPlusUiHeadlessService>();
    }
}

// Test-App Konfiguration (Standard IAppConfiguration!)
public class TestHeadlessApp : IAppConfiguration
{
    public void ConfigureWindow(PlusUiConfiguration configuration)
    {
        configuration.Size = new SizeI(1280, 720);
        // Andere Properties ignoriert (Title, WindowState, etc.)
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
├── Extensions/
│   └── HostApplicationBuilderExtensions.cs  ← AddPlusUiHeadless() - HAUPTWEG
├── Enumerations/
│   └── ImageFormat.cs
├── PlusUiApp.cs                         ← Standalone Entry-Point (optional)
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
3. `IPlusUiHeadlessService` Interface definieren
4. `HeadlessPlatformService` implementieren
5. `HeadlessKeyboardHandler` implementieren
6. `PlatformType.Headless` zu Enum hinzufügen

### Phase 2: Core Rendering & App Setup
7. `PlusUiHeadlessService` Grundgerüst (Implementation von `IPlusUiHeadlessService`)
8. `GetCurrentFrameAsync()` mit PNG-Encoding
9. Integration mit bestehendem `RenderService`
10. **`AddPlusUiHeadless()` Extension Method** (Integration-Pattern - Hauptweg!)
11. `PlusUiApp` Klasse erstellen (Standalone-Pattern - optional)

### Phase 3: Input-System
12. Mouse-Input-Methoden implementieren
13. Keyboard-Input-Methoden implementieren
14. Integration mit bestehendem `InputService`

### Phase 4: Testing & Validation
15. Erstes Test-Projekt erstellen (mit `AddPlusUiHeadless()`)
16. Sample-App für Headless-Tests (mit Standard `IAppConfiguration`)
17. Visual Regression Test schreiben
18. Interaction Test schreiben

### Phase 5: Polish & Documentation
19. JPEG/WebP Format-Support
20. Performance-Optimierungen
21. Dokumentation & Beispiele

---

## 12. Zusammenfassung

Die Headless Platform ist ein **schlankes Add-on** zum bestehenden PlusUi-Framework:

✅ **Wiederverwendet bestehende Abstractions** (IPlatformService, IKeyboardHandler, RenderService, InputService)
✅ **Minimaler Code-Aufwand** (~300-400 LOC geschätzt)
✅ **Klare Separation of Concerns**
✅ **On-Demand Rendering** (kein kontinuierlicher Loop)
✅ **Perfekt für Tests und Remote-Szenarien**

**Kern-Konzept**: Die UI wird normal aufgebaut und verwaltet, nur das Rendering geht in den Speicher statt auf ein Display.
