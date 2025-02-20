namespace PlusUi.core;

public interface IThemeService
{
    string CurrentTheme { get; }
    bool SetStyle(Theme theme);
    bool SetStyle(string theme);
}
