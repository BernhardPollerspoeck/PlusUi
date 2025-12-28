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

### Inkonsistente Benennung
10. Test-Projekt heißt `UiPlus.core.Tests` statt `PlusUi.core.Tests` (inkonsistent mit Hauptprojekt)

### TODO/FIXME im Code
11. `source/PlusUi.core/Services/ImageLoaderService.cs:27` - TODO: Logger nicht static und von DI

---

## Fehlende Features

### Layout-Controls
12. WrapPanel fehlt (automatisches Umbrechen von Elementen)
13. Expander/Accordion Control fehlt
14. DockPanel fehlt
15. Canvas (absolute Positionierung) fehlt
16. UniformGrid fehlt

### Text & Rich Content
17. RichTextBox fehlt
18. TextBlock mit Inline-Formatierung fehlt
19. Markdown-Rendering fehlt
20. HTML-Rendering fehlt

### Media & Grafik
21. Video Player Control fehlt
22. SVG-Support fehlt
23. Drawing/Canvas Control für Custom-Rendering fehlt

### Daten-Controls
24. TreeView mit Edit-Support fehlt
25. PropertyGrid/Inspector fehlt
26. Spreadsheet-ähnliches Control fehlt
27. Weitere DataGrid Column-Typen fehlen (z.B. ComboBox, DatePicker)

### Formulare & Validierung
28. Validierungs-Framework fehlt komplett
29. Fehler-Indikatoren fehlen
30. Form-Validierungs-Helper fehlen
31. Required-Field-Markierung fehlt

### Theming & Styling
32. Theme-Enum hat nur "Default" - mehr Themes nötig
33. Dynamisches Theme-Switching nicht dokumentiert/implementiert
34. CSS-ähnliche Selektoren fehlen (nur Typ-basiert)
35. ID/Class-basiertes Styling fehlt

### Druck & Export
36. Print Preview fehlt
37. Export zu PDF fehlt
38. Export zu Bildern fehlt

### Accessibility
39. Web-Plattform Accessibility komplett fehlend
40. Screen-Reader-Tests für iOS/Android fehlen

---

## Architektur-Probleme

### Abstraktionen
41. SKColor (SkiaSharp) leckt in Core - keine Farbabstraktion vorhanden
42. Direkte Abhängigkeit auf Silk.NET in Core (nicht optional)
43. ServiceProviderService ist Service-Locator Antipattern

### Code-Qualität
44. Auskommentierter Code in Web-Plattform sollte entfernt werden
45. GlobalSuppressions.cs enthält nur Pragma-Disables

---

## Test-Abdeckung

### Fehlende Tests
46. Web-Plattform hat keine Tests
47. H.264 Video-Rendering hat keine Tests
48. Headless-Plattform hat keine Tests
49. TreeView nur minimal getestet
50. DataGrid-Virtualisierung nicht getestet
51. DatePicker/TimePicker unter-getestet
52. TabControl unter-getestet
53. Drag-and-Drop Szenarien nicht getestet
54. iOS/Android Gesture-Handling nicht getestet

---

## Plattform-Vollständigkeit

### iOS
55. iOS in Produktion ungetestet
56. iOS Haptic-Feedback unvollständig

### Android
57. Android in Produktion ungetestet
58. Android Back-Button-Handling unklar

### Desktop
59. Kein UWP/WinUI 3 native Integration
60. Kein GTK# native Integration für Linux

---

## Dokumentation

61. Keine Dokumentation für Source-Generator-Output
62. Plattform-Support-Matrix nicht dokumentiert
63. CLAUDE.md Richtlinien werden nicht enforced (partial class Verstöße)
64. API-Dokumentation fehlt
65. Getting-Started-Guide fehlt
66. Migration-Guide fehlt

---

## Zusammenfassung

| Kategorie | Anzahl |
|-----------|--------|
| Kritische Probleme | 11 |
| Fehlende Features | 29 |
| Architektur-Probleme | 5 |
| Fehlende Tests | 9 |
| Plattform-Vollständigkeit | 6 |
| Dokumentation | 6 |
| **Gesamt** | **66** |
