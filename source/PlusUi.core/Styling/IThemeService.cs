namespace PlusUi.core;

public interface IThemeService
{
    string CurrentTheme { get; }
    bool SetTheme(Theme theme);
    bool SetTheme(string theme);
}
