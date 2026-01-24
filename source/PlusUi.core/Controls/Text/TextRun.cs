using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Binding;
using PlusUi.core.Services;

namespace PlusUi.core;

/// <summary>
/// Represents a styled segment of text within a RichTextLabel.
/// Each run can have its own color, font weight, style, size, and family.
/// Properties set to null will inherit from the parent RichTextLabel.
/// </summary>
public class TextRun
{
    private readonly IExpressionPathService _expressionPathService;
    private readonly List<(string[] path, Action update)> _bindings = [];
    private RichTextLabel? _parent;

    public string Text { get; private set; }
    public Color? Color { get; private set; }
    public FontWeight? FontWeight { get; private set; }
    public FontStyle? FontStyle { get; private set; }
    public float? FontSize { get; private set; }
    public string? FontFamily { get; private set; }

    public TextRun(string text)
    {
        Text = text;
        _expressionPathService = ServiceProviderService.ServiceProvider?.GetService<IExpressionPathService>()
            ?? new ExpressionPathService();
    }

    internal void SetParent(RichTextLabel? parent)
    {
        _parent = parent;
    }

    private void NotifyParentChanged()
    {
        _parent?.InvalidateFromRun();
    }

    public TextRun SetText(string text)
    {
        if (Text != text)
        {
            Text = text;
            NotifyParentChanged();
        }
        return this;
    }

    public TextRun SetColor(Color? color)
    {
        if (Color != color)
        {
            Color = color;
            NotifyParentChanged();
        }
        return this;
    }

    public TextRun SetFontWeight(FontWeight? weight)
    {
        if (FontWeight != weight)
        {
            FontWeight = weight;
            NotifyParentChanged();
        }
        return this;
    }

    public TextRun SetFontStyle(FontStyle? style)
    {
        if (FontStyle != style)
        {
            FontStyle = style;
            NotifyParentChanged();
        }
        return this;
    }

    public TextRun SetFontSize(float? size)
    {
        if (FontSize != size)
        {
            FontSize = size;
            NotifyParentChanged();
        }
        return this;
    }

    public TextRun SetFontFamily(string? family)
    {
        if (FontFamily != family)
        {
            FontFamily = family;
            NotifyParentChanged();
        }
        return this;
    }

    public TextRun BindText(Expression<Func<string>> expr)
    {
        var path = _expressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        _bindings.Add((path, () => SetText(getter())));
        return this;
    }

    public TextRun BindColor(Expression<Func<Color?>> expr)
    {
        var path = _expressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        _bindings.Add((path, () => SetColor(getter())));
        return this;
    }

    public TextRun BindFontWeight(Expression<Func<FontWeight?>> expr)
    {
        var path = _expressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        _bindings.Add((path, () => SetFontWeight(getter())));
        return this;
    }

    public TextRun BindFontStyle(Expression<Func<FontStyle?>> expr)
    {
        var path = _expressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        _bindings.Add((path, () => SetFontStyle(getter())));
        return this;
    }

    public TextRun BindFontSize(Expression<Func<float?>> expr)
    {
        var path = _expressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        _bindings.Add((path, () => SetFontSize(getter())));
        return this;
    }

    public TextRun BindFontFamily(Expression<Func<string?>> expr)
    {
        var path = _expressionPathService.GetPropertyPath(expr);
        var getter = expr.Compile();
        _bindings.Add((path, () => SetFontFamily(getter())));
        return this;
    }

    internal IEnumerable<(string[] path, Action update)> GetBindings() => _bindings;

    internal void UpdateBindings()
    {
        foreach (var (_, update) in _bindings)
        {
            update();
        }
    }

    internal void UpdateBindings(string propertyName)
    {
        foreach (var (path, update) in _bindings)
        {
            if (path.Contains(propertyName))
            {
                update();
            }
        }
    }
}
