# PlusUi - Backlog für Marktreife

> Erstellt: 2024-12-14
> Status: Audit abgeschlossen

---

## Übersicht

| Kategorie | Anzahl | Geschätzte Gesamtzeit |
|-----------|--------|----------------------|
| Controls (Kritisch) | 5 | 13-19 Tage |
| Controls (Hoch) | 2 | 6-9 Tage |
| Features | 3 | 8-14 Tage |
| Plattformen | 3 | 6-10 Tage |
| **Gesamt** | **13** | **33-52 Tage** |

---

## CONTROLS

---

### CTRL-001: TabControl

| | |
|---|---|
| **Typ** | Neues Control |
| **Priorität** | 🔴 Kritisch |
| **Zeitschätzung** | 2-3 Tage |

**Beschreibung:**
Ein Container-Control das mehrere Views in Tabs organisiert. Der Benutzer kann zwischen Tabs wechseln um verschiedene Inhalte anzuzeigen. Standard-UI-Pattern für Einstellungen, Multi-Document-Interfaces und kategorisierte Inhalte.

**Anforderungen:**

- [ ] `TabControl` Klasse die von `UiLayoutElement` erbt
- [ ] `TabItem` Klasse für einzelne Tabs mit Header und Content
- [ ] Property `Tabs` (Collection von TabItems)
- [ ] Property `SelectedIndex` / `SelectedTab`
- [ ] Property `TabPosition` (Top, Bottom, Left, Right)
- [ ] Fluent API: `SetTabs()`, `AddTab()`, `SetSelectedIndex()`, `SetTabPosition()`
- [ ] Binding: `BindSelectedIndex()`, `BindTabs()`
- [ ] Event/Command bei Tab-Wechsel
- [ ] Styling für Tab-Header (Active/Inactive States)
- [ ] Keyboard-Support (Pfeiltasten zum Wechseln)

**API-Beispiel:**
```csharp
new TabControl()
    .AddTab(new TabItem()
        .SetHeader("Allgemein")
        .SetContent(new VStack().Children(...)))
    .AddTab(new TabItem()
        .SetHeader("Erweitert")
        .SetContent(new VStack().Children(...)))
    .SetSelectedIndex(0)
    .SetTabPosition(TabPosition.Top)
```

**Abhängigkeiten:** Keine

**Aufwandsverteilung:**
- Grundstruktur & Layout: 0.5 Tage
- Tab-Header Rendering & Styling: 0.5 Tage
- Selection-Logic & Events: 0.5 Tage
- Keyboard-Support: 0.25 Tage
- Tests & Feinschliff: 0.25-1 Tag

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

### CTRL-003: DatePicker

| | |
|---|---|
| **Typ** | Neues Control |
| **Priorität** | 🔴 Kritisch |
| **Zeitschätzung** | 3-4 Tage |

**Beschreibung:**
Control zur Auswahl eines Datums. Zeigt aktuelles Datum und öffnet bei Klick einen Kalender-Popup zur Auswahl. Standard in jeder Formular-Anwendung.

**Anforderungen:**

- [ ] `DatePicker` Klasse
- [ ] Property `SelectedDate` (DateTime? oder DateOnly?)
- [ ] Property `MinDate` / `MaxDate` für Bereichseinschränkung
- [ ] Property `DisplayFormat` (z.B. "dd.MM.yyyy")
- [ ] Property `Placeholder`
- [ ] Kalender-Popup mit Monats-/Jahresnavigation
- [ ] Fluent API: `SetSelectedDate()`, `SetMinDate()`, `SetMaxDate()`, `SetDisplayFormat()`
- [ ] Binding: `BindSelectedDate()` (Two-Way)
- [ ] Wochenstart konfigurierbar (Montag/Sonntag)
- [ ] Heute-Button im Kalender
- [ ] Keyboard-Support (Datum direkt tippen)

**API-Beispiel:**
```csharp
new DatePicker()
    .SetPlaceholder("Geburtsdatum wählen")
    .SetDisplayFormat("dd.MM.yyyy")
    .SetMinDate(new DateOnly(1900, 1, 1))
    .SetMaxDate(DateOnly.FromDateTime(DateTime.Today))
    .BindSelectedDate(nameof(vm.BirthDate), () => vm.BirthDate, v => vm.BirthDate = v)
```

**Abhängigkeiten:** Popup-System (vorhanden)

**Aufwandsverteilung:**
- DatePicker-Control Grundstruktur: 0.5 Tage
- Kalender-Grid Layout: 1 Tag
- Monats-/Jahresnavigation: 0.5 Tage
- Min/Max-Validierung & Styling: 0.5 Tage
- Keyboard-Input: 0.25 Tage
- Tests & Feinschliff: 0.25-1 Tag

---

### CTRL-004: TimePicker

| | |
|---|---|
| **Typ** | Neues Control |
| **Priorität** | 🔴 Kritisch |
| **Zeitschätzung** | 1.5-2 Tage |

**Beschreibung:**
Control zur Auswahl einer Uhrzeit. Zeigt aktuelle Zeit und öffnet bei Klick einen Time-Selector. Wichtig für Termin- und Planungsanwendungen.

**Anforderungen:**

- [ ] `TimePicker` Klasse
- [ ] Property `SelectedTime` (TimeSpan? oder TimeOnly?)
- [ ] Property `MinTime` / `MaxTime`
- [ ] Property `DisplayFormat` (z.B. "HH:mm" oder "hh:mm tt")
- [ ] Property `MinuteIncrement` (1, 5, 15, 30)
- [ ] Property `Is24HourFormat`
- [ ] Popup mit Stunden/Minuten-Auswahl (Spinner oder Dropdown)
- [ ] Fluent API: `SetSelectedTime()`, `SetMinuteIncrement()`, `Set24HourFormat()`
- [ ] Binding: `BindSelectedTime()` (Two-Way)

**API-Beispiel:**
```csharp
new TimePicker()
    .SetSelectedTime(new TimeOnly(9, 0))
    .SetMinuteIncrement(15)
    .Set24HourFormat(true)
    .BindSelectedTime(nameof(vm.StartTime), () => vm.StartTime, v => vm.StartTime = v)
```

**Abhängigkeiten:** Popup-System (vorhanden)

**Aufwandsverteilung:**
- TimePicker-Control Grundstruktur: 0.5 Tage
- Stunden/Minuten-Auswahl UI: 0.5 Tage
- Format & Increment Logic: 0.25 Tage
- Tests & Feinschliff: 0.25-0.75 Tage

---

### CTRL-005: RadioButton / RadioGroup

| | |
|---|---|
| **Typ** | Neues Control |
| **Priorität** | 🔴 Kritisch |
| **Zeitschätzung** | 1-1.5 Tage |

**Beschreibung:**
Gruppe von Auswahloptionen bei der nur eine Option gleichzeitig ausgewählt sein kann. Standard für exklusive Auswahlen (Geschlecht, Zahlungsart, etc.).

**Anforderungen:**

- [ ] `RadioButton` Klasse (einzelner Button)
- [ ] `RadioGroup` Klasse (Container für RadioButtons)
- [ ] Property `IsSelected` (RadioButton)
- [ ] Property `GroupName` (zur Gruppierung ohne Container)
- [ ] Property `SelectedValue` / `SelectedIndex` (RadioGroup)
- [ ] Property `Orientation` (Horizontal/Vertical)
- [ ] Fluent API: `SetIsSelected()`, `SetGroupName()`, `SetOrientation()`
- [ ] Binding: `BindSelectedValue()` (Two-Way)
- [ ] Automatische Deselektion anderer Buttons in Gruppe
- [ ] Keyboard-Support (Pfeiltasten)

**API-Beispiel:**
```csharp
new RadioGroup()
    .SetOrientation(Orientation.Vertical)
    .Children(
        new RadioButton().SetText("Option A").SetValue("A"),
        new RadioButton().SetText("Option B").SetValue("B"),
        new RadioButton().SetText("Option C").SetValue("C")
    )
    .BindSelectedValue(nameof(vm.Choice), () => vm.Choice, v => vm.Choice = v)
```

**Abhängigkeiten:** Keine (ähnlich wie Checkbox)

**Aufwandsverteilung:**
- RadioButton Rendering (Kreis): 0.25 Tage
- RadioGroup Container & Selection-Logic: 0.5 Tage
- Binding & Keyboard: 0.25 Tage
- Tests: 0.25-0.5 Tage

---

### CTRL-006: Menu / ContextMenu

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

### CTRL-007: TreeView

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

## FEATURES

---

### FEAT-001: Accessibility (Barrierefreiheit)

| | |
|---|---|
| **Typ** | Neues Feature |
| **Priorität** | 🔴 Kritisch |
| **Zeitschätzung** | 5-8 Tage |

**Beschreibung:**
Unterstützung für Barrierefreiheit um das Framework für Menschen mit Behinderungen nutzbar zu machen. Rechtlich relevant (EU Accessibility Act 2025, ADA in USA).

**Anforderungen:**

**Screen Reader Support:**
- [ ] `AccessibilityLabel` Property auf allen Controls
- [ ] `AccessibilityHint` Property für zusätzliche Beschreibung
- [ ] `AccessibilityRole` Property (Button, Checkbox, Heading, etc.)
- [ ] Platform-spezifische Integration (Windows: UI Automation, macOS: NSAccessibility, etc.)

**Keyboard Navigation:**
- [ ] Vollständige Tab-Navigation (siehe FEAT-002)
- [ ] Skip-Links für Hauptbereiche
- [ ] Focus-Indicator (sichtbarer Fokus-Ring)

**Visual:**
- [ ] High-Contrast-Mode Support
- [ ] Minimum Touch-Target-Größe (44x44 pt)
- [ ] Skalierbare Schriftgrößen

**API-Beispiel:**
```csharp
new Button()
    .SetText("X")
    .SetAccessibilityLabel("Schließen")
    .SetAccessibilityHint("Schließt das aktuelle Fenster")
    .SetAccessibilityRole(AccessibilityRole.Button)

new Image()
    .SetSource("logo.png")
    .SetAccessibilityLabel("Firmenlogo")
```

**Abhängigkeiten:** Focus/Tab-Navigation (FEAT-002)

**Aufwandsverteilung:**
- Accessibility-Properties auf UiElement: 0.5 Tage
- AccessibilityRole Enum & Defaults: 0.25 Tage
- Focus-Ring Rendering: 0.5 Tage
- Windows UI Automation Integration: 1-2 Tage
- macOS NSAccessibility Integration: 1-2 Tage
- Android/iOS Accessibility: 1-2 Tage
- Tests: 0.5-1 Tag

**Hinweis:** Kann schrittweise implementiert werden. Minimum für Start: AccessibilityLabel + Focus-Ring + Keyboard-Navigation.

---

### FEAT-002: Focus und Tab-Navigation

| | |
|---|---|
| **Typ** | Neues Feature |
| **Priorität** | 🔴 Kritisch |
| **Zeitschätzung** | 2-3 Tage |

**Beschreibung:**
Vollständiges Focus-Management für Keyboard-Navigation. Ermöglicht Benutzern die Anwendung ohne Maus zu bedienen.

**Anforderungen:**

**Focus-System:**
- [ ] `IsFocusable` Property auf allen Controls
- [ ] `IsFocused` Property (readonly)
- [ ] `Focus()` Methode zum programmatischen Fokussieren
- [ ] `FocusNext()` / `FocusPrevious()` Methoden
- [ ] Globaler Focus-Manager Service

**Tab-Navigation:**
- [ ] `TabIndex` Property (Reihenfolge)
- [ ] `TabStop` Property (true/false - soll Tab hier stoppen?)
- [ ] Tab-Taste navigiert zum nächsten focusbaren Element
- [ ] Shift+Tab navigiert zurück
- [ ] Tab-Gruppen/Scopes für komplexe Layouts

**Visual Feedback:**
- [ ] Focus-Ring/Indicator (konfigurierbar)
- [ ] `FocusedBackground` / `FocusedBorderColor` Properties

**API-Beispiel:**
```csharp
new Entry()
    .SetTabIndex(1)
    .SetIsFocusable(true)

new Button()
    .SetTabIndex(2)
    .SetTabStop(true)

// Programmatisch fokussieren
myEntry.Focus();

// Focus-Styling
style.AddStyle<Button>(Theme.Default, b => b
    .SetFocusRingColor(SKColors.Blue)
    .SetFocusRingWidth(2));
```

**Abhängigkeiten:** InputService-Erweiterung

**Aufwandsverteilung:**
- Focus-Properties auf UiElement: 0.25 Tage
- FocusManager Service: 0.5 Tage
- Tab-Navigation Logic: 0.5 Tage
- Focus-Ring Rendering: 0.25 Tage
- Keyboard-Event-Handling (Tab/Shift+Tab): 0.25 Tage
- Tests: 0.25-0.75 Tage

---

### FEAT-003: Page Transitions (Optional)

| | |
|---|---|
| **Typ** | Neues Feature |
| **Priorität** | 🟢 Nice-to-have |
| **Zeitschätzung** | 2-3 Tage |

**Beschreibung:**
Animierte Übergänge bei Navigation zwischen Pages. Verbessert die User Experience und gibt visuelles Feedback bei Seitenwechseln.

**Anforderungen:**

- [ ] `IPageTransition` Interface
- [ ] Eingebaute Transitions: `FadeTransition`, `SlideTransition`, `NoneTransition`
- [ ] Property `Transition` auf NavigationService oder Page-Level
- [ ] Property `Duration` (Dauer in ms)
- [ ] Property `Easing` (Linear, EaseIn, EaseOut, EaseInOut)
- [ ] Forward/Backward unterschiedliche Richtungen

**API-Beispiel:**
```csharp
// Global setzen
navigationService.SetDefaultTransition(new SlideTransition()
    .SetDuration(250)
    .SetDirection(SlideDirection.Left)
    .SetEasing(Easing.EaseOut));

// Oder pro Navigation
navigationService.NavigateTo<DetailsPage>(new SlideTransition());

// Oder auf Page-Level
public class MyPage : UiPageElement
{
    public override IPageTransition Transition => new FadeTransition(200);
}
```

**Abhängigkeiten:** RenderService-Erweiterung für Animation-Loop

**Aufwandsverteilung:**
- IPageTransition Interface & Basis: 0.25 Tage
- Animation-Loop in RenderService: 0.5 Tage
- Easing-Functions: 0.25 Tage
- FadeTransition: 0.25 Tage
- SlideTransition: 0.5 Tage
- NavigationService Integration: 0.25 Tage
- Tests: 0.25-0.75 Tage

**Hinweis:** Kann komplett weggelassen werden. User können Transitions auch selbst über Property-Animation implementieren.

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
| **CTRL-001** | TabControl | 🔴 Kritisch | 2-3 Tage |
| **CTRL-002** | DataGrid | 🔴 Kritisch | 5-7 Tage |
| **CTRL-003** | DatePicker | 🔴 Kritisch | 3-4 Tage |
| **CTRL-004** | TimePicker | 🔴 Kritisch | 1.5-2 Tage |
| **CTRL-005** | RadioButton | 🔴 Kritisch | 1-1.5 Tage |
| **FEAT-002** | Focus/Tab-Navigation | 🔴 Kritisch | 2-3 Tage |
| **FEAT-001** | Accessibility | 🔴 Kritisch | 5-8 Tage |
| | **Kritisch Gesamt** | | **19.5-28.5 Tage** |
| **CTRL-006** | Menu/ContextMenu | 🟡 Hoch | 3-4 Tage |
| **CTRL-007** | TreeView | 🟡 Hoch | 3-5 Tage |
| **PLAT-001** | Android Verbesserungen | 🟡 Hoch | 2-3 Tage |
| **PLAT-002** | iOS Verbesserungen | 🟡 Hoch | 2-4 Tage |
| **PLAT-003** | Web Verbesserungen | 🟡 Mittel | 1-2 Tage |
| | **Hoch/Mittel Gesamt** | | **11-18 Tage** |
| **FEAT-003** | Page Transitions | 🟢 Optional | 2-3 Tage |
| | **Optional Gesamt** | | **2-3 Tage** |

### Empfohlene Reihenfolge für MVP

1. **FEAT-002** Focus/Tab-Navigation (Grundlage für alles)
2. **CTRL-005** RadioButton (schneller Win)
3. **CTRL-004** TimePicker (schneller Win)
4. **CTRL-001** TabControl (häufig benötigt)
5. **CTRL-003** DatePicker (häufig benötigt)
6. **CTRL-002** DataGrid (komplex, aber wichtig)
7. **FEAT-001** Accessibility (parallel entwickeln)
8. **PLAT-001/002/003** Plattform-Verbesserungen

### Gesamtaufwand

| Szenario | Aufwand |
|----------|---------|
| **MVP (nur Kritisch)** | 19.5-28.5 Tage |
| **Vollständig (ohne Optional)** | 30.5-46.5 Tage |
| **Alles inkl. Optional** | 32.5-49.5 Tage |

---

*Zeitschätzungen basieren auf einem erfahrenen Entwickler mit Kenntnis der Codebasis. Faktoren wie Testing, Code Review und unvorhergesehene Komplexität können die tatsächliche Zeit beeinflussen.*
