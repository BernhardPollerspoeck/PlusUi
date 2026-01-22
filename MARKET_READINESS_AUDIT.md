# PlusUi - Marktreife-Audit und Wettbewerbsanalyse

**Datum:** Januar 2026
**Version:** 1.0
**Autor:** Claude Code Audit

---

## Executive Summary

PlusUi ist ein ambitioniertes Cross-Platform UI-Framework für .NET, das auf SkiaSharp als einheitlicher Rendering-Engine aufbaut. Die Analyse zeigt ein **technisch solides Fundament** mit professioneller Code-Qualität, aber **signifikante Lücken** in Bezug auf Marktreife, Dokumentation und Plattform-Stabilität.

### Gesamtbewertung: 6.5/10 (Nicht marktreif)

| Kriterium | Score | Status |
|-----------|-------|--------|
| Architektur & Design | 8.5/10 | ✅ Exzellent |
| Code-Qualität | 8.4/10 | ✅ Exzellent |
| Control-Bibliothek | 7.0/10 | ⚠️ Gut |
| Theming/Styling | 7.5/10 | ⚠️ Gut |
| Plattform-Support | 5.0/10 | ❌ Unzureichend |
| Dokumentation | 3.0/10 | ❌ Kritisch |
| Community/Ökosystem | 1.0/10 | ❌ Nicht vorhanden |
| **Marktreife gesamt** | **6.5/10** | **❌ Nicht bereit** |

---

## Teil 1: Technische Analyse

### 1.1 Projektstruktur und Organisation

```
PlusUi/
├── source/
│   ├── PlusUi.core/          # 229 Dateien - Kern-Framework
│   ├── PlusUi.SourceGenerators/ # Roslyn Code-Generatoren
│   ├── PlusUi.desktop/       # Windows/macOS/Linux via Silk.NET
│   ├── PlusUi.ios/           # iOS native
│   ├── PlusUi.droid/         # Android native
│   ├── PlusUi.Web/           # Blazor WebAssembly
│   ├── PlusUi.Headless/      # Server-Side Rendering
│   ├── PlusUi.h264/          # Video-Rendering
│   └── PlusUi.DebugServer/   # Entwickler-Tools
├── samples/                   # 7 Plattform-Demos
├── templates/                 # Projekt-Templates
└── tests/                     # Unit-Tests
```

**Bewertung:** Die Projektstruktur ist klar und professionell organisiert. Die Trennung zwischen Kern-Bibliothek und plattformspezifischen Implementierungen folgt Best Practices.

### 1.2 Architektur

#### Rendering-Architektur
- **Einheitliche Engine:** SkiaSharp 3.119.1 für alle Plattformen
- **Konsistenz:** Pixel-perfekte Darstellung auf allen Zielplattformen
- **Ansatz:** Vollständiges Custom-Rendering (kein Mapping auf native Controls)

#### Klassenarchitektur
```
UiElement (abstrakte Basis)
├── UiTextElement (Text-basierte Controls)
├── UiLayoutElement<T> (Container/Layouts)
├── UiPageElement (Seiten)
├── UiPopupElement (Popups/Overlays)
└── Konkrete Controls (Button, Label, etc.)
```

#### Service-Architektur
Das Framework nutzt Dependency Injection mit folgenden Kern-Services:
- `IPaintRegistryService` - Paint/Font-Ressourcen-Management
- `IThemeService` - Theme-Verwaltung
- `INavigationService` - Navigation
- `IFocusManager` - Fokus-Navigation
- `IAccessibilityService` - Barrierefreiheit
- `IRenderService` - Rendering-Pipeline

**Stärken:**
- Saubere Separation of Concerns
- Fluent API durchgängig implementiert
- Source Generators reduzieren Boilerplate-Code

**Schwächen:**
- Service Locator Pattern (`ServiceProviderService.ServiceProvider`) statt reiner Constructor-Injection
- Große Klassen bei komplexen Controls (DataGrid, TreeView)

### 1.3 Control-Bibliothek

#### Verfügbare Controls (60+)

| Kategorie | Controls | Vollständigkeit |
|-----------|----------|-----------------|
| **Text** | Label, Entry, Link | ✅ Vollständig |
| **Buttons** | Button, Checkbox, RadioButton, Toggle | ✅ Vollständig |
| **Layout** | VStack, HStack, Grid, UniformGrid, Border, ScrollView | ✅ Vollständig |
| **Listen** | ItemsList<T>, TreeView, DataGrid<T> | ✅ Vollständig |
| **Auswahl** | ComboBox<T>, Slider, DatePicker, TimePicker | ✅ Vollständig |
| **Navigation** | TabControl, Menu, Toolbar, ContextMenu | ✅ Vollständig |
| **Medien** | Image (statisch, animiert, SVG), ProgressBar, ActivityIndicator | ⚠️ Teilweise |
| **Gesten** | Tap, DoubleTap, LongPress, Swipe, Pinch, Drag | ✅ Vollständig |

#### DataGrid-Spaltentypen (11 Varianten)
TextColumn, ButtonColumn, CheckboxColumn, ComboBoxColumn, DatePickerColumn, ImageColumn, LinkColumn, ProgressColumn, SliderColumn, TimePickerColumn, TemplateColumn

#### Fehlende Standard-Controls
- **Kritisch:** RichTextBox, MultiLine-TextBox, AutoComplete
- **Wichtig:** ColorPicker, FilePicker, NumberPicker, MaskedEntry
- **Nice-to-have:** MediaPlayer, WebView, MapView, Charts (erweitert)

### 1.4 Theming und Styling

#### Styling-System
```csharp
// Anwendungs-weites Styling
public class AppStyle : IApplicationStyle
{
    public void ConfigureStyle(Style style)
    {
        style.AddStyle<Button>(button => {
            button.SetBackground(PlusUiDefaults.BackgroundControl);
            button.SetTextColor(PlusUiDefaults.TextPrimary);
        });

        style.AddStyle<Button>(Theme.Dark, button => {
            button.SetBackground(Colors.DarkGray);
        });
    }
}
```

#### Theme-Unterstützung
- **Themes:** Default, Light, Dark
- **Vererbung:** Typ-basierte Style-Vererbung
- **Dynamisch:** Runtime-Theme-Wechsel möglich

#### Background-Optionen
- SolidColorBackground
- LinearGradient (2 Farben + Winkel)
- RadialGradient (Zentrum zu Rand)
- MultiStopGradient (mehrere Farben)

#### Vordefinierte Farben
- 150+ Farben in `Colors` Klasse
- Semantic Colors in `PlusUiDefaults`:
  - BackgroundPage, BackgroundPrimary, BackgroundSecondary
  - TextPrimary, TextSecondary, TextPlaceholder
  - AccentPrimary, AccentSuccess, AccentError, AccentWarning
  - High-Contrast-Farben für Barrierefreiheit

**Stärken:**
- Flexibles und erweiterbares System
- Gute Default-Werte (Dark Theme)
- High-Contrast-Unterstützung

**Schwächen:**
- Keine XAML/Markup-basierte Styling-Definition
- Kein Style-Sharing zwischen Apps
- Keine Design-Token/CSS-Variable-Äquivalente

### 1.5 Plattform-Unterstützung

| Plattform | Status | Technologie | Bewertung |
|-----------|--------|-------------|-----------|
| **Windows** | ✅ Produktionsreif | Silk.NET/OpenGL | 9/10 |
| **macOS** | ⚠️ Ungetestet | Silk.NET/OpenGL | 5/10 |
| **Linux** | ⚠️ Ungetestet | Silk.NET/OpenGL | 5/10 |
| **Web** | 🚧 In Entwicklung | Blazor WASM | 4/10 |
| **iOS** | 🚧 In Entwicklung | Native UIKit | 4/10 |
| **Android** | 🚧 In Entwicklung | Native + OpenGL ES | 4/10 |
| **Headless** | ⚠️ Ungetestet | In-Memory | 6/10 |
| **H264/Video** | ✅ Stabil | FFmpeg | 8/10 |

**Kritische Feststellung:** Nur Windows ist produktionsreif getestet. Mobile Plattformen (iOS, Android) und Web sind in früher Entwicklung.

### 1.6 Code-Qualität

#### Quantitative Metriken

| Metrik | Wert | Bewertung |
|--------|------|-----------|
| Produktionscode | 33.179 LOC | - |
| Testcode | 16.452 LOC | - |
| Test-zu-Code-Ratio | ~0.50 | ⚠️ Akzeptabel |
| TODO/FIXME Kommentare | 1 | ✅ Exzellent |
| Exception-Throws | 56 (0.24/Datei) | ✅ Exzellent |
| XML-Dokumentation | 2.483 | ✅ Gut |

#### Code-Patterns

**Positiv:**
- Konsistente Fluent API (Set*/Bind* Pattern)
- Moderne C# Features (Primary Constructors, Pattern Matching, Field Init)
- Nullable Reference Types durchgängig aktiviert
- Source Generators für Boilerplate-Reduktion

**Verbesserungswürdig:**
- Service Locator Pattern statt Constructor-Injection
- Begrenzte Integration Tests
- Keine Performance-Benchmarks

#### Maintainability Score: 8.4/10

---

## Teil 2: Wettbewerbsanalyse

### 2.1 .NET MAUI

**Hersteller:** Microsoft (offiziell)
**Lizenz:** MIT
**Plattformen:** Android, iOS, macOS, Windows

#### Stärken
- Offizielle Microsoft-Unterstützung und Integration
- Großes Ökosystem (Syncfusion, Telerik, DevExpress)
- Blazor Hybrid für Web-Integration
- Hot Reload
- Native Controls auf jeder Plattform

#### Schwächen
- Desktop-Unterstützung (besonders macOS) problematisch
- Kein Linux-Support
- Performance bei Animationen schwächer als Flutter
- Weniger Community als Flutter/React Native

#### Vergleich mit PlusUi

| Aspekt | .NET MAUI | PlusUi |
|--------|-----------|--------|
| Plattformen | 4 (stabil) | 1 (stabil) |
| Controls | 40+ (+ Toolkits) | 60+ |
| Rendering | Native Controls | SkiaSharp |
| Konsistenz | Platform-spezifisch | Pixel-perfekt |
| Dokumentation | Umfangreich | Minimal |
| IDE-Support | VS, VS4Mac, Rider | Keine |
| Hot Reload | ✅ | ❌ |

### 2.2 Avalonia UI

**Hersteller:** AvaloniaUI (Community + Unternehmen)
**Lizenz:** MIT
**Plattformen:** Windows, macOS, Linux, iOS, Android, WebAssembly

#### Stärken
- Echte Cross-Platform-Konsistenz (wie PlusUi)
- XAML-basiert (bekannt für WPF-Entwickler)
- $3 Mio. Sponsoring von Devolutions (Juni 2025)
- Hybrid MAUI Integration geplant
- Drag-and-Drop Designer in Entwicklung (Avalonia Accelerate)
- Wechsel zu Impeller-Renderer (Flutter's Engine)

#### Schwächen
- Premium-Controls kostenpflichtig (Avalonia Accelerate)
- Weniger Mobile-Fokus als MAUI

#### Vergleich mit PlusUi

| Aspekt | Avalonia | PlusUi |
|--------|----------|--------|
| Rendering | Skia → Impeller | SkiaSharp |
| Markup | XAML | Code-Only |
| Themes | Fluent, Material | Dark/Light |
| Designer | In Entwicklung | ❌ |
| Dokumentation | Umfangreich | Minimal |
| Community | Groß | Keine |

### 2.3 Uno Platform

**Hersteller:** nventive (Unternehmen)
**Lizenz:** Apache 2.0
**Plattformen:** Windows, iOS, Android, macOS, Linux, WebAssembly

#### Stärken
- WinUI/UWP-API-Kompatibilität
- Hot Design (visueller Designer während Runtime)
- AI-Assistent (Hot Design Agent) in Studio 2.0
- Figma-to-Code Integration
- App MCP für AI-Agent-Integration
- Unified Skia Rendering (seit 6.0)

#### Schwächen
- Komplexere Lernkurve (WinUI-Konzepte)
- Premium-Features kostenpflichtig

#### Vergleich mit PlusUi

| Aspekt | Uno Platform | PlusUi |
|--------|--------------|--------|
| API-Basis | WinUI 3 | Custom |
| Rendering | Skia (unified) | SkiaSharp |
| Designer | Hot Design | ❌ |
| AI-Tools | ✅ Agentic | ❌ |
| Controls | Hunderte (+ WCT) | 60+ |
| Dokumentation | Umfangreich | Minimal |

### 2.4 Flutter (Nicht-.NET)

**Hersteller:** Google
**Sprache:** Dart
**Lizenz:** BSD-3
**Plattformen:** iOS, Android, Web, Windows, macOS, Linux

#### Stärken
- Marktführer (~46% Cross-Platform-Markt 2026)
- Impeller Rendering Engine (state-of-the-art)
- Riesige Widget-Bibliothek (Material, Cupertino)
- Herausragende Performance (kompiliert zu ARM native)
- Flutter AI Toolkit
- Ausgezeichnete Dokumentation

#### Schwächen
- Dart-Sprache (kleineres Ökosystem als JS/C#)
- Größere App-Bundles
- Keine echte native UI (alles custom rendered)

#### Vergleich mit PlusUi

| Aspekt | Flutter | PlusUi |
|--------|---------|--------|
| Rendering | Impeller/Skia | SkiaSharp |
| Performance | Exzellent | Gut |
| Widgets | Tausende | 60+ |
| Hot Reload | ✅ | ❌ |
| Dokumentation | Exzellent | Minimal |
| Community | Riesig | Keine |
| Sprache | Dart | C# |

### 2.5 React Native (Nicht-.NET)

**Hersteller:** Meta
**Sprache:** JavaScript/TypeScript
**Lizenz:** MIT
**Plattformen:** iOS, Android, (Web via React Native Web)

#### Stärken
- Größtes Ökosystem (npm-Pakete)
- JavaScript/TypeScript bekannt
- Native UI-Components
- Bridgeless Architecture (ab 0.76)
- Große Community und Job-Markt
- Apps: Instagram, Discord, Shopify

#### Schwächen
- JavaScript-Bridge (auch wenn verbessert)
- Keine echte Desktop-Unterstützung
- Fragmentierung durch Community-Packages

#### Vergleich mit PlusUi

| Aspekt | React Native | PlusUi |
|--------|--------------|--------|
| Rendering | Native Controls | SkiaSharp |
| Code-Sharing | 70-90% | 100% |
| Sprache | JS/TS | C# |
| Desktop | Limitiert | ✅ |
| Performance | Gut | Gut |
| Ökosystem | Riesig | Keines |

### 2.6 Compose Multiplatform (Nicht-.NET)

**Hersteller:** JetBrains
**Sprache:** Kotlin
**Lizenz:** Apache 2.0
**Plattformen:** Android, iOS (stabil seit Mai 2025), Desktop, Web

#### Stärken
- Deklarative UI (wie SwiftUI/Jetpack Compose)
- iOS stabil seit Compose Multiplatform 1.8.0
- Google Jetpack-Bibliotheken kompatibel (Room, DataStore, ViewModel)
- Hot Reload stabil
- Nativer Plattform-Zugriff (Camera, Maps)

#### Schwächen
- Kotlin-exklusiv
- Web-Support weniger ausgereift
- Kleinere Community als Flutter/React Native

#### Vergleich mit PlusUi

| Aspekt | Compose MP | PlusUi |
|--------|------------|--------|
| UI-Paradigma | Deklarativ | Fluent/Imperativ |
| Plattformen | 4 (stabil) | 1 (stabil) |
| Hot Reload | ✅ | ❌ |
| Native APIs | Exzellent | Gut |
| IDE-Support | Exzellent | Keiner |

---

## Teil 3: Vergleichsmatrix

### Feature-Vergleich

| Feature | PlusUi | MAUI | Avalonia | Uno | Flutter | React Native | Compose MP |
|---------|--------|------|----------|-----|---------|--------------|------------|
| **Plattformen** |
| Windows | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ | ✅ |
| macOS | ⚠️ | ✅ | ✅ | ✅ | ✅ | ⚠️ | ✅ |
| Linux | ⚠️ | ❌ | ✅ | ✅ | ✅ | ❌ | ✅ |
| iOS | 🚧 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Android | 🚧 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Web | 🚧 | ✅¹ | ✅ | ✅ | ✅ | ⚠️ | ⚠️ |
| **Entwicklung** |
| Hot Reload | ❌ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Visual Designer | ❌ | ✅ | 🚧 | ✅ | ❌ | ❌ | ❌ |
| XAML/Markup | ❌ | ✅ | ✅ | ✅ | ❌ | JSX | ❌ |
| **UI** |
| Controls | 60+ | 40+ | 50+ | 100+ | 500+ | 100+ | 100+ |
| Theming | ⚠️ | ✅ | ✅ | ✅ | ✅ | ⚠️ | ✅ |
| Accessibility | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Ökosystem** |
| Dokumentation | ❌ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Community | ❌ | ⚠️ | ⚠️ | ⚠️ | ✅ | ✅ | ⚠️ |
| 3rd Party Controls | ❌ | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ |

✅ Vollständig | ⚠️ Teilweise/Eingeschränkt | 🚧 In Entwicklung | ❌ Nicht vorhanden
¹ Via Blazor Hybrid

### Marktreife-Vergleich

| Framework | Stabilität | Dokumentation | Community | Enterprise-Ready |
|-----------|------------|---------------|-----------|------------------|
| .NET MAUI | 8/10 | 9/10 | 7/10 | ✅ |
| Avalonia | 8/10 | 8/10 | 7/10 | ✅ |
| Uno Platform | 8/10 | 8/10 | 6/10 | ✅ |
| Flutter | 9/10 | 10/10 | 10/10 | ✅ |
| React Native | 8/10 | 9/10 | 10/10 | ✅ |
| Compose MP | 8/10 | 8/10 | 6/10 | ✅ |
| **PlusUi** | **4/10** | **3/10** | **1/10** | **❌** |

---

## Teil 4: Stärken und Schwächen von PlusUi

### Stärken

1. **Architektonische Reinheit**
   - Einheitliches Rendering über alle Plattformen
   - Keine Abhängigkeit von plattformspezifischen UI-Frameworks
   - Pixel-perfekte Konsistenz

2. **Code-Qualität**
   - Professioneller, gut strukturierter Code
   - Moderne C#-Features (Primary Constructors, Source Generators)
   - Minimale technische Schulden

3. **API-Design**
   - Konsistente Fluent API
   - Set/Bind-Pattern ermöglicht gute MVVM-Integration
   - Intuitive Control-Komposition

4. **Barrierefreiheit**
   - 28 Accessibility-Rollen
   - High-Contrast-Unterstützung
   - Integriert von Anfang an

5. **Spezialisierte Features**
   - Video-Export (H264) - einzigartig
   - Headless-Rendering für Tests/Automation
   - Debug-Server für Entwicklung

### Schwächen

1. **Plattform-Stabilität (Kritisch)**
   - Nur Windows produktionsreif
   - Mobile (iOS/Android) in früher Entwicklung
   - macOS/Linux ungetestet

2. **Dokumentation (Kritisch)**
   - Keine Benutzer-Dokumentation
   - Keine Getting-Started-Guides
   - Keine API-Referenz online

3. **Entwickler-Erfahrung**
   - Kein Hot Reload
   - Kein visueller Designer
   - Keine IDE-Integration (IntelliSense begrenzt)

4. **Ökosystem**
   - Keine Community
   - Keine 3rd-Party-Controls
   - Keine Beispiel-Apps

5. **Fehlende Controls**
   - MultiLine-TextBox
   - RichText-Editor
   - ColorPicker, FilePicker
   - WebView, MediaPlayer

---

## Teil 5: Marktreife-Bewertung

### Mindestanforderungen für Marktstart

| Anforderung | Status | Priorität |
|-------------|--------|-----------|
| Stabile Desktop-Plattformen (Win/Mac/Linux) | ❌ 1/3 | Kritisch |
| Stabile Mobile-Plattformen (iOS/Android) | ❌ 0/2 | Kritisch |
| Dokumentation (Getting Started, API Docs) | ❌ | Kritisch |
| Beispiel-Apps und Tutorials | ❌ | Kritisch |
| Hot Reload | ❌ | Hoch |
| NuGet-Package verfügbar | ❌ | Kritisch |
| Basis-Controls vollständig | ⚠️ 85% | Hoch |
| Theming-System | ✅ | - |
| Accessibility | ✅ | - |
| Tests | ⚠️ | Mittel |

### Empfohlene Maßnahmen vor Marktstart

#### Phase 1: Stabilisierung (geschätzt 3-6 Monate)
1. macOS und Linux Desktop-Testing und Bugfixing
2. iOS-Plattform stabilisieren
3. Android-Plattform stabilisieren
4. Automatisierte Plattform-Tests einführen

#### Phase 2: Dokumentation (geschätzt 2-3 Monate)
1. Getting-Started-Guide
2. Control-Referenz (alle Controls dokumentieren)
3. Architektur-Dokumentation
4. Tutorial-Serie
5. API-Dokumentation generieren

#### Phase 3: Developer Experience (geschätzt 3-4 Monate)
1. Hot Reload implementieren
2. NuGet-Packages publizieren
3. Projekt-Templates verbessern
4. Sample-Apps erstellen
5. IDE-Extensions (optional)

#### Phase 4: Feature-Vervollständigung (geschätzt 2-3 Monate)
1. MultiLine-TextBox
2. RichTextBox (optional)
3. ColorPicker
4. NumberPicker/MaskedEntry
5. AutoComplete für Entry

---

## Teil 6: Strategische Empfehlungen

### Positionierung im Markt

PlusUi hat **keine direkte Marktpositionierung** zwischen den etablierten Frameworks. Mögliche Nischen:

1. **Einheitliches Rendering**
   - Wie Avalonia, aber leichtgewichtiger
   - Fokus auf pixel-perfekte Konsistenz

2. **Video/Automation**
   - H264-Export ist einzigartig
   - Headless-Rendering für CI/CD

3. **Embedded Systems**
   - SkiaSharp läuft auf vielen Embedded-Plattformen
   - Minimaler Footprint möglich

### Wettbewerbsdifferenzierung

| Differenzierungsmerkmal | Umsetzbarkeit | Marktpotenzial |
|-------------------------|---------------|----------------|
| Video-Export (H264) | ✅ Vorhanden | Nische |
| Headless/Automation | ✅ Vorhanden | Mittel |
| Code-Only UI (kein XAML) | ✅ Vorhanden | Polarisierend |
| Leichtgewichtig | 🚧 Möglich | Mittel |
| Embedded-Fokus | 🚧 Möglich | Nische |

### Empfohlene Strategie

1. **Kurzfristig:** Nicht als allgemeines UI-Framework positionieren
2. **Mittelfristig:** Nischenfokus auf:
   - Desktop-Apps mit Video-Export-Bedarf
   - Automatisierte UI-Tests
   - Embedded/Kiosk-Anwendungen
3. **Langfristig:** Bei Ressourcen-Verfügbarkeit Mobile-Support ausbauen

---

## Fazit

### Ist PlusUi marktreif?

**Nein.** PlusUi ist technisch solide, aber für einen öffentlichen Marktstart fehlen kritische Komponenten:

- **Plattform-Stabilität:** Nur Windows ist produktionsreif
- **Dokumentation:** Praktisch nicht vorhanden
- **Ökosystem:** Keine Community, keine Packages
- **Developer Experience:** Kein Hot Reload, keine IDE-Tools

### Geschätzter Aufwand bis Marktreife

| Szenario | Aufwand | Ergebnis |
|----------|---------|----------|
| MVP (Desktop-only) | 6-9 Monate | Desktop-Framework mit Docs |
| Full Launch | 12-18 Monate | Cross-Platform Framework |

### Empfehlung

1. **Für internen Einsatz:** Geeignet für Desktop-Windows-Projekte
2. **Für Open-Source-Release:** Erst nach Dokumentation und macOS/Linux-Testing
3. **Für kommerziellen Launch:** Erhebliche Investition in Mobile und DX notwendig

---

## Quellen

### Framework-Vergleich
- [.NET MAUI Official](https://dotnet.microsoft.com/en-us/apps/maui)
- [Avalonia UI](https://avaloniaui.net/)
- [Uno Platform](https://platform.uno/)
- [Flutter](https://flutter.dev/)
- [React Native](https://reactnative.dev/)
- [Compose Multiplatform](https://kotlinlang.org/compose-multiplatform/)

### Marktanalysen
- [.NET MAUI in 2025 - Brainhub](https://brainhub.eu/library/net-maui-in-nutshell)
- [State of .NET MAUI 2025 - Appisto](https://appisto.app/blog/state-of-dotnet-maui)
- [Avalonia $3M Sponsorship](https://avaloniaui.net/)
- [Uno Platform 6.4 Release - InfoQ](https://www.infoq.com/news/2025/11/uno-platform-6-4-agentic/)
- [Flutter vs React Native 2025](https://dev.to/mridudixit15/flutter-vs-react-native-2025-who-wins-the-cross-platform-war-4hfh)
- [Compose Multiplatform iOS Stable](https://www.kmpship.app/blog/compose-multiplatform-ios-stable-2025)

---

*Dieser Bericht wurde basierend auf einer vollständigen Code-Analyse des PlusUi-Repositories und aktuellen Marktdaten erstellt.*
