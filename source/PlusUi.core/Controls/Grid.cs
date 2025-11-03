using PlusUi.core.Attributes;
using PlusUi.core.Controls.GridHelper;

namespace PlusUi.core;

[GenerateShadowMethods]
public partial class Grid : UiLayoutElement
{
    protected override bool NeedsMeasure => true;

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
        foreach (var child in _children)
        {
            // Get the natural size without stretch
            var childSize = child.Element.Measure(availableSize, true);
            childNaturalSizes[child.Element] = childSize; // Store for later use
        }

        // Determine Auto sizes based on children's natural sizes
        foreach (var child in _children)
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
        var totalFixedWidth = _columns.Where(c => c.Type != Column.Star).Sum(c => c.MeasuredSize);
        var remainingWidthForStars = Math.Max(0, availableSize.Width - totalFixedWidth);
        var totalStarWeight = _columns.Where(c => c.Type == Column.Star).Sum(c => c.FixedSize ?? 0);

        if (totalStarWeight > 0)
        {
            foreach (var column in _columns.Where(c => c.Type == Column.Star))
            {
                column.MeasuredSize = remainingWidthForStars * (column.FixedSize ?? 0) / totalStarWeight;
            }
        }

        var totalFixedHeight = _rows.Where(r => r.Type != Row.Star).Sum(r => r.MeasuredSize);
        var remainingHeightForStars = Math.Max(0, availableSize.Height - totalFixedHeight);
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
        foreach (var child in _children)
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
        foreach (var child in _children)
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
