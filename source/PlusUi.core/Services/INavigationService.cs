namespace PlusUi.core;

public interface INavigationService
{
    void NavigateTo<TPage>() where TPage : UiPageElement;
}
