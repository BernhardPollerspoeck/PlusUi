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
10. `source/PlusUi.core/Controls/DataGrid/DataGrid.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
11. `source/PlusUi.core/Controls/DataGrid/DataGridColumn.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
12. `source/PlusUi.core/Controls/DataGrid/DataGridColumnWidth.cs` - Kein `partial class`
13. `source/PlusUi.core/Controls/DataGrid/DataGridRowStyle.cs` - Kein `partial class`
14. `source/PlusUi.core/Controls/GestureDetector.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
15. `source/PlusUi.core/Controls/ItemsList.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
16. `source/PlusUi.core/Controls/MenuItem.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
17. `source/PlusUi.core/Controls/RawUserControl.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
18. `source/PlusUi.core/Controls/Scrollbar.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`
19. `source/PlusUi.core/Controls/TooltipAttachment.cs` - Kein `partial class`, kein `[GenerateShadowMethods]`

### Platzhalter/Leere Projekte
20. `tests/PlusUi.Regression.Core/Class1.cs` - Leere Regression-Test-Hülle, kein echter Test

### Inkonsistente Benennung
21. Ordner `Enumerations/` und `Enums/` existieren beide - sollte konsolidiert werden
22. Test-Projekt heißt `UiPlus.core.Tests` statt `PlusUi.core.Tests` (inkonsistent mit Hauptprojekt)

### TODO/FIXME im Code
23. `source/PlusUi.core/Services/ImageLoaderService.cs:27` - TODO: Logger nicht static und von DI
24. `source/PlusUi.core/Controls/Grid.cs:65` - TODO: Method shadowing erstellt nur 1-Argument-Version
25. `source/UiPlus.core.Tests/ToolbarTests.cs:427` - BUG: ScrollView misst bei großer Breite, arrangiert bei schmaler Breite

---

## Fehlende Features

### Layout-Controls
26. WrapPanel fehlt (automatisches Umbrechen von Elementen)
27. Expander/Accordion Control fehlt
28. DockPanel fehlt
29. Canvas (absolute Positionierung) fehlt
30. UniformGrid fehlt

### Text & Rich Content
31. RichTextBox fehlt
32. TextBlock mit Inline-Formatierung fehlt
33. Markdown-Rendering fehlt
34. HTML-Rendering fehlt

### Media & Grafik
35. Video Player Control fehlt
36. SVG-Support fehlt
37. Drawing/Canvas Control für Custom-Rendering fehlt

### Daten-Controls
38. TreeView mit Edit-Support fehlt
39. PropertyGrid/Inspector fehlt
40. Spreadsheet-ähnliches Control fehlt
41. Weitere DataGrid Column-Typen fehlen (z.B. ComboBox, DatePicker)

### Formulare & Validierung
42. Validierungs-Framework fehlt komplett
43. Fehler-Indikatoren fehlen
44. Form-Validierungs-Helper fehlen
45. Required-Field-Markierung fehlt

### Theming & Styling
46. Theme-Enum hat nur "Default" - mehr Themes nötig
47. Dynamisches Theme-Switching nicht dokumentiert/implementiert
48. CSS-ähnliche Selektoren fehlen (nur Typ-basiert)
49. ID/Class-basiertes Styling fehlt

### Druck & Export
50. Print Preview fehlt
51. Export zu PDF fehlt
52. Export zu Bildern fehlt

### Accessibility
53. Web-Plattform Accessibility komplett fehlend
54. Screen-Reader-Tests für iOS/Android fehlen

---

## Architektur-Probleme

### Abstraktionen
55. SKColor (SkiaSharp) leckt in Core - keine Farbabstraktion vorhanden
56. Direkte Abhängigkeit auf Silk.NET in Core (nicht optional)
57. ServiceProviderService ist Service-Locator Antipattern

### Code-Qualität
58. Auskommentierter Code in Web-Plattform sollte entfernt werden
59. GlobalSuppressions.cs enthält nur Pragma-Disables

---

## Test-Abdeckung

### Fehlende Tests
60. Web-Plattform hat keine Tests
61. H.264 Video-Rendering hat keine Tests
62. Headless-Plattform hat keine Tests
63. TreeView nur minimal getestet
64. DataGrid-Virtualisierung nicht getestet
65. DatePicker/TimePicker unter-getestet
66. TabControl unter-getestet
67. Drag-and-Drop Szenarien nicht getestet
68. iOS/Android Gesture-Handling nicht getestet

---

## Plattform-Vollständigkeit

### iOS
69. iOS in Produktion ungetestet
70. iOS Haptic-Feedback unvollständig

### Android
71. Android in Produktion ungetestet
72. Android Back-Button-Handling unklar

### Desktop
73. Kein UWP/WinUI 3 native Integration
74. Kein GTK# native Integration für Linux

---

## Dokumentation

75. Keine Dokumentation für Source-Generator-Output
76. Plattform-Support-Matrix nicht dokumentiert
77. CLAUDE.md Richtlinien werden nicht enforced (partial class Verstöße)
78. API-Dokumentation fehlt
79. Getting-Started-Guide fehlt
80. Migration-Guide fehlt

---

## Zusammenfassung

| Kategorie | Anzahl |
|-----------|--------|
| Kritische Probleme | 25 |
| Fehlende Features | 29 |
| Architektur-Probleme | 5 |
| Fehlende Tests | 9 |
| Plattform-Vollständigkeit | 6 |
| Dokumentation | 6 |
| **Gesamt** | **80** |
