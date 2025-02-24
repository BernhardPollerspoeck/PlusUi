namespace PlusUi.core;

public class Style(IThemeService themeService)
{
    private static readonly Dictionary<string, List<KeyValuePair<Type, Action<UiElement>>>> _actions = [];

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

    private static void ApplyStyle<TElement>(string theme, TElement element)
        where TElement : UiElement
    {
        if (!_actions.TryGetValue(theme, out List<KeyValuePair<Type, Action<UiElement>>>? value))
        {
            return;
        }
        foreach (var style in value.Where(a => a.Key.IsAssignableFrom(element.GetType()))
            .Select(a => a.Value as Action<TElement>))
        {
            style(element);
        }
    }
}
