using PlusUi.core.Attributes;
using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// A group of toolbar icons with optional separator and priority for overflow handling.
/// </summary>
/// <example>
/// <code>
/// new ToolbarIconGroup()
///     .AddIcon(new Button().SetIcon("undo"))
///     .AddIcon(new Button().SetIcon("redo"))
///     .SetSeparator(true)
///     .SetPriority(10);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class ToolbarIconGroup : UiLayoutElement<ToolbarIconGroup>
{
    #region ShowSeparator
    internal bool ShowSeparator
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    }
    public ToolbarIconGroup SetSeparator(bool show)
    {
        ShowSeparator = show;
        return this;
    }
    public ToolbarIconGroup BindSeparator(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => ShowSeparator = propertyGetter());
        return this;
    }
    #endregion

    #region Spacing
    internal float Spacing
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = 4;
    public ToolbarIconGroup SetSpacing(float spacing)
    {
        Spacing = spacing;
        return this;
    }
    public ToolbarIconGroup BindSpacing(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => Spacing = propertyGetter());
        return this;
    }
    #endregion

    #region Priority
    internal int Priority
    {
        get => field;
        set
        {
            field = value;
        }
    } = 0;
    public ToolbarIconGroup SetPriority(int priority)
    {
        Priority = priority;
        return this;
    }
    public ToolbarIconGroup BindPriority(string propertyName, Func<int> propertyGetter)
    {
        RegisterBinding(propertyName, () => Priority = propertyGetter());
        return this;
    }
    #endregion

    #region SeparatorColor
    internal SKColor SeparatorColor
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = new SKColor(200, 200, 200);
    public ToolbarIconGroup SetSeparatorColor(SKColor color)
    {
        SeparatorColor = color;
        return this;
    }
    public ToolbarIconGroup BindSeparatorColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => SeparatorColor = propertyGetter());
        return this;
    }
    #endregion

    #region SeparatorWidth
    internal float SeparatorWidth
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = 1;
    public ToolbarIconGroup SetSeparatorWidth(float width)
    {
        SeparatorWidth = width;
        return this;
    }
    public ToolbarIconGroup BindSeparatorWidth(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => SeparatorWidth = propertyGetter());
        return this;
    }
    #endregion

    #region SeparatorMargin
    internal float SeparatorMargin
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = 8;
    public ToolbarIconGroup SetSeparatorMargin(float margin)
    {
        SeparatorMargin = margin;
        return this;
    }
    public ToolbarIconGroup BindSeparatorMargin(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => SeparatorMargin = propertyGetter());
        return this;
    }
    #endregion

    public ToolbarIconGroup(params UiElement[] icons)
    {
        foreach (var icon in icons)
        {
            icon.Parent = this;
        }
        Children.AddRange(icons);
    }

    public ToolbarIconGroup AddIcon(UiElement icon)
    {
        AddChild(icon);
        return this;
    }

    #region measure/arrange
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var childAvailableSize = new Size(availableSize.Width, availableSize.Height);

        // First measure all non-stretching children
        foreach (var child in Children.Where(c => c.HorizontalAlignment is not HorizontalAlignment.Stretch))
        {
            var result = child.Measure(childAvailableSize, dontStretch);
            childAvailableSize = new Size(
                Math.Max(0, childAvailableSize.Width - (result.Width + child.Margin.Horizontal)),
                childAvailableSize.Height);
        }

        // Split available space for stretching children
        var stretchingChildren = Children.Where(c => c.HorizontalAlignment is HorizontalAlignment.Stretch).ToList();
        var stretchWidth = stretchingChildren.Count > 0
            ? childAvailableSize.Width / stretchingChildren.Count
            : 0;
        stretchingChildren.ForEach(child => child.Measure(new Size(stretchWidth, childAvailableSize.Height), dontStretch));

        // Calculate total width including spacing
        var width = 0f;
        for (var i = 0; i < Children.Count; i++)
        {
            width += Children[i].ElementSize.Width + Children[i].Margin.Horizontal;
            if (i < Children.Count - 1)
            {
                width += Spacing;
            }
        }

        // Add separator width if needed
        if (ShowSeparator)
        {
            width += SeparatorMargin + SeparatorWidth + SeparatorMargin;
        }

        var height = Children.Count > 0
            ? Children.Max(c => c.ElementSize.Height + c.Margin.Vertical)
            : 0;

        return new Size(width, height);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var positionX = HorizontalAlignment switch
        {
            HorizontalAlignment.Center => bounds.Left + ((bounds.Width - ElementSize.Width) / 2),
            HorizontalAlignment.Right => bounds.Right - ElementSize.Width - Margin.Right,
            _ => bounds.Left + Margin.Left,
        };
        var positionY = VerticalAlignment switch
        {
            VerticalAlignment.Center => bounds.Top + ((bounds.Height - ElementSize.Height) / 2),
            VerticalAlignment.Bottom => bounds.Bottom - ElementSize.Height - Margin.Bottom,
            _ => bounds.Top + Margin.Top,
        };

        var y = positionY;
        var x = positionX;

        foreach (var child in Children)
        {
            var childTopBound = child.VerticalAlignment switch
            {
                VerticalAlignment.Center => y + ((ElementSize.Height - child.ElementSize.Height) / 2),
                VerticalAlignment.Bottom => y + ElementSize.Height - child.ElementSize.Height,
                _ => y,
            };
            child.Arrange(new Rect(x, childTopBound, child.ElementSize.Width, child.ElementSize.Height));
            x += child.ElementSize.Width + child.Margin.Horizontal + Spacing;
        }

        return new Point(positionX, positionY);
    }
    #endregion

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);

        if (!IsVisible || !ShowSeparator)
        {
            return;
        }

        // Render separator to the right of the group
        var separatorX = Position.X + VisualOffset.X + ElementSize.Width - SeparatorMargin - SeparatorWidth;
        var separatorHeight = ElementSize.Height * 0.6f; // 60% of toolbar height
        var separatorY = Position.Y + VisualOffset.Y + (ElementSize.Height - separatorHeight) / 2;

        using var paint = new SKPaint
        {
            Color = SeparatorColor,
            StrokeWidth = SeparatorWidth,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };

        canvas.DrawLine(
            separatorX,
            separatorY,
            separatorX,
            separatorY + separatorHeight,
            paint);
    }

    public override UiElement? HitTest(Point point)
    {
        foreach (var child in Children)
        {
            var result = child.HitTest(point);
            if (result is not null)
            {
                return result;
            }
        }
        return base.HitTest(point);
    }
}
