using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

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
            if (_navigationContainer.Page is not null)
            {
                _navigationContainer.Page.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
            try
            {
                var page = serviceProvider.GetRequiredService(pageType) as UiPageElement
                    ?? throw new Exception("Page not found");
                _navigationContainer.Page = page;
                _navigationContainer.Page.ViewModel.PropertyChanged += OnViewModelPropertyChanged;
                _navigationContainer.Page.BuildPage();
            }
            catch (Exception e)
            {
                throw new Exception("You need to register your Page in Program.cs", e);
            }
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not null)
        {
            _navigationContainer?.Page.UpdateBindings(e.PropertyName);
        }
    }

    public void Initialize()
    {
        _navigationContainer ??= serviceProvider.GetRequiredService<NavigationContainer>();
        NavigateTo(_navigationContainer.Page.GetType(), true);
    }
}
