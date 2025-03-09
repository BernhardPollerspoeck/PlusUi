namespace PlusUi.core;

public class Style(IThemeService themeService)
{
    protected readonly Dictionary<string, List<KeyValuePair<Type, Action<UiElement>>>> _actions = [];
    private readonly Dictionary<string, List<KeyValuePair<Type, Action<UiElement>>>> _pageActions = [];


    #region style
    public Style AddStyle<TElement>(Action<TElement> styleAction)
       where TElement : UiElement
    {
        if (!_actions.TryGetValue(Theme.Default.ToString(), out List<KeyValuePair<Type, Action<UiElement>>>? value))
        {
            value = [];
            _actions.Add(Theme.Default.ToString(), value);
        }

        value.Add(new(
            typeof(TElement),
            element => styleAction((TElement)element)));
        return this;
    }
    public Style AddStyle<TElement>(Theme theme, Action<TElement> styleAction)
        where TElement : UiElement
    {
        if (!_actions.TryGetValue(theme.ToString(), out List<KeyValuePair<Type, Action<UiElement>>>? value))
        {
            value = [];
            _actions.Add(theme.ToString(), value);
        }
        value.Add(new(
            typeof(TElement),
            element => styleAction((TElement)element)));
        return this;
    }
    public Style AddStyle<TElement>(string theme, Action<TElement> styleAction)
        where TElement : UiElement
    {
        if (!_actions.TryGetValue(theme, out List<KeyValuePair<Type, Action<UiElement>>>? value))
        {
            value = [];
            _actions.Add(theme, value);
        }

        value.Add(new(
            typeof(TElement),
            element => styleAction((TElement)element)));
        return this;
    }
    #endregion

    #region pageStyle
    internal void SetPageStyle(Style tmpStyle)
    {
        _pageActions.Clear();
        foreach (var (key, value) in tmpStyle._actions)
        {
            _pageActions.Add(key, value);
        }
    }
    #endregion

    #region apply
    internal void ApplyStyle<TElement>(TElement element)
        where TElement : UiElement
    {
        ApplyStyle(Theme.Default.ToString(), element);
        if (themeService.CurrentTheme == Theme.Default.ToString())
        {
            return;
        }
        ApplyStyle(themeService.CurrentTheme, element);
    }

    private void ApplyStyle<TElement>(string theme, TElement element)
        where TElement : UiElement
    {
        if (_actions.TryGetValue(theme, out List<KeyValuePair<Type, Action<UiElement>>>? value))
        {
            ApplyStyleToElement(element, value);
        }

        if (_pageActions.TryGetValue(theme, out List<KeyValuePair<Type, Action<UiElement>>>? pageValue))
        {
            ApplyStyleToElement(element, pageValue);
        }
    }

    private static void ApplyStyleToElement<TElement>(TElement element, List<KeyValuePair<Type, Action<UiElement>>> value)
        where TElement : UiElement
    {
        foreach (var style in value.Where(a => a.Key.IsAssignableFrom(element.GetType()))
                 .Select(a => a.Value as Action<TElement>))
        {
            style(element);
        }
    }
    #endregion
}
