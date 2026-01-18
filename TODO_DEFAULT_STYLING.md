# Default Styling für PlusUi Controls

## Ziel
Einheitliches, modernes Dark Theme für alle PlusUi Controls basierend auf dem DebugServer-Design.

## Referenz: DebugServer Styling

### Farbpalette (Dark Theme)

#### Hintergründe
| Verwendung | Farbe | Code |
|------------|-------|------|
| Page Background | Sehr dunkel | `Color(25, 25, 25)` |
| Content Background | Dunkel | `Color(30, 30, 30)` |
| Panel/Card Background | Mittel-dunkel | `Color(40, 40, 40)` oder `Color(45, 45, 45)` |
| Tab Header Background | | `Color(35, 35, 35)` |
| Active Tab Background | | `Color(45, 45, 45)` oder `Color(50, 50, 50)` |
| Button Background | | `Color(60, 60, 60)` |
| Button Hover | | `Color(70, 70, 70)` |
| Entry/Input Background | | `Color(40, 40, 40)` |
| Line/Border Color | | `Color(60, 60, 60)` |

#### Text Farben
| Verwendung | Farbe | Code |
|------------|-------|------|
| Primary Text | Weiß | `Colors.White` |
| Secondary Text | Hellgrau | `Colors.LightGray` |
| Muted/Disabled | Grau | `Colors.Gray` oder `Color(120, 120, 120)` oder `Color(150, 150, 150)` |
| Accent/Links | Hellblau | `Colors.LightBlue` |
| Highlight/Active | Gelb | `Colors.Yellow` |
| Success | Grün | `Color(100, 200, 100)` |
| Info | Blau | `Color(100, 150, 255)` oder `Color(100, 200, 255)` |
| Warning | Orange | `Color(255, 180, 100)` |
| Special | Lila | `Color(200, 100, 200)` |

#### Abstände & Größen
| Element | Wert |
|---------|------|
| CornerRadius (Buttons) | 3-4px |
| CornerRadius (Cards/Panels) | 4-8px |
| Padding (Buttons) | `Margin(8, 4)` oder `Margin(6, 4)` |
| Padding (Cards) | `Margin(16, 12)` oder `Margin(16)` |
| Margin (Panels) | 8px |
| Spacing (HStack/VStack) | 4-12px |
| TextSize (Small) | 11px |
| TextSize (Normal) | 12-13px |
| TextSize (Header) | 14-16px |
| TextSize (Large Value) | 20px |

### Beispiel-Komponenten aus DebugServer

#### Button
```csharp
new Button()
    .SetText("Action")
    .SetTextColor(Colors.White)
    .SetTextSize(12)
    .SetBackground(new Color(60, 60, 60))
    .SetHoverBackground(new SolidColorBackground(new Color(70, 70, 70)))
    .SetCornerRadius(4)
    .SetPadding(new Margin(8, 4))
```

#### Card/Panel
```csharp
new Border()
    .SetBackground(new Color(45, 45, 45))
    .SetCornerRadius(8)
    .SetMargin(new Margin(8))
    .AddChild(content)
```

#### Entry/Input
```csharp
new Entry()
    .SetTextColor(Colors.LightGray)
    .SetTextSize(12)
    .SetBackground(new Color(40, 40, 40))
    .SetPadding(new Margin(4, 2))
```

#### TabControl
```csharp
new TabControl()
    .SetHeaderBackgroundColor(new Color(35, 35, 35))
    .SetActiveTabBackgroundColor(new Color(50, 50, 50))
    .SetHeaderTextSize(13)
```

#### TreeView
```csharp
new TreeView()
    .SetItemHeight(28)
    .SetIndentation(20)
    .SetExpanderSize(14)
    .SetShowLines(true)
    .SetLineColor(new Color(60, 60, 60))
    .SetBackground(new Color(30, 30, 30))
```

---

## PlusUi.Demo Struktur

### Seiten
1. **MainPage (Introduction)**
   - Willkommenstext
   - Kurze Beschreibung von PlusUi
   - Button-Liste zu allen Control-Demo-Seiten

2. **Control Demo Pages** (je eine Seite pro Control)
   - Button
   - Label
   - Entry
   - Checkbox
   - RadioButton
   - ComboBox
   - DatePicker / TimePicker
   - TabControl
   - TreeView
   - DataGrid
   - ScrollView
   - Image
   - Border
   - Grid / VStack / HStack
   - Menu
   - Toolbar
   - ... (weitere Controls)

### Aufbau jeder Demo-Seite
- Titel des Controls
- Kurze Beschreibung
- Live-Beispiele mit verschiedenen Varianten
- Code-Beispiele (optional)

---

## TODO

- [ ] Default Styles für alle Controls definieren
- [ ] StyleSheet/Theme-System implementieren (falls noch nicht vorhanden)
- [ ] MainPage mit Introduction und Navigation erstellen
- [ ] Demo-Seiten für jeden Control-Typ erstellen
- [ ] Konsistentes Styling über alle Controls hinweg sicherstellen
