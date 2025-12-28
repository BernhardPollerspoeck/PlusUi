using SkiaSharp;

namespace PlusUi.core;

public enum ScrollbarOrientation { Vertical, Horizontal }

public class Scrollbar : UiElement<Scrollbar>, IDraggableControl
{
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Scrollbar;
    protected internal override bool IsFocusable => false;

    private float _maxScrollOffset;
    private Action<float>? _onValueChanged;

    public ScrollbarOrientation Orientation { get; private set; } = ScrollbarOrientation.Vertical;

    public Scrollbar SetOrientation(ScrollbarOrientation orientation)
    {
        Orientation = orientation;
        InvalidateMeasure();
        return this;
    }

    public Scrollbar BindOrientation(string propertyName, Func<ScrollbarOrientation> getter)
    {
        RegisterBinding(propertyName, () => SetOrientation(getter()));
        return this;
    }

    #region Sizing

    public float Width { get; private set; } = 12f;

    public Scrollbar SetWidth(float width)
    {
        Width = width;
        InvalidateMeasure();
        return this;
    }

    public Scrollbar BindWidth(string propertyName, Func<float> getter)
    {
        RegisterBinding(propertyName, () => SetWidth(getter()));
        return this;
    }

    public float MinThumbSize { get; private set; } = 20f;

    public Scrollbar SetMinThumbSize(float size)
    {
        MinThumbSize = size;
        return this;
    }

    public Scrollbar BindMinThumbSize(string propertyName, Func<float> getter)
    {
        RegisterBinding(propertyName, () => SetMinThumbSize(getter()));
        return this;
    }

    #endregion

    #region Colors

    public Color ThumbColor { get; private set; } = new Color(100, 100, 100);

    public Scrollbar SetThumbColor(Color color)
    {
        ThumbColor = color;
        return this;
    }

    public Scrollbar BindThumbColor(string propertyName, Func<Color> getter)
    {
        RegisterBinding(propertyName, () => SetThumbColor(getter()));
        return this;
    }

    public Color ThumbHoverColor { get; private set; } = new Color(140, 140, 140);

    public Scrollbar SetThumbHoverColor(Color color)
    {
        ThumbHoverColor = color;
        return this;
    }

    public Scrollbar BindThumbHoverColor(string propertyName, Func<Color> getter)
    {
        RegisterBinding(propertyName, () => SetThumbHoverColor(getter()));
        return this;
    }

    public Color ThumbDragColor { get; private set; } = new Color(160, 160, 160);

    public Scrollbar SetThumbDragColor(Color color)
    {
        ThumbDragColor = color;
        return this;
    }

    public Scrollbar BindThumbDragColor(string propertyName, Func<Color> getter)
    {
        RegisterBinding(propertyName, () => SetThumbDragColor(getter()));
        return this;
    }

    public Color TrackColor { get; private set; } = new Color(40, 40, 40, 100);

    public Scrollbar SetTrackColor(Color color)
    {
        TrackColor = color;
        return this;
    }

    public Scrollbar BindTrackColor(string propertyName, Func<Color> getter)
    {
        RegisterBinding(propertyName, () => SetTrackColor(getter()));
        return this;
    }

    #endregion

    #region Corner Radius

    public float ThumbCornerRadius { get; private set; } = 4f;

    public Scrollbar SetThumbCornerRadius(float radius)
    {
        ThumbCornerRadius = radius;
        return this;
    }

    public Scrollbar BindThumbCornerRadius(string propertyName, Func<float> getter)
    {
        RegisterBinding(propertyName, () => SetThumbCornerRadius(getter()));
        return this;
    }

    public float TrackCornerRadius { get; private set; } = 4f;

    public Scrollbar SetTrackCornerRadius(float radius)
    {
        TrackCornerRadius = radius;
        return this;
    }

    public Scrollbar BindTrackCornerRadius(string propertyName, Func<float> getter)
    {
        RegisterBinding(propertyName, () => SetTrackCornerRadius(getter()));
        return this;
    }

    #endregion

    #region Auto Hide

    public bool AutoHide { get; private set; } = false;

    public Scrollbar SetAutoHide(bool autoHide)
    {
        AutoHide = autoHide;
        return this;
    }

    public Scrollbar BindAutoHide(string propertyName, Func<bool> getter)
    {
        RegisterBinding(propertyName, () => SetAutoHide(getter()));
        return this;
    }

    public int AutoHideDelay { get; private set; } = 1000;

    public Scrollbar SetAutoHideDelay(int delayMs)
    {
        AutoHideDelay = delayMs;
        return this;
    }

    public Scrollbar BindAutoHideDelay(string propertyName, Func<int> getter)
    {
        RegisterBinding(propertyName, () => SetAutoHideDelay(getter()));
        return this;
    }

    #endregion

    #region Scroll State

    public float Value { get; private set; }
    public float ViewportRatio { get; private set; } = 1f;
    public bool IsHovered { get; private set; }
    public bool IsDragging { get; set; }

    private float _opacity = 1f;
    private DateTime _lastScrollTime = DateTime.MinValue;

    public Scrollbar SetOnValueChanged(Action<float> callback)
    {
        _onValueChanged = callback;
        return this;
    }

    public Scrollbar BindOnValueChanged(string propertyName, Func<Action<float>?> getter)
    {
        RegisterBinding(propertyName, () => _onValueChanged = getter());
        return this;
    }

    public void UpdateScrollState(float scrollOffset, float maxScrollOffset, float viewportSize, float contentSize)
    {
        _maxScrollOffset = maxScrollOffset;

        if (maxScrollOffset <= 0)
        {
            Value = 0;
            ViewportRatio = 1f;
            return;
        }

        Value = scrollOffset / maxScrollOffset;
        ViewportRatio = Math.Clamp(viewportSize / contentSize, 0.05f, 1f);
        _lastScrollTime = DateTime.Now;
        _opacity = 1f;
    }

    public void HandleDrag(float deltaX, float deltaY)
    {
        if (_maxScrollOffset <= 0) return;

        var trackLength = Orientation == ScrollbarOrientation.Vertical
            ? ElementSize.Height
            : ElementSize.Width;

        var thumbLength = Math.Max(MinThumbSize, trackLength * ViewportRatio);
        var availableTrack = trackLength - thumbLength;

        if (availableTrack <= 0) return;

        var delta = Orientation == ScrollbarOrientation.Vertical ? deltaY : deltaX;
        var valueDelta = delta / availableTrack;
        var newValue = Math.Clamp(Value + valueDelta, 0, 1);

        if (Math.Abs(newValue - Value) > 0.0001f)
        {
            Value = newValue;
            var newScrollOffset = Value * _maxScrollOffset;
            _onValueChanged?.Invoke(newScrollOffset);
        }
    }

    public void SetHovered(bool hovered)
    {
        IsHovered = hovered;
        if (hovered) _opacity = 1f;
    }

    #endregion

    #region Thumb Calculation

    public SKRect GetThumbRect()
    {
        var trackLength = Orientation == ScrollbarOrientation.Vertical
            ? ElementSize.Height
            : ElementSize.Width;

        var thumbLength = Math.Max(MinThumbSize, trackLength * ViewportRatio);
        var availableTrack = trackLength - thumbLength;
        var thumbOffset = availableTrack * Value;

        if (Orientation == ScrollbarOrientation.Vertical)
        {
            return new SKRect(
                Position.X,
                Position.Y + thumbOffset,
                Position.X + Width,
                Position.Y + thumbOffset + thumbLength);
        }
        else
        {
            return new SKRect(
                Position.X + thumbOffset,
                Position.Y,
                Position.X + thumbOffset + thumbLength,
                Position.Y + Width);
        }
    }

    public float GetValueFromPosition(float position)
    {
        var trackLength = Orientation == ScrollbarOrientation.Vertical
            ? ElementSize.Height
            : ElementSize.Width;

        var thumbLength = Math.Max(MinThumbSize, trackLength * ViewportRatio);
        var availableTrack = trackLength - thumbLength;

        if (availableTrack <= 0) return 0;

        var relativePos = Orientation == ScrollbarOrientation.Vertical
            ? position - Position.Y - (thumbLength / 2)
            : position - Position.X - (thumbLength / 2);

        return Math.Clamp(relativePos / availableTrack, 0, 1);
    }

    #endregion

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        if (Orientation == ScrollbarOrientation.Vertical)
            return new Size(Width, availableSize.Height);
        else
            return new Size(availableSize.Width, Width);
    }

    public override void Render(SKCanvas canvas)
    {
        if (!IsVisible) return;

        // Handle auto-hide opacity
        if (AutoHide && !IsHovered && !IsDragging)
        {
            var elapsed = (DateTime.Now - _lastScrollTime).TotalMilliseconds;
            if (elapsed > AutoHideDelay)
            {
                _opacity = Math.Max(0, _opacity - 0.05f);
            }
        }
        else
        {
            _opacity = 1f;
        }

        if (_opacity <= 0) return;

        var alpha = (byte)(_opacity * 255);

        // Draw track
        var trackRect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + (Orientation == ScrollbarOrientation.Vertical ? Width : ElementSize.Width),
            Position.Y + VisualOffset.Y + (Orientation == ScrollbarOrientation.Vertical ? ElementSize.Height : Width));

        using var trackPaint = new SKPaint
        {
            Color = TrackColor.WithAlpha((byte)(TrackColor.Alpha * _opacity)),
            IsAntialias = true
        };

        if (TrackCornerRadius > 0)
            canvas.DrawRoundRect(trackRect, TrackCornerRadius, TrackCornerRadius, trackPaint);
        else
            canvas.DrawRect(trackRect, trackPaint);

        // Draw thumb
        var thumbRect = GetThumbRect();
        thumbRect.Offset(VisualOffset.X, VisualOffset.Y);

        var thumbColor = IsDragging ? ThumbDragColor : (IsHovered ? ThumbHoverColor : ThumbColor);
        using var thumbPaint = new SKPaint
        {
            Color = thumbColor.WithAlpha((byte)(thumbColor.Alpha * _opacity)),
            IsAntialias = true
        };

        if (ThumbCornerRadius > 0)
            canvas.DrawRoundRect(thumbRect, ThumbCornerRadius, ThumbCornerRadius, thumbPaint);
        else
            canvas.DrawRect(thumbRect, thumbPaint);
    }

    public override UiElement? HitTest(Point point)
    {
        if (!IsVisible || _opacity <= 0) return null;

        var bounds = new SKRect(Position.X, Position.Y,
            Position.X + (Orientation == ScrollbarOrientation.Vertical ? Width : ElementSize.Width),
            Position.Y + (Orientation == ScrollbarOrientation.Vertical ? ElementSize.Height : Width));

        if (point.X >= bounds.Left && point.X <= bounds.Right &&
            point.Y >= bounds.Top && point.Y <= bounds.Bottom)
        {
            return this;
        }

        return null;
    }
}
