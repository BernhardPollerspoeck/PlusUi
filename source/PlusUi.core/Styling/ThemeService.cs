using Microsoft.Extensions.DependencyInjection;

namespace PlusUi.core;

public class ThemeService : IThemeService
{
    public string CurrentTheme { get; private set; } = Theme.Default.ToString();

    public bool SetTheme(Theme theme)
    {
        CurrentTheme = theme.ToString();
        var container = ServiceProviderService.ServiceProvider?.GetRequiredService<NavigationContainer>();
        container?.CurrentPage.ApplyStyles();
        return true;
    }

    public bool SetTheme(string theme)
    {
        CurrentTheme = theme;
        var container = ServiceProviderService.ServiceProvider?.GetRequiredService<NavigationContainer>();
        container?.CurrentPage.ApplyStyles();
        return true;
    }
}
