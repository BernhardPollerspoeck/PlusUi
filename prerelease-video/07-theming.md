# 7. Theming

## Features

- Built-in themes: Default, Light, Dark
- Custom themes (any string name)
- Type-safe styles per control type
- Per-theme style overrides
- Per-page style overrides
- Runtime theme switching

**Narration:**

> "Theming. Light mode, dark mode, or whatever mode you want.
>
> PlusUi comes with three built-in themes: Default, Light, and Dark. But you're not stuck with those. Create your own. Call it 'corporate-blue' or 'midnight' or whatever fits your brand.
>
> Here's how it works: you define styles per control type. All buttons get this background, all labels get this text color. That's your base. Then you add overrides per theme. In dark mode, buttons get a different background. In your custom theme, something else entirely.
>
> Need something different on a specific page? Per-page style overrides. Your settings page can have its own look without affecting the rest of the app.
>
> It's type-safe. You're not writing CSS selectors and hoping they match. You're writing C# - 'for all Buttons, do this'. The compiler has your back.
>
> And you can switch themes at runtime. One line. The whole UI updates. User toggles dark mode in settings? Done. No restart, no reload, just instant theme change.
>
> Define once. Override per theme. Override per page. Switch whenever you want."

## Visuals

- Dunkler Hintergrund (#1E1E1E)
- Code-Block groß und lesbar (Syntax Highlighting)
- Sprecher in Ecke

```csharp
// App-wide styles (in your Style class)
public class AppStyle(IThemeService themeService) : Style(themeService)
{
    public AppStyle Configure()
    {
        // Base style for all Buttons
        AddStyle<Button>(b => b
            .SetCornerRadius(8)
            .SetPadding(new Margin(16, 8)));

        // Theme overrides
        AddStyle<Button>(Theme.Dark, b => b
            .SetBackground(new SolidColorBackground(Colors.DarkGray))
            .SetTextColor(Colors.White));

        AddStyle<Button>(Theme.Light, b => b
            .SetBackground(new SolidColorBackground(Colors.LightGray))
            .SetTextColor(Colors.Black));

        // Custom theme
        AddStyle<Button>("corporate-blue", b => b
            .SetBackground(new SolidColorBackground(Colors.Navy))
            .SetTextColor(Colors.White));

        return this;
    }
}

// Per-page style override
public class SettingsPage(SettingsViewModel vm) : UiPageElement(vm)
{
    public override Style? PageStyle => new Style(ThemeService)
        .AddStyle<Button>(b => b
            .SetCornerRadius(16)
            .SetBackground(new SolidColorBackground(Colors.Orange)));

    protected override UiElement Build() => ...
}

// Switch theme at runtime
themeService.SetTheme(Theme.Dark);
themeService.SetTheme("corporate-blue");
```

**Übergang zu Section 8:** Slide (Page Transition)
