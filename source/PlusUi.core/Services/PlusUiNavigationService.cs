using Microsoft.Extensions.DependencyInjection;

namespace PlusUi.core;

internal class PlusUiNavigationService(IServiceProvider serviceProvider) : INavigationService
{

    public void NavigateTo<TPage>() where TPage : UiPageElement
    {
        var navContainer = serviceProvider.GetRequiredService<NavigationContainer>();
        if (navContainer.Page.GetType() != typeof(TPage))
        {
            navContainer.Page = serviceProvider.GetRequiredService<TPage>();
            navContainer.Page.BuildPage();
        }
    }
}
