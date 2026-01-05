using Microsoft.Extensions.DependencyInjection;
using PlusUi.core.Services;
using PlusUi.core.Services.DebugBridge;
using SkiaSharp;
using System.Collections;
using System.Collections.Specialized;

namespace PlusUi.core;

/// <summary>
/// A combo box (dropdown) control that allows users to select an item from a list.
/// Supports data binding and custom display formatting.
/// </summary>
/// <typeparam name="T">The type of items in the combo box.</typeparam>
/// <example>
/// <code>
/// // Simple combo box with string items
/// new ComboBox<string>()
///     .SetItemsSource(new[] { "Option 1", "Option 2", "Option 3" })
///     .SetPlaceholder("Select an option...");
///
/// // Combo box with custom objects and display function
/// new ComboBox<Person>()
///     .SetItemsSource(people)
///     .SetDisplayFunc(person => person.Name)
///     .BindSelectedItem(nameof(vm.SelectedPerson), () => vm.SelectedPerson, p => vm.SelectedPerson = p);
/// </code>
/// </example>
public partial class ComboBox<T> : UiElement, IInputControl, IFocusable, IKeyboardInputHandler, IDebugInspectable
{
    /// <summary>
    /// Returns the dropdown overlay for debug inspection.
    /// </summary>
    IEnumerable<UiElement> IDebugInspectable.GetDebugChildren() =>
        _dropdownOverlay != null ? [_dropdownOverlay] : [];

    private IEnumerable<T>? _itemsSource;
    internal readonly List<T> _cachedItems = new();
    internal const float DropdownMaxHeight = 200f;
    internal const float ItemHeight = 32f;
    private const float ArrowSize = 8f;
    private IOverlayService? _overlayService;
    private ComboBoxDropdownOverlay<T>? _dropdownOverlay;
    private IPlatformService? _platformService;

    #region ItemsSource
    internal IEnumerable<T>? ItemsSource
    {
        get => field;
        set
        {
            // Unsubscribe from old collection
            if (_itemsSource is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnCollectionChanged;
            }

            field = value;
            _itemsSource = value;

            // Subscribe to new collection
            if (_itemsSource is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnCollectionChanged;
            }

            RefreshCache();
            InvalidateMeasure();
        }
    }

    public ComboBox<T> SetItemsSource(IEnumerable<T>? items)
    {
        ItemsSource = items;
        return this;
    }

    public ComboBox<T> BindItemsSource(string propertyName, Func<IEnumerable<T>?> propertyGetter)
    {
        RegisterBinding(propertyName, () => ItemsSource = propertyGetter());
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
            {
                return;
            }

            field = value;

            // Update SelectedIndex
            if (value != null)
            {
                var index = _cachedItems.IndexOf(value);
                if (index >= 0)
                {
                    _selectedIndex = index;
                }
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

    public ComboBox<T> BindSelectedItem(string propertyName, Func<T?> propertyGetter, Action<T?> propertySetter)
    {
        RegisterBinding(propertyName, () => SelectedItem = propertyGetter());
        RegisterSetter(nameof(SelectedItem), propertySetter);
        return this;
    }
    #endregion

    #region SelectedIndex
    private int _selectedIndex = -1;

    internal int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex == value)
            {
                return;
            }

            _selectedIndex = value;

            // Update SelectedItem
            if (value >= 0 && value < _cachedItems.Count)
            {
                SelectedItem = _cachedItems[value];
            }
            else
            {
                SelectedItem = default;
            }
        }
    }

    public ComboBox<T> SetSelectedIndex(int index)
    {
        SelectedIndex = index;
        return this;
    }

    public ComboBox<T> BindSelectedIndex(string propertyName, Func<int> propertyGetter, Action<int> propertySetter)
    {
        RegisterBinding(propertyName, () => SelectedIndex = propertyGetter());
        RegisterSetter(nameof(SelectedIndex), propertySetter);
        return this;
    }
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
            {
                RegisterDropdownOverlay();
            }
            else
            {
                UnregisterDropdownOverlay();
            }

            InvalidateMeasure();
        }
    }

    public ComboBox<T> SetIsOpen(bool isOpen)
    {
        IsOpen = isOpen;
        return this;
    }

    public ComboBox<T> BindIsOpen(string propertyName, Func<bool> propertyGetter)
    {
        RegisterBinding(propertyName, () => IsOpen = propertyGetter());
        return this;
    }

    private void RegisterDropdownOverlay()
    {
        _overlayService ??= ServiceProviderService.ServiceProvider?.GetService<IOverlayService>();
        if (_overlayService == null)
            return;

        _dropdownOverlay = new ComboBoxDropdownOverlay<T>(this);
        _overlayService.RegisterOverlay(_dropdownOverlay);
    }

    private void UnregisterDropdownOverlay()
    {
        if (_overlayService != null && _dropdownOverlay != null)
        {
            _overlayService.UnregisterOverlay(_dropdownOverlay);
            _dropdownOverlay = null;
        }
    }
    #endregion

    #region Placeholder
    internal string? Placeholder { get; set; }

    public ComboBox<T> SetPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return this;
    }

    public ComboBox<T> BindPlaceholder(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => Placeholder = propertyGetter());
        return this;
    }
    #endregion

    #region PlaceholderColor
    internal SKColor PlaceholderColor { get; set; } = new SKColor(180, 180, 180);

    public ComboBox<T> SetPlaceholderColor(SKColor color)
    {
        PlaceholderColor = color;
        return this;
    }

    public ComboBox<T> BindPlaceholderColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => PlaceholderColor = propertyGetter());
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
    } = SKColors.White;

    public ComboBox<T> SetTextColor(SKColor color)
    {
        TextColor = color;
        return this;
    }

    public ComboBox<T> BindTextColor(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextColor = propertyGetter());
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
    } = 14f;

    public ComboBox<T> SetTextSize(float size)
    {
        TextSize = size;
        return this;
    }

    public ComboBox<T> BindTextSize(string propertyName, Func<float> propertyGetter)
    {
        RegisterBinding(propertyName, () => TextSize = propertyGetter());
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

    public ComboBox<T> BindDisplayFunc(string propertyName, Func<Func<T, string>> propertyGetter)
    {
        RegisterBinding(propertyName, () => DisplayFunc = propertyGetter());
        return this;
    }
    #endregion

    #region Padding
    internal Margin Padding
    {
        get => field;
        set
        {
            field = value;
            InvalidateMeasure();
        }
    } = new Margin(12, 8);

    public ComboBox<T> SetPadding(Margin padding)
    {
        Padding = padding;
        return this;
    }

    public ComboBox<T> BindPadding(string propertyName, Func<Margin> propertyGetter)
    {
        RegisterBinding(propertyName, () => Padding = propertyGetter());
        return this;
    }
    #endregion

    #region DropdownBackground
    internal SKColor DropdownBackground { get; set; } = new SKColor(40, 40, 40);

    public ComboBox<T> SetDropdownBackground(SKColor color)
    {
        DropdownBackground = color;
        return this;
    }

    public ComboBox<T> BindDropdownBackground(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => DropdownBackground = propertyGetter());
        return this;
    }
    #endregion

    #region HoverBackground
    internal SKColor HoverBackground { get; set; } = new SKColor(60, 60, 60);

    public ComboBox<T> SetHoverBackground(SKColor color)
    {
        HoverBackground = color;
        return this;
    }

    public ComboBox<T> BindHoverBackground(string propertyName, Func<SKColor> propertyGetter)
    {
        RegisterBinding(propertyName, () => HoverBackground = propertyGetter());
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

    public ComboBox<T> SetFontFamily(string fontFamily)
    {
        FontFamily = fontFamily;
        return this;
    }

    public ComboBox<T> BindFontFamily(string propertyName, Func<string?> propertyGetter)
    {
        RegisterBinding(propertyName, () => FontFamily = propertyGetter());
        return this;
    }
    #endregion

    private SKFont _font;
    private SKPaint _paint;

    internal int _hoveredIndex = -1;
    internal int _scrollStartIndex = 0; // First visible item index for scrolling

    /// <inheritdoc />
    protected internal override bool IsFocusable => true;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.ComboBox;

    public ComboBox()
    {
        SetDesiredSize(new Size(200, 40));
        UpdatePaint();
    }

    private void UpdatePaint()
    {
        // Skip if PaintRegistry not available (during shutdown)
        if (PaintRegistry == null)
            return;

        // Release old paint if exists (for property changes)
        if (_paint != null)
        {
            PaintRegistry.Release(_paint, _font);
        }

        // Get or create paint from registry
        var typeface = string.IsNullOrEmpty(FontFamily)
            ? SKTypeface.FromFamilyName(null)
            : SKTypeface.FromFamilyName(FontFamily);

        (_paint, _font) = PaintRegistry.GetOrCreate(
            color: TextColor,
            size: TextSize,
            typeface: typeface
        );
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityLabel()
    {
        return AccessibilityLabel ?? Placeholder ?? "Dropdown";
    }

    /// <inheritdoc />
    public override string? GetComputedAccessibilityValue()
    {
        if (!string.IsNullOrEmpty(AccessibilityValue))
        {
            return AccessibilityValue;
        }
        return SelectedItem != null ? DisplayFunc(SelectedItem) : null;
    }

    /// <inheritdoc />
    public override AccessibilityTrait GetComputedAccessibilityTraits()
    {
        var traits = base.GetComputedAccessibilityTraits();
        if (IsOpen)
        {
            traits |= AccessibilityTrait.Expanded;
        }
        traits |= AccessibilityTrait.HasPopup;
        return traits;
    }

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
        // Close dropdown when losing focus
        IsOpen = false;
    }
    #endregion

    #region IKeyboardInputHandler
    /// <inheritdoc />
    public bool HandleKeyboardInput(PlusKey key)
    {
        if (!IsOpen)
        {
            // When closed, Enter/Space opens the dropdown
            if (key == PlusKey.Enter || key == PlusKey.Space)
            {
                IsOpen = true;
                // Initialize hover to current selection
                _hoveredIndex = _selectedIndex;
                ScrollToSelection();
                return true;
            }
            // Arrow up/down can also open and navigate
            if (key == PlusKey.ArrowUp || key == PlusKey.ArrowDown)
            {
                IsOpen = true;
                _hoveredIndex = _selectedIndex;
                ScrollToSelection();
                return true;
            }
            return false;
        }

        // When open, navigate items
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
                else if (_hoveredIndex < 0 && _cachedItems.Count > 0)
                {
                    _hoveredIndex = _cachedItems.Count - 1;
                    EnsureHoveredItemVisible();
                }
                return true;
            case PlusKey.ArrowDown:
                if (_hoveredIndex < _cachedItems.Count - 1)
                {
                    _hoveredIndex++;
                    EnsureHoveredItemVisible();
                }
                else if (_hoveredIndex < 0 && _cachedItems.Count > 0)
                {
                    _hoveredIndex = 0;
                    EnsureHoveredItemVisible();
                }
                return true;
            case PlusKey.Enter:
            case PlusKey.Space:
                if (_hoveredIndex >= 0 && _hoveredIndex < _cachedItems.Count)
                {
                    SelectItem(_hoveredIndex);
                }
                IsOpen = false;
                return true;
            default:
                return false;
        }
    }

    private void SelectItem(int index)
    {
        if (index < 0 || index >= _cachedItems.Count) return;

        _selectedIndex = index;
        SelectedItem = _cachedItems[index];

        // Notify setters
        if (_setter.TryGetValue(nameof(SelectedItem), out var itemSetters))
        {
            foreach (var setter in itemSetters)
            {
                setter(SelectedItem);
            }
        }
        if (_setter.TryGetValue(nameof(SelectedIndex), out var indexSetters))
        {
            foreach (var setter in indexSetters)
            {
                setter(SelectedIndex);
            }
        }
    }

    private void EnsureHoveredItemVisible()
    {
        if (_hoveredIndex < 0) return;

        var dropdownRect = GetDropdownRect();
        var visibleItemCount = (int)(dropdownRect.Height / ItemHeight);

        // Scroll up if hovered is above visible area
        if (_hoveredIndex < _scrollStartIndex)
        {
            _scrollStartIndex = _hoveredIndex;
        }
        // Scroll down if hovered is below visible area
        else if (_hoveredIndex >= _scrollStartIndex + visibleItemCount)
        {
            _scrollStartIndex = _hoveredIndex - visibleItemCount + 1;
        }

        // Clamp scroll start
        _scrollStartIndex = Math.Max(0, Math.Min(_scrollStartIndex, _cachedItems.Count - visibleItemCount));
    }

    private void ScrollToSelection()
    {
        if (_selectedIndex < 0)
        {
            _scrollStartIndex = 0;
            return;
        }

        var dropdownRect = GetDropdownRect();
        var visibleItemCount = (int)(dropdownRect.Height / ItemHeight);

        // Center the selection in the visible area
        _scrollStartIndex = Math.Max(0, _selectedIndex - visibleItemCount / 2);
        _scrollStartIndex = Math.Min(_scrollStartIndex, Math.Max(0, _cachedItems.Count - visibleItemCount));
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
        {
            _cachedItems.AddRange(_itemsSource);
        }

        // Validate selected index
        if (_selectedIndex >= _cachedItems.Count)
        {
            _selectedIndex = -1;
            SelectedItem = default;
        }
    }

    #region IInputControl
    public void InvokeCommand()
    {
        IsOpen = !IsOpen;
    }
    #endregion

    protected override Margin? GetDebugPadding() => Padding;

    public override void Render(SKCanvas canvas)
    {
        base.Render(canvas);
        if (!IsVisible)
        {
            return;
        }

        // Render the main combo box button
        RenderComboBoxButton(canvas);

        // Dropdown is rendered via OverlayService (above all page content)
    }

    private void RenderComboBoxButton(SKCanvas canvas)
    {
        var rect = new SKRect(
            Position.X + VisualOffset.X,
            Position.Y + VisualOffset.Y,
            Position.X + VisualOffset.X + ElementSize.Width,
            Position.Y + VisualOffset.Y + ElementSize.Height);

        // Determine display text
        var displayText = SelectedItem != null
            ? DisplayFunc(SelectedItem)
            : (Placeholder ?? string.Empty);

        var showingPlaceholder = SelectedItem == null && !string.IsNullOrEmpty(Placeholder);

        // Render text
        if (!string.IsNullOrEmpty(displayText))
        {
            var originalColor = _paint.Color;
            if (showingPlaceholder)
            {
                _paint.Color = PlaceholderColor;
            }

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
            {
                _paint.Color = originalColor;
            }
        }

        // Draw dropdown arrow
        RenderArrow(canvas, rect);
    }

    private void RenderArrow(SKCanvas canvas, SKRect rect)
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
            // Up arrow
            arrowPath.MoveTo(arrowCenterX - ArrowSize / 2, arrowCenterY + ArrowSize / 4);
            arrowPath.LineTo(arrowCenterX + ArrowSize / 2, arrowCenterY + ArrowSize / 4);
            arrowPath.LineTo(arrowCenterX, arrowCenterY - ArrowSize / 4);
        }
        else
        {
            // Down arrow
            arrowPath.MoveTo(arrowCenterX - ArrowSize / 2, arrowCenterY - ArrowSize / 4);
            arrowPath.LineTo(arrowCenterX + ArrowSize / 2, arrowCenterY - ArrowSize / 4);
            arrowPath.LineTo(arrowCenterX, arrowCenterY + ArrowSize / 4);
        }
        arrowPath.Close();

        canvas.DrawPath(arrowPath, arrowPaint);
    }

    /// <summary>
    /// Calculates the dropdown rectangle with intelligent positioning.
    /// Opens upward if there's not enough space below.
    /// </summary>
    internal SKRect GetDropdownRect()
    {
        var dropdownHeight = Math.Min(_cachedItems.Count * ItemHeight, DropdownMaxHeight);
        var comboBoxBottom = Position.Y + VisualOffset.Y + ElementSize.Height;
        var comboBoxTop = Position.Y + VisualOffset.Y;

        // Get window size to check available space
        _platformService ??= ServiceProviderService.ServiceProvider?.GetService<IPlatformService>();
        var windowHeight = _platformService?.WindowSize.Height ?? 800f;

        // Check if dropdown fits below
        var spaceBelow = windowHeight - comboBoxBottom;
        var opensUpward = spaceBelow < dropdownHeight && comboBoxTop > spaceBelow;

        if (opensUpward)
        {
            // Open upward
            return new SKRect(
                Position.X + VisualOffset.X,
                comboBoxTop - dropdownHeight,
                Position.X + VisualOffset.X + ElementSize.Width,
                comboBoxTop);
        }
        else
        {
            // Open downward (default)
            return new SKRect(
                Position.X + VisualOffset.X,
                comboBoxBottom,
                Position.X + VisualOffset.X + ElementSize.Width,
                comboBoxBottom + dropdownHeight);
        }
    }

    internal void RenderDropdown(SKCanvas canvas)
    {
        if (_cachedItems.Count == 0)
        {
            return;
        }

        var dropdownRect = GetDropdownRect();
        var dropdownHeight = dropdownRect.Height;

        // Draw dropdown background
        using var bgPaint = new SKPaint
        {
            Color = DropdownBackground,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRoundRect(dropdownRect, CornerRadius, CornerRadius, bgPaint);

        // Clip to rounded rect for item rendering (so hover effects respect corner radius)
        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(dropdownRect, CornerRadius, CornerRadius));

        // Draw items
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

            // Highlight hovered or selected item
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

            // Draw item text
            var itemText = DisplayFunc(item);
            canvas.DrawText(
                itemText,
                itemRect.Left + Padding.Left,
                itemRect.Top + (ItemHeight / 2) + (textHeight / 2),
                SKTextAlign.Left,
                _font,
                _paint);
        }

        // Restore canvas (remove clipping)
        canvas.Restore();

        // Draw dropdown border (after restore so it's not clipped)
        using var borderPaint = new SKPaint
        {
            Color = TextColor,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };
        canvas.DrawRoundRect(dropdownRect, CornerRadius, CornerRadius, borderPaint);
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        // Get font metrics for height calculation
        _font.GetFontMetrics(out var fontMetrics);
        var textHeight = fontMetrics.Descent - fontMetrics.Ascent;

        var width = DesiredSize?.Width ?? 200f;
        var height = textHeight + Padding.Top + Padding.Bottom;

        return new Size(
            Math.Min(width, availableSize.Width),
            Math.Min(height, availableSize.Height));
    }

    /// <summary>
    /// Invokes the setters for two-way binding after selection changes.
    /// </summary>
    internal void InvokeSetters()
    {
        if (_setter.TryGetValue(nameof(SelectedItem), out var itemSetters))
        {
            foreach (var setter in itemSetters)
            {
                setter(SelectedItem);
            }
        }

        if (_setter.TryGetValue(nameof(SelectedIndex), out var indexSetters))
        {
            foreach (var setter in indexSetters)
            {
                setter(SelectedIndex);
            }
        }
    }

    public override UiElement? HitTest(Point point)
    {
        // Dropdown clicks are handled by the overlay
        // Click outside is handled by InputService dismissing overlays

        // Check if clicking on combo box button
        if (point.X >= Position.X && point.X <= Position.X + ElementSize.Width &&
            point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height)
        {
            return this;
        }

        return null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe from collection changes
            if (_itemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged -= OnCollectionChanged;
            }

            UnregisterDropdownOverlay();

            // Release paint from registry (safe even if ClearAll already called or during shutdown)
            if (_paint != null)
            {
                PaintRegistry?.Release(_paint, _font);
            }
        }
        base.Dispose(disposing);
    }
}
