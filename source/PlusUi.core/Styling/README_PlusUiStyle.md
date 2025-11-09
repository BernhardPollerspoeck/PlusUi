# PlusUiStyle - Modern Theming for PlusUi

Eine produktionsreife Style-Klasse mit Light und Dark Mode Support für alle PlusUi Controls.

## Features

✅ **Vollständige Theme-Unterstützung** - Light & Dark Mode mit modernen Farbpaletten
✅ **Alle Controls gestylt** - Einheitliches Design für Button, Entry, Label, Toggle, Slider, etc.
✅ **Modern Design** - Inspiriert von Tailwind CSS und modernen Design-Systemen
✅ **Sofort einsatzbereit** - Direkt nutzen oder als Vorlage für eigene Styles
✅ **Schatten & Radien** - Subtile Schatten und abgerundete Ecken für moderne UI
✅ **Dokumentiert** - Klar strukturiert mit Kommentaren

## Verwendung

### Basis-Setup

```csharp
// In App.cs oder wo du deine App konfigurierst
builder.StylePlusUi<PlusUiStyle>();
```

### Theme wechseln

```csharp
public class MyPage(IThemeService themeService) : UiPageElement
{
    protected override UiElement Build()
    {
        return new VStack(
            new Button()
                .SetText("Light Mode")
                .OnClick(() => themeService.SetTheme(Theme.Light)),

            new Button()
                .SetText("Dark Mode")
                .OnClick(() => themeService.SetTheme(Theme.Dark))
        );
    }
}
```

### Eigene Anpassungen

Du kannst PlusUiStyle als Basis verwenden und einzelne Controls überschreiben:

```csharp
public class MyCustomStyle : PlusUiStyle
{
    public override void ConfigureStyle(Style style)
    {
        // Basis-Styles von PlusUiStyle anwenden
        base.ConfigureStyle(style);

        // Eigene Anpassungen hinzufügen
        style.AddStyle<Button>(Theme.Light, element => element
            .SetCornerRadius(20)  // Runder Button
            .SetBackground(new SolidColorBackground(SKColor.Parse("#ff6b6b"))));
    }
}
```

## Farbpalette

### Light Theme

- **Primary**: Blue 600 (#2563eb)
- **Background**: White (#ffffff)
- **Surface**: Slate 50/100
- **Text**: Slate 900/500
- **Success**: Green 600
- **Error**: Red 600

### Dark Theme

- **Primary**: Blue 500 (#3b82f6)
- **Background**: Slate 900 (#0f172a)
- **Surface**: Slate 800/700
- **Text**: Slate 50/400
- **Success**: Green 500
- **Error**: Red 500

## Gestylte Controls

- ✅ **Text**: Label, Link
- ✅ **Input**: Entry (mit Placeholder-Support)
- ✅ **Buttons**: Button
- ✅ **Selection**: Checkbox, Toggle, Slider
- ✅ **Progress**: ProgressBar, ActivityIndicator
- ✅ **Visual**: Border, Separator, Solid
- ✅ **Layout**: HStack, VStack, Grid, ScrollView (ohne Margins)

## Demo

Schau dir die **StyleDemoPage** im Sandbox-Projekt an für ein Live-Beispiel aller gestylten Controls!

```bash
# Sandbox starten und "🎨 PlusUiStyle Demo" klicken
```

## Von Grund auf selbst erstellen

Wenn du komplett eigene Styles möchtest:

1. **Kopiere** `PlusUiStyle.cs` als Vorlage
2. **Ändere** die Farben in den Color Palette Regionen
3. **Passe** Größen und Abstände an (Sizing & Spacing Constants)
4. **Registriere** deine Style-Klasse in `App.cs`

## Tipps

- **Default Theme**: Wird immer zuerst angewendet, dann das aktuelle Theme
- **Layout-Container**: HStack, VStack, Grid haben keine Margins, damit du flexibel bist
- **Per-Control Override**: `.IgnoreStyling()` um Styles für einzelne Controls zu ignorieren
- **Page-Styles**: Überschreibe `ConfigurePageStyles()` für seitenspezifische Styles

---

**Viel Spaß beim Stylen! 🎨**
