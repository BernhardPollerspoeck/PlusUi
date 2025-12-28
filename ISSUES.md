# PlusUi - Offene Punkte

## Kritische Probleme

### Web-Plattform (Blazor)
1. `source/PlusUi.Web/PlusUiWebApp.cs` - Setup-Code komplett auskommentiert (Zeilen 22-32)
2. `source/PlusUi.Web/PlusUiWebApp.cs` - Referenziert nicht-existierende `PlusUiRootComponent` Klasse
3. `source/PlusUi.Web/WebKeyboardHandler.cs` - Nur 3 von 30+ Keys gemappt (Enter, Backspace, Tab)
4. `source/PlusUi.Web/WebKeyboardHandler.cs` - Show()/Hide() Methoden leer (Zeilen 18-30)
5. `source/PlusUi.Web/WebKeyboardHandler.cs` - 18 auskommentierte Key-Mappings (Zeilen 70-80)
6. Web-Plattform hat keine Touch-Behandlung
7. Web-Plattform hat keine Gesture-Unterstützung
8. Web-Plattform Accessibility-Bridges fehlen komplett
9. Keine Blazor Root-Component im Codebase vorhanden

### Controls ohne `partial class` (Architektur-Verstoß)
10. `source/PlusUi.core/Controls/DataGrid.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
11. `source/PlusUi.core/Controls/DataGridColumn.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
12. `source/PlusUi.core/Controls/DataGridColumnWidth.cs` - Kein `partial class`
13. `source/PlusUi.core/Controls/DataGridRowStyle.cs` - Kein `partial class`
14. `source/PlusUi.core/Controls/GestureDetector.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
15. `source/PlusUi.core/Controls/ItemsList.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
16. `source/PlusUi.core/Controls/MenuItem.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
17. `source/PlusUi.core/Controls/RawUserControl.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
18. `source/PlusUi.core/Controls/Scrollbar.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
19. `source/PlusUi.core/Controls/TooltipAttachment.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`

### Mehrere Klassen pro Datei (Code-Organisation)
20. `source/PlusUi.core/Controls/DataGridColumn.cs` - 5 Klassen: DataGridColumn<T>, DataGridTextColumn<T>, DataGridCheckboxColumn<T>, DataGridButtonColumn<T>, DataGridTemplateColumn<T>
21. `source/PlusUi.core/Services/FontRegistryService.cs` - Interface + Klasse: IFontRegistryService, FontRegistryService
22. `source/PlusUi.core/Styling/Backgrounds/MultiStopGradient.cs` - 2 Typen: MultiStopGradient, GradientStop
23. `source/PlusUi.ios/TouchGestureRecognizer.cs` - 3 Klassen: TouchGestureRecognizer, LongPressGestureRecognizer, PinchGestureRecognizer
24. `source/PlusUi.h264/FrameRenderService.cs` - 2 Klassen: FrameRenderService, AudioSequenceConverter
25. `source/PlusUi.desktop/Accessibility/DesktopAccessibilitySettingsService.cs` - Klasse + Struct: DesktopAccessibilitySettingsService, HIGHCONTRAST

### Platzhalter/Leere Projekte
26. `tests/PlusUi.Regression.Core/Class1.cs` - Leere Regression-Test-Hülle, kein echter Test

### Inkonsistente Benennung
27. Ordner `Enumerations/` und `Enums/` existieren beide - sollte konsolidiert werden
28. Test-Projekt heißt `UiPlus.core.Tests` statt `PlusUi.core.Tests` (inkonsistent mit Hauptprojekt)

### TODO/FIXME im Code
29. `source/PlusUi.core/Services/ImageLoaderService.cs:27` - TODO: Logger nicht static und von DI
30. `source/PlusUi.core/Controls/Grid.cs:65` - TODO: Method shadowing erstellt nur 1-Argument-Version
31. `source/UiPlus.core.Tests/ToolbarTests.cs:427` - BUG: ScrollView misst bei großer Breite, arrangiert bei schmaler Breite

---

## Fehlende Features

### Layout-Controls
32. WrapPanel fehlt (automatisches Umbrechen von Elementen)
33. Expander/Accordion Control fehlt
34. DockPanel fehlt
35. Canvas (absolute Positionierung) fehlt
36. UniformGrid fehlt

### Text & Rich Content
37. RichTextBox fehlt
38. TextBlock mit Inline-Formatierung fehlt
39. Markdown-Rendering fehlt
40. HTML-Rendering fehlt

### Media & Grafik
41. Video Player Control fehlt
42. SVG-Support fehlt
43. Drawing/Canvas Control für Custom-Rendering fehlt

### Daten-Controls
44. TreeView mit Edit-Support fehlt
45. PropertyGrid/Inspector fehlt
46. Spreadsheet-ähnliches Control fehlt
47. Weitere DataGrid Column-Typen fehlen (z.B. ComboBox, DatePicker)

### Formulare & Validierung
48. Validierungs-Framework fehlt komplett
49. Fehler-Indikatoren fehlen
50. Form-Validierungs-Helper fehlen
51. Required-Field-Markierung fehlt

### Theming & Styling
52. Theme-Enum hat nur "Default" - mehr Themes nötig
53. Dynamisches Theme-Switching nicht dokumentiert/implementiert
54. CSS-ähnliche Selektoren fehlen (nur Typ-basiert)
55. ID/Class-basiertes Styling fehlt

### Druck & Export
56. Print Preview fehlt
57. Export zu PDF fehlt
58. Export zu Bildern fehlt

### Accessibility
59. Web-Plattform Accessibility komplett fehlend
60. Screen-Reader-Tests für iOS/Android fehlen

---

## Architektur-Probleme

### Abstraktionen
61. SKColor (SkiaSharp) leckt in Core - keine Farbabstraktion vorhanden
62. Direkte Abhängigkeit auf Silk.NET in Core (nicht optional)
63. ServiceProviderService ist Service-Locator Antipattern

### Code-Qualität
64. Auskommentierter Code in Web-Plattform sollte entfernt werden
65. GlobalSuppressions.cs enthält nur Pragma-Disables

---

## Test-Abdeckung

### Fehlende Tests
66. Web-Plattform hat keine Tests
67. H.264 Video-Rendering hat keine Tests
68. Headless-Plattform hat keine Tests
69. TreeView nur minimal getestet
70. DataGrid-Virtualisierung nicht getestet
71. DatePicker/TimePicker unter-getestet
72. TabControl unter-getestet
73. Drag-and-Drop Szenarien nicht getestet
74. iOS/Android Gesture-Handling nicht getestet

---

## Plattform-Vollständigkeit

### iOS
75. iOS in Produktion ungetestet
76. iOS Haptic-Feedback unvollständig

### Android
77. Android in Produktion ungetestet
78. Android Back-Button-Handling unklar

### Desktop
79. Kein UWP/WinUI 3 native Integration
80. Kein GTK# native Integration für Linux

---

## Dokumentation

81. Keine Dokumentation für Source-Generator-Output
82. Plattform-Support-Matrix nicht dokumentiert
83. CLAUDE.md Richtlinien werden nicht enforced (partial class Verstöße)
84. API-Dokumentation fehlt
85. Getting-Started-Guide fehlt
86. Migration-Guide fehlt

---

## Zusammenfassung

| Kategorie | Anzahl |
|-----------|--------|
| Kritische Probleme | 31 |
| Fehlende Features | 29 |
| Architektur-Probleme | 5 |
| Fehlende Tests | 9 |
| Plattform-Vollständigkeit | 6 |
| Dokumentation | 6 |
| **Gesamt** | **86** |
