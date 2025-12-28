# PlusUi - Backlog für Marktreife

> Erstellt: 2024-12-14
> Aktualisiert: 2024-12-28

---

## Übersicht

| Kategorie | Anzahl | Geschätzte Gesamtzeit |
|-----------|--------|----------------------|
| Controls (Hoch) | 1 | 3-4 Tage |
| **Gesamt** | **1** | **3-4 Tage** |

---

## CONTROLS

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

## ZUSAMMENFASSUNG

### Nach Priorität sortiert

| ID | Titel | Priorität | Zeit |
|----|-------|-----------|------|
| **CTRL-004** | Menu/ContextMenu | 🟡 Hoch | 3-4 Tage |
| | **Hoch Gesamt** | | **3-4 Tage** |

### Gesamtaufwand

| Szenario | Aufwand |
|----------|---------|
| **Vollständig** | 3-4 Tage |

---

*Zeitschätzungen basieren auf einem erfahrenen Entwickler mit Kenntnis der Codebasis. Faktoren wie Testing, Code Review und unvorhergesehene Komplexität können die tatsächliche Zeit beeinflussen.*
