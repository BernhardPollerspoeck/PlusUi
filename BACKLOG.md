# PlusUi - Backlog für Marktreife

> Erstellt: 2024-12-14
> Aktualisiert: 2024-12-28

---

## Übersicht

| Kategorie | Anzahl | Geschätzte Gesamtzeit |
|-----------|--------|----------------------|
| Controls (Kritisch) | 1 | 5-7 Tage |
| Controls (Hoch) | 2 | 6-9 Tage |
| Plattformen | 3 | 5-9 Tage |
| **Gesamt** | **6** | **16-25 Tage** |

---

## CONTROLS

---

### CTRL-002: DataGrid / Table

| | |
|---|---|
| **Typ** | Neues Control |
| **Priorität** | 🔴 Kritisch |
| **Zeitschätzung** | 5-7 Tage |

**Beschreibung:**
Tabellarische Darstellung von Daten mit Spalten und Zeilen. Essentiell für Business-Anwendungen, Datenverwaltung und Listen mit mehreren Attributen pro Eintrag.

**Anforderungen:**

- [ ] `DataGrid<T>` Klasse (generisch für Typsicherheit)
- [ ] `DataGridColumn` Klasse mit Header, Binding, Width
- [ ] Property `ItemsSource` (Collection)
- [ ] Property `Columns` (Collection von DataGridColumn)
- [ ] Property `SelectedItem` / `SelectedItems`
- [ ] Column-Typen: Text, Checkbox, Button, Custom Template
- [ ] Column-Sizing: Auto, Fixed, Star (*)
- [ ] Fluent API: `SetItemsSource()`, `AddColumn()`, `SetSelectionMode()`
- [ ] Binding für ItemsSource mit INotifyCollectionChanged
- [ ] Virtualisierung für Performance (wie ItemsList)
- [ ] Optional: Sortierung per Column-Header-Klick
- [ ] Optional: Column Resizing

**API-Beispiel:**
```csharp
new DataGrid<Person>()
    .SetItemsSource(() => vm.Persons)
    .AddColumn(new DataGridTextColumn<Person>()
        .SetHeader("Name")
        .SetBinding(p => p.Name)
        .SetWidth(GridLength.Star(2)))
    .AddColumn(new DataGridTextColumn<Person>()
        .SetHeader("Alter")
        .SetBinding(p => p.Age.ToString())
        .SetWidth(GridLength.Auto))
    .AddColumn(new DataGridCheckboxColumn<Person>()
        .SetHeader("Aktiv")
        .SetBinding(p => p.IsActive, (p, v) => p.IsActive = v))
```

**Abhängigkeiten:** Basiert auf ItemsList-Virtualisierung

**Aufwandsverteilung:**
- Grundstruktur & Column-System: 1 Tag
- Header-Rendering & Sizing: 1 Tag
- Row-Rendering & Virtualisierung: 1.5 Tage
- Selection & Binding: 0.5 Tage
- Column-Typen (Text, Checkbox, Button): 1 Tag
- Tests & Feinschliff: 0.5-1.5 Tage

---

### CTRL-004: Menu / ContextMenu

| | |
|---|---|
| **Typ** | Neues Control |
| **Priorität** | 🟡 Hoch |
| **Zeitschätzung** | 3-4 Tage |

**Beschreibung:**
Hierarchisches Menü-System für Anwendungsmenüs und Kontextmenüs (Rechtsklick). Standard auf Desktop-Plattformen.

**Anforderungen:**

- [ ] `Menu` Klasse (Hauptmenü-Leiste)
- [ ] `ContextMenu` Klasse (Popup-Menü)
- [ ] `MenuItem` Klasse mit Text, Icon, Shortcut, Command
- [ ] `MenuSeparator` Klasse
- [ ] Verschachtelte Untermenüs (SubItems)
- [ ] Property `Items` (Collection)
- [ ] Property `IsEnabled` pro MenuItem
- [ ] Property `IsChecked` für Toggle-MenuItems
- [ ] Fluent API: `AddItem()`, `AddSeparator()`, `SetIcon()`, `SetShortcut()`
- [ ] Command-Binding für MenuItems
- [ ] Keyboard-Navigation (Pfeiltasten, Enter, Escape)
- [ ] ContextMenu an beliebiges Control anhängen

**API-Beispiel:**
```csharp
// Hauptmenü
new Menu()
    .AddItem(new MenuItem()
        .SetText("Datei")
        .AddSubItem(new MenuItem().SetText("Neu").SetShortcut("Ctrl+N").SetCommand(vm.NewCommand))
        .AddSubItem(new MenuItem().SetText("Öffnen").SetShortcut("Ctrl+O"))
        .AddSubItem(new MenuSeparator())
        .AddSubItem(new MenuItem().SetText("Beenden")))

// ContextMenu
myControl.SetContextMenu(new ContextMenu()
    .AddItem(new MenuItem().SetText("Kopieren").SetCommand(vm.CopyCommand))
    .AddItem(new MenuItem().SetText("Einfügen").SetCommand(vm.PasteCommand)))
```

**Abhängigkeiten:** Popup/Overlay-System (vorhanden)

**Aufwandsverteilung:**
- MenuItem & MenuSeparator: 0.5 Tage
- Menu (Hauptleiste): 0.5 Tage
- ContextMenu & Positioning: 0.5 Tage
- Submenüs (verschachtelt): 0.75 Tage
- Keyboard-Navigation: 0.5 Tage
- Shortcut-Anzeige & Icons: 0.25 Tage
- Tests & Feinschliff: 0.5-1 Tag

---

### CTRL-005: TreeView

| | |
|---|---|
| **Typ** | Neues Control |
| **Priorität** | 🟡 Hoch |
| **Zeitschätzung** | 3-5 Tage |

**Beschreibung:**
Hierarchische Baumdarstellung von Daten. Wichtig für Dateiexplorer, Kategorien, Organisationsstrukturen und verschachtelte Daten.

**Anforderungen:**

- [ ] `TreeView` Klasse
- [ ] `TreeViewItem` Klasse mit Children, IsExpanded, IsSelected
- [ ] Property `ItemsSource` (hierarchische Collection)
- [ ] Property `SelectedItem`
- [ ] Property `ItemTemplate` für Custom-Rendering
- [ ] Property `ChildrenSelector` (Func um Children zu ermitteln)
- [ ] Expand/Collapse Icons (+/-)
- [ ] Fluent API: `SetItemsSource()`, `SetItemTemplate()`, `SetChildrenSelector()`
- [ ] Binding: `BindItemsSource()`, `BindSelectedItem()`
- [ ] Lazy Loading für große Bäume
- [ ] Keyboard-Navigation (Pfeiltasten, Enter für Expand)
- [ ] Virtualisierung für Performance

**API-Beispiel:**
```csharp
new TreeView<FolderItem>()
    .SetItemsSource(() => vm.RootFolders)
    .SetChildrenSelector(f => f.SubFolders)
    .SetItemTemplate(f => new HStack().Children(
        new Image().SetSource(f.Icon),
        new Label().SetText(f.Name)
    ))
    .BindSelectedItem(nameof(vm.SelectedFolder), () => vm.SelectedFolder, v => vm.SelectedFolder = v)
```

**Abhängigkeiten:** Basiert auf ItemsList-Konzepten

**Aufwandsverteilung:**
- TreeViewItem Struktur: 0.5 Tage
- Hierarchisches Layout & Indentation: 1 Tag
- Expand/Collapse Logic & Icons: 0.5 Tage
- Selection & Navigation: 0.5 Tage
- ItemTemplate & Binding: 0.5 Tage
- Virtualisierung (optional): 0.5-1 Tag
- Tests & Feinschliff: 0.5-1 Tag

---

## PLATTFORMEN

---

### PLAT-001: Android - Keyboard & Gesten vervollständigen

| | |
|---|---|
| **Typ** | Plattform-Verbesserung |
| **Priorität** | 🟡 Hoch |
| **Zeitschätzung** | 2-3 Tage |

**Beschreibung:**
Die Android-Implementierung hat grundlegendes Touch/Tap-Handling aber benötigt erweiterte Keyboard-Unterstützung und Gesten-Erkennung.

**Anforderungen:**

**Keyboard:**
- [ ] Vollständige Hardware-Keyboard-Unterstützung
- [ ] Korrektes Key-Mapping für alle Tasten
- [ ] IME-Handling für verschiedene Sprachen
- [ ] Keyboard-Events an InputService weiterleiten

**Gesten:**
- [ ] Long-Press erkennen
- [ ] Swipe-Gesten (Left, Right, Up, Down)
- [ ] Pinch-to-Zoom (für zukünftige Controls)
- [ ] Double-Tap

**Sonstiges:**
- [ ] Back-Button-Handling (Hardware/Navigation)
- [ ] Soft-Keyboard Show/Hide Events
- [ ] Screen-Rotation-Handling

**Dateien:** `/home/user/PlusUi/source/PlusUi.droid/`

**Abhängigkeiten:** Keine

**Aufwandsverteilung:**
- Hardware-Keyboard Mapping: 0.5 Tage
- GestureDetector erweitern: 1 Tag
- Back-Button Integration: 0.25 Tage
- Keyboard Show/Hide Events: 0.25 Tage
- Tests auf echtem Device: 0.5-1 Tag

---

### PLAT-002: iOS - Touch-Gesten erweitern

| | |
|---|---|
| **Typ** | Plattform-Verbesserung |
| **Priorität** | 🟡 Hoch |
| **Zeitschätzung** | 2-4 Tage |

**Beschreibung:**
Die iOS-Implementierung hat grundlegendes Touch-Handling aber benötigt erweiterte Gesten-Erkennung für native iOS UX.

**Anforderungen:**

**Gesten:**
- [ ] Long-Press Gesture Recognizer
- [ ] Swipe Gesture Recognizer (alle Richtungen)
- [ ] Pinch Gesture Recognizer
- [ ] Pan Gesture Recognizer (für Drag)
- [ ] Edge-Swipe für Back-Navigation

**Keyboard:**
- [ ] Keyboard-Avoidance (ScrollView automatisch anpassen)
- [ ] Hardware-Keyboard-Support (iPad)
- [ ] Keyboard-Toolbar (Done-Button etc.)

**Sonstiges:**
- [ ] Safe-Area-Insets berücksichtigen (Notch, Home-Indicator)
- [ ] Dark-Mode System-Integration
- [ ] Haptic Feedback Integration

**Dateien:** `/home/user/PlusUi/source/PlusUi.ios/`

**Abhängigkeiten:** Keine

**Aufwandsverteilung:**
- Gesture Recognizers hinzufügen: 1 Tag
- Keyboard-Avoidance: 0.5 Tage
- Safe-Area-Insets: 0.25 Tage
- Edge-Swipe Navigation: 0.25 Tage
- Tests auf echtem Device: 0.5-1.5 Tage

---

### PLAT-003: Web - DPI-Skalierung und Verbesserungen

| | |
|---|---|
| **Typ** | Plattform-Verbesserung |
| **Priorität** | 🟡 Mittel |
| **Zeitschätzung** | 1-2 Tage |

**Beschreibung:**
Die Web/Blazor-Implementierung ist neu und benötigt einige Verbesserungen für Production-Readiness.

**Anforderungen:**

**DPI/Scaling:**
- [ ] `window.devicePixelRatio` abfragen via JS Interop
- [ ] Canvas-Größe dynamisch anpassen (nicht hardcoded)
- [ ] Resize-Events behandeln

**Input:**
- [ ] Vollständiges Keyboard-Event-Handling
- [ ] Touch-Events für Mobile Browser
- [ ] Clipboard-Support (Copy/Paste)

**Sonstiges:**
- [ ] Browser-Back-Button Integration
- [ ] URL-Routing (optional)
- [ ] Loading-State während WASM-Init

**Dateien:** `/home/user/PlusUi/source/PlusUi.Web/`

**Abhängigkeiten:** Keine

**Aufwandsverteilung:**
- JS Interop für DPI: 0.25 Tage
- Dynamic Canvas Sizing: 0.25 Tage
- Keyboard-Events komplett: 0.25 Tage
- Touch-Events für Mobile: 0.25 Tage
- Tests in verschiedenen Browsern: 0.25-0.5 Tage

---

## ZUSAMMENFASSUNG

### Nach Priorität sortiert

| ID | Titel | Priorität | Zeit |
|----|-------|-----------|------|
| **CTRL-002** | DataGrid | 🔴 Kritisch | 5-7 Tage |
| | **Kritisch Gesamt** | | **5-7 Tage** |
| **CTRL-004** | Menu/ContextMenu | 🟡 Hoch | 3-4 Tage |
| **CTRL-005** | TreeView | 🟡 Hoch | 3-5 Tage |
| **PLAT-001** | Android Verbesserungen | 🟡 Hoch | 2-3 Tage |
| **PLAT-002** | iOS Verbesserungen | 🟡 Hoch | 2-4 Tage |
| **PLAT-003** | Web Verbesserungen | 🟡 Mittel | 1-2 Tage |
| | **Hoch/Mittel Gesamt** | | **11-18 Tage** |

### Empfohlene Reihenfolge für MVP

1. **CTRL-002** DataGrid (komplex, aber wichtig)
2. **PLAT-001/002/003** Plattform-Verbesserungen

### Gesamtaufwand

| Szenario | Aufwand |
|----------|---------|
| **MVP (nur Kritisch)** | 5-7 Tage |
| **Vollständig** | 16-25 Tage |

---

*Zeitschätzungen basieren auf einem erfahrenen Entwickler mit Kenntnis der Codebasis. Faktoren wie Testing, Code Review und unvorhergesehene Komplexität können die tatsächliche Zeit beeinflussen.*
