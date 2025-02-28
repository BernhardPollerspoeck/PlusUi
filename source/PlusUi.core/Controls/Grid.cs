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

    #region Children
    private readonly List<GridItem> _children = [];
    public new List<UiElement> Children => [.. _children.Select(c => c.Element)];

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
    private readonly List<(Column Type, int Size)> _columns = [];
    private readonly List<Func<int>> _boundColumns = [];

    public Grid AddColumn(Column column, int size = 1)
    {
        _columns.Add((column, size));
        return this;
    }
    public Grid AddColumn(int size)
    {
        return AddColumn(Column.Absolute, size);
    }
    public Grid AddBoundColumn(Column column, Func<int> sizeGetter)
    {
        _columns.Add((column, 0));
        _boundColumns.Add(sizeGetter);
        return this;
    }
    public Grid AddBoundColumn(Func<int> sizeGetter)
    {
        return AddBoundColumn(Column.Absolute, sizeGetter);
    }
    #endregion

    #region Rows
    private readonly List<(Row Type, int Size)> _rows = [];
    private readonly List<Func<int>> _boundRows = [];

    public Grid AddRow(Row row, int size = 1)
    {
        _rows.Add((row, size));
        return this;
    }
    public Grid AddRow(int size)
    {
        return AddRow(Row.Absolute, size);
    }
    public Grid AddBoundRow(Row row, Func<int> sizeGetter)
    {
        _rows.Add((row, 0));
        _boundRows.Add(sizeGetter);
        return this;
    }
    public Grid AddBoundRow(Func<int> sizeGetter)
    {
        return AddBoundRow(Row.Absolute, sizeGetter);
    }
    #endregion

    public Grid()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
    }

    protected override Size MeasureInternal(Size availableSize)
    {
        // Create arrays to track sizes
        var columnWidths = new float[_columns.Count];
        var rowHeights = new float[_rows.Count];

        // Capture fixed row and column sizes
        for (var i = 0; i < _columns.Count; i++)
        {
            var (type, size) = _columns[i];
            if (type == Column.Absolute)
            {
                columnWidths[i] = size;
            }
        }

        for (var i = 0; i < _rows.Count; i++)
        {
            var (type, size) = _rows[i];
            if (type == Row.Absolute)
            {
                rowHeights[i] = size;
            }
        }

        // Apply bound column/row sizes
        for (var i = 0; i < _boundColumns.Count && i < _columns.Count; i++)
        {
            if (_columns[i].Type == Column.Absolute)
            {
                columnWidths[i] = _boundColumns[i]();
            }
        }

        for (var i = 0; i < _boundRows.Count && i < _rows.Count; i++)
        {
            if (_rows[i].Type == Row.Absolute)
            {
                rowHeights[i] = _boundRows[i]();
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
                }
                else
                {
                    availableWidth += columnWidths[columnIndex];
                }
            }

            var rowCount = child.RowSpan;
            for (var i = 0; i < rowCount && i + child.Row < _rows.Count; i++)
            {
                var rowIndex = child.Row + i;
                if (_rows[rowIndex].Type == Row.Auto)
                {
                    availableHeight = availableSize.Height;
                }
                else
                {
                    availableHeight += rowHeights[rowIndex];
                }
            }

            var childSize = child.Element.Measure(new Size(availableWidth, availableHeight));

            // Calculate row and column sizes for auto
            for (var i = 0; i < child.ColumnSpan && i + child.Column < _columns.Count; i++)
            {
                var columnIndex = child.Column + i;
                if (_columns[columnIndex].Type == Column.Auto)
                {
                    // For multi-column spanning, distribute proportionally (simple approach)
                    var columnContribution = childSize.Width / child.ColumnSpan;
                    columnWidths[columnIndex] = Math.Max(columnWidths[columnIndex], columnContribution);
                }
            }

            for (var i = 0; i < child.RowSpan && i + child.Row < _rows.Count; i++)
            {
                var rowIndex = child.Row + i;
                if (_rows[rowIndex].Type == Row.Auto)
                {
                    // For multi-row spanning, distribute proportionally (simple approach)
                    var rowContribution = childSize.Height / child.RowSpan;
                    rowHeights[rowIndex] = Math.Max(rowHeights[rowIndex], rowContribution);
                }
            }
        }

        // Calculate sizes for star columns
        var totalWidth = columnWidths.Sum();
        float totalStarWeight = 0;

        for (var i = 0; i < _columns.Count; i++)
        {
            if (_columns[i].Type == Column.Star)
            {
                totalStarWeight += _columns[i].Size;
            }
        }

        if (totalStarWeight > 0)
        {
            var remainingWidth = availableSize.Width - columnWidths.Sum(w => w);
            if (remainingWidth > 0)
            {
                for (var i = 0; i < _columns.Count; i++)
                {
                    if (_columns[i].Type == Column.Star)
                    {
                        columnWidths[i] = remainingWidth * (_columns[i].Size / totalStarWeight);
                        totalWidth += columnWidths[i];
                    }
                }
            }
        }

        // Calculate sizes for star rows
        var totalHeight = rowHeights.Sum();
        float totalStarHeightWeight = 0;

        for (var i = 0; i < _rows.Count; i++)
        {
            if (_rows[i].Type == Row.Star)
            {
                totalStarHeightWeight += _rows[i].Size;
            }
        }

        if (totalStarHeightWeight > 0)
        {
            var remainingHeight = availableSize.Height - rowHeights.Sum(h => h);
            if (remainingHeight > 0)
            {
                for (var i = 0; i < _rows.Count; i++)
                {
                    if (_rows[i].Type == Row.Star)
                    {
                        rowHeights[i] = remainingHeight * (_rows[i].Size / totalStarHeightWeight);
                        totalHeight += rowHeights[i];
                    }
                }
            }
        }

        return new Size(totalWidth, totalHeight);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var columnWidths = new float[_columns.Count];
        var rowHeights = new float[_rows.Count];

        // Calculate column widths
        for (var i = 0; i < _columns.Count; i++)
        {
            var (type, size) = _columns[i];
            if (type == Column.Absolute)
            {
                columnWidths[i] = size;
            }
            else if (type == Column.Star)
            {
                columnWidths[i] = bounds.Width / _columns.Count;
            }
            else if (type == Column.Auto)
            {
                columnWidths[i] = 0;
            }
        }

        // Apply bound column sizes
        for (var i = 0; i < _boundColumns.Count; i++)
        {
            columnWidths[i] = _boundColumns[i]();
        }

        // Calculate row heights
        for (var i = 0; i < _rows.Count; i++)
        {
            var (type, size) = _rows[i];
            if (type == Row.Absolute)
            {
                rowHeights[i] = size;
            }
            else if (type == Row.Star)
            {
                rowHeights[i] = bounds.Height / _rows.Count;
            }
            else if (type == Row.Auto)
            {
                rowHeights[i] = 0;
            }
        }

        // Apply bound row sizes
        for (var i = 0; i < _boundRows.Count; i++)
        {
            rowHeights[i] = _boundRows[i]();
        }

        // Arrange children
        foreach (var child in _children)
        {
            var x = columnWidths.Take(child.Column).Sum();
            var y = rowHeights.Take(child.Row).Sum();
            var width = columnWidths.Skip(child.Column).Take(child.ColumnSpan).Sum();
            var height = rowHeights.Skip(child.Row).Take(child.RowSpan).Sum();

            child.Element.Arrange(new Rect(x, y, width, height));
        }

        return new Point(bounds.X, bounds.Y);
    }
}
