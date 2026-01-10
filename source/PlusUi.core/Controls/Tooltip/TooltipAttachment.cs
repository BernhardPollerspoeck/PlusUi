using PlusUi.core.Binding;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// Configuration class for tooltip properties attached to a UI element.
/// </summary>
public class TooltipAttachment
{
    private readonly Dictionary<string, List<Action>> _bindings = [];
    private readonly ExpressionPathService _expressionPathService = new();

    #region Content
    /// <summary>
    /// The content to display in the tooltip. Can be a string or UiElement.
    /// </summary>
    internal object? Content { get; set; }

    /// <summary>
    /// Sets the tooltip content to a string.
    /// </summary>
    public TooltipAttachment SetContent(string content)
    {
        Content = content;
        return this;
    }

    /// <summary>
    /// Sets the tooltip content to a custom UI element.
    /// </summary>
    public TooltipAttachment SetContent(UiElement content)
    {
        Content = content;
        return this;
    }

    /// <summary>
    /// Binds the tooltip content to a property.
    /// </summary>
    public TooltipAttachment BindContent(Expression<Func<object?>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => Content = getter());
        return this;
    }
    #endregion

    #region Placement
    /// <summary>
    /// The preferred placement of the tooltip relative to the target element.
    /// </summary>
    internal TooltipPlacement Placement { get; set; } = TooltipPlacement.Auto;

    /// <summary>
    /// Sets the tooltip placement.
    /// </summary>
    public TooltipAttachment SetPlacement(TooltipPlacement placement)
    {
        Placement = placement;
        return this;
    }

    /// <summary>
    /// Binds the tooltip placement to a property.
    /// </summary>
    public TooltipAttachment BindPlacement(Expression<Func<TooltipPlacement>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => Placement = getter());
        return this;
    }
    #endregion

    #region ShowDelay
    /// <summary>
    /// The delay in milliseconds before showing the tooltip.
    /// </summary>
    internal int ShowDelay { get; set; } = 500;

    /// <summary>
    /// Sets the delay before showing the tooltip.
    /// </summary>
    public TooltipAttachment SetShowDelay(int milliseconds)
    {
        ShowDelay = Math.Max(0, milliseconds);
        return this;
    }

    /// <summary>
    /// Binds the show delay to a property.
    /// </summary>
    public TooltipAttachment BindShowDelay(Expression<Func<int>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => ShowDelay = Math.Max(0, getter()));
        return this;
    }
    #endregion

    #region HideDelay
    /// <summary>
    /// The delay in milliseconds before hiding the tooltip.
    /// </summary>
    internal int HideDelay { get; set; } = 0;

    /// <summary>
    /// Sets the delay before hiding the tooltip.
    /// </summary>
    public TooltipAttachment SetHideDelay(int milliseconds)
    {
        HideDelay = Math.Max(0, milliseconds);
        return this;
    }

    /// <summary>
    /// Binds the hide delay to a property.
    /// </summary>
    public TooltipAttachment BindHideDelay(Expression<Func<int>> propertyExpression)
    {
        var path = _expressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterBinding(path, () => HideDelay = Math.Max(0, getter()));
        return this;
    }
    #endregion

    #region Bindings
    private void RegisterBinding(string[] propertyNames, Action updateAction)
    {
        foreach (var propertyName in propertyNames)
        {
            if (!_bindings.TryGetValue(propertyName, out var updateActions))
            {
                updateActions = [];
                _bindings.Add(propertyName, updateActions);
            }
            updateActions.Add(updateAction);
        }
        updateAction();
    }

    /// <summary>
    /// Updates all bindings for this tooltip attachment.
    /// </summary>
    internal void UpdateBindings()
    {
        foreach (var propertyGroup in _bindings)
        {
            foreach (var update in propertyGroup.Value)
            {
                update();
            }
        }
    }

    /// <summary>
    /// Updates bindings for a specific property.
    /// </summary>
    internal void UpdateBindings(string propertyName)
    {
        if (_bindings.TryGetValue(propertyName, out var updateActions))
        {
            foreach (var update in updateActions)
            {
                update();
            }
        }
    }
    #endregion
}
