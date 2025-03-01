using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlusUi.core;

public class Grid : UiLayoutElement<Grid>
{
    private class GridItem(UiElement child, int row, int column, int rowSpan, int columnSpan)
    {
        public UiElement Element { get; } = child;
        public int Row { get; } = row;
        public int Column { get; } = column;
        public int RowSpan { get; } = rowSpan;
        public int ColumnSpan { get; } = columnSpan;
    }

    private class RowColumnItem<TType>(TType type, float? fixedSize = null, Func<float>? boundSize = null)
        where TType : struct
    {
        public TType Type { get; } = type;

        public float? FixedSize { get; } = fixedSize;
        public Func<float>? BoundSize { get; } = boundSize;

        public float MeasuredSize { get; set; }

        public event Action? SizeChanged;

        public float GetSize()
        {
            var newSize = FixedSize ?? BoundSize?.Invoke() ?? 0;
            if (newSize != MeasuredSize)
            {
                MeasuredSize = newSize;
                SizeChanged?.Invoke();
            }
            return MeasuredSize;
        }
    }

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
        foreach (var child in _children)
        {
            child.Element.InvalidateMeasure();
        }
    }

    protected override Size MeasureInternal(Size availableSize)
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
            var columnCount = child.ColumnSpan;
            for (var i = 0; i < columnCount && i + child.Column < _columns.Count; i++)
            {
                var columnIndex = child.Column + i;
                if (_columns[columnIndex].Type == Column.Auto)
                {
                    availableWidth = availableSize.Width;
                    break; // If any column is Auto, give full width
                }
                else
                {
                    availableWidth += _columns[columnIndex].MeasuredSize;
                }
            }

            var rowCount = child.RowSpan;
            for (var i = 0; i < rowCount && i + child.Row < _rows.Count; i++)
            {
                var rowIndex = child.Row + i;
                if (_rows[rowIndex].Type == Row.Auto)
                {
                    availableHeight = availableSize.Height;
                    break; // If any row is Auto, give full height
                }
                else
                {
                    availableHeight += _rows[rowIndex].MeasuredSize;
                }
            }

            // Adjust available size by the child's margin
            availableWidth -= child.Element.Margin.Left + child.Element.Margin.Right;
            availableHeight -= child.Element.Margin.Top + child.Element.Margin.Bottom;

            var childSize = child.Element.Measure(new Size(availableWidth, availableHeight));

            // Calculate row and column sizes for auto
            for (var i = 0; i < child.ColumnSpan && i + child.Column < _columns.Count; i++)
            {
                var columnIndex = child.Column + i;
                if (_columns[columnIndex].Type == Column.Auto)
                {
                    // For multi-column spanning, distribute proportionally (simple approach)
                    var columnContribution = (childSize.Width + child.Element.Margin.Left + child.Element.Margin.Right) / child.ColumnSpan;
                    _columns[columnIndex].MeasuredSize = Math.Max(_columns[columnIndex].MeasuredSize, columnContribution);
                }
            }
            for (var i = 0; i < child.RowSpan && i + child.Row < _rows.Count; i++)
            {
                var rowIndex = child.Row + i;
                if (_rows[rowIndex].Type == Row.Auto)
                {
                    // For multi-row spanning, distribute proportionally (simple approach)
                    var rowContribution = (childSize.Height + child.Element.Margin.Top + child.Element.Margin.Bottom) / child.RowSpan;
                    _rows[rowIndex].MeasuredSize = Math.Max(_rows[rowIndex].MeasuredSize, rowContribution);
                }
            }
        }

        // Recalculate sizes for star columns with final measurements
        totalFixedWidth = _columns.Where(c => c.Type != Column.Star).Sum(c => c.MeasuredSize);
        if (totalStarWeight > 0)
        {
            var remainingWidth = availableSize.Width - totalFixedWidth;
            if (remainingWidth > 0)
            {
                foreach (var column in _columns.Where(c => c.Type == Column.Star))
                {
                    column.MeasuredSize = remainingWidth * (column.FixedSize ?? 0) / totalStarWeight;
                }
            }
        }

        // Recalculate sizes for star rows with final measurements
        totalFixedHeight = _rows.Where(r => r.Type != Row.Star).Sum(r => r.MeasuredSize);
        if (totalStarHeightWeight > 0)
        {
            var remainingHeight = availableSize.Height - totalFixedHeight;
            if (remainingHeight > 0)
            {
                foreach (var row in _rows.Where(r => r.Type == Row.Star))
                {
                    row.MeasuredSize = remainingHeight * (row.FixedSize ?? 0) / totalStarHeightWeight;
                }
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
