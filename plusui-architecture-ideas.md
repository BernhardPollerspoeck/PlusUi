# PlusUI Architecture Ideas

## DI-basierte Control Factory

### Konzept
Statt `new Button()` eine statische Factory `UI.Button` oder `UI.Button()` die intern via DI resolvet.

### Vorteile
- Keine `new` Statements mehr
- Controls returnen Interfaces (`IButton`) statt konkrete Typen
- Implementierungen austauschbar (V1, V2, Custom)
- Kunden können eigene Controls als Default registrieren
- Theoretisch: Third-Party Control Vendors könnten für PlusUI bauen

### Registrierung
```csharp
services.AddPlusUI(options => {
    options.UseButton<MyThemedButton>();
    options.UseLabel<CustomLabel>();
});
```

### Property vs Methode

**Option A: Methode**
```csharp
UI.Button().SetText("Hi")
```
- Konventionell, klar dass neue Instanz
- IDE: grün.gelb().gelb()

**Option B: Property**
```csharp
UI.Button.SetText("Hi")
```
- Unkonventionell (Getter erzeugt neue Instanz)
- IDE: grün.weiß.gelb() - bessere visuelle Differenzierung
- Entwickler legen großen Wert auf Syntax Highlighting/Farben

**Entscheidung:** Offen - beides trivial, eins wählen.

### Beispiel vorher/nachher

**Vorher:**
```csharp
return new VStack(
    new Label()
        .SetText("Counter App")
        .SetTextSize(32),
    new Button()
        .SetText("Click Me!")
        .SetCommand(vm.IncrementCommand)
);
```

**Nachher:**
```csharp
return UI.VStack(
    UI.Label()
        .SetText("Counter App")
        .SetTextSize(32),
    UI.Button()
        .SetText("Click Me!")
        .SetCommand(vm.IncrementCommand)
);
```

### Interne Umsetzung
```csharp
public static class UI
{
    private static IServiceProvider _serviceProvider;
    
    public static IButton Button() => _serviceProvider.GetRequiredService<IButton>();
    // oder als Property:
    public static IButton Button => _serviceProvider.GetRequiredService<IButton>();
}
```

## User-definierte Style Presets

### Konzept
User definiert wiederverwendbare Styles via Enum, wendet sie per `.Style()` an.

### API
```csharp
// User definiert Enum:
enum MyStyles { Header, Primary, Danger, CardTitle }

// User registriert Styles:
Styles.Define(MyStyles.Header, (Label l) => l
    .SetTextSize(32)
    .SetTextColor(SKColors.White)
    .SetHorizontalTextAlignment(HorizontalTextAlignment.Center)
    .SetMargin(0, 20));

Styles.Define(MyStyles.Primary, (Button b) => b
    .SetBackground(new SolidColorBackground(SKColors.DodgerBlue))
    .SetCornerRadius(8)
    .SetPadding(20, 10));

// Anwendung:
UI.Label("Counter App").Style(MyStyles.Header)
UI.Button("Click Me!").Style(MyStyles.Primary)
```

### Kaskadierung
Reihenfolge zählt - später überschreibt früher:
```csharp
UI.Button("Hi")
    .Style(MyStyles.Primary)
    .Style(MyStyles.Large)  // überschreibt Size-relevantes von Primary
```

Hierarchie: Global → Per Control → Preset

### Technische Umsetzung
```csharp
void Style(Enum style)
{
    int value = Convert.ToInt32(style);
    // resolve from registry
}
```

**Boxing-Overhead:** ~40 Bytes pro Call, aber Style wird nur beim Build resolved → akzeptabel. Bei 500 Controls mit 3 Style-Calls = ~60KB einmalig. Falls später problematisch → Source Generator für zero-alloc StyleKeys.


## Offene Fragen
- Interface-Design: schlank starten, Interface-Segregation (`IClickable`, `IStyleable`)?
- Performance-Session steht noch aus

---
*Notiert: Januar 2025*
