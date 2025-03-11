using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlusUi.core.Services;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(PlusUiHotReloadManager))]

namespace PlusUi.core.Services;

internal class PlusUiHotReloadManager
{

    public static void ClearCache(Type[]? _)
    {
        //we dont maintain internal caches currently, so nothing has to be done here
    }

    public static void UpdateApplication(Type[]? updatedTypes)
    {
        var logger = ServiceProviderService.ServiceProvider?.GetRequiredService<ILogger<PlusUiHotReloadManager>>();

        logger?.LogInformation("HotReload is updating the following Types:");
        foreach (var type in updatedTypes ?? [])
        {
            logger?.LogInformation("- {name}", type.FullName);
        }

        var navigationContainer = ServiceProviderService.ServiceProvider?.GetRequiredService<NavigationContainer>();
        if (updatedTypes?
            .Any(t => t == navigationContainer?.Page.GetType()
                || t == navigationContainer?.Page.ViewModel.GetType()
                || t.IsAssignableTo(typeof(UserControl))) is true)
        {
            var internalNavigationService = ServiceProviderService.ServiceProvider?.GetRequiredService<PlusUiNavigationService>();
            internalNavigationService?.Initialize();
            logger?.LogInformation("NavigationContainer updated");
        }
    }
}
