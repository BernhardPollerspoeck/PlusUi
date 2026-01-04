# PlusUi Debug Bridge - Implementation Plan

## Übersicht

Eine Debug Bridge für PlusUi, die per `builder.EnableDebugBridge("192.168.1.100", 5555)` konfiguriert wird. Ermöglicht Echtzeit-Inspektion, Performance-Monitoring und Remote-Property-Editing via WebSocket.

**ARCHITEKTUR (UMGEKEHRT!):**
- **Debug Desktop App** = WebSocket **SERVER** (läuft auf Entwickler-PC)
- **Debugged Apps** (Mobile/Web/Desktop) = WebSocket **CLIENTS** (connecten zu Debug Server)
- **Vorteil:** Mobile-Geräte müssen keinen Server öffnen (NAT/Firewall-Probleme)

## Architektur

```
Debug Desktop App (WebSocket SERVER)          Debugged Apps (WS CLIENTS)
├─ DebugBridgeServer (Port 5555)              ├─ DebugBridgeClient
├─ TreeInspectorPage (TreeView)               ├─ DebugTreeInspector
├─ PerformanceGraphControl (Custom)           ├─ DebugPerformanceMonitor
├─ LogViewerPage                              ├─ DebugLogCollector
└─ PropertyEditorPage                         └─ DebugInvalidationTracker
         ▲
         │ WebSocket Connection
         │
    Mobile/Web/Desktop App connects
```

**Technologie:**
- `System.Net.WebSockets` (cross-platform, built-in)
- JSON-Serialization mit `System.Text.Json`
- Reflection für Property-Zugriff
- **Runtime-Configuration** (kein `#if DEBUG`!)

## Hauptkomponenten

### 1. DebugBridgeClient (in debugged app)
**Datei:** `source/PlusUi.core/Services/Debug/DebugBridgeClient.cs`

- WebSocket-**CLIENT** der zu Debug-Server connectet
- Nutzt `ClientWebSocket`
- Auto-Reconnect bei Connection-Loss
- Sendet UI-Tree, Performance-Daten, Logs an Server
- Empfängt Property-Set-Commands vom Server

**Kernmethoden:**
- `ConnectAsync(string serverUrl)` - Verbindet zu Debug Server
- `SendAsync(DebugMessage)` - Sendet Daten an Server
- `HandleCommandAsync(DebugMessage)` - Verarbeitet Commands (Property-Set)

### 2. DebugBridgeServer (in Debug Desktop App)
**Datei:** `source/PlusUi.DebugClient/Services/DebugBridgeServer.cs`

- WebSocket-**SERVER** auf Port 5555
- Nutzt `HttpListener` + `AcceptWebSocketAsync()`
- Multi-Client-Support (mehrere Apps gleichzeitig debuggen)
- Empfängt UI-Tree, Performance, Logs von Clients
- Sendet Property-Set-Commands an Clients

**Kernmethoden:**
- `Start()` - Startet Server auf Port 5555
- `BroadcastToClientAsync(string clientId, DebugMessage)` - Sendet an spezifischen Client
- `HandleClientMessageAsync(string clientId, DebugMessage)` - Verarbeitet Client-Daten

### 3. DebugTreeInspector
**Datei:** `source/PlusUi.core/Services/Debug/DebugTreeInspector.cs`

- Traversiert UI-Tree via **neues Interface `IDebugInspectable`** (internal)
- Extrahiert Properties via Reflection (`BindingFlags.Public | NonPublic | Instance`)
- Element-ID-Mapping für Referenzen (`Dictionary<UiElement, string>`)
- Type-aware Serialization (SKColor → Hex, Enums → String)

**Neues Interface:**
```csharp
// In PlusUi.core/Services/Debug/IDebugInspectable.cs
internal interface IDebugInspectable
{
    IEnumerable<UiElement> GetDebugChildren();
}
```

**Implementation:**
- `UiLayoutElement` implementiert: `return Children;`
- `UiPageElement` implementiert: `return [ContentTree];`
- Andere Controls: `return [];`

**Kernmethoden:**
- `SerializeTree()` - Root-to-Leaf Traversierung via IDebugInspectable
- `ExtractProperties(UiElement)` - Reflection-basierte Property-Extraktion
- `GetElementDetails(string id)` - Details für einzelnes Element

### 4. DebugPerformanceMonitor
**Datei:** `source/PlusUi.core/Services/Debug/DebugPerformanceMonitor.cs`

- Implementiert `IAppMonitor` Interface
- Sammelt Frame-Timing, Measure/Arrange/Render-Zeiten
- Sendet Performance-Daten via `DebugBridgeClient` an Server
- Batched Sending (alle 10 Frames, um Flooding zu vermeiden)
- Thread-safe mit `lock`

**Integration:**
- Wird in `RenderService` Constructor injected (bereits vorbereitet!)
- `RenderService.cs:9` akzeptiert `IAppMonitor? appMonitor`
- Registrierung via DI: `builder.Services.AddSingleton<IAppMonitor>(...)`

### 5. DebugLogCollector
**Datei:** `source/PlusUi.core/Services/Debug/DebugLogCollector.cs`

- Implementiert `ILogger` Interface (Decorator-Pattern)
- Wraps existierende Logger
- Sammelt Logs und sendet via `DebugBridgeClient` an Server
- `ILoggerProvider` für DI-Integration

### 6. DebugInvalidationTracker
**Datei:** `source/PlusUi.core/Services/Debug/DebugInvalidationTracker.cs`

- Hooks in `InvalidationTracker.RequestRender()` via Event
- Captured Stack Traces (mit Throttling: max 10/sec)
- Tracking welche Elemente invalidieren
- Sendet Events via `DebugBridgeClient` an Server

**Integration:**
- **Kleine Änderung nötig:** Event in `InvalidationTracker.cs:52` hinzufügen
- `public event EventHandler<string>? RenderRequested;`
- `RenderRequested?.Invoke(this, reason ?? "Unknown");` nach Line 54

### 7. DebugPropertyReflector
**Datei:** `source/PlusUi.core/Services/Debug/DebugPropertyReflector.cs`

- **Läuft im debugged app** (empfängt Commands vom Server)
- Setzt Properties via Reflection
- Type-Conversion: `SKColor.Parse()`, `Enum.Parse()`, `Convert.ChangeType()`
- Triggert `InvalidateMeasure()` nach Property-Change
- Element-Cache für schnelle Lookup

## Datenmodelle

**Datei:** `source/PlusUi.core/Services/Debug/Models/`

Alle Models in separate Files:

```csharp
// DebugMessage.cs - Base Message
public class DebugMessage
{
    public string Type { get; set; }  // "ui_tree", "performance_frame", "log_entry", etc.
    public object? Data { get; set; }
}

// TreeNodeDto.cs
public class TreeNodeDto
{
    public string Id { get; set; }
    public string Type { get; set; }
    public bool IsVisible { get; set; }
    public List<PropertyDto> Properties { get; set; }
    public List<TreeNodeDto> Children { get; set; }
}

// PropertyDto.cs
public class PropertyDto
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public bool CanWrite { get; set; }
    public bool IsInternal { get; set; }
}

// PerformanceFrameDto.cs
public class PerformanceFrameDto
{
    public DateTimeOffset Timestamp { get; set; }
    public double FrameTimeMs { get; set; }
    public double Fps { get; set; }
    public double MeasureTimeMs { get; set; }
    public double ArrangeTimeMs { get; set; }
    public double RenderTimeMs { get; set; }
    public int MeasureSkipped { get; set; }
    public int ArrangeSkipped { get; set; }
    public int PropertySkipped { get; set; }
}

// LogEntryDto.cs
public class LogEntryDto
{
    public DateTimeOffset Timestamp { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string? Exception { get; set; }
}

// InvalidationEventDto.cs
public class InvalidationEventDto
{
    public DateTimeOffset Timestamp { get; set; }
    public string Reason { get; set; }
    public string StackTrace { get; set; }
}

// PropertySetRequestDto.cs
public class PropertySetRequestDto
{
    public string ElementId { get; set; }
    public string PropertyName { get; set; }
    public string Value { get; set; }
}
```

## Fluent API Extension (für debugged apps)

**Datei:** `source/PlusUi.core/Extensions/DebugBridgeExtensions.cs`

```csharp
public static class DebugBridgeExtensions
{
    /// <summary>
    /// Enables debug bridge client that connects to a debug server.
    /// Example: builder.EnableDebugBridge("192.168.1.100", 5555)
    /// Only enabled when host is provided - minimal overhead when disabled.
    /// </summary>
    public static IPlusUiAppBuilder EnableDebugBridge(
        this IPlusUiAppBuilder builder,
        string? host = null,
        int port = 5555)
    {
        // Disabled if host not provided
        if (string.IsNullOrEmpty(host))
            return builder;

        // PFLICHT: Environment check (nur in Development aktivieren)
        // Standard: Host Environment checken
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                         ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (environment != "Development")
            return builder;

        var serverUrl = $"ws://{host}:{port}";

        // Register DebugBridgeClient singleton (nullable!)
        builder.Services.AddSingleton<DebugBridgeClient?>(sp => {
            var navContainer = sp.GetRequiredService<NavigationContainer>();
            var invalidationTracker = sp.GetRequiredService<InvalidationTracker>();
            var logger = sp.GetService<ILogger<DebugBridgeClient>>();

            var client = new DebugBridgeClient(serverUrl, navContainer, invalidationTracker, logger);
            _ = client.ConnectAsync(); // Fire-and-forget connect
            return client;
        });

        // Register DebugPerformanceMonitor as IAppMonitor (nullable!)
        builder.Services.AddSingleton<IAppMonitor?>(sp => {
            var client = sp.GetService<DebugBridgeClient>();
            return client != null ? new DebugPerformanceMonitor(client) : null;
        });

        // Register DebugLogCollector as ILoggerProvider (nullable!)
        builder.Services.AddSingleton<ILoggerProvider?>(sp => {
            var client = sp.GetService<DebugBridgeClient>();
            return client != null ? new DebugLogCollectorProvider(client) : null;
        });

        return builder;
    }
}
```

**GLOBAL, nicht per-Page!** Builder-Extension ist app-weit, nicht page-spezifisch.

**WICHTIG - KEIN `#if DEBUG`!**
- PlusUi Framework wird immer als **RELEASE** ausgeliefert (NuGet-Package)
- `#if DEBUG` im PlusUi-Code wäre **immer FALSE** (Framework = Release)
- User-App kann DEBUG oder PROD sein, aber PlusUi sieht das nicht via `#if DEBUG`

**Stattdessen:**
- **Disabled by default** (host = null)
- **PFLICHT: Environment-Check** - Nur in Development (ASPNETCORE_ENVIRONMENT / DOTNET_ENVIRONMENT)
- **Nullable Services** (`IAppMonitor?`, `ILoggerProvider?`, `DebugBridgeClient?`)
- **Host und Port getrennt** - User muss nicht "ws://" Protocol kennen

**Zero-Overhead wenn disabled:**
- Extension wird nicht aufgerufen (User omittet `.EnableDebugBridge()`)
- Oder Extension returned sofort bei `host == null`
- Keine Services registriert, kein Memory

**Pattern folgt:** `PlusUiAppBuilderExtensions.cs` (Lines 13-58)

## Debug Server Desktop Application

**Neues Projekt:** `source/PlusUi.DebugServer/` (Desktop-App)

```
PlusUi.DebugServer.csproj           # .NET 10.0 Desktop
Program.cs                          # Entry point
App.cs                              # PlusUi app config
Services/
├── DebugBridgeServer.cs            # WebSocket SERVER (Port 5555)
├── DebugDataStore.cs               # Data binding store
└── DebugClientManager.cs           # Multi-client tracking
Controls/
└── PerformanceGraphControl.cs      # Custom Graph-Control (PlusUi-based)
Pages/
├── MainPage.cs                     # Main layout mit Tabs
├── TreeInspectorPage.cs            # TreeView mit Properties
├── PerformancePage.cs              # Custom PerformanceGraphControl
└── LogViewerPage.cs                # Log entries
ViewModels/
├── MainViewModel.cs                # Connected clients, selection
├── TreeInspectorViewModel.cs
├── PerformanceViewModel.cs
└── LogViewerViewModel.cs
```

**DebugBridgeServer Key Features:**
- `HttpListener` + `AcceptWebSocketAsync()` für WS-Server
- Multi-Client-Support (mehrere Apps gleichzeitig)
- Event-basiert: `ClientMessageReceived` Event für UI-Updates
- `SendToClientAsync(clientId, DebugMessage)` für Commands

**UI:**
- **Dark Mode** wie Visual Studio (schwarzer Hintergrund, grau/blau Accents)
- Client-Selector (Dropdown/Tabs für mehrere verbundene Apps)
- TreeView für UI-Hierarchy (verwendet PlusUi TreeView Control!)
- **PerformanceGraphControl** (Custom PlusUi UserControl mit Canvas-Rendering)
  - Line-Graph für FPS über Zeit
  - Bar-Chart für Measure/Arrange/Render Times
  - Echtzeit-Update (60 FPS Data-Points)
- Log Table mit Filtering (LogLevel, Search)
- Property Inspector mit Edit-Capability (sendet Set-Commands an Client)

## Implementation Phasen

### Phase 1: Core Infrastructure (Prio 1)
**Dateien zu erstellen:**
1. `source/PlusUi.core/Services/Debug/Models/*.cs` - Alle DTOs
2. `source/PlusUi.core/Services/Debug/IDebugInspectable.cs` - Interface für Tree-Traversal
3. `source/PlusUi.core/Services/Debug/DebugBridgeClient.cs` - Client (läuft in debugged app)
4. `source/PlusUi.core/Extensions/DebugBridgeExtensions.cs` - Fluent API
5. `source/PlusUi.DebugServer/Services/DebugBridgeServer.cs` - Server (Desktop-App)

**Änderungen:**
- `InvalidationTracker.cs`: Event hinzufügen (Lines 52-54)
- `UiLayoutElement.cs`: `IDebugInspectable` implementieren
- `UiPageElement.cs`: `IDebugInspectable` implementieren

**Tests:**
- Server startet auf Port 5555 (Desktop-App)
- Client connectet von Sandbox-App
- Grundlegendes Message-Routing funktioniert

### Phase 2: Tree Inspection (Prio 1)
**Dateien zu erstellen:**
6. `source/PlusUi.core/Services/Debug/DebugTreeInspector.cs`

**Tests:**
- Sandbox-App Tree wird serialized
- Alle Properties werden extrahiert
- IDebugInspectable.GetDebugChildren() korrekt traversiert
- Desktop-App empfängt und zeigt Tree

### Phase 3: Performance Monitoring (Prio 1)
**Dateien zu erstellen:**
7. `source/PlusUi.core/Services/Debug/DebugPerformanceMonitor.cs`
8. `source/PlusUi.DebugServer/Controls/PerformanceGraphControl.cs` - Custom Graph

**Tests:**
- IAppMonitor wird in RenderService injected
- Frame-Timing wird gesammelt
- Daten werden an Debug-Server gesendet (alle 10 Frames)
- PerformanceGraphControl zeigt Live-Graph

### Phase 4: Logging & Invalidation (Prio 2)
**Dateien zu erstellen:**
9. `source/PlusUi.core/Services/Debug/DebugLogCollector.cs`
10. `source/PlusUi.core/Services/Debug/DebugInvalidationTracker.cs`

**Tests:**
- Logs werden gesammelt und an Server gesendet
- Invalidation Events mit Stack Traces
- Throttling funktioniert
- Desktop-App zeigt Logs in Echtzeit

### Phase 5: Property Editing (Prio 2)
**Dateien zu erstellen:**
11. `source/PlusUi.core/Services/Debug/DebugPropertyReflector.cs`

**Tests:**
- Desktop-App sendet Property-Set-Command
- Client empfängt und setzt Property via Reflection
- Type-Conversion (SKColor, Enums) funktioniert
- InvalidateMeasure wird getriggert
- UI updated sichtbar

### Phase 6: Debug Server UI Polish (Prio 1)
**Dateien zu erstellen:**
12. Alle Pages und ViewModels in `PlusUi.DebugServer/`
13. Dark Mode Styling (Visual Studio Theme)

**Tests:**
- Multi-Client-Support (mehrere Apps gleichzeitig)
- Client-Switcher UI funktioniert
- UI Tree wird angezeigt (TreeView)
- Performance Charts zeigen Live-Daten
- Logs werden angezeigt mit Filtering
- Property-Editing funktioniert
- Dark Mode sieht gut aus

### Phase 7: Polish & Testing (Prio 3)
- End-to-End Tests mit Sandbox
- Performance-Profiling
- Dokumentation (XML Comments)
- README für Debug Client

## Kritische Files (in Reihenfolge)

1. **source/PlusUi.core/Services/Debug/IDebugInspectable.cs** - Interface für Tree-Traversal
2. **source/PlusUi.core/Services/Debug/DebugBridgeClient.cs** - WebSocket Client (in app)
3. **source/PlusUi.DebugServer/Services/DebugBridgeServer.cs** - WebSocket Server (Desktop)
4. **source/PlusUi.core/Extensions/DebugBridgeExtensions.cs** - Fluent API Entry Point
5. **source/PlusUi.core/Services/Debug/DebugTreeInspector.cs** - UI Tree Serialization
6. **source/PlusUi.core/Services/Debug/DebugPerformanceMonitor.cs** - IAppMonitor Implementation
7. **source/PlusUi.DebugServer/Controls/PerformanceGraphControl.cs** - Custom Graph Control

## Integration Points

### 1. IDebugInspectable Interface
**File:** `source/PlusUi.core/Services/Debug/IDebugInspectable.cs` (NEU)

```csharp
namespace PlusUi.core.Services.Debug;

/// <summary>
/// Internal interface for debug tree traversal.
/// Controls implement this to expose their children for debugging.
/// </summary>
internal interface IDebugInspectable
{
    /// <summary>
    /// Returns all children for debug inspection.
    /// </summary>
    IEnumerable<UiElement> GetDebugChildren();
}
```

### 2. UiLayoutElement Implementation
**File:** `source/PlusUi.core/CoreElements/UiLayoutElement.cs`

```csharp
public abstract partial class UiLayoutElement : UiElement, IDebugInspectable
{
    // ... existing code ...

    // Add:
    IEnumerable<UiElement> IDebugInspectable.GetDebugChildren() => Children;
}
```

### 3. UiPageElement Implementation
**File:** `source/PlusUi.core/CoreElements/UiPageElement.cs`

```csharp
public abstract partial class UiPageElement : UiLayoutElement, IDebugInspectable
{
    // ... existing code ...

    // Add (override von UiLayoutElement):
    IEnumerable<UiElement> IDebugInspectable.GetDebugChildren() => [ContentTree];
}
```

### 4. InvalidationTracker Enhancement
**File:** `source/PlusUi.core/Services/Rendering/InvalidationTracker.cs`

```csharp
// Add after line 43 (inside class):
public event EventHandler<string>? RenderRequested;

// Modify line 52-54 in RequestRender():
public void RequestRender([CallerMemberName] string? reason = null)
{
    _logger?.LogTrace("Manual render requested: {Reason}", reason);

    RenderRequested?.Invoke(this, reason ?? "Unknown");

    var wasNeeded = NeedsRendering;
    // ... rest unchanged
}
```

**KEIN `#if DEBUG`!** Event ist immer da, wird aber nur genutzt wenn Debug-Client enabled.

### 5. RenderService IAppMonitor
**File:** `source/PlusUi.core/Services/RenderService.cs`

**Keine Änderung nötig!** Constructor akzeptiert bereits `IAppMonitor? appMonitor` (Line 9)
- Timing-Calls bereits implementiert (Lines 15, 28, 43, 61, 94-97)
- **Nullable ist perfekt:** Im RELEASE ist `appMonitor` einfach `null`, kein Overhead
- Nur DI-Registration in Extension nötig

## Message Protocol

### Server → Client Messages

| Type | Data Type | Beschreibung |
|------|-----------|--------------|
| `ui_tree` | `TreeNodeDto` | Vollständiger UI Tree |
| `element_details` | `TreeNodeDto` | Einzelnes Element Details |
| `performance_frame` | `PerformanceFrameDto` | Frame Timing |
| `log_entry` | `LogEntryDto` | Log Message |
| `invalidation` | `InvalidationEventDto` | Invalidation Event |

### Client → Server Messages

| Type | Data Type | Beschreibung |
|------|-----------|--------------|
| `get_tree` | - | Tree Refresh anfordern |
| `get_element_details` | `string` (elementId) | Element-Details |
| `set_property` | `PropertySetRequestDto` | Property ändern |

## Usage Example

### Debugged App (z.B. Sandbox, Mobile App)

```csharp
// In App.cs or Program.cs
var builder = PlusUiApp.CreateBuilder(args);

builder
    .AddPage<MainPage>()
    .WithViewModel<MainViewModel>()
    .EnableDebugBridge("192.168.1.100", 5555);  // Connect to Debug Server
    // .EnableDebugBridge();  // Disabled (null or omit host)

var app = builder.Build();
app.Run();
```

**Wenn disabled (Extension nicht aufgerufen):** Zero memory overhead.

**Environment-Check (automatisch in Extension):**
```csharp
// App.cs - Einfach enablen, Extension checkt Environment
builder.EnableDebugBridge("192.168.1.100", 5555);

// Extension prüft automatisch:
// - Nur wenn ASPNETCORE_ENVIRONMENT="Development" → aktiviert
// - Sonst: sofortiger return, keine Services registriert
```

**Production-Safety:**
- User kann `.EnableDebugBridge(...)` im Code lassen
- Extension aktiviert NUR in Development (Environment-Check)
- In Production: Sofortiger return, zero overhead

### Debug Server App

```csharp
// In PlusUi.DebugServer/App.cs
var builder = PlusUiApp.CreateBuilder(args);

builder
    .AddPage<MainPage>()
    .WithViewModel<MainViewModel>()
    .StylePlusUi<DarkThemeStyle>();  // Visual Studio Dark Mode

var app = builder.Build();
app.Run();

// Server startet automatisch auf Port 5555 beim App-Start
```

**Multi-Device Workflow:**
1. Start Debug Server App auf PC (läuft auf `192.168.1.100:5555`)
2. Start Sandbox App mit `.EnableDebugBridge("192.168.1.100", 5555)`
3. Start Mobile App mit `.EnableDebugBridge("192.168.1.100", 5555)`
4. Debug Server zeigt beide Clients, kann zwischen ihnen switchen
5. Live-Inspection, Performance-Monitoring, Property-Editing für beide Apps gleichzeitig

## Performance Considerations

**WICHTIG:** PlusUi Framework wird IMMER als RELEASE ausgeliefert (NuGet-Package)!

**Disabled (Extension nicht aufgerufen):** **ZERO Overhead**
- User omittet `.EnableDebugBridge()` komplett
- Keine Services registriert
- Keine DLL-Größen-Erhöhung durch Services (sind internal, tree-shaked wenn nicht genutzt)
- **ZERO Runtime-Overhead**

**Enabled aber host = null:** Zero Runtime Overhead
- Extension returned sofort nach null-Check
- Keine Services registriert
- Geschätzt: <0.001ms overhead (nur null-Check)

**Enabled mit host, Not Connected:** Low Overhead
- WebSocket versucht zu connecten (async, non-blocking)
- IAppMonitor-Calls führen zu Quick-Returns (kein connected Client)
- Services sind registriert aber inaktiv
- Geschätzt: <0.1ms pro Frame

**Enabled mit host, Connected:** Moderate Overhead
- Frame Timing: ~0.05ms (Stopwatch)
- Tree Serialization (on-demand): ~5-10ms für 1000 Elemente
- Stack Traces (throttled): ~1-2ms pro Capture
- WebSocket Send: non-blocking (async)
- Geschätzt: ~0.5-1ms pro Frame

**Optimierungen:**
- Serialization auf Background-Thread (nicht UI-Thread blockieren)
- Batched Sending (alle 10 Frames, nicht 60)
- Lazy Stack Trace Capture (nur wenn Server requests)
- Element-ID Caching (Dictionary)
- Auto-Reconnect mit Exponential Backoff (nicht spammen bei Connection Loss)

## Runtime Configuration Strategy

**KRITISCH:** PlusUi Framework wird IMMER als RELEASE ausgeliefert (NuGet-Package)!
- **KEIN `#if DEBUG` im PlusUi-Code!** (wäre immer FALSE, da Framework = Release)
- User-App kann DEBUG oder PROD sein, aber PlusUi sieht das nicht via `#if DEBUG`

**Stattdessen: Runtime-Configuration**

**Extension (DebugBridgeExtensions.cs):**
- **Disabled by default** (host = null)
- User aktiviert explizit via `.EnableDebugBridge(host, port)`
- **PFLICHT: Environment-Check** (nur in Development)
  - Checkt `ASPNETCORE_ENVIRONMENT` oder `DOTNET_ENVIRONMENT`
  - Nur wenn "Development" → Services registrieren
- Services werden nur registriert wenn Extension aufgerufen wird UND Environment = Development

**Debug-Services (DebugBridgeClient.cs, etc.):**
- **Immer kompiliert** (sind Teil vom Framework)
- Services sind `internal` (nicht public API)
- Ermöglicht einfacheres Testen
- Tree-Shaking: Wenn nicht genutzt, minimaler DLL-Impact

**Service-Injection:**
- Alle Debug-Services sind **nullable** (`IAppMonitor?`, `ILoggerProvider?`)
- RenderService, etc. arbeiten mit nullable Dependencies
- Wenn Extension nicht aufgerufen: Dependencies sind `null`, kein Overhead

**Vorteile:**
- **Disabled:** Zero overhead (Extension nicht aufgerufen)
- **Enabled:** Volle Funktionalität
- Services bleiben testbar
- Funktioniert mit PlusUi als NuGet-Package (Release-Build)

**PFLICHT: Environment-Check:**
```csharp
// Standard-Weg: Host Environment checken
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                 ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
if (environment != "Development")
    return builder;
```

**Warum PFLICHT?**
- Verhindert versehentlichen Debug-Client in Production
- Standard .NET Environment-Variable (ASP.NET Core, Worker Services)
- User muss nicht #if DEBUG in seiner App machen

**API:**
- `.EnableDebugBridge(host, port)` - Separate Parameter (kein "ws://" nötig)
- User-friendly API, kein Protocol-Wissen erforderlich

## Thread Safety

1. **WebSocket Communication:**
   - `SemaphoreSlim` für Client-List
   - Async/Await für non-blocking sends
   - Fire-and-Forget Broadcasting

2. **Performance Data:**
   - `lock` für Frame-History Queue
   - Pre-allocated Queue (capacity: 300)

3. **Tree Serialization:**
   - Synchron auf UI-Thread
   - Snapshot von Element-Referenzen

## Challenges & Solutions

1. **Circular References:** `HashSet<UiElement>` visited tracking
2. **Mobile Platforms:** Network IPs statt localhost (future)
3. **Complex Type Editing:** Whitelist editierbare Types
4. **Large Trees:** Lazy Loading (future)
5. **Hot Reload:** Re-send Tree on rebuild (future)

## Next Steps nach Approval

1. **Folder Structure anlegen:** `Services/Debug/` und `Services/Debug/Models/`
2. **DTOs implementieren:** Start mit DebugMessage, TreeNodeDto, PropertyDto
3. **DebugBridgeServer implementieren:** Basic WebSocket Server mit Client-Management
4. **Extension implementieren:** Fluent API mit DI-Registration
5. **Test mit Sandbox:** Einfache Verbindung herstellen, Tree serialisieren
6. **Iterativ erweitern:** Performance Monitoring, Logging, Property Editing
7. **Debug Client bauen:** Separate Desktop-App für Inspektion

## Offene Fragen (BEANTWORTET)

✅ **Soll der Debug Server part der Solution sein?**
- **JA**, part der Solution unter `source/PlusUi.DebugServer/`

✅ **Performance Charts: Custom oder externe Library?**
- **Custom PlusUi UserControl** (`PerformanceGraphControl.cs`)
- Canvas-Rendering mit SkiaSharp
- Line-Graph für FPS, Bar-Chart für Timing
- Wiederverwendbar und stylebar

✅ **Theme/Styling?**
- **Dark Mode** wie Visual Studio
- Schwarzer Hintergrund, grau/blau Accents
- High-Contrast für Readability

## Erfolgs-Kriterien

✅ **ZERO overhead wenn disabled** (Extension nicht aufgerufen oder Production)
✅ **KEIN `#if DEBUG` im PlusUi-Code** (Framework ist immer Release!)
✅ Fluent API: `builder.EnableDebugBridge("192.168.1.100", 5555)` - Host/Port getrennt (GLOBAL, nicht per-Page)
✅ **Nullable Services** - IAppMonitor?, ILoggerProvider?, DebugBridgeClient?
✅ **PFLICHT: Environment-Check** - Nur in Development (ASPNETCORE_ENVIRONMENT / DOTNET_ENVIRONMENT)
✅ **Reversed Architecture:** Desktop-App ist SERVER, Apps sind CLIENTS
✅ UI Tree mit allen Properties wird angezeigt (via `IDebugInspectable`)
✅ Performance Monitoring zeigt FPS, Frame-Timing, Measure/Arrange/Render
✅ Custom `PerformanceGraphControl` mit Live-Graphen
✅ Invalidation Tracking mit Stack Traces
✅ Logging wird gesammelt und angezeigt
✅ Remote Property-Editing funktioniert (Server → Client Commands)
✅ Multi-Client-Support (mehrere Apps gleichzeitig debuggen)
✅ Cross-Platform (Desktop/Web/iOS/Android CLIENTS, Desktop SERVER)
✅ Thread-safe Implementation
✅ **Dark Mode** UI wie Visual Studio
✅ Auto-Reconnect bei Connection Loss

---

**Gesamtaufwand:** ~14-18 Tage für vollständige Implementation mit Tests, Polish und Custom Graph Control

## Zusammenfassung der Architektur-Änderungen

### Ursprünglicher Plan (FALSCH):
- ❌ Debugged App = WebSocket Server
- ❌ Debug Client = WebSocket Client
- ❌ `#if DEBUG` im PlusUi-Code (Framework ist IMMER Release!)

### Finaler Plan (RICHTIG):
- ✅ **Debug Desktop App = WebSocket SERVER** (Port 5555)
- ✅ **Debugged Apps = WebSocket CLIENTS** (connecten zu Server)
- ✅ **Runtime Configuration:** KEIN `#if DEBUG` in PlusUi (Framework = Release), disabled by default
- ✅ **Nullable Services:** IAppMonitor?, ILoggerProvider?, DebugBridgeClient?
- ✅ **PFLICHT Environment-Check:** ASPNETCORE_ENVIRONMENT / DOTNET_ENVIRONMENT = "Development"
- ✅ **IDebugInspectable Interface** für Tree-Traversal (internal)
- ✅ **GLOBAL** Extension (nicht per-Page)
- ✅ **Host/Port getrennt** - User muss nicht "ws://" kennen
- ✅ **Custom Graph Control** (PlusUi-based)
- ✅ **Dark Mode** Styling

**Vorteil der umgekehrten Architektur:**
- Mobile-Geräte müssen keinen Server öffnen (NAT/Firewall-freundlich)
- Desktop-App kann mehrere Clients gleichzeitig debuggen
- Einfacher für Remote-Debugging (Developer-PC ist Server, Devices connecten)
