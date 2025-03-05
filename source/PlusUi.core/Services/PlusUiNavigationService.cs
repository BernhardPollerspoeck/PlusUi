using Microsoft.Extensions.DependencyInjection;
using PlusUi.core;

namespace PlusUi.core;

public class PlusUiNavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private NavigationContainer? _navigationContainer;

    public void NavigateTo<TPage>() where TPage : UiPageElement
    {
        NavigateTo(typeof(TPage), false);
    }


    private void NavigateTo(Type pageType, bool isInitCall)
    {
        if (_navigationContainer is null)
        {
            throw new Exception("NavigationContainer is not initialized");
        }

        if (isInitCall || _navigationContainer.Page.GetType() != pageType)
        {
            _navigationContainer.Page?.Disappearing();
            try
            {
                var page = serviceProvider.GetRequiredService(pageType) as UiPageElement 
                    ?? throw new Exception("Page not found");
                _navigationContainer.Page = page;
                _navigationContainer.Page.ViewModel.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName is not null)
                    {
                        _navigationContainer.Page.UpdateBindings(e.PropertyName);
                    }
                };
                _navigationContainer.Page.BuildPage();
            }
            catch (Exception e)
            {
                throw new Exception("You need to register your Page in Program.cs", e);
            }
        }
    }

    public void Initialize()
    {
        _navigationContainer ??= serviceProvider.GetRequiredService<NavigationContainer>();
        NavigateTo(_navigationContainer.Page.GetType(), true);
    }
}
