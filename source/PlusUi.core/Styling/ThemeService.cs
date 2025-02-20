namespace PlusUi.core;

public class ThemeService : IThemeService
{
    public string CurrentTheme { get; private set; } = Theme.Default.ToString();

    public bool SetStyle(Theme theme)
    {
        CurrentTheme = theme.ToString();
        return true;
    }

    public bool SetStyle(string theme)
    {
        CurrentTheme = theme;
        return true;
    }
}
