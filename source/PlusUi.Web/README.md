# PlusUi.Web - WebAssembly Target für PlusUi

Ermöglicht es, PlusUi-Anwendungen als statische WebAssembly-Apps zu deployen.

## 🎯 Features

- ✅ **Gleicher Code wie Desktop** - Dein `Sandbox` Projekt läuft unverändert
- ✅ **Static Hosting** - Kann auf jedem Webserver gehostet werden
- ✅ **Null Platform-spezifischer Code** - `App.cs` bleibt identisch
- ✅ **SkiaSharp Rendering** - Gleiche Darstellung wie Desktop
- ✅ **Input-Handling** - Maus, Touch, Tastatur, Scroll

## 📦 Projekt-Struktur

```
YourSolution.sln
├── source/
│   ├── PlusUi/              (Core Framework)
│   ├── PlusUi.Desktop/      (Silk.NET Host)
│   └── PlusUi.Web/          (Blazor WASM Host) ← NEU
├── samples/
│   ├── Sandbox/             (Deine UI - platform-agnostic)
│   ├── Sandbox.Desktop/     (Desktop Startup)
│   └── Sandbox.Web/         (Web Startup) ← NEU
```

## 🚀 Setup

### 1. PlusUi.Web Library erstellen

```xml
<!-- PlusUi.Web/PlusUi.Web.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.0" />
    <PackageReference Include="SkiaSharp.Views.Blazor" Version="3.0.0" />
    <ProjectReference Include="..\PlusUi\PlusUi.csproj" />
  </ItemGroup>
</Project>
```

### 2. Web Startup Projekt erstellen

```xml
<!-- Sandbox.Web/Sandbox.Web.csproj -->
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\..\source\PlusUi.Web\PlusUi.Web.csproj" />
    <ProjectReference Include="..\Sandbox\Sandbox.csproj" />
  </ItemGroup>
</Project>
```

### 3. Program.cs

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PlusUi.Web;
using Sandbox;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var app = new PlusUiWebApp(builder);
await app.CreateApp(b => new App()); // Gleiche App wie Desktop!
```

### 4. wwwroot/index.html

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Your PlusUi App</title>
    <base href="/" />
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        html, body, #app { width: 100%; height: 100%; overflow: hidden; }
    </style>
</head>
<body>
    <div id="app">Loading...</div>
    <script src="_framework/blazor.webassembly.js"></script>
</body>
</html>
```

## 🏗️ Build & Run

### Development
```bash
cd Sandbox.Web
dotnet run
```
→ Öffnet Browser auf `http://localhost:5000`

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### Deploy (Static Hosting)
Die `publish/` Folder enthält alle statischen Files:
```
publish/wwwroot/
├── index.html
├── _framework/
│   ├── blazor.webassembly.js
│   ├── dotnet.wasm
│   └── *.dll (deine App + Dependencies)
```

**Hosting-Optionen:**
- GitHub Pages
- Netlify
- Vercel
- Azure Static Web Apps
- Jeder Webserver (nginx, Apache, etc.)

## 🔍 Wie es funktioniert

### Desktop vs. Web

**Desktop (`PlusUi.Desktop`):**
```
Silk.NET → OpenGL → SkiaSharp → RenderService
   ↓
WindowManager (IHostedService)
   ↓
InputService ← Mouse/Keyboard Events
```

**Web (`PlusUi.Web`):**
```
Blazor → HTML Canvas → SkiaSharp → RenderService
   ↓
PlusUiRootComponent (Razor)
   ↓
InputService ← Browser Events
```

### Gemeinsame Services (Platform-Agnostic)
- ✅ `RenderService` - Identisch
- ✅ `InputService` - Identisch
- ✅ `NavigationContainer` - Identisch
- ✅ `PlusUiPopupService` - Identisch
- ✅ Dein kompletter UI-Code - Identisch

### Platform-Spezifisch
- ❌ `WindowManager` (Desktop) vs. `PlusUiRootComponent` (Web)
- ❌ `DesktopKeyboardHandler` vs. `WebKeyboardHandler`

## ⚠️ Bekannte Einschränkungen

### 1. Window-Konfiguration
Web ignoriert einige `PlusUiConfiguration` Settings:
```csharp
public void ConfigureWindow(PlusUiConfiguration config)
{
    config.Title = "App";           // ✅ Wird zu <title>
    config.Size = new(1200, 800);   // ❌ Ignoriert (Browser Viewport)
    config.WindowBorder = Hidden;   // ❌ Ignoriert
    config.IsWindowTransparent = true; // ❌ Ignoriert
}
```

### 2. Performance
- Initial Load: 2-3 MB Download (WASM Runtime + DLLs)
- Rendering: ~10-20% langsamer als Desktop
- Lösung: AOT Compilation (`<RunAOTCompilation>true</RunAOTCompilation>`)

### 3. Display Density
Desktop ermittelt automatisch HiDPI (Retina). Web nutzt aktuell `1.0f`.
→ TODO: JSInterop für `window.devicePixelRatio`

## 🎨 Styling & Themes

Funktioniert identisch wie Desktop:
```csharp
public void ConfigureApp(HostApplicationBuilder builder)
{
    builder.StylePlusUi<YourCustomStyle>();
}
```

## 🐛 Debugging

### Browser DevTools
```javascript
// In Browser Console
console.log(Blazor); // Blazor WASM API
```

### .NET Debugging
Visual Studio / VS Code unterstützt WASM Debugging:
1. Set Breakpoint in C# Code
2. F5 - Debug startet Browser
3. Breakpoint wird getroffen

## 📊 Performance-Optimierungen

### 1. AOT Compilation
```xml
<PropertyGroup>
  <RunAOTCompilation>true</RunAOTCompilation>
</PropertyGroup>
```
→ ~30% schneller, aber längere Build-Zeit

### 2. Trimming
```xml
<PropertyGroup>
  <PublishTrimmed>true</PublishTrimmed>
</PropertyGroup>
```
→ Kleinere Download-Größe

### 3. Lazy Loading
Für große Apps: Seiten als separate DLLs
```xml
<ItemGroup>
  <BlazorWebAssemblyLazyLoad Include="HeavyFeature.dll" />
</ItemGroup>
```

## 🔄 Migration von Desktop → Web

**Schritt 1:** Projekt erstellen (siehe Setup)
**Schritt 2:** `App.cs` unverändert übernehmen
**Schritt 3:** Build & Run

**Das war's!** 🎉

Wenn dein `Sandbox` Projekt keine platform-spezifischen Dependencies hat (Silk.NET, etc.), läuft es sofort.

## 🆘 Troubleshooting

### "Vector2D not found"
`Vector2D<int>` ist von Silk.NET. Im Core solltest du `System.Numerics.Vector2` nutzen.

Fix in `RenderService`:
```csharp
// Vorher (Desktop-only)
public void Render(GL? gl, SKCanvas canvas, GRContext? grContext, Vector2D<int> size)

// Nachher (Cross-platform)
public void Render(GL? gl, SKCanvas canvas, GRContext? grContext, Vector2 size)
```

### Canvas wird nicht angezeigt
Check Browser Console für Fehler. Typisch:
- SkiaSharp DLL fehlt → Check `.csproj` PackageReferences
- Blazor nicht geladen → Check `index.html` hat `blazor.webassembly.js`

### Input funktioniert nicht
Stelle sicher dass Canvas Focus hat:
```html
<SKCanvasView tabindex="0" style="outline: none;" />
```

## 📝 TODOs

- [ ] HiDPI Support via JSInterop
- [ ] Window Resize Events
- [ ] Touch Gestures (Pinch, Rotate)
- [ ] Virtual Keyboard für Mobile
- [ ] File Drag & Drop
- [ ] Clipboard API

## 📄 Lizenz

Gleiche Lizenz wie PlusUi (MIT)
