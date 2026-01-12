using PlusUi.core.Attributes;
using PlusUi.core.Controls.GridHelper;
using System.ComponentModel;

namespace PlusUi.core;

/// <summary>
/// A flexible grid layout that arranges child elements in rows and columns.
/// Supports absolute, proportional (star), and auto-sizing for rows and columns.
/// </summary>
/// <remarks>
/// Grid uses row and column definitions to create a flexible layout:
/// - Absolute: Fixed pixel size
/// - Star (*): Proportional sizing (e.g., 1*, 2*, 3* divides available space)
/// - Auto: Sizes to fit content
/// </remarks>
/// <example>
/// <code>
/// // 2x2 grid with fixed sizes
/// new Grid()
///     .AddRowDefinition(Row.Absolute, 50)
///     .AddRowDefinition(Row.Auto)
///     .AddColumnDefinition(Column.Star, 1)
///     .AddColumnDefinition(Column.Star, 2)
///     .AddChild(new Label().SetText("Top Left"), row: 0, column: 0)
///     .AddChild(new Button().SetText("Top Right"), row: 0, column: 1);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class Grid : UiLayoutElement
{
    public override string? GetComputedAccessibilityLabel()
    {
        if (AccessibilityLabel != null) return AccessibilityLabel;
        return Children.Count > 0 ? $"Grid layout ({_rows.Count} rows, {_columns.Count} columns)" : null;
    }

    public override string? GetComputedAccessibilityValue()
    {
        if (AccessibilityValue != null) return AccessibilityValue;
        return Children.Count > 0 ? $"{Children.Count} item{(Children.Count == 1 ? "" : "s")}" : null;
    }

    public override INotifyPropertyChanged? Context
    {
        get => base.Context;
        internal set
        {
            if (base.Context is not null)
            {
                base.Context.PropertyChanged -= HandleContextPropertyChanged;
            }
            base.Context = value;
            if (value is not null)
            {
                value.PropertyChanged += HandleContextPropertyChanged;
            }
        }
    }
    private void HandleContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        foreach (var column in Columns.Where(c => c.PropertyName == e.PropertyName))
        {
            column.GetSize();
        }
        foreach (var row in Rows.Where(r => r.PropertyName == e.PropertyName))
        {
            row.GetSize();
        }
    }


    #region Children
    private readonly List<GridItem> _children = [];
    public override List<UiElement> Children => [.. _children.Select(c => c.Element)];

    public Grid AddChild(UiElement child, int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1)
    {
        child.Parent = this;
        _children.Add(new(child, row, column, rowSpan, columnSpan));
        return this;
    }
    public new Grid AddChild(UiElement child)
    {
        return AddChild(child, 0, 0, 1, 1);
    }
    public new Grid RemoveChild(UiElement child)
    {
        var item = _children.FirstOrDefault(x => x.Element == child);
        if (item != null)
        {
            _children.Remove(item);
        }
        return this;
    }
    public new Grid ClearChildren()
    {
        _children.Clear();
        return this;
    }
    #endregion

    #region Columns
    private readonly List<RowColumnItem<Column>> _columns = [];
    internal IReadOnlyList<RowColumnItem<Column>> Columns => _columns;
    public Grid AddColumn(Column column, float size = 1)
    {
        var columnItem = new RowColumnItem<Column>(null, column, size);
        columnItem.SizeChanged += OnRowColumnSizeChanged;
        _columns.Add(columnItem);
        return this;
    }
    public Grid AddColumn(float size)
    {
        return AddColumn(Column.Absolute, size);
    }
    public Grid AddBoundColumn(string propertyName, Column column, Func<float> sizeGetter)
    {
        var columnItem = new RowColumnItem<Column>(propertyName, column, null, sizeGetter);
        columnItem.SizeChanged += OnRowColumnSizeChanged;
        _columns.Add(columnItem);
        return this;
    }
    public Grid AddBoundColumn(string propertyName, Func<float> sizeGetter)
    {
        return AddBoundColumn(propertyName, Column.Absolute, sizeGetter);
    }
    #endregion

    #region Rows
    private readonly List<RowColumnItem<Row>> _rows = [];
    internal IReadOnlyList<RowColumnItem<Row>> Rows => _rows;
    public Grid AddRow(Row row, int size = 1)
    {
        var rowItem = new RowColumnItem<Row>(null, row, size);
        rowItem.SizeChanged += OnRowColumnSizeChanged;
        _rows.Add(rowItem);
        return this;
    }
    public Grid AddRow(int size)
    {
        return AddRow(Row.Absolute, size);
    }
    public Grid AddBoundRow(string propertyName, Row row, Func<float> sizeGetter)
    {
        var rowItem = new RowColumnItem<Row>(propertyName, row, null, sizeGetter);
        rowItem.SizeChanged += OnRowColumnSizeChanged;
        _rows.Add(rowItem);
        return this;
    }
    public Grid AddBoundRow(string propertyName, Func<float> sizeGetter)
    {
        return AddBoundRow(propertyName, Row.Absolute, sizeGetter);
    }
    #endregion

    public Grid()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
    }

    private void OnRowColumnSizeChanged()
    {
        InvalidateMeasure();
    }

    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        if (_rows.Count == 0)
        {
            AddRow(Row.Auto);
        }
        if (_columns.Count == 0)
        {
            AddColumn(Column.Auto);
        }

        // Reduce available size by Grid's margin
        var availableWidthForColumns = Math.Max(0, availableSize.Width - Margin.Horizontal);
        var availableHeightForRows = Math.Max(0, availableSize.Height - Margin.Vertical);
        var availableSizeForChildren = new Size(availableWidthForColumns, availableHeightForRows);

        // Dictionary to cache child natural sizes
        var childNaturalSizes = new Dictionary<UiElement, Size>();

        // Capture fixed row and column sizes
        for (var i = 0; i < _columns.Count; i++)
        {
            _columns[i].MeasuredSize = _columns[i].Type == Column.Absolute ? _columns[i].GetSize() : 0;
        }

        for (var i = 0; i < _rows.Count; i++)
        {
            _rows[i].MeasuredSize = _rows[i].Type == Row.Absolute ? _rows[i].GetSize() : 0;
        }

        // First pass: Measure children to determine Auto column/row sizes
        // Use dontStretch=true to get natural sizes without stretching
        foreach (var child in _children.ToList())
        {
            // Get the natural size without stretch
            var childSize = child.Element.Measure(availableSizeForChildren, true);
            childNaturalSizes[child.Element] = childSize; // Store for later use
        }

        // Determine Auto sizes based on children's natural sizes
        foreach (var child in _children.ToList())
        {
            var columnCount = Math.Min(child.ColumnSpan, _columns.Count - child.Column);
            var rowCount = Math.Min(child.RowSpan, _rows.Count - child.Row);
            var childSize = childNaturalSizes[child.Element]; // Use cached size

            // Calculate column sizes for Auto columns
            if (child.ColumnSpan > 1)
            {
                // For multi-column spanning with auto columns, distribute proportionally
                var totalAutoWidth = childSize.Width + child.Element.Margin.Left + child.Element.Margin.Right;
                var autoColumns = new List<int>();

                for (var i = 0; i < columnCount; i++)
                {
                    var columnIndex = child.Column + i;
                    if (columnIndex < _columns.Count && _columns[columnIndex].Type == Column.Auto)
                    {
                        autoColumns.Add(columnIndex);
                    }
                }

                if (autoColumns.Count > 0)
                {
                    var widthPerAutoColumn = totalAutoWidth / autoColumns.Count;
                    foreach (var columnIndex in autoColumns)
                    {
                        _columns[columnIndex].MeasuredSize = Math.Max(_columns[columnIndex].MeasuredSize, widthPerAutoColumn);
                    }
                }
            }
            else
            {
                // Single column
                for (var i = 0; i < columnCount; i++)
                {
                    var columnIndex = child.Column + i;
                    if (columnIndex < _columns.Count && _columns[columnIndex].Type == Column.Auto)
                    {
                        _columns[columnIndex].MeasuredSize = Math.Max(
                            _columns[columnIndex].MeasuredSize,
                            childSize.Width + child.Element.Margin.Left + child.Element.Margin.Right);
                    }
                }
            }

            // Calculate row sizes for Auto rows
            if (child.RowSpan > 1)
            {
                // For multi-row spanning with auto rows, distribute proportionally
                var totalAutoHeight = childSize.Height + child.Element.Margin.Top + child.Element.Margin.Bottom;
                var autoRows = new List<int>();

                for (var i = 0; i < rowCount; i++)
                {
                    var rowIndex = child.Row + i;
                    if (rowIndex < _rows.Count && _rows[rowIndex].Type == Row.Auto)
                    {
                        autoRows.Add(rowIndex);
                    }
                }

                if (autoRows.Count > 0)
                {
                    var heightPerAutoRow = totalAutoHeight / autoRows.Count;
                    foreach (var rowIndex in autoRows)
                    {
                        _rows[rowIndex].MeasuredSize = Math.Max(_rows[rowIndex].MeasuredSize, heightPerAutoRow);
                    }
                }
            }
            else
            {
                // Single row
                for (var i = 0; i < rowCount; i++)
                {
                    var rowIndex = child.Row + i;
                    if (rowIndex < _rows.Count && _rows[rowIndex].Type == Row.Auto)
                    {
                        _rows[rowIndex].MeasuredSize = Math.Max(
                            _rows[rowIndex].MeasuredSize,
                            childSize.Height + child.Element.Margin.Top + child.Element.Margin.Bottom);
                    }
                }
            }
        }

        // Calculate star sizes - first pass
        // Use available size AFTER accounting for Grid's margin
        var totalFixedWidth = _columns.Where(c => c.Type != Column.Star).Sum(c => c.MeasuredSize);
        var remainingWidthForStars = Math.Max(0, availableWidthForColumns - totalFixedWidth);
        var totalStarWeight = _columns.Where(c => c.Type == Column.Star).Sum(c => c.FixedSize ?? 0);

        if (totalStarWeight > 0)
        {
            foreach (var column in _columns.Where(c => c.Type == Column.Star))
            {
                column.MeasuredSize = remainingWidthForStars * (column.FixedSize ?? 0) / totalStarWeight;
            }
        }

        var totalFixedHeight = _rows.Where(r => r.Type != Row.Star).Sum(r => r.MeasuredSize);
        var remainingHeightForStars = Math.Max(0, availableHeightForRows - totalFixedHeight);
        var totalStarHeightWeight = _rows.Where(r => r.Type == Row.Star).Sum(r => r.FixedSize ?? 0);

        if (totalStarHeightWeight > 0)
        {
            foreach (var row in _rows.Where(r => r.Type == Row.Star))
            {
                row.MeasuredSize = remainingHeightForStars * (row.FixedSize ?? 0) / totalStarHeightWeight;
            }
        }

        // Second pass: Measure children with final column/row sizes
        // This time use the actual size constraints and respect stretch alignment
        foreach (var child in _children.ToList())
        {
            // Calculate the available size for this child based on the grid columns/rows it spans
            float availableWidth = 0;
            float availableHeight = 0;

            var columnCount = Math.Min(child.ColumnSpan, _columns.Count - child.Column);
            for (var i = 0; i < columnCount; i++)
            {
                var columnIndex = child.Column + i;
                if (columnIndex < _columns.Count)
                {
                    availableWidth += _columns[columnIndex].MeasuredSize;
                }
            }

            var rowCount = Math.Min(child.RowSpan, _rows.Count - child.Row);
            for (var i = 0; i < rowCount; i++)
            {
                var rowIndex = child.Row + i;
                if (rowIndex < _rows.Count)
                {
                    availableHeight += _rows[rowIndex].MeasuredSize;
                }
            }

            // Adjust available size by the child's margin
            availableWidth -= child.Element.Margin.Left + child.Element.Margin.Right;
            availableHeight -= child.Element.Margin.Top + child.Element.Margin.Bottom;

            // Ensure we don't pass negative sizes to children
            availableWidth = Math.Max(0, availableWidth);
            availableHeight = Math.Max(0, availableHeight);

            // Final measurement with correct constraints and allowing stretch
            child.Element.Measure(new Size(availableWidth, availableHeight));
        }

        var totalWidth = _columns.Sum(c => c.MeasuredSize);
        var totalHeight = _rows.Sum(r => r.MeasuredSize);

        return new Size(totalWidth, totalHeight);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        // First call base.ArrangeInternal to get the grid's position with margin applied
        var gridPosition = base.ArrangeInternal(bounds);

        var columnWidths = _columns.Select(c => c.MeasuredSize).ToArray();
        var rowHeights = _rows.Select(r => r.MeasuredSize).ToArray();

        // Arrange children
        foreach (var child in _children.ToList())
        {
            var x = gridPosition.X + columnWidths.Take(child.Column).Sum();
            var y = gridPosition.Y + rowHeights.Take(child.Row).Sum();
            var width = columnWidths.Skip(child.Column).Take(child.ColumnSpan).Sum();
            var height = rowHeights.Skip(child.Row).Take(child.RowSpan).Sum();

            child.Element.Arrange(new Rect(x, y, width, height));
        }
        return gridPosition;
    }

    public override UiElement? HitTest(Point point)
    {
        foreach (var child in Children.ToList())
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
