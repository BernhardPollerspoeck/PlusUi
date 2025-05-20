using SkiaSharp;

namespace PlusUi.core;

public class ScrollView : UiLayoutElement<ScrollView>, IScrollableControl
{
    private readonly UiElement _content;
    
    #region CanScrollHorizontally
    internal bool CanScrollHorizontally 
    { 
        get => field; 
        set => field = value;
    } = true;
    
    public ScrollView SetCanScrollHorizontally(bool canScroll)
    {
        CanScrollHorizontally = canScroll;
        return this;
    }
    
    public ScrollView BindCanScrollHorizontally(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => CanScrollHorizontally = propertyGetter());
        return this;
    }
    #endregion
    
    #region CanScrollVertically
    internal bool CanScrollVertically 
    { 
        get => field; 
        set => field = value;
    } = true;
    
    public ScrollView SetCanScrollVertically(bool canScroll)
    {
        CanScrollVertically = canScroll;
        return this;
    }
    
    public ScrollView BindCanScrollVertically(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => CanScrollVertically = propertyGetter());
        return this;
    }
    #endregion
    
    #region HorizontalOffset
    internal float HorizontalOffset 
    { 
        get => field; 
        set
        {
            var maxOffset = Math.Max(0, _content.ElementSize.Width - ElementSize.Width);
            field = Math.Clamp(value, 0, maxOffset);
            InvalidateMeasure();
        }
    }
    
    public ScrollView SetHorizontalOffset(float offset)
    {
        HorizontalOffset = offset;
        return this;
    }
    
    public ScrollView BindHorizontalOffset(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => HorizontalOffset = propertyGetter());
        return this;
    }
    #endregion
    
    #region VerticalOffset
    internal float VerticalOffset 
    { 
        get => field; 
        set
        {
            var maxOffset = Math.Max(0, _content.ElementSize.Height - ElementSize.Height);
            field = Math.Clamp(value, 0, maxOffset);
            InvalidateMeasure();
        }
    }
    
    public ScrollView SetVerticalOffset(float offset)
    {
        VerticalOffset = offset;
        return this;
    }
    
    public ScrollView BindVerticalOffset(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => VerticalOffset = propertyGetter());
        return this;
    }
    #endregion
    
    public ScrollView(UiElement content)
    {
        _content = content;
        _content.Parent = this;
        Children.Add(_content);
    }
    
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // Measure content without constraints (or with very large constraints)
        _content.Measure(new Size(
            CanScrollHorizontally ? float.MaxValue : availableSize.Width, 
            CanScrollVertically ? float.MaxValue : availableSize.Height), 
            dontStretch);
        
        return availableSize;
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
        
        // Arrange content with offset for scrolling
        _content.Arrange(new Rect(
            positionX - HorizontalOffset, 
            positionY - VerticalOffset,
            _content.ElementSize.Width,
            _content.ElementSize.Height));
            
        return new(positionX, positionY);
    }
    
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        
        // Save canvas state and apply clipping
        canvas.Save();
        var rect = new SKRect(
            Position.X,
            Position.Y,
            Position.X + ElementSize.Width,
            Position.Y + ElementSize.Height);
            
        // Apply clipping with corner radius if needed
        if (CornerRadius > 0)
        {
            canvas.ClipRoundRect(new SKRoundRect(rect, CornerRadius, CornerRadius));
        }
        else
        {
            canvas.ClipRect(rect);
        }
        
        // Render content (already clipped)
        _content.Render(canvas);
        
        // Restore canvas state
        canvas.Restore();
    }
    
    public override UiElement? HitTest(Point point)
    {
        // First check if the point is within our bounds
        if (!(point.X >= Position.X && point.X <= Position.X + ElementSize.Width &&
              point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height))
        {
            return null;
        }
        
        // Then check if any child was hit
        var adjustedPoint = new Point(point.X + HorizontalOffset, point.Y + VerticalOffset);
        var childHit = _content.HitTest(adjustedPoint);
        
        // Don't return layout controls (like Grid), return this ScrollView instead
        if (childHit is UiLayoutElement)
        {
            return this;
        }
        
        // Return the hit child or this
        return childHit ?? this;
    }
    
    #region IScrollableControl Implementation
    public bool IsScrolling { get; set; }
    
    public void HandleScroll(float deltaX, float deltaY)
    {
        if (CanScrollHorizontally)
        {
            HorizontalOffset += deltaX;
        }
        
        if (CanScrollVertically)
        {
            VerticalOffset += deltaY;
        }
    }
    #endregion
}