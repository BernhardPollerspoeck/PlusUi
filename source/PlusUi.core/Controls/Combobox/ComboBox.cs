using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Attributes;
using PlusUi.core.Services;
using PlusUi.core.Services.DebugBridge;
using PlusUi.core.UiPropGen;
using SkiaSharp;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A combo box (dropdown) control that allows users to select an item from a list.
/// Supports data binding and custom display formatting.
/// </summary>
/// <typeparam name="T">The type of items in the combo box.</typeparam>
[GenerateShadowMethods]
public partial class ComboBox<T> : ComboBox, IDebugInspectable
{
    IEnumerable<UiElement> IDebugInspectable.GetDebugChildren() =>
        _dropdownOverlay != null ? [_dropdownOverlay] : [];

    private IEnumerable<T>? _itemsSource;
    internal readonly List<T> _cachedItems = [];
    private ComboBoxDropdownOverlay<T>? _dropdownOverlay;
    private Action<T?>? _onSelectionChanged;

    #region Abstract Implementations
    protected override int GetItemCount() => _cachedItems.Count;

    protected override string? GetDisplayText() =>
        SelectedItem != null ? DisplayFunc(SelectedItem) : null;

    protected override void SelectItemAt(int index)
    {
        if (index < 0 || index >= _cachedItems.Count) return;

        _selectedIndex = index;
        SelectedItem = _cachedItems[index];

        _onSelectionChanged?.Invoke(SelectedItem);

        if (_setter.TryGetValue(nameof(SelectedItem), out var itemSetters))
            foreach (var setter in itemSetters)
                setter(SelectedItem);

        if (_setter.TryGetValue(nameof(SelectedIndex), out var indexSetters))
            foreach (var setter in indexSetters)
                setter(SelectedIndex);
    }

    protected override void OnSelectedIndexChanged(int index)
    {
        if (index >= 0 && index < _cachedItems.Count)
            SelectedItem = _cachedItems[index];
        else
            SelectedItem = default;
    }

    protected override void OnDropdownOpened()
    {
        _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
        if (_overlayService == null)
            return;

        _dropdownOverlay = new ComboBoxDropdownOverlay<T>(this);
        _overlayService.RegisterOverlay(_dropdownOverlay);
    }

    protected override void OnDropdownClosed()
    {
        if (_overlayService != null && _dropdownOverlay != null)
        {
            _overlayService.UnregisterOverlay(_dropdownOverlay);
            _dropdownOverlay = null;
        }
    }
    #endregion

    #region ItemsSource
    internal IEnumerable<T>? ItemsSource
    {
        get => field;
        set
        {
            if (_itemsSource is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= OnCollectionChanged;

            field = value;
            _itemsSource = value;

            if (_itemsSource is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += OnCollectionChanged;

            RefreshCache();
            InvalidateMeasure();
        }
    }

    public ComboBox<T> SetItemsSource(IEnumerable<T>? items)
    {
        ItemsSource = items;
        return this;
    }

    public ComboBox<T> BindItemsSource(Expression<Func<IEnumerable<T>?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => ItemsSource = getter());
        return this;
    }
    #endregion

    #region SelectedItem
    internal T? SelectedItem
    {
        get => field;
        set
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;

            if (value != null)
            {
                var index = _cachedItems.IndexOf(value);
                if (index >= 0)
                    _selectedIndex = index;
            }
            else
            {
                _selectedIndex = -1;
            }

            InvalidateMeasure();
        }
    }

    public ComboBox<T> SetSelectedItem(T? item)
    {
        SelectedItem = item;
        return this;
    }

    public ComboBox<T> BindSelectedItem(Expression<Func<T?>> propertyExpression, Action<T?> propertySetter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedItem = getter());
        foreach (var segment in path)
            RegisterSetter<T?>(segment, propertySetter);
        RegisterSetter<T?>(nameof(SelectedItem), propertySetter);
        return this;
    }
    #endregion

    #region SelectedIndex (fluent wrappers)
    public new ComboBox<T> SetSelectedIndex(int index)
    {
        base.SetSelectedIndex(index);
        return this;
    }

    public new ComboBox<T> BindSelectedIndex(Expression<Func<int>> propertyExpression, Action<int> propertySetter)
    {
        base.BindSelectedIndex(propertyExpression, propertySetter);
        return this;
    }
    #endregion

    #region Placeholder/PlaceholderColor (fluent wrappers)
    public new ComboBox<T> SetPlaceholder(string? value)
    {
        base.SetPlaceholder(value);
        return this;
    }

    public new ComboBox<T> BindPlaceholder(Expression<Func<string?>> propertyExpression)
    {
        base.BindPlaceholder(propertyExpression);
        return this;
    }

    public new ComboBox<T> SetPlaceholderColor(Color value)
    {
        base.SetPlaceholderColor(value);
        return this;
    }

    public new ComboBox<T> BindPlaceholderColor(Expression<Func<Color>> propertyExpression)
    {
        base.BindPlaceholderColor(propertyExpression);
        return this;
    }
    #endregion

    #region DisplayFunc
    internal Func<T, string> DisplayFunc { get; set; } = item => item?.ToString() ?? string.Empty;

    public ComboBox<T> SetDisplayFunc(Func<T, string> displayFunc)
    {
        DisplayFunc = displayFunc;
        InvalidateMeasure();
        return this;
    }

    public ComboBox<T> BindDisplayFunc(Expression<Func<Func<T, string>>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DisplayFunc = getter());
        return this;
    }
    #endregion

    #region OnSelectionChanged
    public ComboBox<T> SetOnSelectionChanged(Action<T?> callback)
    {
        _onSelectionChanged = callback;
        return this;
    }
    #endregion

    #region Accessibility
    public override string? GetComputedAccessibilityValue()
    {
        if (!string.IsNullOrEmpty(AccessibilityValue))
            return AccessibilityValue;
        return SelectedItem != null ? DisplayFunc(SelectedItem) : null;
    }
    #endregion

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshCache();
        InvalidateMeasure();
    }

    private void RefreshCache()
    {
        _cachedItems.Clear();
        if (_itemsSource != null)
            _cachedItems.AddRange(_itemsSource);

        if (_selectedIndex >= _cachedItems.Count)
        {
            _selectedIndex = -1;
            SelectedItem = default;
        }
    }

    internal void RenderDropdown(SKCanvas canvas)
    {
        if (_cachedItems.Count == 0)
            return;

        var dropdownRect = GetDropdownRect();
        var dropdownHeight = dropdownRect.Height;

        using var bgPaint = new SKPaint
        {
            Color = DropdownBackground,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRoundRect(dropdownRect, CornerRadius, CornerRadius, bgPaint);

        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(dropdownRect, CornerRadius, CornerRadius));

        _font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        var visibleItemCount = (int)(dropdownHeight / ItemHeight);
        var endIndex = Math.Min(_scrollStartIndex + visibleItemCount, _cachedItems.Count);

        for (int i = _scrollStartIndex; i < endIndex; i++)
        {
            var item = _cachedItems[i];
            var displayIndex = i - _scrollStartIndex;
            var itemY = dropdownRect.Top + (displayIndex * ItemHeight);
            var itemRect = new SKRect(
                dropdownRect.Left,
                itemY,
                dropdownRect.Right,
                itemY + ItemHeight);

            if (i == _hoveredIndex || i == _selectedIndex)
            {
                using var hoverPaint = new SKPaint
                {
                    Color = i == _hoveredIndex ? HoverBackground : new SKColor(50, 50, 50),
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };
                canvas.DrawRect(itemRect, hoverPaint);
            }

            var itemText = DisplayFunc(item);
            canvas.DrawText(
                itemText,
                itemRect.Left + Padding.Left,
                itemRect.Top + (ItemHeight / 2) + (textHeight / 2),
                SKTextAlign.Left,
                _font,
                _paint);
        }

        canvas.Restore();

        using var borderPaint = new SKPaint
        {
            Color = TextColor,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };
        canvas.DrawRoundRect(dropdownRect, CornerRadius, CornerRadius, borderPaint);
    }

    internal void InvokeSetters()
    {
        if (_setter.TryGetValue(nameof(SelectedItem), out var itemSetters))
            foreach (var setter in itemSetters)
                setter(SelectedItem);

        if (_setter.TryGetValue(nameof(SelectedIndex), out var indexSetters))
            foreach (var setter in indexSetters)
                setter(SelectedIndex);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_itemsSource is INotifyCollectionChanged collection)
                collection.CollectionChanged -= OnCollectionChanged;

            OnDropdownClosed();
        }
        base.Dispose(disposing);
    }
}

/// <summary>
/// Non-generic base class for ComboBox. Contains T-independent logic.
/// </summary>
[GenerateShadowMethods]
[UiPropGenPadding]
[UiPropGenPlaceholder]
[UiPropGenPlaceholderColor]
public abstract partial class ComboBox : UiElement, IInputControl, IFocusable, IKeyboardInputHandler
{
    internal const float DropdownMaxHeight = 200f;
    internal static readonly float ItemHeight = PlusUiDefaults.ItemHeight;
    protected const float ArrowSize = 8f;

    protected internal override bool IsFocusable => true;
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.ComboBox;

    internal SKFont _font = null!;
    internal SKPaint _paint = null!;
    internal int _hoveredIndex = -1;
    internal int _scrollStartIndex = 0;
    internal int _selectedIndex = -1;
    protected IOverlayService? _overlayService;
    private IPlatformService? _platformService;

    #region Abstract Methods
    protected abstract int GetItemCount();
    protected abstract string? GetDisplayText();
    protected abstract void SelectItemAt(int index);
    protected abstract void OnSelectedIndexChanged(int index);
    protected abstract void OnDropdownOpened();
    protected abstract void OnDropdownClosed();
    #endregion

    #region IsOpen
    internal bool IsOpen
    {
        get => field;
        set
        {
            if (field == value)
                return;

            field = value;

            if (value)
                OnDropdownOpened();
            else
                OnDropdownClosed();

            InvalidateMeasure();
        }
    }

    public ComboBox SetIsOpen(bool isOpen)
    {
        IsOpen = isOpen;
        return this;
    }

    public ComboBox BindIsOpen(Expression<Func<bool>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => IsOpen = getter());
        return this;
    }
    #endregion

    #region SelectedIndex
    internal int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex == value)
                return;

            _selectedIndex = value;
            OnSelectedIndexChanged(value);
        }
    }

    public ComboBox SetSelectedIndex(int index)
    {
        SelectedIndex = index;
        return this;
    }

    public ComboBox BindSelectedIndex(Expression<Func<int>> propertyExpression, Action<int> propertySetter)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => SelectedIndex = getter());
        foreach (var segment in path)
            RegisterSetter<int>(segment, propertySetter);
        RegisterSetter<int>(nameof(SelectedIndex), propertySetter);
        return this;
    }
    #endregion

    #region TextColor
    internal SKColor TextColor
    {
        get => field;
        set
        {
            field = value;
            UpdatePaint();
        }
    } = PlusUiDefaults.TextPrimary;

    public ComboBox SetTextColor(SKColor color)
    {
        TextColor = color;
        return this;
    }

    public ComboBox BindTextColor(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TextColor = getter());
        return this;
    }
    #endregion

    #region TextSize
    internal float TextSize
    {
        get => field;
        set
        {
            field = value;
            UpdatePaint();
            InvalidateMeasure();
        }
    } = PlusUiDefaults.FontSize;

    public ComboBox SetTextSize(float size)
    {
        TextSize = size;
        return this;
    }

    public ComboBox BindTextSize(Expression<Func<float>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => TextSize = getter());
        return this;
    }
    #endregion

    #region DropdownBackground
    internal SKColor DropdownBackground { get; set; } = PlusUiDefaults.BackgroundPrimary;

    public ComboBox SetDropdownBackground(SKColor color)
    {
        DropdownBackground = color;
        return this;
    }

    public ComboBox BindDropdownBackground(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => DropdownBackground = getter());
        return this;
    }
    #endregion

    #region HoverBackground
    internal SKColor HoverBackground { get; set; } = PlusUiDefaults.BackgroundHover;

    public ComboBox SetHoverBackground(SKColor color)
    {
        HoverBackground = color;
        return this;
    }

    public ComboBox BindHoverBackground(Expression<Func<SKColor>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => HoverBackground = getter());
        return this;
    }
    #endregion

    #region FontFamily
    internal string? FontFamily
    {
        get => field;
        set
        {
            field = value;
            UpdatePaint();
            InvalidateMeasure();
        }
    }

    public ComboBox SetFontFamily(string fontFamily)
    {
        FontFamily = fontFamily;
        return this;
    }

    public ComboBox BindFontFamily(Expression<Func<string?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => FontFamily = getter());
        return this;
    }
    #endregion

    protected ComboBox()
    {
        SetDesiredSize(new Size(200, 40));
        SetBackground(new SolidColorBackground(PlusUiDefaults.BackgroundInput));
        SetCornerRadius(PlusUiDefaults.CornerRadius);
        SetHighContrastBackground(PlusUiDefaults.HcInputBackground);
        SetHighContrastForeground(PlusUiDefaults.HcForeground);
        PlaceholderColor = PlusUiDefaults.TextPlaceholder;
        UpdatePaint();
    }

    [MemberNotNull(nameof(_font), nameof(_paint))]
    private void UpdatePaint()
    {
        if (_paint is not null && _font is not null)
            PaintRegistry.Release(_paint, _font);

        var typeface = string.IsNullOrEmpty(FontFamily)
            ? SKTypeface.FromFamilyName(null)
            : SKTypeface.FromFamilyName(FontFamily);

        (_paint, _font) = PaintRegistry.GetOrCreate(
            color: TextColor,
            size: TextSize,
            typeface: typeface
        );
    }

    #region IInputControl
    public void InvokeCommand() => IsOpen = !IsOpen;
    #endregion

    #region IFocusable
    bool IFocusable.IsFocusable => IsFocusable;
    int? IFocusable.TabIndex => TabIndex;
    bool IFocusable.TabStop => TabStop;
    bool IFocusable.IsFocused
    {
        get => IsFocused;
        set => IsFocused = value;
    }
    void IFocusable.OnFocus() => OnFocus();
    void IFocusable.OnBlur()
    {
        OnBlur();
        IsOpen = false;
    }
    #endregion

    #region IKeyboardInputHandler
    public bool HandleKeyboardInput(PlusKey key)
    {
        var itemCount = GetItemCount();

        if (!IsOpen)
        {
            if (key == PlusKey.Enter || key == PlusKey.Space)
            {
                IsOpen = true;
                _hoveredIndex = _selectedIndex;
                ScrollToSelection();
                return true;
            }
            if (key == PlusKey.ArrowUp || key == PlusKey.ArrowDown)
            {
                IsOpen = true;
                _hoveredIndex = _selectedIndex;
                ScrollToSelection();
                return true;
            }
            return false;
        }

        switch (key)
        {
            case PlusKey.Escape:
                IsOpen = false;
                return true;
            case PlusKey.ArrowUp:
                if (_hoveredIndex > 0)
                {
                    _hoveredIndex--;
                    EnsureHoveredItemVisible();
                }
                else if (_hoveredIndex < 0 && itemCount > 0)
                {
                    _hoveredIndex = itemCount - 1;
                    EnsureHoveredItemVisible();
                }
                return true;
            case PlusKey.ArrowDown:
                if (_hoveredIndex < itemCount - 1)
                {
                    _hoveredIndex++;
                    EnsureHoveredItemVisible();
                }
                else if (_hoveredIndex < 0 && itemCount > 0)
                {
                    _hoveredIndex = 0;
                    EnsureHoveredItemVisible();
                }
                return true;
            case PlusKey.Enter:
            case PlusKey.Space:
                if (_hoveredIndex >= 0 && _hoveredIndex < itemCount)
                    SelectItemAt(_hoveredIndex);
                IsOpen = false;
                return true;
            default:
                return false;
        }
    }
    #endregion

    #region Navigation
    protected void EnsureHoveredItemVisible()
    {
        if (_hoveredIndex < 0) return;

        var itemCount = GetItemCount();
        var dropdownRect = GetDropdownRect();
        var visibleItemCount = (int)(dropdownRect.Height / ItemHeight);

        if (_hoveredIndex < _scrollStartIndex)
            _scrollStartIndex = _hoveredIndex;
        else if (_hoveredIndex >= _scrollStartIndex + visibleItemCount)
            _scrollStartIndex = _hoveredIndex - visibleItemCount + 1;

        _scrollStartIndex = Math.Max(0, Math.Min(_scrollStartIndex, itemCount - visibleItemCount));
    }

    protected void ScrollToSelection()
    {
        if (_selectedIndex < 0)
        {
            _scrollStartIndex = 0;
            return;
        }

        var itemCount = GetItemCount();
        var dropdownRect = GetDropdownRect();
        var visibleItemCount = (int)(dropdownRect.Height / ItemHeight);

        _scrollStartIndex = Math.Max(0, _selectedIndex - visibleItemCount / 2);
        _scrollStartIndex = Math.Min(_scrollStartIndex, Math.Max(0, itemCount - visibleItemCount));
    }
    #endregion

    #region Accessibility
    public override string? GetComputedAccessibilityLabel() =>
        AccessibilityLabel ?? Placeholder ?? "Dropdown";

    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = base.GetComputedAccessibilityTraits();
        if (IsOpen)
            traits |= AccessibilityTrait.Expanded;
        traits |= AccessibilityTrait.HasPopup;
        return traits;
    }
    #endregion

    #region Rendering
    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
            return;

        RenderComboBoxButton(canvas);
    }

    private void RenderComboBoxButton(SKCanvas canvas)
    {
        var rect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        var displayText = GetDisplayText() ?? (Placeholder ?? string.Empty);
        var showingPlaceholder = GetDisplayText() == null && !string.IsNullOrEmpty(Placeholder);

        if (!string.IsNullOrEmpty(displayText))
        {
            var originalColor = _paint.Color;
            if (showingPlaceholder)
                _paint.Color = PlaceholderColor;

            _font.GetFontMetrics(out var fontMetrics);
            var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

            canvas.DrawText(
                displayText,
                rect.Left + Padding.Left,
                rect.Top + Padding.Top + textHeight,
                SKTextAlign.Left,
                _font,
                _paint);

            if (showingPlaceholder)
                _paint.Color = originalColor;
        }

        RenderArrow(canvas, rect);
    }

    protected void RenderArrow(SKCanvas canvas, SKRect rect)
    {
        var arrowCenterX = rect.Right - Padding.Right - ArrowSize - 4;
        var arrowCenterY = rect.Top + rect.Height / 2;

        using var arrowPaint = new SKPaint
        {
            Color = TextColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        var arrowPath = new SKPath();
        if (IsOpen)
        {
            arrowPath.MoveTo(arrowCenterX - ArrowSize / 2, arrowCenterY + ArrowSize / 4);
            arrowPath.LineTo(arrowCenterX + ArrowSize / 2, arrowCenterY + ArrowSize / 4);
            arrowPath.LineTo(arrowCenterX, arrowCenterY - ArrowSize / 4);
        }
        else
        {
            arrowPath.MoveTo(arrowCenterX - ArrowSize / 2, arrowCenterY - ArrowSize / 4);
            arrowPath.LineTo(arrowCenterX + ArrowSize / 2, arrowCenterY - ArrowSize / 4);
            arrowPath.LineTo(arrowCenterX, arrowCenterY + ArrowSize / 4);
        }
        arrowPath.Close();

        canvas.DrawPath(arrowPath, arrowPaint);
    }
    #endregion

    #region Layout
    internal SKRect GetDropdownRect()
    {
        var itemCount = GetItemCount();
        var dropdownHeight = Math.Min(itemCount * ItemHeight, DropdownMaxHeight);
        var comboBoxBottom = Position.Y + VisualOffset.Y + ElementSize.Height;
        var comboBoxTop = Position.Y + VisualOffset.Y;

        _platformService ??= ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();
        var windowHeight = _platformService?.WindowSize.Height ?? 800f;

        var spaceBelow = windowHeight - comboBoxBottom;
        var opensUpward = spaceBelow < dropdownHeight && comboBoxTop > spaceBelow;

        if (opensUpward)
        {
            return new SKRect(
                Position.X + VisualOffset.X,
                comboBoxTop - dropdownHeight,
                Position.X + VisualOffset.X + ElementSize.Width,
                comboBoxTop);
        }
        else
        {
            return new SKRect(
                Position.X + VisualOffset.X,
                comboBoxBottom,
                Position.X + VisualOffset.X + ElementSize.Width,
                comboBoxBottom + dropdownHeight);
        }
    }

    protected override Margin? GetDebugPadding() => Padding;

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        _font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        var width = DesiredSize?.Width ?? 200f;
        var height = textHeight + Padding.Top + Padding.Bottom;

        return new Size(
            Math.Min(width, availableSize.Width),
            Math.Min(height, availableSize.Height));
    }

    public override UiElement? HitTest(Point point)
    {
        if (point.X >= Position.X && point.X <= Position.X + ElementSize.Width &&
            point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height)
        {
            return this;
        }
        return null;
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_paint is not null && _font is not null)
                PaintRegistry.Release(_paint, _font);
        }
        base.Dispose(disposing);
    }
}
