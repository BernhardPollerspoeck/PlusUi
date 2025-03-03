using PlusUi.core.Controls.GridHelper;

namespace PlusUi.core;
public class Grid : UiLayoutElement<Grid>
{
    protected override bool NeadsMeasure => true;

    #region Children
    private readonly List<GridItem> _children = [];
    public override List<UiElement> Children => [.. _children.Select(c => c.Element)];

    public Grid AddChild(UiElement child, int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1)
    {
        child.Parent = this;
        _children.Add(new(child, row, column, rowSpan, columnSpan));
        return this;
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
    public IReadOnlyList<RowColumnItem<Column>> Columns => _columns;
    public Grid AddColumn(Column column, float size = 1)
    {
        var columnItem = new RowColumnItem<Column>(column, size);
        columnItem.SizeChanged += OnRowColumnSizeChanged;
        _columns.Add(columnItem);
        return this;
    }
    public Grid AddColumn(float size)
    {
        return AddColumn(Column.Absolute, size);
    }
    public Grid AddBoundColumn(Column column, Func<float> sizeGetter)
    {
        var columnItem = new RowColumnItem<Column>(column, null, sizeGetter);
        columnItem.SizeChanged += OnRowColumnSizeChanged;
        _columns.Add(columnItem);
        return this;
    }
    public Grid AddBoundColumn(Func<float> sizeGetter)
    {
        return AddBoundColumn(Column.Absolute, sizeGetter);
    }
    #endregion

    #region Rows
    private readonly List<RowColumnItem<Row>> _rows = [];
    public IReadOnlyList<RowColumnItem<Row>> Rows => _rows;
    public Grid AddRow(Row row, int size = 1)
    {
        var rowItem = new RowColumnItem<Row>(row, size);
        rowItem.SizeChanged += OnRowColumnSizeChanged;
        _rows.Add(rowItem);
        return this;
    }
    public Grid AddRow(int size)
    {
        return AddRow(Row.Absolute, size);
    }
    public Grid AddBoundRow(Row row, Func<float> sizeGetter)
    {
        var rowItem = new RowColumnItem<Row>(row, null, sizeGetter);
        rowItem.SizeChanged += OnRowColumnSizeChanged;
        _rows.Add(rowItem);
        return this;
    }
    public Grid AddBoundRow(Func<float> sizeGetter)
    {
        return AddBoundRow(Row.Absolute, sizeGetter);
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
        foreach (var child in _children)
        {
            child.Element.InvalidateMeasure();
        }
    }

    public override Size MeasureInternal(Size availableSize)
    {
        // Capture fixed row and column sizes
        for (var i = 0; i < _columns.Count; i++)
        {
            if (_columns[i].Type == Column.Absolute)
            {
                _columns[i].MeasuredSize = _columns[i].GetSize();
            }
        }
        for (var i = 0; i < _rows.Count; i++)
        {
            if (_rows[i].Type == Row.Absolute)
            {
                _rows[i].MeasuredSize = _rows[i].GetSize();
            }
        }

        















        // Pre-calculate star sizes for initial measure pass
        // This allows us to have reasonable sizes for star columns/rows before child measurement
        var totalFixedWidth = _columns.Where(c => c.Type == Column.Absolute).Sum(c => c.MeasuredSize);
        var remainingWidthForStars = Math.Max(0, availableSize.Width - totalFixedWidth);
        var totalStarWeight = _columns.Where(c => c.Type == Column.Star).Sum(c => c.FixedSize ?? 0);
        if (totalStarWeight > 0)
        {
            foreach (var column in _columns.Where(c => c.Type == Column.Star))
            {
                column.MeasuredSize = remainingWidthForStars * (column.FixedSize ?? 0) / totalStarWeight;
            }
        }

        var totalFixedHeight = _rows.Where(r => r.Type == Row.Absolute).Sum(r => r.MeasuredSize);
        var remainingHeightForStars = Math.Max(0, availableSize.Height - totalFixedHeight);
        var totalStarHeightWeight = _rows.Where(r => r.Type == Row.Star).Sum(r => r.FixedSize ?? 0);
        if (totalStarHeightWeight > 0)
        {
            foreach (var row in _rows.Where(r => r.Type == Row.Star))
            {
                row.MeasuredSize = remainingHeightForStars * (row.FixedSize ?? 0) / totalStarHeightWeight;
            }
        }

        // Measure all children
        foreach (var child in _children)
        {
            // Calculate the available size for this child
            float availableWidth = 0;
            float availableHeight = 0;

            // For auto-sized columns/rows, pass through the available size
            // For fixed/star, use the assigned size
            var columnCount = Math.Min(child.ColumnSpan, _columns.Count - child.Column);
            var hasAutoColumn = false;

            for (var i = 0; i < columnCount; i++)
            {
                var columnIndex = child.Column + i;
                if (columnIndex < _columns.Count) // Ensure we don't go out of bounds
                {
                    if (_columns[columnIndex].Type == Column.Auto)
                    {
                        hasAutoColumn = true;
                    }
                    else
                    {
                        availableWidth += _columns[columnIndex].MeasuredSize;
                    }
                }
            }

            if (hasAutoColumn)
            {
                availableWidth = availableSize.Width;
            }

            var rowCount = Math.Min(child.RowSpan, _rows.Count - child.Row);
            var hasAutoRow = false;

            for (var i = 0; i < rowCount; i++)
            {
                var rowIndex = child.Row + i;
                if (rowIndex < _rows.Count) // Ensure we don't go out of bounds
                {
                    if (_rows[rowIndex].Type == Row.Auto)
                    {
                        hasAutoRow = true;
                    }
                    else
                    {
                        availableHeight += _rows[rowIndex].MeasuredSize;
                    }
                }
            }

            if (hasAutoRow)
            {
                availableHeight = availableSize.Height;
            }

            // Adjust available size by the child's margin
            availableWidth -= child.Element.Margin.Left + child.Element.Margin.Right;
            availableHeight -= child.Element.Margin.Top + child.Element.Margin.Bottom;

            // Ensure we don't pass negative sizes to children
            availableWidth = Math.Max(0, availableWidth);
            availableHeight = Math.Max(0, availableHeight);

            var childSize = child.Element.Measure(new Size(availableWidth, availableHeight));

            // Calculate row and column sizes for auto
            if (child.ColumnSpan > 1)
            {
                // For multi-column spanning with auto columns, distribute proportionally
                float totalAutoWidth = childSize.Width + child.Element.Margin.Left + child.Element.Margin.Right;
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

            if (child.RowSpan > 1)
            {
                // For multi-row spanning with auto rows, distribute proportionally
                float totalAutoHeight = childSize.Height + child.Element.Margin.Top + child.Element.Margin.Bottom;
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

        // Recalculate sizes for star columns with final measurements
        totalFixedWidth = _columns.Where(c => c.Type != Column.Star).Sum(c => c.MeasuredSize);
        if (totalStarWeight > 0)
        {
            var remainingWidth = Math.Max(0, availableSize.Width - totalFixedWidth);
            foreach (var column in _columns.Where(c => c.Type == Column.Star))
            {
                column.MeasuredSize = remainingWidth * (column.FixedSize ?? 0) / totalStarWeight;
            }
        }

        // Recalculate sizes for star rows with final measurements
        totalFixedHeight = _rows.Where(r => r.Type != Row.Star).Sum(r => r.MeasuredSize);
        if (totalStarHeightWeight > 0)
        {
            var remainingHeight = Math.Max(0, availableSize.Height - totalFixedHeight);
            foreach (var row in _rows.Where(r => r.Type == Row.Star))
            {
                row.MeasuredSize = remainingHeight * (row.FixedSize ?? 0) / totalStarHeightWeight;
            }
        }

        var totalWidth = _columns.Sum(c => c.MeasuredSize) + Margin.Left + Margin.Right;
        var totalHeight = _rows.Sum(r => r.MeasuredSize) + Margin.Top + Margin.Bottom;

        return new Size(totalWidth, totalHeight);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var columnWidths = _columns.Select(c => c.MeasuredSize).ToArray();
        var rowHeights = _rows.Select(r => r.MeasuredSize).ToArray();

        // Arrange children
        foreach (var child in _children)
        {
            var x = columnWidths.Take(child.Column).Sum();
            var y = rowHeights.Take(child.Row).Sum();
            var width = columnWidths.Skip(child.Column).Take(child.ColumnSpan).Sum();
            var height = rowHeights.Skip(child.Row).Take(child.RowSpan).Sum();

            child.Element.Arrange(new Rect(x, y, width, height));
        }
        return base.ArrangeInternal(bounds);
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
