using PlusUi.core.Attributes;
using SkiaSharp;
using System.Collections.Specialized;

namespace PlusUi.core;

/// <summary>
/// A high-performance virtualized data grid control for displaying tabular data.
/// Supports multiple column types, selection, alternating row styles, and custom row styling.
/// </summary>
/// <typeparam name="T">The type of items in the grid.</typeparam>
public class DataGrid<T> : UiLayoutElement<DataGrid<T>>, IScrollableControl, IInputControl, IHoverableControl
{
    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Grid;

    /// <summary>
    /// Returns Children plus the two scrollbars for debug inspection.
    /// </summary>
    protected override IEnumerable<UiElement> GetDebugChildrenCore() =>
        Children.Concat(new[] { VerticalScrollbar, HorizontalScrollbar });

    private int _hoveredRowIndex = -1;

    private IEnumerable<T>? _itemsSource;
    private readonly Dictionary<int, List<UiElement>> _realizedRows = new();
    private readonly Dictionary<int, float> _rowPositions = new();
    private int _firstVisibleIndex;
    private int _lastVisibleIndex;
    private List<UiElement>? _headerCells;
    private float[] _columnWidths = [];

    public DataGrid()
    {
        VerticalScrollbar = new Scrollbar()
            .SetOrientation(ScrollbarOrientation.Vertical);
        HorizontalScrollbar = new Scrollbar()
            .SetOrientation(ScrollbarOrientation.Horizontal);
        InitializeScrollbars();
    }

    #region Cell Padding

    public Margin CellPadding { get; private set; } = new Margin(8, 4);

    public DataGrid<T> SetCellPadding(Margin padding)
    {
        CellPadding = padding;
        InvalidateMeasure();
        return this;
    }

    public DataGrid<T> BindCellPadding(string propertyName, Func<Margin> getter)
    {
        RegisterBinding(propertyName, () => SetCellPadding(getter()));
        return this;
    }

    #endregion

    #region Grid Lines

    public bool ShowRowSeparators { get; private set; } = true;

    public DataGrid<T> SetShowRowSeparators(bool show)
    {
        ShowRowSeparators = show;
        return this;
    }

    public DataGrid<T> BindShowRowSeparators(string propertyName, Func<bool> getter)
    {
        RegisterBinding(propertyName, () => SetShowRowSeparators(getter()));
        return this;
    }

    public bool ShowColumnSeparators { get; private set; } = true;

    public DataGrid<T> SetShowColumnSeparators(bool show)
    {
        ShowColumnSeparators = show;
        return this;
    }

    public DataGrid<T> BindShowColumnSeparators(string propertyName, Func<bool> getter)
    {
        RegisterBinding(propertyName, () => SetShowColumnSeparators(getter()));
        return this;
    }

    public SKColor SeparatorColor { get; private set; } = new SKColor(60, 60, 60);

    public DataGrid<T> SetSeparatorColor(SKColor color)
    {
        SeparatorColor = color;
        return this;
    }

    public DataGrid<T> BindSeparatorColor(string propertyName, Func<SKColor> getter)
    {
        RegisterBinding(propertyName, () => SetSeparatorColor(getter()));
        return this;
    }

    public float SeparatorThickness { get; private set; } = 1f;

    public DataGrid<T> SetSeparatorThickness(float thickness)
    {
        SeparatorThickness = thickness;
        return this;
    }

    public DataGrid<T> BindSeparatorThickness(string propertyName, Func<float> getter)
    {
        RegisterBinding(propertyName, () => SetSeparatorThickness(getter()));
        return this;
    }

    public SKColor HeaderSeparatorColor { get; private set; } = new SKColor(100, 100, 100);

    public DataGrid<T> SetHeaderSeparatorColor(SKColor color)
    {
        HeaderSeparatorColor = color;
        return this;
    }

    public DataGrid<T> BindHeaderSeparatorColor(string propertyName, Func<SKColor> getter)
    {
        RegisterBinding(propertyName, () => SetHeaderSeparatorColor(getter()));
        return this;
    }

    #endregion

    #region Columns

    /// <summary>
    /// Gets the collection of columns in the grid.
    /// </summary>
    public List<DataGridColumn<T>> Columns { get; } = [];

    /// <summary>
    /// Adds a column to the grid.
    /// </summary>
    /// <param name="column">The column to add.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> AddColumn(DataGridColumn<T> column)
    {
        Columns.Add(column);
        // If we have data but rows weren't created yet (because columns were empty), rebuild now
        if (_itemsSource != null && _headerCells == null)
        {
            RebuildRows();
        }
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Removes a column from the grid.
    /// </summary>
    /// <param name="column">The column to remove.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> RemoveColumn(DataGridColumn<T> column)
    {
        Columns.Remove(column);
        InvalidateMeasure();
        return this;
    }

    #endregion

    #region ItemsSource

    /// <summary>
    /// Gets the data source for the grid.
    /// </summary>
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

            InvalidateMeasure();
            RebuildRows();
        }
    }

    /// <summary>
    /// Sets the data source for the grid.
    /// </summary>
    /// <param name="items">The collection of items to display.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> SetItemsSource(IEnumerable<T>? items)
    {
        ItemsSource = items;
        return this;
    }

    /// <summary>
    /// Binds the items source to a property.
    /// </summary>
    /// <param name="propertyName">The property name to bind to.</param>
    /// <param name="propertyGetter">A function that gets the items collection.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> BindItemsSource(string propertyName, Func<IEnumerable<T>?> propertyGetter)
    {
        RegisterBinding(propertyName, () => ItemsSource = propertyGetter());
        return this;
    }

    #endregion

    #region HeaderHeight

    /// <summary>
    /// Gets the height of the header row in pixels.
    /// </summary>
    public float HeaderHeight { get; private set; } = 36f;

    /// <summary>
    /// Sets the height of the header row.
    /// </summary>
    /// <param name="height">The height in pixels.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> SetHeaderHeight(float height)
    {
        HeaderHeight = height;
        InvalidateMeasure();
        return this;
    }

    #endregion

    #region RowHeight

    /// <summary>
    /// Gets the height of each data row in pixels.
    /// </summary>
    public float RowHeight { get; private set; } = 32f;

    /// <summary>
    /// Sets the height of data rows.
    /// </summary>
    /// <param name="height">The height in pixels.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> SetRowHeight(float height)
    {
        RowHeight = height;
        InvalidateMeasure();
        return this;
    }

    #endregion

    #region SelectionMode

    /// <summary>
    /// Gets the selection mode of the grid.
    /// </summary>
    public SelectionMode SelectionMode { get; private set; } = SelectionMode.Single;

    /// <summary>
    /// Sets the selection mode.
    /// </summary>
    /// <param name="mode">The selection mode.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> SetSelectionMode(SelectionMode mode)
    {
        SelectionMode = mode;
        return this;
    }

    #endregion

    #region Selection

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    public T? SelectedItem { get; private set; }

    /// <summary>
    /// Gets the collection of selected items.
    /// </summary>
    public List<T> SelectedItems { get; } = [];

    private Action<T?>? _selectedItemSetter;

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="item">The item to select, or null to clear selection.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> SetSelectedItem(T? item)
    {
        if (SelectionMode == SelectionMode.None)
        {
            return this;
        }

        if (item == null)
        {
            ClearSelection();
        }
        else
        {
            if (SelectionMode == SelectionMode.Single)
            {
                SelectedItems.Clear();
            }

            if (!SelectedItems.Contains(item))
            {
                SelectedItems.Add(item);
            }

            SelectedItem = item;
        }

        _selectedItemSetter?.Invoke(item);
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Binds the selected item to a property with two-way binding.
    /// </summary>
    /// <param name="propertyName">The property name to bind to.</param>
    /// <param name="propertyGetter">A function that gets the selected item.</param>
    /// <param name="propertySetter">An action that sets the selected item.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> BindSelectedItem(string propertyName, Func<T?> propertyGetter, Action<T?> propertySetter)
    {
        _selectedItemSetter = propertySetter;
        RegisterBinding(propertyName, () => SetSelectedItem(propertyGetter()));
        return this;
    }

    /// <summary>
    /// Selects an item in the grid.
    /// </summary>
    /// <param name="item">The item to select.</param>
    public void SelectItem(T item)
    {
        if (SelectionMode == SelectionMode.None)
        {
            return;
        }

        if (SelectionMode == SelectionMode.Single)
        {
            SelectedItems.Clear();
        }

        if (!SelectedItems.Contains(item))
        {
            SelectedItems.Add(item);
        }

        SelectedItem = item;
        _selectedItemSetter?.Invoke(item);
        InvalidateMeasure();
    }

    /// <summary>
    /// Deselects an item in the grid.
    /// </summary>
    /// <param name="item">The item to deselect.</param>
    public void DeselectItem(T item)
    {
        SelectedItems.Remove(item);

        if (EqualityComparer<T>.Default.Equals(SelectedItem, item))
        {
            SelectedItem = SelectedItems.FirstOrDefault();
            _selectedItemSetter?.Invoke(SelectedItem);
        }

        InvalidateMeasure();
    }

    /// <summary>
    /// Clears all selections.
    /// </summary>
    public void ClearSelection()
    {
        SelectedItems.Clear();
        SelectedItem = default;
        _selectedItemSetter?.Invoke(default);
        InvalidateMeasure();
    }

    #endregion

    #region Row Styling

    /// <summary>
    /// Gets whether alternating row styles are enabled.
    /// </summary>
    public bool AlternatingRowStyles { get; private set; } = true;

    /// <summary>
    /// Sets whether alternating row styles are enabled.
    /// </summary>
    /// <param name="enabled">True to enable alternating row styles.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> SetAlternatingRowStyles(bool enabled)
    {
        AlternatingRowStyles = enabled;
        InvalidateMeasure();
        return this;
    }

    /// <summary>
    /// Gets the style for even-indexed rows.
    /// </summary>
    public DataGridRowStyle? EvenRowStyle { get; private set; }

    /// <summary>
    /// Sets the style for even-indexed rows.
    /// </summary>
    /// <param name="background">The background for even rows.</param>
    /// <param name="foreground">The text color for even rows.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> SetEvenRowStyle(IBackground? background, Color? foreground = null)
    {
        EvenRowStyle = new DataGridRowStyle(background, foreground);
        return this;
    }

    /// <summary>
    /// Gets the style for odd-indexed rows.
    /// </summary>
    public DataGridRowStyle? OddRowStyle { get; private set; }

    /// <summary>
    /// Sets the style for odd-indexed rows.
    /// </summary>
    /// <param name="background">The background for odd rows.</param>
    /// <param name="foreground">The text color for odd rows.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> SetOddRowStyle(IBackground? background, Color? foreground = null)
    {
        OddRowStyle = new DataGridRowStyle(background, foreground);
        return this;
    }

    /// <summary>
    /// Gets the custom row style callback.
    /// </summary>
    public Func<T, int, DataGridRowStyle>? RowStyleCallback { get; private set; }

    /// <summary>
    /// Sets a custom callback for row styling.
    /// </summary>
    /// <param name="callback">A function that returns the style for each row.</param>
    /// <returns>This grid for method chaining.</returns>
    public DataGrid<T> SetRowStyleCallback(Func<T, int, DataGridRowStyle> callback)
    {
        RowStyleCallback = callback;
        return this;
    }

    #endregion

    #region Scrolling

    public float ScrollOffset { get; private set; }
    public float HorizontalScrollOffset { get; private set; }
    public bool IsScrolling { get; set; }

    public DataGrid<T> SetScrollOffset(float offset)
    {
        var maxOffset = CalculateMaxScrollOffset();
        ScrollOffset = Math.Clamp(offset, 0, maxOffset);
        UpdateScrollbarStates();
        InvalidateMeasure();
        UpdateVisibleRange();
        return this;
    }

    public DataGrid<T> SetHorizontalScrollOffset(float offset)
    {
        var maxOffset = CalculateMaxHorizontalScrollOffset();
        HorizontalScrollOffset = Math.Clamp(offset, 0, maxOffset);
        UpdateScrollbarStates();
        InvalidateMeasure();
        return this;
    }

    public void HandleScroll(float deltaX, float deltaY)
    {
        SetScrollOffset(ScrollOffset + deltaY);
        SetHorizontalScrollOffset(HorizontalScrollOffset + deltaX);
    }

    private float CalculateMaxScrollOffset()
    {
        var itemCount = _itemsSource?.Count() ?? 0;
        var totalContentHeight = HeaderHeight + (itemCount * RowHeight);
        return Math.Max(0, totalContentHeight - ElementSize.Height);
    }

    private float CalculateMaxHorizontalScrollOffset()
    {
        var totalWidth = _columnWidths.Sum();
        return Math.Max(0, totalWidth - ElementSize.Width);
    }

    #endregion

    #region Scrollbars

    public Scrollbar VerticalScrollbar { get; }
    public Scrollbar HorizontalScrollbar { get; }

    private void InitializeScrollbars()
    {
        VerticalScrollbar.SetOnValueChanged(offset => SetScrollOffset(offset));
        HorizontalScrollbar.SetOnValueChanged(offset => SetHorizontalScrollOffset(offset));
    }

    public bool ShowVerticalScrollbar { get; private set; } = true;

    public DataGrid<T> SetShowVerticalScrollbar(bool show)
    {
        ShowVerticalScrollbar = show;
        InvalidateMeasure();
        return this;
    }

    public bool ShowHorizontalScrollbar { get; private set; } = true;

    public DataGrid<T> SetShowHorizontalScrollbar(bool show)
    {
        ShowHorizontalScrollbar = show;
        InvalidateMeasure();
        return this;
    }

    private void UpdateScrollbarStates()
    {
        var itemCount = _itemsSource?.Count() ?? 0;
        var totalContentHeight = HeaderHeight + (itemCount * RowHeight);
        var totalContentWidth = _columnWidths.Sum();

        VerticalScrollbar.UpdateScrollState(
            ScrollOffset,
            CalculateMaxScrollOffset(),
            ElementSize.Height,
            totalContentHeight);

        HorizontalScrollbar.UpdateScrollState(
            HorizontalScrollOffset,
            CalculateMaxHorizontalScrollOffset(),
            ElementSize.Width,
            totalContentWidth);
    }

    #endregion

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateMeasure();
        RebuildRows();
    }

    private void RebuildRows()
    {
        _realizedRows.Clear();
        _rowPositions.Clear();
        Children.Clear();
        _headerCells = null;

        if (_itemsSource == null || Columns.Count == 0)
        {
            return;
        }

        // Don't update visible range here - wait for MeasureInternal with actual size
        // UpdateVisibleRange will be called from MeasureInternal
    }

    private void UpdateVisibleRange(float viewportHeight = 0)
    {
        if (_itemsSource == null || Columns.Count == 0)
        {
            return;
        }

        var items = _itemsSource.ToList();

        // Create header cells even when there are no items
        if (_headerCells == null)
        {
            _headerCells = CreateHeaderCells();
            Children.Clear();
            foreach (var headerCell in _headerCells)
            {
                headerCell.Parent = this;
                Children.Add(headerCell);
            }
        }

        if (items.Count == 0)
        {
            return;
        }

        // Use provided viewport height, or fall back to ElementSize (for scroll updates)
        if (viewportHeight <= 0)
        {
            viewportHeight = ElementSize.Height;
        }

        // Still no valid height? Show at least some rows for initial layout
        if (viewportHeight <= 0)
        {
            viewportHeight = 500; // Reasonable default for initial layout
        }

        // Calculate visible range based on scroll offset
        var contentStartY = HeaderHeight;

        // Find first visible row
        var firstVisibleRow = (int)Math.Floor(Math.Max(0, ScrollOffset - contentStartY) / RowHeight);
        firstVisibleRow = Math.Max(0, Math.Min(firstVisibleRow, items.Count - 1));

        // Find last visible row
        var visibleRows = (int)Math.Ceiling((viewportHeight - contentStartY + ScrollOffset) / RowHeight);
        var lastVisibleRow = Math.Min(firstVisibleRow + visibleRows + 1, items.Count - 1);

        // Realize rows in visible range
        for (int i = firstVisibleRow; i <= lastVisibleRow; i++)
        {
            if (!_realizedRows.ContainsKey(i))
            {
                var rowCells = CreateRowCells(items[i], i);
                _realizedRows[i] = rowCells;
            }

            _rowPositions[i] = contentStartY + (i * RowHeight);
        }

        // Remove rows outside visible range
        var keysToRemove = _realizedRows.Keys.Where(k => k < firstVisibleRow || k > lastVisibleRow).ToList();
        foreach (var key in keysToRemove)
        {
            _realizedRows.Remove(key);
            _rowPositions.Remove(key);
        }

        // Update children
        Children.Clear();

        // Add header cells
        if (_headerCells == null)
        {
            _headerCells = CreateHeaderCells();
        }
        foreach (var headerCell in _headerCells)
        {
            headerCell.Parent = this;
            Children.Add(headerCell);
        }

        // Add visible row cells
        foreach (var rowCells in _realizedRows.OrderBy(kvp => kvp.Key).SelectMany(kvp => kvp.Value))
        {
            rowCells.Parent = this;
            Children.Add(rowCells);
        }

        _firstVisibleIndex = firstVisibleRow;
        _lastVisibleIndex = lastVisibleRow;
    }

    private List<UiElement> CreateHeaderCells()
    {
        var headerCells = new List<UiElement>();

        foreach (var column in Columns)
        {
            var headerCell = new Label()
                .SetText(column.Header)
                .SetTextSize(14)
                .SetFontWeight(FontWeight.Bold)
                .SetTextColor(Colors.White)
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetVerticalAlignment(VerticalAlignment.Center)
                .SetMargin(CellPadding);
            headerCell.Parent = this;
            headerCells.Add(headerCell);
        }

        return headerCells;
    }

    private List<UiElement> CreateRowCells(T item, int rowIndex)
    {
        var cells = new List<UiElement>();

        foreach (var column in Columns)
        {
            var cell = column.CreateCell(item, rowIndex);
            cell.Parent = this;

            // Apply cell padding and alignment
            cell.SetMargin(CellPadding);
            cell.SetVerticalAlignment(VerticalAlignment.Center);

            // Apply row styling
            var style = GetRowStyle(item, rowIndex);
            if (style.Foreground.HasValue && cell is Label label)
            {
                label.SetTextColor(style.Foreground.Value);
            }

            cells.Add(cell);
        }

        return cells;
    }

    private DataGridRowStyle GetRowStyle(T item, int rowIndex)
    {
        // Check if selected
        if (SelectedItems.Contains(item))
        {
            return new DataGridRowStyle(
                Background: new SolidColorBackground(new Color(0, 120, 215, 80)),
                Foreground: Colors.Black
            );
        }

        // Custom callback takes precedence
        if (RowStyleCallback != null)
        {
            return RowStyleCallback(item, rowIndex);
        }

        // Alternating row styles
        if (AlternatingRowStyles)
        {
            return rowIndex % 2 == 0
                ? EvenRowStyle ?? new DataGridRowStyle()
                : OddRowStyle ?? new DataGridRowStyle();
        }

        return new DataGridRowStyle();
    }

    private void CalculateColumnWidths(float availableWidth)
    {
        if (Columns.Count == 0)
        {
            _columnWidths = [];
            return;
        }

        _columnWidths = new float[Columns.Count];

        // First pass: calculate absolute and auto widths
        float fixedWidth = 0;
        float totalStars = 0;

        for (int i = 0; i < Columns.Count; i++)
        {
            var column = Columns[i];

            switch (column.Width.Type)
            {
                case DataGridColumnWidthType.Absolute:
                    _columnWidths[i] = column.Width.Value;
                    fixedWidth += column.Width.Value;
                    break;

                case DataGridColumnWidthType.Auto:
                    // For now, treat auto as a fixed minimum width
                    _columnWidths[i] = 100; // Default auto width
                    fixedWidth += _columnWidths[i];
                    break;

                case DataGridColumnWidthType.Star:
                    totalStars += column.Width.Value;
                    break;
            }
        }

        // Second pass: distribute remaining space to star columns
        var remainingWidth = Math.Max(0, availableWidth - fixedWidth);
        var pixelsPerStar = totalStars > 0 ? remainingWidth / totalStars : 0;

        for (int i = 0; i < Columns.Count; i++)
        {
            var column = Columns[i];

            if (column.Width.Type == DataGridColumnWidthType.Star)
            {
                _columnWidths[i] = pixelsPerStar * column.Width.Value;
            }

            // Update ActualWidth on column
            column.ActualWidth = _columnWidths[i];
        }
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        CalculateColumnWidths(availableSize.Width);
        UpdateVisibleRange(availableSize.Height);

        // Measure header cells
        if (_headerCells != null)
        {
            for (int c = 0; c < _headerCells.Count && c < _columnWidths.Length; c++)
            {
                _headerCells[c].Measure(new Size(_columnWidths[c], HeaderHeight), true);
            }
        }

        // Measure visible row cells
        for (int i = _firstVisibleIndex; i <= _lastVisibleIndex; i++)
        {
            if (_realizedRows.TryGetValue(i, out var cells))
            {
                for (int c = 0; c < cells.Count && c < _columnWidths.Length; c++)
                {
                    cells[c].Measure(new Size(_columnWidths[c], RowHeight), false);
                }
            }
        }

        // Measure scrollbars (vertical height excludes header)
        VerticalScrollbar.Parent = this;
        HorizontalScrollbar.Parent = this;
        var vScrollHeight = availableSize.Height - HeaderHeight - (ShowHorizontalScrollbar ? HorizontalScrollbar.Width : 0);
        VerticalScrollbar.Measure(new Size(VerticalScrollbar.Width, vScrollHeight), true);
        HorizontalScrollbar.Measure(new Size(availableSize.Width - (ShowVerticalScrollbar ? VerticalScrollbar.Width : 0), HorizontalScrollbar.Width), true);
        UpdateScrollbarStates();

        // Calculate actual size based on DesiredSize or content
        float actualWidth;
        if (DesiredSize?.Width > 0)
        {
            actualWidth = DesiredSize.Value.Width;
        }
        else
        {
            // Use sum of column widths
            actualWidth = _columnWidths.Sum();
        }

        float actualHeight;
        if (DesiredSize?.Height > 0)
        {
            actualHeight = DesiredSize.Value.Height;
        }
        else
        {
            // Header + all rows
            var itemCount = _itemsSource?.Count() ?? 0;
            actualHeight = HeaderHeight + (itemCount * RowHeight);
        }

        // Clamp to available size
        actualWidth = Math.Min(actualWidth, availableSize.Width);
        actualHeight = Math.Min(actualHeight, availableSize.Height);

        return new Size(actualWidth, actualHeight);
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

        // Arrange header cells (with horizontal scroll)
        if (_headerCells != null)
        {
            var cellX = positionX - HorizontalScrollOffset;
            for (int c = 0; c < _headerCells.Count && c < _columnWidths.Length; c++)
            {
                _headerCells[c].Arrange(new Rect(cellX, positionY, _columnWidths[c], HeaderHeight));
                cellX += _columnWidths[c];
            }
        }

        // Arrange visible rows (with both vertical and horizontal scroll)
        for (int rowIndex = _firstVisibleIndex; rowIndex <= _lastVisibleIndex; rowIndex++)
        {
            if (_realizedRows.TryGetValue(rowIndex, out var cells))
            {
                var rowY = positionY + HeaderHeight + (rowIndex * RowHeight) - ScrollOffset;
                var cellX = positionX - HorizontalScrollOffset;

                for (int c = 0; c < cells.Count && c < _columnWidths.Length; c++)
                {
                    cells[c].Arrange(new Rect(cellX, rowY, _columnWidths[c], RowHeight));
                    cellX += _columnWidths[c];
                }
            }
        }

        // Arrange scrollbars (vertical starts below header)
        var vScrollX = positionX + ElementSize.Width - VerticalScrollbar.Width;
        var vScrollY = positionY + HeaderHeight;
        var vScrollHeight = ElementSize.Height - HeaderHeight - (ShowHorizontalScrollbar ? HorizontalScrollbar.Width : 0);
        VerticalScrollbar.Arrange(new Rect(vScrollX, vScrollY, VerticalScrollbar.Width, vScrollHeight));

        var hScrollY = positionY + ElementSize.Height - HorizontalScrollbar.Width;
        var hScrollWidth = ShowVerticalScrollbar ? ElementSize.Width - VerticalScrollbar.Width : ElementSize.Width;
        HorizontalScrollbar.Arrange(new Rect(positionX, hScrollY, hScrollWidth, HorizontalScrollbar.Width));

        return new Point(positionX, positionY);
    }

    public override void Render(SKCanvas canvas)
    {
        if (!IsVisible)
        {
            return;
        }

        var baseX = Position.X + VisualOffset.X;
        var baseY = Position.Y + VisualOffset.Y;
        var headerBottom = baseY + HeaderHeight;

        // === CLIP ENTIRE DATAGRID BOUNDS ===
        canvas.Save();
        var gridClipRect = new SKRect(baseX, baseY, baseX + ElementSize.Width, baseY + ElementSize.Height);
        if (CornerRadius > 0)
        {
            canvas.ClipRoundRect(new SKRoundRect(gridClipRect, CornerRadius, CornerRadius));
        }
        else
        {
            canvas.ClipRect(gridClipRect);
        }

        // Render background
        base.Render(canvas);

        // === RENDER ROWS (clipped below header) ===
        canvas.Save();
        var rowsClipRect = new SKRect(baseX, headerBottom, baseX + ElementSize.Width, baseY + ElementSize.Height);
        canvas.ClipRect(rowsClipRect);

        // Render row backgrounds
        if (_itemsSource != null)
        {
            var items = _itemsSource.ToList();
            for (int rowIndex = _firstVisibleIndex; rowIndex <= _lastVisibleIndex; rowIndex++)
            {
                if (rowIndex >= items.Count) continue;

                var item = items[rowIndex];
                var style = GetRowStyle(item, rowIndex);

                var rowY = baseY + HeaderHeight + (rowIndex * RowHeight) - ScrollOffset;
                var rowRect = new SKRect(baseX, rowY, baseX + ElementSize.Width, rowY + RowHeight);

                if (style.Background != null)
                {
                    style.Background.Render(canvas, rowRect, 0);
                }

                // Draw row separator
                if (ShowRowSeparators)
                {
                    using var rowLinePaint = new SKPaint
                    {
                        Color = SeparatorColor,
                        StrokeWidth = SeparatorThickness,
                        IsAntialias = true
                    };
                    canvas.DrawLine(baseX, rowY + RowHeight, baseX + ElementSize.Width, rowY + RowHeight, rowLinePaint);
                }
            }
        }

        // Render row cells (children except header cells)
        if (_headerCells != null)
        {
            foreach (var child in Children.Skip(_headerCells.Count))
            {
                var childOriginalOffset = child.VisualOffset;
                child.SetVisualOffset(new Point(childOriginalOffset.X + VisualOffset.X, childOriginalOffset.Y + VisualOffset.Y));
                child.Render(canvas);
                child.SetVisualOffset(childOriginalOffset);
            }
        }

        // Draw column separators in rows area (with horizontal scroll)
        if (ShowColumnSeparators)
        {
            using var colLinePaint = new SKPaint
            {
                Color = SeparatorColor,
                StrokeWidth = SeparatorThickness,
                IsAntialias = true
            };

            var columnX = baseX - HorizontalScrollOffset;
            for (int i = 0; i < _columnWidths.Length - 1; i++)
            {
                columnX += _columnWidths[i];
                canvas.DrawLine(columnX, headerBottom, columnX, baseY + ElementSize.Height, colLinePaint);
            }
        }

        canvas.Restore(); // End rows clip

        // === RENDER HEADER (on top of rows) ===
        canvas.Save();
        var headerClipRect = new SKRect(baseX, baseY, baseX + ElementSize.Width, headerBottom);
        canvas.ClipRect(headerClipRect);

        // Header background
        if (_headerCells != null && _headerCells.Count > 0)
        {
            using var headerPaint = new SKPaint { Color = new SKColor(45, 45, 48) };
            canvas.DrawRect(headerClipRect, headerPaint);
        }

        // Render header cells
        if (_headerCells != null)
        {
            foreach (var headerCell in _headerCells)
            {
                var childOriginalOffset = headerCell.VisualOffset;
                headerCell.SetVisualOffset(new Point(childOriginalOffset.X + VisualOffset.X, childOriginalOffset.Y + VisualOffset.Y));
                headerCell.Render(canvas);
                headerCell.SetVisualOffset(childOriginalOffset);
            }
        }

        // Draw column separators in header (with horizontal scroll)
        if (ShowColumnSeparators)
        {
            using var headerColLinePaint = new SKPaint
            {
                Color = new SKColor(80, 80, 80),
                StrokeWidth = SeparatorThickness,
                IsAntialias = true
            };

            var columnX = baseX - HorizontalScrollOffset;
            for (int i = 0; i < _columnWidths.Length - 1; i++)
            {
                columnX += _columnWidths[i];
                canvas.DrawLine(columnX, baseY, columnX, headerBottom, headerColLinePaint);
            }
        }

        canvas.Restore(); // End header clip

        // === HEADER BOTTOM SEPARATOR LINE ===
        using var headerSepPaint = new SKPaint
        {
            Color = HeaderSeparatorColor,
            StrokeWidth = 2f,
            IsAntialias = true
        };
        canvas.DrawLine(baseX, headerBottom, baseX + ElementSize.Width, headerBottom, headerSepPaint);

        // === RENDER SCROLLBARS ===
        if (ShowVerticalScrollbar && CalculateMaxScrollOffset() > 0)
        {
            VerticalScrollbar.Render(canvas);
        }

        if (ShowHorizontalScrollbar && CalculateMaxHorizontalScrollOffset() > 0)
        {
            HorizontalScrollbar.Render(canvas);
        }

        canvas.Restore(); // End grid clip
    }

    public override UiElement? HitTest(Point point)
    {
        if (!(point.X >= Position.X && point.X <= Position.X + ElementSize.Width &&
              point.Y >= Position.Y && point.Y <= Position.Y + ElementSize.Height))
        {
            _hoveredRowIndex = -1;
            return null;
        }

        if (ShowVerticalScrollbar && CalculateMaxScrollOffset() > 0)
        {
            var scrollbarHit = VerticalScrollbar.HitTest(point);
            if (scrollbarHit != null)
            {
                _hoveredRowIndex = -1;
                return scrollbarHit;
            }
        }

        if (ShowHorizontalScrollbar && CalculateMaxHorizontalScrollOffset() > 0)
        {
            var scrollbarHit = HorizontalScrollbar.HitTest(point);
            if (scrollbarHit != null)
            {
                _hoveredRowIndex = -1;
                return scrollbarHit;
            }
        }

        foreach (var child in Children)
        {
            var hit = child.HitTest(point);
            if (hit is IInputControl or ITextInputControl or IToggleButtonControl or IDraggableControl)
            {
                _hoveredRowIndex = -1;
                return hit;
            }
        }

        var relativeY = point.Y - Position.Y - HeaderHeight + ScrollOffset;
        if (relativeY >= 0 && point.Y > Position.Y + HeaderHeight)
        {
            _hoveredRowIndex = (int)(relativeY / RowHeight);
        }
        else
        {
            _hoveredRowIndex = -1;
        }

        return this;
    }

    #region IHoverableControl

    private bool _isHovered;
    public bool IsHovered
    {
        get => _isHovered;
        set
        {
            if (_isHovered != value)
            {
                _isHovered = value;
                if (!value)
                {
                    _hoveredRowIndex = -1;
                }
            }
        }
    }

    #endregion

    #region IInputControl

    public void InvokeCommand()
    {
        if (_hoveredRowIndex < 0 || _itemsSource == null)
        {
            return;
        }

        var items = _itemsSource.ToList();
        if (_hoveredRowIndex >= items.Count)
        {
            return;
        }

        var item = items[_hoveredRowIndex];
        SetSelectedItem(item);
    }

    #endregion
}
