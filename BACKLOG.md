# PlusUi - Backlog für Marktreife

> Erstellt: 2024-12-14
> Aktualisiert: 2024-12-28

---

## Übersicht

| Kategorie | Anzahl | Geschätzte Gesamtzeit |
|-----------|--------|----------------------|
| Controls (Kritisch) | 1 | 5-7 Tage |
| Controls (Hoch) | 2 | 6-9 Tage |
| **Gesamt** | **3** | **11-16 Tage** |

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
- [ ] Verschachtelte Untermenüs (MenuItem hat selbst Items-Collection)
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
        .AddItem(new MenuItem().SetText("Neu").SetShortcut("Ctrl+N").SetCommand(vm.NewCommand))
        .AddItem(new MenuItem().SetText("Öffnen").SetShortcut("Ctrl+O"))
        .AddItem(new MenuSeparator())
        .AddItem(new MenuItem().SetText("Beenden")))

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

## ZUSAMMENFASSUNG

### Nach Priorität sortiert

| ID | Titel | Priorität | Zeit |
|----|-------|-----------|------|
| **CTRL-002** | DataGrid | 🔴 Kritisch | 5-7 Tage |
| | **Kritisch Gesamt** | | **5-7 Tage** |
| **CTRL-004** | Menu/ContextMenu | 🟡 Hoch | 3-4 Tage |
| **CTRL-005** | TreeView | 🟡 Hoch | 3-5 Tage |
| | **Hoch Gesamt** | | **6-9 Tage** |

### Empfohlene Reihenfolge für MVP

1. **CTRL-002** DataGrid (komplex, aber wichtig)

### Gesamtaufwand

| Szenario | Aufwand |
|----------|---------|
| **MVP (nur Kritisch)** | 5-7 Tage |
| **Vollständig** | 11-16 Tage |

---

*Zeitschätzungen basieren auf einem erfahrenen Entwickler mit Kenntnis der Codebasis. Faktoren wie Testing, Code Review und unvorhergesehene Komplexität können die tatsächliche Zeit beeinflussen.*
