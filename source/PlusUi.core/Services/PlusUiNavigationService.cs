using Microsoft.Extensions.DependencyInjection;

namespace PlusUi.core;

internal class PlusUiNavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private NavigationContainer? _navigationContainer;

    public void NavigateTo<TPage>() where TPage : UiPageElement
    {
        if (_navigationContainer is null)
        {
            _navigationContainer = serviceProvider.GetRequiredService<NavigationContainer>();
        }

        if (_navigationContainer.Page.GetType() != typeof(TPage))
        {
            _navigationContainer.Page?.Disappearing();

            _navigationContainer.Page = serviceProvider.GetRequiredService<TPage>();
            _navigationContainer.Page.BuildPage();
        }
    }
}
